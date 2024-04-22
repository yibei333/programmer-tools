using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog;
using SharpDevLib.Extensions.Http;
using NLog.Targets;
using SharpDevLib;
using System.Diagnostics;
using SharpDevLib.Extensions.Encryption;

namespace ProgrammerTools.Extensions;

public static class AppExtension
{
    public static string? LogDirectory { get; private set; }

    public static MauiAppBuilder UseLogging(this MauiAppBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddNLog();
        LogManager
            .Setup()
            .RegisterMauiLog()
            .LoadConfiguration(c =>
            {
                var loglevel = NLog.LogLevel.Info;
#if DEBUG
                loglevel = NLog.LogLevel.Debug;
#endif
                var logfileDirectory = AppDomain.CurrentDomain.BaseDirectory;
#if ANDROID
                var filesDirectory = Android.App.Application.Context.GetExternalFilesDir(null)?.AbsolutePath;
                if (!string.IsNullOrWhiteSpace(filesDirectory)) logfileDirectory = filesDirectory;
#endif
                var logfilePath = logfileDirectory.CombinePath("logs/${shortdate}.log");
                LogDirectory = new FileInfo(logfilePath).Directory?.FullName;
                Debug.WriteLine($"log path:{logfilePath}");
                var fileTarget = new FileTarget
                {
                    Name = "file",
                    FileName = logfilePath,
                    KeepFileOpen = true,
                    ArchiveAboveSize = 1024 * 1024 * 10,//10M
                    ArchiveEvery = FileArchivePeriod.Day,
                    MaxArchiveDays = 30,
                    ArchiveDateFormat = "yyyy-MM-dd"
                };
                c.ForLogger().FilterMinLevel(loglevel).WriteToMauiLog().Targets.Add(fileTarget);
            })
            .GetCurrentClassLogger();
        return builder;
    }

    public static MauiAppBuilder ConfigServices(this MauiAppBuilder builder)
    {
        builder.Services.AddHttp();
        builder.Services.AddEncryption();
        return builder;
    }
}
