using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JetBrains.Annotations;
using MangaUploader.Core.Extensions.Tasks;
using MangaUploader.Core.Models;
using MangaUploader.Core.Services;

namespace MangaUploader.Core.ViewModels;

/// <summary>
/// Main window ViewModel
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    #region Constants
    /// <summary>
    /// Avatar picture size
    /// </summary>
    public static double AvatarSize => 32d;
    /// <summary>
    /// Avatar frame size
    /// </summary>
    public static CornerRadius AvatarCornerRadius => new(AvatarSize);
    #endregion

    #region DI Properties
    /// <summary>
    /// Current GitHub Service
    /// </summary>
    private IGitHubService? GitHubService { get; }
    /// <summary>
    /// Current Clipboard service
    /// </summary>
    private IClipboardService? ClipboardService { get; }
    #endregion

    #region Observable Properties
    /// <summary>
    /// Currently authenticated user
    /// </summary>
    [ObservableProperty]
    public partial UserInfo User { get; set; } = UserInfo.Default;
    /// <summary>
    /// Connect button text
    /// </summary>
    public string ConnectButtonText => this.GitHubService?.IsAuthenticated ?? false ? "Logout" : "Login";
    /// <summary>
    /// Current device flow code
    /// </summary>
    [ObservableProperty]
    public partial string DeviceFlowCode { get; set; } = string.Empty;
    /// <summary>
    /// Repositories that can be edited
    /// </summary>
    [ObservableProperty]
    public partial ImmutableArray<RepositoryInfo> Repositories { get; set; } = ImmutableArray<RepositoryInfo>.Empty;
    /// <summary>
    /// Selected repository
    /// </summary>
    [ObservableProperty, NotifyPropertyChangedFor(nameof(SelectedRepoName))]
    public partial RepositoryInfo? SelectedRepository { get; set; }
    /// <summary>
    /// Selected repository name
    /// </summary>
    public string SelectedRepoName => this.SelectedRepository?.Name ?? string.Empty;
    #endregion

    #region Constructors
    /// <summary>
    /// Default constructor for AppBuilder
    /// </summary>
    public MainWindowViewModel() : this(null, null) { }

    /// <summary>
    /// DI Constructor
    /// </summary>
    /// <param name="gitHubService">Current GitHub Service</param>
    /// <param name="clipboardService">Current Clipboard service</param>
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public MainWindowViewModel(IGitHubService? gitHubService, IClipboardService? clipboardService)
    {
        this.ClipboardService = clipboardService;

        if (gitHubService is null) return;

        this.GitHubService = gitHubService;
        this.GitHubService.OnDeviceFlowCodeAvailable += OnDeviceFlowCodeAvailable;

        // If some credentials are saved, try connecting
        if (this.GitHubService.HasSavedCredentials())
        {
            this.ConnectCommand.ExecuteAsync(null).Forget();
        }
    }

    /// <summary>
    /// Destructor, clears event bindings
    /// </summary>
    ~MainWindowViewModel()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (this.GitHubService is null) return;

        this.GitHubService.OnDeviceFlowCodeAvailable -= OnDeviceFlowCodeAvailable;
    }
    #endregion

    #region Commands
    /// <summary>
    /// Connects to the GitHub Service
    /// </summary>
    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task Connect()
    {
        if (this.GitHubService is null) return;

        // If authenticated, disconnect instead
        if (this.GitHubService.IsAuthenticated)
        {
            this.GitHubService.Disconnect();
            this.User = UserInfo.Default;
            OnPropertyChanged(nameof(this.ConnectButtonText));
            return;
        }

        // Try authenticating to client
        this.User = await this.GitHubService.Authenticate() ?? UserInfo.Default;
        this.DeviceFlowCode = string.Empty;
        OnPropertyChanged(nameof(this.ConnectButtonText));

        // Fetch user's repos if authentication was a success
        if (this.GitHubService.IsAuthenticated)
        {
            FetchRepositories().Forget();   // Running in a separate task to clear up the execution on this command
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Fetches the public repositories owned by the authenticated user
    /// </summary>
    private async Task FetchRepositories()
    {
        if (this.GitHubService is not null)
        {
            this.Repositories = await this.GitHubService.FetchPublicRepos() ?? ImmutableArray<RepositoryInfo>.Empty;
        }
    }
    #endregion

    #region Event Listeners
    /// <summary>
    /// Device flow code available event handler
    /// </summary>
    /// <param name="userCode">Current device flow code</param>
    /// <param name="validFor">How long the code is valid for</param>
    private void OnDeviceFlowCodeAvailable(string userCode, TimeSpan validFor)
    {
        this.DeviceFlowCode = userCode;
        this.ClipboardService?.CopyTextToClipboard(userCode).Forget();
    }
    #endregion
}
