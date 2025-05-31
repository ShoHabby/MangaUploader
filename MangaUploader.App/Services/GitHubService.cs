using System.Collections.Immutable;
using System.ComponentModel;
using GitCredentialManager;
using MangaUploader.Core.Extensions.Collections;
using MangaUploader.Core.Extensions.Logging;
using MangaUploader.Core.Extensions.Strings;
using MangaUploader.Core.Models;
using MangaUploader.Core.Models.Cubari;
using MangaUploader.Core.Services;
using Octokit;

using static MangaUploader.Core.Services.IGitHubService;

namespace MangaUploader.Services;

/// <summary>
/// GitHub Service implementation
/// </summary>
/// <param name="cubariService">Cubari payload service</param>
internal sealed class GitHubService(ICubariService cubariService) : IGitHubService
{
    /// <summary>
    /// Where the credentials where fetched from
    /// </summary>
    private enum CredentialsFetchType
    {
        None,
        Vault,
        Oauth
    }

    #region Constants
    /// <summary>
    /// GitHub service URL
    /// </summary>
    private const string SERVICE = "https://github.com/";
    /// <summary>
    /// UserAgent product name
    /// </summary>
    private const string PRODUCT = "sho-habby-manga-uploader";
    /// <summary>
    /// GitHub App client ID
    /// </summary>
    private const string CLIENT_ID = "Ov23liOpq5oOaViY5O5O";

    /// <summary>
    /// Application scopes
    /// </summary>
    private static readonly ImmutableArray<string> AppScopes = ["public_repo"];
    #endregion

    #region Properties
    /// <summary>
    /// GitHub client
    /// </summary>
    private GitHubClient Client { get; } = new(new ProductHeaderValue(PRODUCT));
    /// <summary>
    /// Credentials manager
    /// </summary>
    private GitCredentialManager.ICredentialStore Vault { get; } = CredentialManager.Create(PRODUCT);
    /// <summary>
    /// Cubari payload service
    /// </summary>
    private ICubariService CubariService { get; } = cubariService;
    /// <inheritdoc />
    public bool IsAuthenticated { get; private set; }
    #endregion

    #region Events
    /// <inheritdoc />
    public event DeviceFlowCodeDelegate? OnDeviceFlowCodeAvailable;
    #endregion

    #region Methods
    /// <inheritdoc />
    public bool HasSavedCredentials() => this.Vault.Get(SERVICE, PRODUCT) is not null;

    /// <inheritdoc />
    public async Task<UserInfo?> Authenticate()
    {
        if (this.IsAuthenticated) return null;

        // Try and get credentials
        (ICredential? credentials, CredentialsFetchType fetchType) = await FetchCredentials();

        // If no credentials could be fetched, exit
        if (credentials is null) return null;

        // Test authentication with client
        User? user = await TestAuthentication(credentials);
        if (user is null)
        {
            // Remove invalid credentials
            this.Vault.Remove(SERVICE, PRODUCT);
            switch (fetchType)
            {
                // If authentication failed from vault, we can get new ones through Oauth
                case CredentialsFetchType.Vault:
                    break;

                // If authentication failed from Oauth, assume something is wrong and quit out
                case CredentialsFetchType.Oauth:
                    return null;

                // Invalid
                case CredentialsFetchType.None:
                default:
                    throw new InvalidEnumArgumentException(nameof(fetchType), (int)fetchType, typeof(CredentialsFetchType));
            }

            // Try and get new credentials, forcing Oauth
            (credentials, _) = await FetchCredentials(false);
            if (credentials is null) return null;

            // Try testing authentication again, if it fails, quit out
            user = await TestAuthentication(credentials);
            if (user is null) return null;
        }

        // Authentication completed
        this.IsAuthenticated = true;
        return new UserInfo(user.Login, user.Email, user.AvatarUrl);
    }

    /// <summary>
    /// Fetches the credentials from the vault, or if none are found,
    /// requests new ones
    /// </summary>
    /// <param name="checkVault">If the vault should be checked for credentials or not</param>
    /// <returns>The fetched credentials if successful, or <see langword="null"/></returns>
    private async Task<(ICredential?, CredentialsFetchType)> FetchCredentials(bool checkVault = true)
    {
        // Try and get the credentials from the vault if possible
        if (checkVault)
        {
            ICredential? credentials = this.Vault.Get(SERVICE, PRODUCT);
            if (credentials is not null) return (credentials, CredentialsFetchType.Vault);
        }

        // Request authentication code
        OauthDeviceFlowRequest request = new(CLIENT_ID);
        request.Scopes.AddRange(AppScopes.AsSpan());
        OauthDeviceFlowResponse response = await this.Client.Oauth.InitiateDeviceFlow(request);

        // Broadcast user code then open browser
        OnDeviceFlowCodeAvailable?.Invoke(response.UserCode, TimeSpan.FromSeconds(response.ExpiresIn));
        await App.GetTopLevel().Launcher.LaunchUriAsync(response.VerificationUri.AsUri());

        // Wait for user to approve app connection
        OauthToken token = await this.Client.Oauth.CreateAccessTokenForDeviceFlow(CLIENT_ID, response);

        // If failed, dump error and exit
        if (string.IsNullOrEmpty(token.AccessToken))
        {
            await this.LogErrorAsync($"Could not get GitHub OAuth token.\n Error: {token.Error}");
            return (null, CredentialsFetchType.None);
        }

        // Store token securely and return it
        this.Vault.AddOrUpdate(SERVICE, PRODUCT, token.AccessToken);
        return (this.Vault.Get(SERVICE, PRODUCT), CredentialsFetchType.Oauth);
    }

    /// <summary>
    /// Test the connection to the GitHub client
    /// </summary>
    /// <param name="credentials">Credentials to test for</param>
    /// <returns><see langword="true"/> if the GitHub connection is successful, otherwise <see langword="false"/></returns>
    private async Task<User?> TestAuthentication(ICredential credentials)
    {
        this.Client.Credentials = new Credentials(credentials.Password, AuthenticationType.Oauth);
        try
        {
            User user = await this.Client.User.Current();
            await this.LogAsync($"Successfully authenticated to the GitHub Client as {user.Login}.");
            return user;
        }
        catch (Exception e)
        {
            // Log error and return
            await this.LogErrorAsync("Invalid credentials detected, deleting existing token...");
            await e.LogExceptionAsync();
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<ImmutableArray<RepositoryInfo>?> FetchPublicRepos()
    {
        if (!this.IsAuthenticated) return null;

        IReadOnlyList<Repository> repos = await this.Client.Repository.GetAllForCurrent();
        ImmutableArray<RepositoryInfo>.Builder reposBuilder = ImmutableArray.CreateBuilder<RepositoryInfo>();
        foreach (Repository repo in repos.Where(r => !r.Archived))
        {
            reposBuilder.Add(new RepositoryInfo(repo.FullName, repo.Id));
        }

        return reposBuilder.ToImmutable();
    }

    /// <inheritdoc />
    /// ReSharper disable once CognitiveComplexity
    public async Task<ImmutableArray<MangaFileInfo>> FetchRepoMangaContents(long repositoryId)
    {
        // Get top level content of repository
        IReadOnlyList<RepositoryContent> contents = await this.Client.Repository.Content.GetAllContents(repositoryId);
        Queue<RepositoryContent> contentsToExplore = new(contents.Count);
        foreach (RepositoryContent exploring in contents)
        {
            contentsToExplore.Enqueue(exploring);
        }

        // Explore entire repository recursively
        List<string> jsonFiles = [];
        while (contentsToExplore.TryDequeue(out RepositoryContent? content))
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (content.Type.Value)
            {
                // Cache json files
                case ContentType.File when content.Name.EndsWith(".json"):
                    jsonFiles.Add(content.Path);
                    break;

                // Add directories contents to exploration list
                case ContentType.Dir:
                    contents = await this.Client.Repository.Content.GetAllContents(repositoryId, content.Path);
                    foreach (RepositoryContent exploring in contents)
                    {
                        contentsToExplore.Enqueue(exploring);
                    }
                    break;
            }
        }

        // Build out manga files
        ImmutableArray<MangaFileInfo>.Builder mangaContents = ImmutableArray.CreateBuilder<MangaFileInfo>(jsonFiles.Count);
        foreach (string filePath in jsonFiles)
        {
            // Fetch file contents
            contents = await this.Client.Repository.Content.GetAllContents(repositoryId, filePath);
            if (contents.Count is not 1) continue;

            // Try converting to manga payload, add if successful
            RepositoryContent file = contents[0];
            Manga? manga = this.CubariService.DeserializeManga(file.Content);
            if (manga is not null)
            {
                mangaContents.Add(new MangaFileInfo(file.Path, file.Sha, repositoryId, manga));
            }
        }

        // Return all valid files
        return mangaContents.ToImmutable();
    }

    /// <inheritdoc />
    public void Disconnect()
    {
        this.Client.Credentials = Credentials.Anonymous;
        this.Vault.Remove(SERVICE, PRODUCT);
        this.IsAuthenticated = false;
    }
    #endregion
}
