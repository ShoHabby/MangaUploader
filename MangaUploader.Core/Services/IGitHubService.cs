using System;
using System.Threading.Tasks;
using Octokit;

namespace MangaUploader.Core.Services;

/// <summary>
/// GitHub service
/// </summary>
public interface IGitHubService
{
    /// <summary>
    /// Device flow code available delegate
    /// </summary>
    /// <param name="userCode">Current device flow user code</param>
    /// <param name="validFor">How long this device code will be valid for</param>
    public delegate void DeviceFlowCodeDelegate(string userCode, TimeSpan validFor);

    /// <summary>
    /// User authentication completed delegate
    /// </summary>
    /// <param name="user">The user that has just authenticated</param>
    public delegate void AuthenticationCompletedDelegate(User user);

    /// <summary>
    /// Callback for device flow code available
    /// </summary>
    event DeviceFlowCodeDelegate? OnDeviceFlowCodeAvailable;

    /// <summary>
    /// Callback for GitHub authentication completed
    /// </summary>
    event AuthenticationCompletedDelegate? OnAuthenticationCompleted;

    /// <summary>
    /// Connect to GitHub client
    /// </summary>
    Task Connect();
}
