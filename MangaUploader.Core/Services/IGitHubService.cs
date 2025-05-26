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
    #region Delegates
    /// <summary>
    /// Device flow code available delegate
    /// </summary>
    /// <param name="userCode">Current device flow user code</param>
    /// <param name="validFor">How long this device code will be valid for</param>
    public delegate void DeviceFlowCodeDelegate(string userCode, TimeSpan validFor);
    #endregion

    #region Events
    /// <summary>
    /// Callback for device flow code available
    /// </summary>
    event DeviceFlowCodeDelegate? OnDeviceFlowCodeAvailable;
    #endregion

    #region Properties
    /// <summary>
    /// If the client is currently authenticated
    /// </summary>
    bool IsAuthenticated { get; }
    #endregion

    #region Methods
    /// <summary>
    /// Authenticates to the GitHub client
    /// </summary>
    /// <returns>The authenticated user info when successful, or <see langword="null"/> on failure</returns>
    Task<UserInfo?> Authenticate();

    /// <summary>
    /// Fetches the user's public repos
    /// </summary>
    /// <returns>An array of public repos owned by the user</returns>
    Task<ImmutableArray<RepositoryInfo>?> FetchPublicRepos();

    /// <summary>
    /// Disconnect from GitHub client
    /// </summary>
    void Disconnect();
    #endregion
}
