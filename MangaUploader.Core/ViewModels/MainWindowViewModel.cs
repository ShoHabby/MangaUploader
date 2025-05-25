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
    /// Default avatar file
    /// </summary>
    private const string DEFAULT_AVATAR = "/Assets/maribshohabby.ico";
    /// <summary>
    /// Default username
    /// </summary>
    private const string DEFAULT_NAME = "Please log in...";

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
    #endregion

    #region Observable Properties
    /// <summary>
    /// User's avatar URL
    /// </summary>
    [ObservableProperty]
    public partial string AvatarURL { get; set; } = DEFAULT_AVATAR;
    /// <summary>
    /// GitHub username
    /// </summary>
    [ObservableProperty]
    public partial string Username { get; set; } = DEFAULT_NAME;
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
    public MainWindowViewModel() : this(null) { }

    /// <summary>
    /// DI Constructor
    /// </summary>
    /// <param name="gitHubService">Current GitHub Service</param>
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, Reason = "DI Constructor")]
    public MainWindowViewModel(IGitHubService? gitHubService)
    {
        if (gitHubService is null) return;

        this.GitHubService                           =  gitHubService;
        this.GitHubService.OnDeviceFlowCodeAvailable += OnDeviceFlowCodeAvailable;
        this.GitHubService.OnAuthenticationFailed    += OnAuthenticationFailed;
        this.GitHubService.OnAuthenticationCompleted += OnAuthenticationCompleted;
        this.GitHubService.OnRepositoriesFetched     += OnRepositoriesFetched;

        this.ConnectCommand.ExecuteAsync(null).Forget();
    }

    /// <summary>
    /// Destructor, clears event bindings
    /// </summary>
    ~MainWindowViewModel()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (this.GitHubService is null) return;

        this.GitHubService.OnDeviceFlowCodeAvailable -= OnDeviceFlowCodeAvailable;
        this.GitHubService.OnAuthenticationFailed    -= OnAuthenticationFailed;
        this.GitHubService.OnAuthenticationCompleted -= OnAuthenticationCompleted;
        this.GitHubService.OnRepositoriesFetched     -= OnRepositoriesFetched;
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

        if (this.GitHubService.IsAuthenticated)
        {
            this.GitHubService.Disconnect();
            this.AvatarURL = DEFAULT_AVATAR;
            this.Username  = DEFAULT_NAME;
            OnPropertyChanged(nameof(this.ConnectButtonText));
        }
        else
        {
            await this.GitHubService.Connect();
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
        this.GitHubService!.CopyDeviceCodeToClipboard().Forget();
    }

    /// <summary>
    /// Authentication failed event handler
    /// </summary>
    private void OnAuthenticationFailed() => this.DeviceFlowCode = string.Empty;

    /// <summary>
    /// Authentication completed event handler
    /// </summary>
    private void OnAuthenticationCompleted(in UserInfo user)
    {
        this.Username       = user.Login;
        this.AvatarURL      = user.AvatarURL;
        this.DeviceFlowCode = string.Empty;

        OnPropertyChanged(nameof(this.ConnectButtonText));
    }

    /// <summary>
    /// Repositories fetched event handler
    /// </summary>
    /// <param name="repositories">The fetched repositories for the user</param>
    private void OnRepositoriesFetched(ImmutableArray<RepositoryInfo> repositories) => this.Repositories = repositories;
    #endregion
}
