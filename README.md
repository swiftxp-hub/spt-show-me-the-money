A BepInEx plugin and an accompanying server mod for SPT (Single Player Tarkov).

![Preview Image MOD](https://raw.githubusercontent.com/swiftxp-hub/spt-show-me-the-money/refs/heads/main/Assets/preview.png)

Notice: A quick-sell companion mod is available [here](https://forge.sp-tarkov.com/mod/2323/show-me-the-money-quick-sell).

___

## Tabs {.tabset}

### SPT 4.0.x
#### What does it do?

The BepInEx plugin modifies the in-game tooltip of items to display price information (in stash and in raid). The dealer who would buy the item for the best price and an average price for the flea market is displayed. The display is divided into "price-per-slot" and "total"-price. The best offer is highlighted. Also flea market taxes can be included. Version >=1.5.0 adds a color coding feature (by default the color scheme familiar from WoW, ranging from poor to legendary).

The accompanying server mod provides an endpoint for the BepInEx plugin, which is used to retrieve the flea market prices.

Several configuration options are offered via the BepInEx configurator.

The mod is written in such a way that the load on the SPT-server is as low as possible.

#### Requirements

Basically none.

#### Installation

##### If you have a completely normal installation of SP-Tarkov (everything inside one folder, on one machine, like the SPT-Installer is doing it):

Extract the contents of the .zip or .7z file into your SPT directory. 

You should end up having the following files copied to your SPT directory:
```
- C:\yourSPTfolder\BepInEx\plugins\com.swiftxp.spt.showmethemoney\SwiftXP.SPT.ShowMeTheMoney.Client.dll
- C:\yourSPTfolder\SPT\user\mods\com.swiftxp.spt.showmethemoney\SwiftXP.SPT.ShowMeTheMoney.Server.dll
```

##### If you have your client and your server separated:

Extract the "BepInEx" folder from the .zip or .7z to your client and extract the "user" folder to your server.

You should end up having the following files copied...

... on your client:
```
- C:\yourSPTclient\BepInEx\plugins\com.swiftxp.spt.showmethemoney\SwiftXP.SPT.ShowMeTheMoney.Client.dll
```
... on your server:
```
- C:\yourSPTserver\SPT\user\mods\com.swiftxp.spt.showmethemoney\SwiftXP.SPT.ShowMeTheMoney.Server.dll
```

##### If you use the Fika headless client

There is no need to install anything to your Fika headless client for this mod to work properly. I would actually recommend to NOT install my mod on your Fika headless client. This also means that I recommend to add my mod to your Exclusions.json, if you use [Corter's Mod Sync](https://github.com/c-orter/ModSync). [Please see his FAQ on how to add sync-exclusions](https://github.com/c-orter/ModSync/wiki/Configuration#exclusions).

#### Configuration

Please use the BepInEx configurator to configure features of the mod (usually accessible by pressing F12 or F1 when you are in-game):

![BepInEx Plugin Configuration](https://raw.githubusercontent.com/swiftxp-hub/spt-show-me-the-money/refs/heads/main/Assets/plugin-configuration.png)

As you can see, you can also manually trigger the plugin to retrieve the current flea market prices from your SPT server (this can be useful when you installed [DrakiaXYZ's SPT-LiveFleaPrices](https://forge.sp-tarkov.com/mod/1131/live-flea-prices) - however, the flea market prices are updated regulary).

Just for clarifcation: This does not trigger DrakiaXYZ's SPT-LiveFleaPrices to query the latest flea prices, but my mod queries the latest flea prices from your SPT server (which are set by SPT-LiveFleaPrices).

#### Remarks

- Changes on the Trader Price Markups (e.g. editable via SVM) are taken into account by this mod.
- I have tried to support multiple languages, but if you notice any problems, please let me know in the comments. Disabling the color coding feature may help in the meantime.

#### Known compatibility

- [SPT-LiveFleaPrices](https://forge.sp-tarkov.com/mod/1131/live-flea-prices) v2.0.1 or newer by DrakiaXYZ
- [UIFixes](https://forge.sp-tarkov.com/mod/1342/ui-fixes) v5.0.2 or newer by Tyfon

#### Planned features/changes
- Fix bug that the damage report/health condition screen my rarely show the price information tooltip.
- Take the condition of an item into account for the flea market price.
- Take armour plates into account for the flea market price.

#### Known problems

- The condition of an item is not taken into account for the flea market price. This means that the price is always displayed as if the item were in perfect condition.
- Armour plates are not taken into account for the flea market price.
- The damage report/health condition screen may rarely show the price information tooltip. I'm almost tempted not to fix it. It's too funny.
- A buddy of mine has a minor problem with the "Toggle-mode for flea tax" where the tooltip sometimes gets "stuck"—meaning the display doesn't jump back when he releases Left-Alt. I haven't been able to figure out what's causing the problem yet. But he also told me that he doesn't think it's a big deal.
- Some weapons have suffixes in their names, such as "Carbine." These are not always colored by the color coding feature.

#### Problems that may occur

- Price information for traders added by other mods may or may not work.
- The "Toggle-mode for flea tax"-feature is implemented in such a way that other mods that modify the tooltip should still work, but still... if you encounter problems, please let me know in the comments.

### SPT 3.11.x
#### What does it do?

The BepInEx plugin modifies the in-game tooltip of items to display price information (in stash and in raid). The dealer who would buy the item for the best price and an average price for the flea market is displayed. The display is divided into "price-per-slot" and "total"-price. The best offer is highlighted. Also flea market taxes can be included. Version >=1.5.0 adds a color coding feature (by default the color scheme familiar from WoW, ranging from poor to legendary).

The accompanying server mod provides several endpoints for the BepInEx plugin, which are used to retrieve the trader prices for EUR and USD and the flea market prices.

Several configuration options are offered via the BepInEx configurator.

The mod is written in such a way that the load on the SPT-server is as low as possible.

#### Requirements

Basically none.

#### Installation

##### If you have a completely normal installation of SP-Tarkov (everything inside one folder, on one machine, like the SPT-Installer is doing it):

Extract the contents of the .zip or .7z file into your SPT directory. 

You should end up having the following files copied to your SPT directory:
```
- C:\yourSPTfolder\BepInEx\plugins\SwiftXP.ShowMeTheMoney.dll
- C:\yourSPTfolder\user\mods\swiftxp-showmethemoney\package.json
- C:\yourSPTfolder\user\mods\swiftxp-showmethemoney\src\mod.ts
```

##### If you have your client and your server separated:

Extract the "BepInEx" folder from the .zip or .7z to your client and extract the "user" folder to your server.

You should end up having the following files copied...

... on your client:
```
- C:\yourSPTclient\BepInEx\plugins\SwiftXP.ShowMeTheMoney.dll
```

... on your server:
```
- C:\yourSPTserver\user\mods\swiftxp-showmethemoney\package.json
- C:\yourSPTserver\user\mods\swiftxp-showmethemoney\src\mod.ts
```

##### If you use the Fika headless client

There is no need to install anything to your Fika headless client for this mod to work properly. I would actually recommend to NOT install my mod on your Fika headless client. This also means that I recommend to add my mod to your Exclusions.json, if you use [Corter's Mod Sync](https://github.com/c-orter/ModSync). [Please see his FAQ on how to add sync-exclusions](https://github.com/c-orter/ModSync/wiki/Configuration#exclusions).

#### Configuration

Please use the BepInEx configurator to configure features of the mod (usually accessible by pressing F12 or F1 when you are in-game):

![BepInEx Plugin Configuration](https://raw.githubusercontent.com/swiftxp-hub/spt-show-me-the-money/refs/heads/main-spt311/Assets/plugin-configuration.png)

![Demonstration of toggle-mode for flea tax](https://raw.githubusercontent.com/swiftxp-hub/spt-show-me-the-money/refs/heads/main-spt311/Assets/toggle-tax-sample.gif)

As you can see, you can also manually trigger the plugin to retrieve the current flea market prices from your SPT server (this can be useful when you installed [DrakiaXYZ's SPT-LiveFleaPrices](https://forge.sp-tarkov.com/mod/1131/live-flea-prices) - however, the flea market prices are updated regulary).

Just for clarifcation: This does not trigger DrakiaXYZ's SPT-LiveFleaPrices to query the latest flea prices, but my mod queries the latest flea prices from your SPT server (which are set by SPT-LiveFleaPrices).

#### Remarks

- Changes on the Trader Price Markups (e.g. editable via SVM) are taken into account by this mod.
- I have tried to support multiple languages, but if you notice any problems, please let me know in the comments. Disabling the color coding feature may help in the meantime.

#### Known compatibility

- [SPT-LiveFleaPrices](https://forge.sp-tarkov.com/mod/1131/live-flea-prices) v1.5.2 by DrakiaXYZ
- [More Checkmarks](https://forge.sp-tarkov.com/mod/861/morecheckmarks) v1.5.17 by TommySoucy
- [UIFixes](https://forge.sp-tarkov.com/mod/1342/ui-fixes) v4.2.2 by Tyfon

#### Known problems

- The condition of an item is not taken into account for the flea market price. This means that the price is always displayed as if the item were in perfect condition.
- Armour plates are not taken into account for the flea market price.
- The damage report/health condition screen may rarely show the price information tooltip. I'm almost tempted not to fix it. It's too funny.
- A buddy of mine has a minor problem with the "Toggle-mode for flea tax" where the tooltip sometimes gets "stuck"—meaning the display doesn't jump back when he releases Left-Alt. I haven't been able to figure out what's causing the problem yet. But he also told me that he doesn't think it's a big deal.
- Some weapons have suffixes in their names, such as "Carbine." These are not always colored by the color coding feature.

#### Problems that may occur

- Price information for traders added by other mods may or may not work. E.g. [Couturier](https://forge.sp-tarkov.com/mod/2239/couturier-gear-and-clothing-pack) v1.2.0 by turbodestroyer seems to work fine.
- The "Toggle-mode for flea tax"-feature is implemented in such a way that other mods that modify the tooltip should still work, but still... if you encounter problems, please let me know in the comments.

#### Support for 3.11.x

I will try to continue supporting SPT 3.11.x as long as the SPT team also supports this version. However, I cannot promise this 100%. Support will be limited to bug fixes and minor adjustments.

{.endtabset}

___

##### Support and feature requests

Please note that I maintain all my mods in my spare time. Therefore, I can only invest a limited amount of time, especially when it comes to support requests.

##### Motivation behind this mod

Me and my friends are playing SPT in coop with FIKA. Until now, we have always used a different mod from another modder to display the best prices for items. Unfortunately, I realized that this mod put a lot of strain on our very small VServer we use for the SPT-server. A lot of small requests were being sent to the SPT-server. This regularly led to quite severe lags. Our tiny little VServer was not able to keep up.

Since I had wanted to create mods for SPT for quite some time, I took this as an opportunity to write a new plugin for us with the premise to put as little pressure on the SPT-server as possible. Now that we have been using it for a while, I have decided to make it publicly available.

Shout-out to the SPT-Team and all other SPT modders out there. You're amazing!