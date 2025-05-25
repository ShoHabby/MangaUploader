using System;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace MangaUploader.Core.Extensions.Logging;

/// <summary>
/// Logging extension methods
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Logs a message to the console
    /// </summary>
    /// <param name="caller">Caller object</param>
    /// <param name="message">Message to log</param>
    public static void Log(this object caller, string message) => Console.WriteLine($"[{caller.GetType().Name}]: {message}");

    /// <inheritdoc cref="Log"/>
    public static async Task LogAsync(this object caller, string message) => await Console.Out.WriteLineAsync($"[{caller.GetType().Name}]: {message}");

    /// <summary>
    /// Logs an error message to the console
    /// </summary>
    /// <param name="caller">Caller object</param>
    /// <param name="message">Error message to log</param>
    public static void LogError(this object caller, string message) => Console.Error.WriteLine($"[{caller.GetType().Name}]: {message}");

    /// <inheritdoc cref="LogError"/>
    public static async Task LogErrorAsync(this object caller, string message) => await Console.Error.WriteLineAsync($"[{caller.GetType().Name}]: {message}");

    /// <summary>
    /// Logs an exception console
    /// </summary>
    /// <param name="exception">Exception to log</param>
    public static void LogException(this Exception exception) => Console.Error.WriteLine($"[{exception.GetType().Name}]: {exception.Message}\n{exception.StackTrace}");

    /// <inheritdoc cref="LogException"/>
    public static async Task LogExceptionAsync(this Exception exception) => await Console.Error.WriteLineAsync($"[{exception.GetType().Name}]: {exception.Message}\n{exception.StackTrace}");
}
