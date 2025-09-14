# Show me the money SPT-mod

A BepInEx plugin and an accompanying server mod for SPT (Single Player Tarkov).

![Preview Image MOD](https://raw.githubusercontent.com/swiftxp-hub/spt-show-me-the-money/refs/heads/main/Assets/preview.png)

## What does it do?

The BepInEx plugin modifies the in-game tooltip of items to display price information (in stash and in raid). The dealer who would buy the item for the best price and a flea market selling price is displayed. The display is divided into "price-per-slot" and "total"-price. The best offer is highlighted. The flea markt price is always the lowest expected profit (please see the "[For nerds](#for-nerds)" section for more details on this). Also flea market taxes can be included (experimental function). Version 1.5.0 also adds a color coding feature (by default the color scheme familiar from WoW, ranging from poor to legendary).

The accompanying server mod provides three endpoints for the BepInEx plugin, which are used to retrieve the trader prices for EUR and USD, the flea market prices and the price ranges set for the SPT-server's flea market.

Several configuration options are offered via the BepInEx configurator.

The mod is written in such a way that the load on the SPT-server is as low as possible. Only when the game is started are the trader prices for EUR and USD, average flea market prices and ranges retrieved from the SPT-server. After that, all calculations are performed on the client, so that the item tooltip should not have any noticeable delay.

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

![BepInEx Plugin Configuration](https://raw.githubusercontent.com/swiftxp-hub/spt-show-me-the-money/refs/heads/main/Assets/plugin-configuration.png)<br />
*(Default configuration with freshly installed mod)*

![Demonstration of toggle-mode for flea tax](https://raw.githubusercontent.com/swiftxp-hub/spt-show-me-the-money/refs/heads/main/Assets/toggle-tax-sample.gif)<br />
*(Demonstration of toggle-mode for flea tax)*

As you can see, you can also manually trigger the plugin to retrieve the current flea market prices from your SPT server (this can be useful when you installed [DrakiaXYZ's SPT-LiveFleaPrices](https://github.com/DrakiaXYZ/SPT-LiveFleaPrices) - however, the flea market prices are retrieved from the SPT-server every time the game is started). 

Just for clarifcation: This does not trigger DrakiaXYZ's SPT-LiveFleaPrices to query the latest flea prices, but my mod queries the latest flea prices from your SPT server (which are set by SPT-LiveFleaPrices).

Please see the "[For nerds](#for-nerds)" section in this readme, if you want to learn more about the "Currency conversion method".

## Remarks

- Changes on the Trader Price Markups (e.g. editable via SVM) are taken into account by this mod.
- The color coding feature is relatively new. Please let me know in the comments, if you are satisfied with this feature.
- The currency conversion feature is relatively new. Please let me know in the comments, if you are satisfied with this feature.
- The toggle-mode for flea tax(es) is relatively new. Please let me know in the comments, if you are satisfied with this feature.
- I have tried to support multiple languages, but if you notice any problems, please let me know in the comments. Disabling the color coding feature may help in the meantime.

## Known compatibility

- [SPT-LiveFleaPrices](https://github.com/DrakiaXYZ/SPT-LiveFleaPrices) v1.5.2 by DrakiaXYZ
- [More Checkmarks](https://github.com/TommySoucy/MoreCheckmarks) v1.5.17 by TommySoucy
- [UIFixes](https://github.com/tyfon7/UIFixes) v4.2.2 by Tyfon

## Known problems

- **Flea market prices:**<br />The display of flea market prices should always be viewed with a degree of caution. The calculations are only theoretical in nature and may give the impression that the actual flea market offers have different prices. In particular, when flea market taxes are included (experimental feature), differences may theoretically arise, especially when presets and packs are put up for sale, as these are not currently taken fully into account in the calculation (I may integrate this in future versions). For more information on the calculations see the "[For nerds](#for-nerds)" section in this readme.

## Problems that may occur

- Price information for traders added by other mods may or may not work. E.g. [Couturier](https://hub.sp-tarkov.com/files/file/2943-couturier-gear-and-clothing-pack/) v1.2.0 by turbodestroyer seems to work fine.
- The "Toggle-mode for flea tax"-feature is implemented in such a way that other mods that modify the tooltip should still work, but still... I can't test every mod out there before releasing new features. If you encounter problems, please let me know in the comments.

## Tested environment

- SPT 3.11.4 (this mod should work with every SPT 3.11 release, but it's not tested except for version 3.11.4)
- EFT 16.1.3.35392

## Support and feature requests

Please note that I maintain all my mods in my spare time. Therefore, I can only invest a limited amount of time, especially when it comes to support requests. The following principle always applies to support requests: No logs, no support. [Please follow this link to the SPT FAQ to find your logs](https://hub.sp-tarkov.com/faq-question/64-where-can-i-find-my-log-files/).

## Features that may come in the future

- Being able to select which flea market value should be displayed: e.g., Lowest, 1/3, Average, Highest
- ​More appearence options
- Quick-sell (as a seperate companion-mod)
- What-ever comes to my mind or by feature-requests in the comments/SPT-discord

## For nerds

Please note that all of the following information may be incomplete or misinterpreted on my part, especially my knowledge or interpretation of how SPT simulates the flea market.

### How does the mod calculate the flea prices?

The SPT-server only has one price for the flea price for each item. To create offers, the SPT server takes this price and generates random offers using a price range. By default, this is 80%-120% (SPT 3.11.x). The mod takes this "average" flea price and the set minimum of the price range and calculates the lowest expected price of the item on the flea market. 

Using the "FP-100 filter absorber" as an example: 
91.400 * 0,8 = ~86.200

This means you always see the value that gives you "a 100% chance of selling" on SPT's virtual flea market.

The "Include flea tax" option then deducts the estimated fee for listing the item on the flea market. Tarkov's own method is used for this. This should actually be very accurate, but as always, it's a little more complicated in reality. That's why I've marked the function in the mod as "Experimental", because I can't guarantee that it will always be 100% accurate. Also fees for presets and packs are not fully considered by the mod at the moment (I may integrate this in future versions).

However, you can also disable the "Include flea tax" option and only enable "Show flea tax." Then the fee will not be deducted in the tooltip, but you will see what you would likely pay in flea tax. However, the orange highlighting, which indicates which sales option yields more profit, may then be incorrect. 

For items consisting of several parts, i.e., armor or weapons, the mod currently only displays the flea price of the base item. Therefore, if the weapon is heavily modded, this has no effect on the flea price display in the tooltip, as the calculation for this would be quite complex (I may integrate this in future versions). Armor cannot be sold anyway when there are still plates installed.

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