# Show me the money SPT-mod

A BepInEx plugin and an accompanying server mod for SPT (Single Player Tarkov).

![Preview Image MOD](https://raw.githubusercontent.com/swiftxp-hub/spt-show-me-the-money/refs/heads/main/Assets/preview.png)

Notice: A quick-sell companion mod is available [here](https://github.com/swiftxp-hub/spt-show-me-the-money-quick-sell).

## What does it do?

The BepInEx plugin modifies the in-game tooltip of items to display price information (in stash and in raid). The dealer who would buy the item for the best price and an average price for the flea market is displayed. The display is divided into "price-per-slot" and "total"-price. The best offer is highlighted. Also flea market taxes can be included. Version >=1.5.0 adds a color coding feature (by default the color scheme familiar from WoW, ranging from poor to legendary).

The accompanying server mod provides several endpoints for the BepInEx plugin, which are used to retrieve the trader prices for EUR and USD and the flea market prices.

Several configuration options are offered via the BepInEx configurator.

The mod is written in such a way that the load on the SPT-server is as low as possible.

## Requirements

Basically none.

## Installation

### If you have a completely normal installation of SP-Tarkov (everything inside one folder, on one machine, like the SPT-Installer is doing it):

Extract the contents of the .zip file into your SPT directory. 

You should end up having the following files copied to your SPT directory:
- C:\yourSPTfolder\BepInEx\plugins\SwiftXP.ShowMeTheMoney.dll
- C:\yourSPTfolder\user\mods\swiftxp-showmethemoney\package.json
- C:\yourSPTfolder\user\mods\swiftxp-showmethemoney\src\mod.ts

### If you have your client and your server separated:

Extract the "BepInEx" folder from the .zip to your client and extract the "user" folder to your server.

You should end up having the following files copied...

... on your client:
- C:\yourSPTclient\BepInEx\plugins\SwiftXP.ShowMeTheMoney.dll

... on your server:
- C:\yourSPTserver\user\mods\swiftxp-showmethemoney\package.json
- C:\yourSPTserver\user\mods\swiftxp-showmethemoney\src\mod.ts

### If you use the Fika headless client

There is no need to install anything to your Fika headless client for this mod to work properly. I would actually recommend to NOT install my mod on your Fika headless client. This also means that I recommend to add my mod to your Exclusions.json, if you use [Corter's Mod Sync](https://github.com/c-orter/ModSync). [Please see his FAQ on how to add sync-exclusions](https://github.com/c-orter/ModSync/wiki/Configuration#exclusions).

## Configuration

Please use the BepInEx configurator to configure features of the mod (usually accessible by pressing F12 or F1 when you are in-game):

![BepInEx Plugin Configuration](https://raw.githubusercontent.com/swiftxp-hub/spt-show-me-the-money/refs/heads/main/Assets/plugin-configuration.png)

![Demonstration of toggle-mode for flea tax](https://raw.githubusercontent.com/swiftxp-hub/spt-show-me-the-money/refs/heads/main/Assets/toggle-tax-sample.gif)

As you can see, you can also manually trigger the plugin to retrieve the current flea market prices from your SPT server (this can be useful when you installed [DrakiaXYZ's SPT-LiveFleaPrices](https://github.com/DrakiaXYZ/SPT-LiveFleaPrices) - however, the flea market prices are updated regulary).

Just for clarifcation: This does not trigger DrakiaXYZ's SPT-LiveFleaPrices to query the latest flea prices, but my mod queries the latest flea prices from your SPT server (which are set by SPT-LiveFleaPrices).

## Remarks

- Changes on the Trader Price Markups (e.g. editable via SVM) are taken into account by this mod.
- I have tried to support multiple languages, but if you notice any problems, please let me know in the comments. Disabling the color coding feature may help in the meantime.

## Known compatibility

- [SPT-LiveFleaPrices](https://github.com/DrakiaXYZ/SPT-LiveFleaPrices) v1.5.2 by DrakiaXYZ
- [More Checkmarks](https://github.com/TommySoucy/MoreCheckmarks) v1.5.17 by TommySoucy
- [UIFixes](https://github.com/tyfon7/UIFixes) v4.2.2 by Tyfon

## Known problems

- The damage report/health condition screen may rarely show the price information tooltip. I'm almost tempted not to fix it. It's too funny.
- A buddy of mine has a minor problem with the "Toggle-mode for flea tax" where the tooltip sometimes gets "stuck"—meaning the display doesn't jump back when he releases Left-Alt. I haven't been able to figure out what's causing the problem yet. But he also told me that he doesn't think it's a big deal.
- Some weapons have suffixes in their names, such as "Carbine." These are not always colored by the color coding feature.

## Problems that may occur

- Price information for traders added by other mods may or may not work. E.g. [Couturier](https://hub.sp-tarkov.com/files/file/2943-couturier-gear-and-clothing-pack/) v1.2.0 by turbodestroyer seems to work fine.
- The "Toggle-mode for flea tax"-feature is implemented in such a way that other mods that modify the tooltip should still work, but still... if you encounter problems, please let me know in the comments.

## Tested environment

- SPT 3.11.4 (this mod should work with every SPT 3.11 release, but it's not tested except for version 3.11.4)
- EFT 16.1.3.35392

## Support and feature requests

Please note that I maintain all my mods in my spare time. Therefore, I can only invest a limited amount of time, especially when it comes to support requests.

## Features that may come in the future

- ​More appearence options
- What-ever comes to my mind or by feature-requests in the comments/SPT-discord

## For nerds

Please note that all of the following information may be incomplete or misinterpreted on my part, especially my knowledge or interpretation of how SPT simulates the flea market.

### How does the mod calculate the flea prices?

I made major changes on the flea price calculation in version 1.6.0. I will rewrite this section when I have time.

### What is the difference between "Handbook" and "Traders"?​

The short answer is (when your SPT-server uses standard settings [v3.11.x]):

    ​Handbook uses $1 = ₽​​125 and €1 = ₽133 for currency conversions
    Traders uses $1 = ₽​​139 and €1 = P153 for currency conversions

The long answer is... when you ask the SPT-server for currency courses then one will get the values from the "handbook.json" in the SPT-database. To be honest, I have no idea where they are used, but I'm sure there's a good reason for them. That's also why I set them as default settings. They are also used as a fallback, if, for some reason, the mod is unable to retrieve the USD or EUR prices from Peacekeeper and Skier when using the “Traders” setting in my mod.

When you use the "Traders" option then the mod tries retrieve the actual prices you have to pay at Peacekeeper and Skier for USD/EUR. As mentioned before, the "Handbook" prices are used as a fallback, if, for some reason, the mod is unable to retrieve the USD or EUR prices from Peacekeeper and Skier.

## Motivation behind this mod

Me and my friends are playing SPT in coop with FIKA. Until now, we have always used a different mod from another modder to display the best prices for items. Unfortunately, I realized that this mod put a lot of strain on our very small VServer we use for the SPT-server. A lot of small requests were being sent to the SPT-server. This regularly led to quite severe lags. Our tiny little VServer was not able to keep up.

Since I had wanted to create mods for SPT for quite some time, I took this as an opportunity to write a new plugin for us with the premise to put as little pressure on the SPT-server as possible. Now that we have been using it for a while, I have decided to make it publicly available.

Shout-out to the SPT-Team and all other SPT modders out there. You're amazing!