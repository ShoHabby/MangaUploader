using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MangaUploader.Core.Services;

namespace MangaUploader.Core.ViewModels;

/// <summary>
/// Main window ViewModel
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    #region DI Properties
    /// <summary>
    /// Current GitHub Service
    /// </summary>
    private IGitHubService GitHubService { get; }
    #endregion

    #region Observable Properties
    /// <summary>
    /// Current GitHub authentication status
    /// </summary>
    [ObservableProperty]
    public partial string AuthenticationStatus { get; set; } = "Not Authenticated";
    /// <summary>
    /// Current device flow code
    /// </summary>
    [ObservableProperty]
    public partial string DeviceFlowCode { get; set; } = string.Empty;
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor for AppBuilder
    /// </summary>
    public MainWindowViewModel() : this(null!) { }

    /// <summary>
    /// DI Constructor
    /// </summary>
    /// <param name="gitHubService">Current GitHub Service</param>
    public MainWindowViewModel(IGitHubService gitHubService)
    {
        this.GitHubService =  gitHubService;
        this.GitHubService.OnDeviceFlowCodeAvailable += OnDeviceFlowCodeAvailable;
        this.GitHubService.OnAuthenticationCompleted += OnAuthenticationCompleted;
    }

    /// <summary>
    /// Destructor, clears event bindings
    /// </summary>
    ~MainWindowViewModel()
    {
        this.GitHubService.OnDeviceFlowCodeAvailable -= OnDeviceFlowCodeAvailable;
        this.GitHubService.OnAuthenticationCompleted -= OnAuthenticationCompleted;
    }
    #endregion

    #region Commands
    /// <summary>
    /// Connects to the GitHub Service
    /// </summary>
    [RelayCommand]
    private async Task Connect() => await this.GitHubService.Connect();
    #endregion

    #region Event Listeners
    /// <summary>
    /// Device flow code available event handler
    /// </summary>
    /// <param name="userCode">Current device flow code</param>
    /// <param name="validFor">How long the code is valid for</param>
    private void OnDeviceFlowCodeAvailable(string userCode, TimeSpan validFor) => this.DeviceFlowCode = userCode;

    /// <summary>
    /// Authentication completed event handler
    /// </summary>
    private void OnAuthenticationCompleted()
    {
        this.DeviceFlowCode       = string.Empty;
        this.AuthenticationStatus = "Authenticated!";
    }
    #endregion
}
