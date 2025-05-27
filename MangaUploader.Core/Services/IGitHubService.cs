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
    /// Checks if the service has saved credentials
    /// </summary>
    /// <returns><see langword="true"/> if some credentials for the user are saved on the machine, otherwise <see langword="false"/></returns>
    bool HasSavedCredentials();

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
    /// Fetches the specified repository and retrieves its manga contents
    /// </summary>
    /// <param name="repositoryId">Repository ID to fetch</param>
    /// <returns>The manga file contents of the repository</returns>
    Task<ImmutableArray<MangaFileInfo>> FetchRepoMangaContents(long repositoryId);

    /// <summary>
    /// Disconnect from GitHub client
    /// </summary>
    void Disconnect();
    #endregion
}
