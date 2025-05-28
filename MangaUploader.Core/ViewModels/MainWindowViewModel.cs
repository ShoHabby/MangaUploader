using System.Collections.Immutable;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using MangaUploader.Core.Extensions.Logging;
using MangaUploader.Core.Extensions.Tasks;
using MangaUploader.Core.Models;
using MangaUploader.Core.Models.Cubari;
using MangaUploader.Core.Services;

#if DEBUG
using Avalonia.Controls;
#endif

namespace MangaUploader.Core.ViewModels;

/// <summary>
/// Main window ViewModel
/// </summary>
public sealed partial class MainWindowViewModel : ViewModelBase
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

    #region Services
    /// <summary>
    /// Injected GitHub Service
    /// </summary>
    private IGitHubService GitHubService { get; }
    /// <summary>
    /// Injected Clipboard service
    /// </summary>
    private IClipboardService ClipboardService { get; }
    /// <summary>
    /// Injected Cubari service
    /// </summary>
    private ICubariService CubariService { get; }
    /// <summary>
    /// Injected Application Settings service
    /// </summary>
    private IAppSettingsService AppSettingsService { get; }
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
    public string ConnectButtonText => this.IsAuthenticated ? "Logout" : "Login";
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
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(FetchSelectedRepoCommand))]
    public partial RepositoryInfo? SelectedRepository { get; set; }
    /// <summary>
    /// If a valid repository is selected
    /// </summary>
    public bool HasSelectedRepository => this.SelectedRepository is not null;
    /// <summary>
    /// If the GitHub client is currently authenticated
    /// </summary>
    [ObservableProperty, NotifyPropertyChangedFor(nameof(ConnectButtonText))]
    public partial bool IsAuthenticated { get; set; }
    /// <summary>
    /// If we're currently loading some data
    /// </summary>
    [ObservableProperty]
    public partial bool IsLoading { get; set; }
    #endregion

    #region Constructors
    /// <summary>
    /// ViewModel constructor
    /// </summary>
    public MainWindowViewModel()
    {
        #if DEBUG
        if (Design.IsDesignMode)
        {
            // Suppress warnings
            this.GitHubService      = null!;
            this.ClipboardService   = null!;
            this.CubariService      = null!;
            this.AppSettingsService = null!;
            return;
        }
        #endif

        // Fetch services
        this.GitHubService      = Ioc.Default.GetRequiredService<IGitHubService>();
        this.ClipboardService   = Ioc.Default.GetRequiredService<IClipboardService>();
        this.CubariService      = Ioc.Default.GetRequiredService<ICubariService>();
        this.AppSettingsService = Ioc.Default.GetRequiredService<IAppSettingsService>();

        // Subscribe to Oauth flow event
        this.GitHubService.OnDeviceFlowCodeAvailable += OnDeviceFlowCodeAvailable;

        // If some credentials are saved, try connecting
        if (this.GitHubService.HasSavedCredentials())
        {
            this.ConnectCommand.Execute(null);
            this.ConnectCommand.NotifyCanExecuteChanged();
        }
    }

    /// <summary>
    /// Destructor, clears event bindings
    /// </summary>
    ~MainWindowViewModel()
    {
        #if DEBUG
        if (Design.IsDesignMode) return;
        #endif

        this.GitHubService.OnDeviceFlowCodeAvailable -= OnDeviceFlowCodeAvailable;
    }
    #endregion

    #region Commands
    /// <summary>
    /// Connects to the GitHub Service
    /// </summary>
    [RelayCommand]
    private async Task Connect()
    {
        #if DEBUG
        if (Design.IsDesignMode) return;
        #endif

        // If authenticated, disconnect instead
        if (this.GitHubService.IsAuthenticated)
        {
            this.GitHubService.Disconnect();
            this.User            = UserInfo.Default;
            this.IsAuthenticated = false;
            return;
        }

        // Try authenticating to client
        this.IsLoading       = true;
        this.User            = await this.GitHubService.Authenticate() ?? UserInfo.Default;
        this.DeviceFlowCode  = string.Empty;
        this.IsAuthenticated = this.GitHubService.IsAuthenticated;
        this.IsLoading       = false;

        // Fetch user's repos if authentication was a success
        if (this.GitHubService.IsAuthenticated)
        {
            FetchRepositories().Forget();   // Running in a separate task to clear up the execution on this command
        }
    }

    /// <summary>
    /// Json serialization/deserialization test
    /// </summary>
    [RelayCommand]
    private void TestSerialization()
    {
        #if DEBUG
        if (Design.IsDesignMode) return;
        #endif

        string data = File.ReadAllText("Testing/Test.json");
        Manga? manga = this.CubariService.DeserializeManga(data);
        if (manga is null) return;

        string? newData = this.CubariService.SerializeManga(manga);
        if (newData is null) return;

        File.WriteAllText("Testing/SerializedTest.json", newData);
    }

    /// <summary>
    /// Fetches the data from the selected repository
    /// </summary>
    [RelayCommand(CanExecute = nameof(HasSelectedRepository))]
    private async Task FetchSelectedRepo()
    {
        #if DEBUG
        if (Design.IsDesignMode) return;
        #endif

        this.IsLoading = true;
        ImmutableArray<MangaFileInfo> mangas = await this.GitHubService.FetchRepoMangaContents(this.SelectedRepository!.Value.ID);
        foreach (MangaFileInfo manga in mangas)
        {
            await this.LogAsync($"File Path: {manga.Path} SHA: {manga.SHA} Repository ID: {manga.RepositoryID} Title: {manga.Manga.Title}");
        }
        this.IsLoading = false;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Fetches the public repositories owned by the authenticated user
    /// </summary>
    private async Task FetchRepositories()
    {
        this.IsLoading    = true;
        this.Repositories = await this.GitHubService.FetchPublicRepos() ?? ImmutableArray<RepositoryInfo>.Empty;
        this.IsLoading    = false;

        if (!string.IsNullOrEmpty(this.AppSettingsService.Settings.SavedRepositoryName))
        {
            this.SelectedRepository = this.Repositories.FirstOrDefault(r => r.Name == this.AppSettingsService.Settings.SavedRepositoryName);
            if (this.SelectedRepository is not null)
            {
                this.FetchSelectedRepoCommand.Execute(null);
                this.FetchSelectedRepoCommand.NotifyCanExecuteChanged();
            }
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
        this.ClipboardService.CopyTextToClipboard(userCode).Forget();
    }
    #endregion
}
