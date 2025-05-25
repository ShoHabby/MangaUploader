using System;
using System.Threading.Tasks;

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
    public delegate void DeviceFlowCodeDelegate(string userCode, TimeSpan validFor);

    /// <summary>
    /// Callback for device flow code available
    /// </summary>
    event DeviceFlowCodeDelegate OnDeviceFlowCodeAvailable;

    /// <summary>
    /// Callback for GitHub authentication completed
    /// </summary>
    event Action OnAuthenticationCompleted;

    /// <summary>
    /// Connect to GitHub client
    /// </summary>
    Task Connect();
}
