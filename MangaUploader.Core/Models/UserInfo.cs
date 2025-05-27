namespace MangaUploader.Core.Models;

/// <summary>
/// GitHub user info
/// </summary>
/// <param name="Login">GitHub username</param>
/// <param name="Email">GitHub primary email address</param>
/// <param name="AvatarURL">GitHub avatar URL</param>
public readonly record struct UserInfo(string Login, string Email, string AvatarURL)
{
    #region Constants
    /// <summary>
    /// Default User object
    /// </summary>
    public static UserInfo Default { get; } = new("Please log in...", string.Empty, "/Assets/maribshohabby.ico");
    #endregion
}
