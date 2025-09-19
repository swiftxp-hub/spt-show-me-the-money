using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SwiftXP.SPT.ShowMeTheMoney.Configuration.Migrations;

public class RenamedBindingsMigration
{
    public bool IsApplicable(ConfigFile configFile)
    {
        try
        {
            Type configFileType = configFile.GetType();
            PropertyInfo orphanedEntriesInfo = configFileType.GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);

            object orphanedEntriesObj = orphanedEntriesInfo.GetValue(configFile);
            if (orphanedEntriesObj is not null)
            {
                Dictionary<ConfigDefinition, string>? orphanedEntries = orphanedEntriesObj as Dictionary<ConfigDefinition, string>;
                if (orphanedEntries is not null)
                {
                    if (orphanedEntries.Any(x => string.Equals(x.Key.Section, "1. Main settings", StringComparison.InvariantCultureIgnoreCase)
                        && string.Equals(x.Key.Key, "Show trader price(s)", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        return true;
                    }
                }
            }
        }
        catch (Exception) { }

        return false;
    }

    public void Migrate(PluginConfiguration pluginConfiguration, ConfigFile configFile)
    {
        Plugin.SimpleSptLogger.LogInfo("Found old plug-in settings. Migrating 1.x.x to 1.5.2...");

        try
        {
            Type configFileType = configFile.GetType();
            PropertyInfo orphanedEntriesInfo = configFileType.GetProperty("OrphanedEntries", BindingFlags.NonPublic | BindingFlags.Instance);

            object orphanedEntriesObj = orphanedEntriesInfo.GetValue(configFile);
            if (orphanedEntriesObj is not null)
            {
                Dictionary<ConfigDefinition, string>? orphanedEntries = orphanedEntriesObj as Dictionary<ConfigDefinition, string>;
                if (orphanedEntries is not null)
                {
                    TryMigrateShowTraderPrices(pluginConfiguration, orphanedEntries);
                    TryMigrateShowFleaPrices(pluginConfiguration, orphanedEntries);

                    TryMigrateIncludeFleaTax(pluginConfiguration, orphanedEntries);
                    TryMigrateShowFleaTax(pluginConfiguration, orphanedEntries);
                    TryMigrateToggleModeForFleaTax(pluginConfiguration, orphanedEntries);
                    TryMigrateToggleModeKey(pluginConfiguration, orphanedEntries);

                    TryRemoveOldUpdateFleaPricesButton(orphanedEntries);

                    configFile.Save();
                }
            }
        }
        catch (Exception exception)
        {
            Plugin.SimpleSptLogger.LogException(exception);
        }
    }

    private void TryMigrateShowTraderPrices(PluginConfiguration pluginConfiguration, Dictionary<ConfigDefinition, string> orphanedEntries)
    {
        KeyValuePair<ConfigDefinition, string> binding
            = orphanedEntries.FirstOrDefault(x => x.Key.Section.Contains("Main settings", StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(x.Key.Key, "Show trader price(s)", StringComparison.InvariantCultureIgnoreCase));

        if (binding.Key != null)
        {
            bool bindingValue = bool.Parse(binding.Value);
            pluginConfiguration.EnableTraderPrices.Value = bindingValue;

            orphanedEntries.Remove(binding.Key);
        }
    }

    private void TryMigrateShowFleaPrices(PluginConfiguration pluginConfiguration, Dictionary<ConfigDefinition, string> orphanedEntries)
    {
        KeyValuePair<ConfigDefinition, string> binding
            = orphanedEntries.FirstOrDefault(x => x.Key.Section.Contains("Main settings", StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(x.Key.Key, "Show flea price(s)", StringComparison.InvariantCultureIgnoreCase));

        if (binding.Key != null)
        {
            bool bindingValue = bool.Parse(binding.Value);
            pluginConfiguration.EnableFleaPrices.Value = bindingValue;

            orphanedEntries.Remove(binding.Key);
        }
    }

    private void TryMigrateIncludeFleaTax(PluginConfiguration pluginConfiguration, Dictionary<ConfigDefinition, string> orphanedEntries)
    {
        KeyValuePair<ConfigDefinition, string> binding
            = orphanedEntries.FirstOrDefault(x => x.Key.Section.Contains("Experimental settings", StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(x.Key.Key, "Include flea tax", StringComparison.InvariantCultureIgnoreCase));

        if (binding.Key != null)
        {
            bool bindingValue = bool.Parse(binding.Value);
            pluginConfiguration.IncludeFleaTax = bindingValue;

            orphanedEntries.Remove(binding.Key);
        }
    }

    private void TryMigrateShowFleaTax(PluginConfiguration pluginConfiguration, Dictionary<ConfigDefinition, string> orphanedEntries)
    {
        KeyValuePair<ConfigDefinition, string> binding
            = orphanedEntries.FirstOrDefault(x => x.Key.Section.Contains("Experimental settings", StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(x.Key.Key, "Show flea tax", StringComparison.InvariantCultureIgnoreCase));

        if (binding.Key != null)
        {
            bool bindingValue = bool.Parse(binding.Value);
            pluginConfiguration.ShowFleaTax = bindingValue;

            orphanedEntries.Remove(binding.Key);
        }
    }

    private void TryMigrateToggleModeForFleaTax(PluginConfiguration pluginConfiguration, Dictionary<ConfigDefinition, string> orphanedEntries)
    {
        KeyValuePair<ConfigDefinition, string> binding
            = orphanedEntries.FirstOrDefault(x => x.Key.Section.Contains("Experimental settings", StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(x.Key.Key, "Toggle-mode for flea tax", StringComparison.InvariantCultureIgnoreCase));

        if (binding.Key != null)
        {
            bool bindingValue = bool.Parse(binding.Value);
            pluginConfiguration.FleaTaxToggleMode.Value = bindingValue;

            orphanedEntries.Remove(binding.Key);
        }
    }

    private void TryMigrateToggleModeKey(PluginConfiguration pluginConfiguration, Dictionary<ConfigDefinition, string> orphanedEntries)
    {
        KeyValuePair<ConfigDefinition, string> binding
            = orphanedEntries.FirstOrDefault(x => x.Key.Section.Contains("Experimental settings", StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(x.Key.Key, "Toggle-mode key", StringComparison.InvariantCultureIgnoreCase));

        if (binding.Key != null)
        {
            KeyboardShortcut bindingValue = KeyboardShortcut.Deserialize(binding.Value);
            pluginConfiguration.FleaTaxToggleKey.Value = bindingValue;

            orphanedEntries.Remove(binding.Key);
        }
    }

    private void TryRemoveOldUpdateFleaPricesButton(Dictionary<ConfigDefinition, string> orphanedEntries)
    {
        KeyValuePair<ConfigDefinition, string> binding
            = orphanedEntries.FirstOrDefault(x => x.Key.Section.Contains("Manual update", StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(x.Key.Key, "Update flea prices", StringComparison.InvariantCultureIgnoreCase));

        if (binding.Key != null)
        {
            orphanedEntries.Remove(binding.Key);
        }
    }
}