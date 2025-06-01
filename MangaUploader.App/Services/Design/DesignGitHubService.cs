#if DEBUG
using System.Collections.Immutable;
using MangaUploader.Core.Models;
using MangaUploader.Core.Models.Cubari;
using MangaUploader.Core.Services;

namespace MangaUploader.Services.Design;

/// <summary>
/// Design time GitHub service
/// </summary>
public class DesignGitHubService : IGitHubService
{
    #region Events
    /// <inheritdoc />
    #pragma warning disable CS0067
    public event IGitHubService.DeviceFlowCodeDelegate? OnDeviceFlowCodeAvailable;
    #pragma warning restore CS0067
    #endregion Events

    #region Properties
    /// <inheritdoc />
    public bool IsAuthenticated { get; private set; }
    #endregion

    #region Methods
    /// <inheritdoc />
    public bool HasSavedCredentials() => true;

    /// <inheritdoc />
    public Task<UserInfo?> Authenticate()
    {
        this.IsAuthenticated = true;
        return Task.FromResult<UserInfo?>(new UserInfo("Sho Habby", "mangauploader@shohabby.ca", "/Assets/maribshohabby.ico"));
    }

    /// <inheritdoc />
    public Task<ImmutableArray<RepositoryInfo>?> FetchPublicRepos() => Task.FromResult<ImmutableArray<RepositoryInfo>?>(ImmutableArray.Create(
    // ReSharper disable once RedundantExplicitParamsArrayCreation
    [
        new RepositoryInfo("AAAA", 0L),
        new RepositoryInfo("BBBB", 1L),
        new RepositoryInfo("CCCC", 2L),
        new RepositoryInfo("DDDD", 3L),
        new RepositoryInfo("EEEE", 4L)
    ]));

    /// <inheritdoc />
    public Task<ImmutableArray<MangaFileInfo>> FetchRepoMangaContents(long repositoryId) => Task.FromResult(ImmutableArray.Create(
    [
        new MangaFileInfo("Test.json", string.Empty, repositoryId, new Manga
        {
            Title       = "Title",
            Artist      = "Artist",
            Author      = "Author",
            Description = "Description",
            Cover       = null,
            Chapters    = new SortedDictionary<decimal, Chapter>
            {
                [0m] = new()
                {
                    Title       = "Title",
                    Volume      = 0m,
                    LastUpdated = DateTimeOffset.UnixEpoch.LocalDateTime,
                    Groups      = new OrderedDictionary<Groups, Entry>
                    {
                        [new Groups { Names = ["Group"] }] = new ProxyEntry
                        {
                            Type = ProxyEntry.ProxyType.None,
                            ID   = string.Empty
                        }
                    }
                }
            }
        })
    ]));

    /// <inheritdoc />
    public void Disconnect() => this.IsAuthenticated = false;
    #endregion
}
#endif
