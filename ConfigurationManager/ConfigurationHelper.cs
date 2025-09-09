using System;
using BepInEx.Configuration;
using UnityEngine;

namespace SwiftXP.ShowMeTheMoney.ConfigurationManager;

public static class ConfigurationHelper
{
    public static ConfigEntry<T> BindConfiguration<T>(this ConfigFile
        configFile,
        string section,
        string key,
        T defaultValue,
        string description,
        int order)
    {
        return configFile.Bind(
            section,
            key,
            defaultValue,
            new ConfigDescription(description, null, new ConfigurationManagerAttributes { IsAdvanced = false, Order = order })
        );
    }

    public static void CreateButton(this ConfigFile configFile,
        string configSection,
        string configEntryName,
        string buttonName,
        string description,
        Action @function,
        int? order = null)
    {
        Action<ConfigEntryBase> drawer = (ConfigEntryBase entry) =>
        {
            if (GUILayout.Button(buttonName, GUILayout.ExpandWidth(true)))
            {
                @function();
            }
        };

        ConfigDescription configDescription = new(
            description,
            null,
            new ConfigurationManagerAttributes { Order = order, CustomDrawer = drawer }
        );

        configFile.Bind(configSection, configEntryName, "", configDescription);
    }
}