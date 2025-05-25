using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using GitCredentialManager;
using MangaUploader.Core.Extensions.Collections;
using MangaUploader.Core.Extensions.Logging;
using MangaUploader.Core.Services;
using Octokit;

using static MangaUploader.Core.Services.IGitHubService;

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
    private static readonly ImmutableArray<string> AppScopes = ["public_repo"];
    #endregion

    #region Fields
    private string deviceFlowCode = string.Empty;
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
    public event AuthenticationFailedDelegate? OnAuthenticationFailed;
    /// <inheritdoc />
    public event AuthenticationCompletedDelegate? OnAuthenticationCompleted;
    #endregion

    #region Properties
    /// <inheritdoc />
    public bool IsAuthenticated { get; private set; }
    #endregion

    #region Methods
    /// <inheritdoc />
    public async Task Connect()
    {
        if (this.IsAuthenticated) return;

        // Try and get credentials from vault
        ICredential? credentials = this.Vault.Get(SERVICE, PRODUCT);
        if (credentials is null)
        {
            // If it fails, request them through Device Flow
            credentials = await RequestAccessToken();
            // If that still fails, quit out
            if (credentials is null)
            {
                OnAuthenticationFailed?.Invoke();
                return;
            }
        }

        // Test authentication with client
        User? user = await TestAuthentication(credentials);
        if (user is null)
        {
            // If it fails, the token may have expired, request a new access token
            credentials = await RequestAccessToken();
            // If that fails, or if the connection test fails again, quit out
            if (credentials is null)
            {
                OnAuthenticationFailed?.Invoke();
                return;
            }

            user = await TestAuthentication(credentials);
            if (user is null)
            {
                OnAuthenticationFailed?.Invoke();
                return;
            }
        }

        // Authentication completed, notify of such
        this.IsAuthenticated = true;
        OnAuthenticationCompleted?.Invoke(new UserData(user.Login, user.Email, user.AvatarUrl));
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
        this.deviceFlowCode = response.UserCode;
        OnDeviceFlowCodeAvailable?.Invoke(response.UserCode, TimeSpan.FromSeconds(response.ExpiresIn));
        await App.GetTopLevel().Launcher.LaunchUriAsync(new Uri(response.VerificationUri));

        // Wait for user to approve app connection
        OauthToken token = await this.Client.Oauth.CreateAccessTokenForDeviceFlow(CLIENT_ID, response);
        this.deviceFlowCode = string.Empty;

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
            this.Vault.Remove(SERVICE, PRODUCT);
            await this.LogErrorAsync("Invalid credentials detected, deleting existing token...");
            await e.LogExceptionAsync();
            return null;
        }
    }

    /// <inheritdoc />
    public void Disconnect()
    {
        this.Client.Credentials = Credentials.Anonymous;
        this.Vault.Remove(SERVICE, PRODUCT);
        this.IsAuthenticated = false;
    }

    /// <inheritdoc />
    public async Task CopyDeviceCodeToClipboard()
    {
        // Make sure a valid device code exists
        if (string.IsNullOrEmpty(this.deviceFlowCode)) return;

        // Set the text in the clipboard
        await App.GetTopLevel().Clipboard!.SetTextAsync(this.deviceFlowCode);
    }
    #endregion
}
