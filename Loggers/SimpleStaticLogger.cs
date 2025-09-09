using System;
using BepInEx.Logging;

namespace SwiftXP.ShowMeTheMoney.Loggers;

public class SimpleStaticLogger
{
    public static SimpleStaticLogger Instance => instance.Value;

    private static readonly Lazy<SimpleStaticLogger> instance = new(() => new SimpleStaticLogger());

    private ManualLogSource logger;

    private SimpleStaticLogger()
    {
        logger = Logger.CreateLogSource($"{MyPluginInfo.PLUGIN_GUID}");
    }

    public void LogDebug(object data)
    {
        Log(LogLevel.Debug, data);
    }

    public void LogError(object data)
    {
        Log(LogLevel.Error, data);
    }

    public void LogInfo(object data)
    {
        Log(LogLevel.Info, data);
    }

    public void Log(LogLevel logLevel, object data)
    {
        logger ??= Logger.CreateLogSource($"{MyPluginInfo.PLUGIN_GUID}");
        logger.Log(logLevel, data);
    }
}