using BepInEx;
using SwiftXP.SPT.ShowMeTheMoney.Client.Configuration;
using SwiftXP.SPT.Common.Loggers;
using SPT.Reflection.Patching;
using SwiftXP.SPT.ShowMeTheMoney.Client.Services;
using System.Threading;
using System;
using SwiftXP.SPT.ShowMeTheMoney.Client.Patches;
using SwiftXP.SPT.Common.ConfigurationManager;
using SwiftXP.SPT.ShowMeTheMoney.Client.Data;

namespace SwiftXP.SPT.ShowMeTheMoney.Client;

[BepInPlugin("com.swiftxp.spt.showmethemoney", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.SPT.custom", "4.0.11")]
[BepInProcess("EscapeFromTarkov.exe")]
public class Plugin : BaseUnityPlugin, IDisposable
{

    public static void EnableTooltipUpdatePatch()
    {
        s_tooltipUpdatePatch!.Enable();
    }

    public static void DisableTooltipUpdatePatch()
    {
        s_tooltipUpdatePatch!.Disable();
    }

    private void Awake()
    {
        SimpleSptLogger simpleSptLogger = new(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION);
        PluginConfiguration pluginConfiguration = new(Config, OnForceUpdate);

        PluginContextDataHolder.SetContextInstances(simpleSptLogger, pluginConfiguration);

        _fleaPriceUpdaterCancellationTokenSource = new CancellationTokenSource();
        _partialRagfairConfigServiceCancellationTokenSource = new CancellationTokenSource();

        _ = new PartialRagfairConfigService(simpleSptLogger)
            .GetPartialRagfairConfigAsync(_partialRagfairConfigServiceCancellationTokenSource.Token);

        _fleaPriceUpdaterService = new FleaPriceUpdaterService(simpleSptLogger);
        _ = _fleaPriceUpdaterService.ContinuouslyUpdateFleaPricesAsync(_fleaPriceUpdaterCancellationTokenSource.Token);

        EnablePatches();
    }

    private static void EnablePatches()
    {
        new TraderClassPatch().Enable();

        new EditBuildScreenShowPatch().Enable();
        new EditBuildScreenClosePatch().Enable();

        new GridItemOnPointerEnterPatch().Enable();
        new GridItemOnPointerExitPatch().Enable();

        new InventoryScreenClosePatch().Enable();

        new SimpleTooltipShowPatch().Enable();

        s_tooltipUpdatePatch = new TooltipUpdatePatch();

        if (PluginContextDataHolder.Current.Configuration?.FleaTaxToggleMode.IsEnabled() ?? false)
            EnableTooltipUpdatePatch();
    }

    private void OnForceUpdate()
    {
        if (_fleaPriceUpdaterService != null)
            _fleaPriceUpdaterService.ForceUpdate();
    }

    private void OnDestroy()
    {
        if (_fleaPriceUpdaterCancellationTokenSource != null)
        {
            _fleaPriceUpdaterCancellationTokenSource.Cancel();
            _fleaPriceUpdaterCancellationTokenSource.Dispose();
            _fleaPriceUpdaterCancellationTokenSource = null;
        }

        if (_partialRagfairConfigServiceCancellationTokenSource != null)
        {
            _partialRagfairConfigServiceCancellationTokenSource.Cancel();
            _partialRagfairConfigServiceCancellationTokenSource.Dispose();
            _partialRagfairConfigServiceCancellationTokenSource = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            OnDestroy();
        }
    }

    private CancellationTokenSource? _fleaPriceUpdaterCancellationTokenSource;

    private CancellationTokenSource? _partialRagfairConfigServiceCancellationTokenSource;

    private FleaPriceUpdaterService? _fleaPriceUpdaterService;

    private static ModulePatch? s_tooltipUpdatePatch;
}