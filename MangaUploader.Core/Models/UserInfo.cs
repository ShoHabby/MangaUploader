namespace MangaUploader.Core.Models;

/// <summary>
/// GitHub user data
/// </summary>
/// <param name="Login">GitHub UserName</param>
/// <param name="Email">GitHub primary email address</param>
/// <param name="AvatarURL">GitHub avatar URL</param>
public readonly record struct UserInfo(string Login, string Email, string AvatarURL)
{
    /// <summary>
    /// Default User object
    /// </summary>
    public static UserInfo Default { get; } = new("Please log in...", string.Empty, "/Assets/maribshohabby.ico");
}
