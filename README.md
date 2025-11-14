A BepInEx plugin and an accompanying server mod for SPT (Single Player Tarkov).

![Preview Image MOD](https://raw.githubusercontent.com/swiftxp-hub/spt-show-me-the-money/refs/heads/main/Assets/preview.png)

**Notice:** A quick-sell addon is also available.

___

## Tabs {.tabset}

### SPT 4.0.x
#### What does it do?

The BepInEx plugin modifies the in-game item tooltip to display price information (both in stash and in raid). It shows the trader who offers the best purchase price for the item, as well as the flea market price that targets a 100% sell chance. The display is divided into "price per slot" and "total price" with the best offer highlighted. Flea market taxes can also be included.

Version 1.5.0 and later add a color-coding feature (by default using the familiar World of Warcraft color scheme, ranging from *poor* to *legendary*).

The accompanying server mod provides an endpoint for the BepInEx plugin, which is used to retrieve some server configuration values and flea market prices.

Several configuration options are available through the BepInEx configurator.

The mod is designed to minimize the load on the SPT server.

#### Requirements

Basically none.

#### Installation

##### If you have a completely normal installation of SP-Tarkov (everything inside one folder, on one machine, as the SPT Installer sets it up):

Extract the contents of the `.zip` or `.7z` file into your SPT directory. 

You should end up with the following files copied to your SPT directory:
```
- C:\yourSPTfolder\BepInEx\plugins\com.swiftxp.spt.showmethemoney\SwiftXP.SPT.ShowMeTheMoney.Client.dll
- C:\yourSPTfolder\SPT\user\mods\com.swiftxp.spt.showmethemoney\SwiftXP.SPT.ShowMeTheMoney.Server.dll
```

##### If you have your client and server separated:

Extract the `BepInEx` folder from the `.zip` or `.7z` file to your client, and extract the `user` folder to your server.

You should end up with the following files copied...

... on your client:
```
- C:\yourSPTclient\BepInEx\plugins\com.swiftxp.spt.showmethemoney\SwiftXP.SPT.ShowMeTheMoney.Client.dll
```
... on your server:
```
- C:\yourSPTserver\SPT\user\mods\com.swiftxp.spt.showmethemoney\SwiftXP.SPT.ShowMeTheMoney.Server.dll
```

##### If you use the Fika headless client

There is no need to install anything to your Fika headless client for this mod to work properly. In fact, I recommend **not** installing my mod on your Fika headless client. This also means I recommend adding my mod to your `Exclusions.json` if you use [Corter's Mod Sync](https://github.com/c-orter/ModSync).
[Please see his FAQ on how to add sync exclusions.](https://github.com/c-orter/ModSync/wiki/Configuration#exclusions)

#### Configuration

Please use the BepInEx configurator to adjust the mod’s features (usually accessible by pressing **F12** or **F1** while in-game):

![BepInEx Plugin Configuration](https://raw.githubusercontent.com/swiftxp-hub/spt-show-me-the-money/refs/heads/main/Assets/plugin-configuration.png)

As you can see, you can also manually trigger the plugin to retrieve the current flea market prices from your SPT server. This can be useful if you have installed [DrakiaXYZ's SPT-LiveFleaPrices](https://forge.sp-tarkov.com/mod/1131/live-flea-prices) — however, flea market prices are updated regularly.

Just for clarification: This does not trigger DrakiaXYZ's SPT-LiveFleaPrices to query the latest flea prices. Instead, my mod retrieves the latest flea prices from your SPT server (which are set by SPT-LiveFleaPrices).

#### Remarks

- Changes to trader price markups (e.g., editable via SVM) are taken into account by this mod.
- I have tried to support multiple languages, but if you notice any problems, please let me know in the comments. Disabling the color-coding feature may help in the meantime.

#### Known compatibility

- [SPT-LiveFleaPrices](https://forge.sp-tarkov.com/mod/1131/live-flea-prices) v2.0.1 or newer by DrakiaXYZ  
- [UIFixes](https://forge.sp-tarkov.com/mod/1342/ui-fixes) v5.0.5 or newer by Tyfon  
- [ODT's Item Info - SPT 4.0](https://forge.sp-tarkov.com/mod/2430/odts-item-info-spt-40) v2.0.2 or newer by kobethuy  
  - ODT’s color coding works differently from the color coding in my mod.  
  - ODT will overwrite the color coding of the item name, but the color coding of the prices displayed in the tooltip will remain unchanged.

#### Planned features/changes
- ...

#### Known problems

- A friend of mine has a minor issue with the "Toggle mode for flea tax," where the tooltip sometimes gets “stuck” (the display doesn’t revert when Left-Alt is released). I haven’t been able to figure out the cause yet, but he told me it’s not a big deal.
- Some weapons have suffixes in their names (e.g., “Carbine”). These are not always colored by the color-coding feature.

#### Problems that may occur

- Price information for traders added by other mods may or may not work.
- The "Toggle mode for flea tax" feature is implemented to allow compatibility with other tooltip-modifying mods. However, if you encounter issues, please let me know in the comments.

### SPT 3.11.x
#### What does it do?

The BepInEx plugin modifies the in-game item tooltip to display price information (in stash and in raid). It shows the trader who offers the best purchase price for the item, as well as the average flea market price. The display is divided into "price per slot" and "total price" with the best offer highlighted. Flea market taxes can also be included.  

Version 1.5.0 and later add a color-coding feature (by default using the familiar World of Warcraft color scheme, ranging from *poor* to *legendary*).

The accompanying server mod provides several endpoints for the BepInEx plugin, which are used to retrieve trader prices (in EUR and USD) and flea market prices.

Several configuration options are available through the BepInEx configurator.

The mod is designed to minimize the load on the SPT server.

#### Requirements

Basically none.

#### Installation

##### If you have a completely normal installation of SP-Tarkov (everything inside one folder, on one machine, as the SPT Installer sets it up):

Extract the contents of the `.zip` or `.7z` file into your SPT directory. 

You should end up with the following files copied to your SPT directory:
```
- C:\yourSPTfolder\BepInEx\plugins\SwiftXP.ShowMeTheMoney.dll
- C:\yourSPTfolder\user\mods\swiftxp-showmethemoney\package.json
- C:\yourSPTfolder\user\mods\swiftxp-showmethemoney\src\mod.ts
```

##### If you have your client and server separated:

Extract the `BepInEx` folder from the `.zip` or `.7z` file to your client, and extract the `user` folder to your server.

You should end up with the following files copied...

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

There is no need to install anything to your Fika headless client for this mod to work properly. In fact, I recommend **not** installing my mod on your Fika headless client. This also means I recommend adding my mod to your `Exclusions.json` if you use [Corter's Mod Sync](https://github.com/c-orter/ModSync). [Please see his FAQ on how to add sync exclusions.](https://github.com/c-orter/ModSync/wiki/Configuration#exclusions)

#### Configuration

Please use the BepInEx configurator to adjust the mod’s features (usually accessible by pressing **F12** or **F1** while in-game):

![BepInEx Plugin Configuration](https://raw.githubusercontent.com/swiftxp-hub/spt-show-me-the-money/refs/heads/main-spt311/Assets/plugin-configuration.png)

![Demonstration of toggle mode for flea tax](https://raw.githubusercontent.com/swiftxp-hub/spt-show-me-the-money/refs/heads/main-spt311/Assets/toggle-tax-sample.gif)

As you can see, you can also manually trigger the plugin to retrieve the current flea market prices from your SPT server. This can be useful if you have installed [DrakiaXYZ's SPT-LiveFleaPrices](https://forge.sp-tarkov.com/mod/1131/live-flea-prices) — however, flea market prices are updated regularly.

Just for clarification: This does not trigger DrakiaXYZ's SPT-LiveFleaPrices to query the latest flea prices. Instead, my mod retrieves the latest flea prices from your SPT server (which are set by SPT-LiveFleaPrices).

#### Remarks

- Changes to trader price markups (e.g., editable via SVM) are taken into account by this mod.
- I have tried to support multiple languages, but if you notice any problems, please let me know in the comments. Disabling the color-coding feature may help in the meantime.

#### Known compatibility

- [SPT-LiveFleaPrices](https://forge.sp-tarkov.com/mod/1131/live-flea-prices) v1.5.2 by DrakiaXYZ  
- [More Checkmarks](https://forge.sp-tarkov.com/mod/861/morecheckmarks) v1.5.17 by TommySoucy  
- [UIFixes](https://forge.sp-tarkov.com/mod/1342/ui-fixes) v4.2.2 by Tyfon

#### Known problems

- The condition of an item is not taken into account for the flea market price. This means the price is always displayed as if the item were in perfect condition.  
- Armor plates are not taken into account for the flea market price.  
- The damage report/health condition screen may rarely show the price information tooltip. I'm almost tempted not to fix it — it’s too funny.  
- A friend of mine has a minor issue with the "Toggle mode for flea tax," where the tooltip sometimes gets “stuck” (the display doesn’t revert when Left-Alt is released). I haven’t been able to figure out the cause yet, but he told me it’s not a big deal.  
- Some weapons have suffixes in their names (e.g., “Carbine”). These are not always colored by the color-coding feature.

#### Problems that may occur

- Price information for traders added by other mods may or may not work (e.g., [Couturier](https://forge.sp-tarkov.com/mod/2239/couturier-gear-and-clothing-pack) v1.2.0 by turbodestroyer seems to work fine).  
- The "Toggle mode for flea tax" feature is implemented to allow compatibility with other tooltip-modifying mods. However, if you encounter issues, please let me know in the comments.

#### Support for 3.11.x

I will try to continue supporting SPT 3.11.x as long as the SPT team supports this version. However, I cannot promise this 100%. Support will be limited to bug fixes and minor adjustments.

### FAQ

**Last update of the FAQ:** November 14, 2025  
**FAQ SMTM version:** 2.5.0

- **How do I install SMTM?**  
  - Simply extract the two folders from the `.zip` or `.7z` file into the folder where you installed SPT. A more detailed explanation can be found in the mod's description.

- **How do I configure SMTM?**  
  - Press F12 or F1. This will open the BepInEx Configurator, where you should find SMTM and all its configurable features.

- **Is there a quick-sell addon?**
  - Yes, it is. Please have a look at the "Addon" tab on the mod page.

- **No price information is shown in the tooltip. What can I do?**  
  - Double-check that SMTM is installed correctly. If you're unsure, reinstall it. Check the mod's description for detailed installation instructions.  
  - Avoid using WinRAR to extract `.7z` files. It has been known to occasionally corrupt extracted files.

- **No flea market prices are shown in the tooltip. What can I do?**  
  - By default, flea market prices are only visible once you’ve unlocked the flea market (level requirement).  
    - You can force SMTM to always display flea market prices by enabling the "Always show flea market price" option in the BepInEx Configurator.  
  - Double-check that SMTM is installed correctly. If flea market prices aren’t showing, it usually indicates that the server component of SMTM is not installed correctly. Refer to the mod's description for installation instructions.

- **I get SSL connection errors with this mod. How can I fix this?**
  - While it's not impossible, it's highly unlikely that SMTM is responsible for these SSL connection errors. Tarkov's SSL connection issues can have many causes — Live Tarkov experiences these problems as well. Unfortunately, the potential causes are so varied that they go beyond the scope of support I can provide in this FAQ. The SPT Discord might be a better place to seek help.

- **The flea market prices displayed may differ slightly from the actual average flea market prices. How can I fix this?**  
  - Some degree of inaccuracy is to be expected. There are several reasons for this:
    - SMTM shows the flea market price aimed at achieving a 100% sell chance, not necessarily the average price. In SPT 4.0.4 and SMTM 2.5.0, the achievable flea market price is often slightly higher than the average price.
    - SMTM is designed to keep the load on the SPT server low. As such, only one request is sent every 5 minutes (and only when not in raid) to update the average flea market prices that SMTM uses to calculate the displayed price. A lot can happen in those 5 minutes that influences the flea market price: new offers are created by the SPT server, offers expire, you buy or sell items on the flea market, or mods like "Live Flea Prices" update the price database. These many factors can cause discrepancies in the displayed flea prices. While it’s not perfect, experience shows that SMTM’s calculations are usually accurate enough.
    - The “Live Flea Prices” mod can sometimes cause sharp fluctuations in prices. However, the mod itself is not to blame. The live flea market can often be unpredictable, especially when BSG makes changes that suddenly cause previously unwanted items to become highly sought after. Similarly, trolls can also influence the live flea market. These fluctuations can sometimes be reflected in the offers created by the SPT server. SMTM is designed to ensure that the displayed price still represents a 100% chance of sale, but this is not always guaranteed.

- **The displayed flea market prices differ greatly from what I would expect. Why?**
  - First, please check your settings for SMTM, SPT/SVM, and other mods that may influence prices.
  - If you're sure that SMTM has a problem, please leave a comment. Providing details is greatly appreciated:
    - Which item(s)?
    - What flea market price is displayed?
    - What flea market price did you expect?
    - Screenshots and logs can be very helpful.

- **Armor with installed plates is the same value as without plates. Why?**
  - SPT 4.0.4 does not take plates into account when calculating its chance to buy an offer you created. As a result, you receive the same amount of money for an armor with installed plates as you would for armor without plates. This behavior is mirrored by SMTM. Ergo, the same flea market price is displayed for both armors. However, SMTM 2.5.0 introduced the feature to display the value of the installed armor plates. It is expected that SPT 4.0.5 or later will take plates into account, and I will amend SMTM to reflect this change.

- **Remaining uses of keys don’t appear to affect the flea market price. Why?**  
  - SPT 4.0.4 has a bug in how it calculates the sell chance related to a key’s remaining uses. As a result, a key with only 1 out of 40 uses left sells for the same amount on the flea market as a fully unused (40/40) key. SMTM mirrors this behavior when displaying the flea market price.

{.endtabset}

___

##### Support and feature requests

Please note that I maintain all my mods in my spare time. Therefore, I can only invest a limited amount of time, especially when it comes to support requests.

##### Motivation behind this mod

My friends and I play SPT in co-op using FIKA. Until now, we had been using another mod to display the best prices for items. Unfortunately, I realized that this mod placed a significant load on our small VServer running the SPT server. A large number of small requests were being sent, which regularly caused severe lag. Our tiny VServer simply couldn’t keep up.

Since I had wanted to create mods for SPT for quite some time, I took this as an opportunity to write a new plugin for us — one designed to put as little strain on the SPT server as possible. After we had used it for a while and were satisfied with its performance, I decided to make it publicly available.

**Shout-out to the SPT team and all other SPT modders out there — you're amazing!**