using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using MangaUploader.Core.Models;

namespace MangaUploader.Core.Services;

/// <summary>
/// GitHub service
/// </summary>
public interface IGitHubService
{
    /// <summary>
    /// GitHub user data
    /// </summary>
    /// <param name="Login">GitHub UserName</param>
    /// <param name="Email">GitHub primary email address</param>
    /// <param name="AvatarURL">GitHub avatar URL</param>
    public readonly record struct UserInfo(string Login, string Email, string AvatarURL);

    #region Delegates
    /// <summary>
    /// Device flow code available delegate
    /// </summary>
    /// <param name="userCode">Current device flow user code</param>
    /// <param name="validFor">How long this device code will be valid for</param>
    public delegate void DeviceFlowCodeDelegate(string userCode, TimeSpan validFor);
    /// <summary>
    /// User authentication failed delegate
    /// </summary>
    public delegate void AuthenticationFailedDelegate();
    /// <summary>
    /// User authentication completed delegate
    /// </summary>
    /// <param name="user">The user that has just authenticated</param>
    public delegate void AuthenticationCompletedDelegate(in UserInfo user);
    /// <summary>
    /// User's repositories fetched delegate
    /// </summary>
    public delegate void RepositoriesFetchedDelegate(ImmutableArray<RepositoryInfo> repositories);
    #endregion

    #region Events
    /// <summary>
    /// Callback for device flow code available
    /// </summary>
    event DeviceFlowCodeDelegate? OnDeviceFlowCodeAvailable;
    /// <summary>
    /// Callback for GitHub authentication failed
    /// </summary>
    event AuthenticationFailedDelegate? OnAuthenticationFailed;
    /// <summary>
    /// Callback for GitHub authentication completed
    /// </summary>
    event AuthenticationCompletedDelegate? OnAuthenticationCompleted;
    /// <summary>
    /// Callback for public repositories fetch completion
    /// </summary>
    event RepositoriesFetchedDelegate? OnRepositoriesFetched;
    #endregion

    #region Properties
    /// <summary>
    /// If the client is currently authenticated
    /// </summary>
    bool IsAuthenticated { get; }
    #endregion

    #region Methods
    /// <summary>
    /// Connect to GitHub client
    /// </summary>
    Task Connect();

    /// <summary>
    /// Copies the device flow code to the clipboard
    /// </summary>
    Task CopyDeviceCodeToClipboard();

    /// <summary>
    /// Disconnect from GitHub client
    /// </summary>
    void Disconnect();
    #endregion
}
