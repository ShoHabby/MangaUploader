using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;
using GitCredentialManager;
using MangaUploader.Core.Extensions.Collections;
using MangaUploader.Core.Extensions.Logging;
using MangaUploader.Core.Services;
using Octokit;

using DeviceFlowCodeDelegate = MangaUploader.Core.Services.IGitHubService.DeviceFlowCodeDelegate;

namespace MangaUploader.Services;

/// <summary>
/// GitHub Service implementation
/// </summary>
public class GitHubService : IGitHubService
{
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
    private static readonly ImmutableArray<string> AppScopes = ["public_repo", "gist"];
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
    #endregion

    #region Events
    /// <inheritdoc />
    public event DeviceFlowCodeDelegate? OnDeviceFlowCodeAvailable;

    /// <inheritdoc />
    public event Action? OnAuthenticationCompleted;
    #endregion

    #region Methods
    /// <inheritdoc />
    public async Task Connect()
    {
        // Try and get credentials from vault
        ICredential? credentials = this.Vault.Get(SERVICE, PRODUCT);
        if (credentials is null)
        {
            // If it fails, request them through Device Flow
            credentials = await RequestAccessToken();
            // If that still fails, quit out
            if (credentials is null) return;
        }

        // Test authentication with client
        if (!await TestConnection(credentials))
        {
            // If it fails, request a new access token
            credentials = await RequestAccessToken();
            // If that fails, or if the connection test fails again, quit out
            if (credentials is null || !await TestConnection(credentials)) return;
        }

        // Authentication completed, notify of such
        OnAuthenticationCompleted?.Invoke();
    }

    /// <summary>
    /// Request an OAuth token through Device Flow to authenticate the app with the client
    /// </summary>
    /// <returns>The fetched credentials if successful, or <see langword="null"/></returns>
    private async Task<ICredential?> RequestAccessToken()
    {
        // Request authentication code
        OauthDeviceFlowRequest request = new(CLIENT_ID);
        request.Scopes.AddRange(AppScopes.AsSpan());
        OauthDeviceFlowResponse response = await this.Client.Oauth.InitiateDeviceFlow(request);

        // Broadcast user code then open browser
        OnDeviceFlowCodeAvailable?.Invoke(response.UserCode, TimeSpan.FromSeconds(response.ExpiresIn));
        Process.Start(new ProcessStartInfo
        {
            FileName        = response.VerificationUri,
            UseShellExecute = true
        });

        // Wait for user to approve app connection
        OauthToken token = await this.Client.Oauth.CreateAccessTokenForDeviceFlow(CLIENT_ID, response);

        // If failed, dump error and exit
        if (string.IsNullOrEmpty(token.AccessToken))
        {
            await this.LogErrorAsync($"Could not get GitHub OAuth token.\n Error: {token.Error}");
            return null;
        }

        // Store token securely and return it
        this.Vault.AddOrUpdate(SERVICE, PRODUCT, token.AccessToken);
        return this.Vault.Get(SERVICE, PRODUCT);
    }

    /// <summary>
    /// Test the connection to the GitHub client
    /// </summary>
    /// <param name="credentials">Credentials to test for</param>
    /// <returns><see langword="true"/> if the GitHub connection is successful, otherwise <see langword="false"/></returns>
    private async Task<bool> TestConnection(ICredential credentials)
    {
        this.Client.Credentials = new Credentials(credentials.Password, AuthenticationType.Oauth);
        try
        {
            User user = await this.Client.User.Current();
            await this.LogAsync($"Successfully authenticated to the GitHub Client as {user.Login}.");
            return true;
        }
        catch (Exception e)
        {
            this.Vault.Remove(SERVICE, PRODUCT);
            await this.LogErrorAsync("Invalid credentials detected, deleting existing token...");
            await e.LogExceptionAsync();
            return false;
        }
    }
    #endregion
}
