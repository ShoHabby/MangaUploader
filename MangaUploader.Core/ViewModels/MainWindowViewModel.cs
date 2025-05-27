using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JetBrains.Annotations;
using MangaUploader.Core.Extensions.Logging;
using MangaUploader.Core.Extensions.Tasks;
using MangaUploader.Core.Models;
using MangaUploader.Core.Models.Cubari;
using MangaUploader.Core.Services;

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

    #region DI Properties
    /// <summary>
    /// Current GitHub Service
    /// </summary>
    private IGitHubService? GitHubService { get; }
    /// <summary>
    /// Current Clipboard service
    /// </summary>
    private IClipboardService? ClipboardService { get; }
    /// <summary>
    /// Current Cubari service
    /// </summary>
    private ICubariService? CubariService { get; }
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
    /// Default constructor for AppBuilder
    /// </summary>
    public MainWindowViewModel() : this(null, null, null) { }

    /// <summary>
    /// DI Constructor
    /// </summary>
    /// <param name="gitHubService">Current GitHub Service</param>
    /// <param name="clipboardService">Current Clipboard service</param>
    /// <param name="cubariService">Current Cubari service</param>
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public MainWindowViewModel(IGitHubService? gitHubService, IClipboardService? clipboardService, ICubariService? cubariService)
    {
        this.GitHubService    = gitHubService;
        this.ClipboardService = clipboardService;
        this.CubariService    = cubariService;

        if (this.GitHubService is not null)
        {
            this.GitHubService.OnDeviceFlowCodeAvailable += OnDeviceFlowCodeAvailable;

            // If some credentials are saved, try connecting
            if (this.GitHubService.HasSavedCredentials())
            {
                this.ConnectCommand.Execute(null);
                this.ConnectCommand.NotifyCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Destructor, clears event bindings
    /// </summary>
    ~MainWindowViewModel()
    {
        if (this.GitHubService is not null)
        {
            this.GitHubService.OnDeviceFlowCodeAvailable -= OnDeviceFlowCodeAvailable;
        }
    }
    #endregion

    #region Commands
    /// <summary>
    /// Connects to the GitHub Service
    /// </summary>
    [RelayCommand]
    private async Task Connect()
    {
        if (this.GitHubService is null) return;

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

        // Fetch user's repos if authentication was a success
        if (this.GitHubService.IsAuthenticated)
        {
            FetchRepositories().Forget();   // Running in a separate task to clear up the execution on this command
        }

        this.IsLoading = false;
    }

    /// <summary>
    /// Json serialization/deserialization test
    /// </summary>
    [RelayCommand]
    private void TestSerialization()
    {
        if (this.CubariService is null) return;

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
        if (this.GitHubService is null) return;

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
        if (this.GitHubService is null) { }
        else
        {
            this.IsLoading    = true;
            this.Repositories = await this.GitHubService.FetchPublicRepos() ?? ImmutableArray<RepositoryInfo>.Empty;
            this.IsLoading    = false;
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
