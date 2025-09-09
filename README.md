# Show me the money SPT-mod

A BepInEx plugin and an accompanying server mod for SPT (Single Player Tarkov).

## What does it do?

The BepInEx plugin modifies the in-game tooltip of items to display price informations. The dealer who would buy the item for the best price and a flea market selling price is displayed. The display is divided into "Price per slot" and "Total"-price. The best sales offer is highlighted. The flea markt price is always the lowest expected profit (please see the "[For nerds](#for-nerds)" section for more details on this). Also flea market taxes can be included (experimental function).

The accompanying server mod provides two endpoints for the BepInEx plugin, which are used to retrieve the flea market prices and the price ranges set for the SPT-server's flea market.

Several configuration options are offered via the BepInEx configurator.

The mod is written in such a way that the load on the SPT-server is as low as possible. Only when the game is started are the average flea market prices and ranges retrieved from the SPT-server. After that, all calculations are performed on the client, so that the item tooltip should not have any noticeable delay.

## Requirements

Basically none.

## Installation

Extract the contents of the release zip file into your SPT install directory. Done.

## Known compatibility

- [SPT-LiveFleaPrices](https://github.com/DrakiaXYZ/SPT-LiveFleaPrices) v1.5.2 by DrakiaXYZ

## Known problems

The display of flea market prices should always be viewed with a degree of caution. The calculations are only theoretical in nature and may give the impression that the actual flea market offers have different prices. In particular, when flea market taxes are included (experimental feature), differences may theoretically arise, especially when presets and packs are put up for sale, as these are not currently taken fully into account in the calculation (I may integrate this in future versions). For more information on the calculations see the "[For nerds](#for-nerds)" section in this readme.

## Problems that may occur

I developed and tested the mod exclusively with the English version of EFT. It is therefore possible that the mod may not work properly with other translations of EFT.

## Tested environment

- SPT 3.11.4 (this mod should work with every SPT 3.11 release, but it's not tested except for version 3.11.4)
- EFT 16.1.3.35392 (English version)

## Support and feature requests

Please note that I maintain all my mods in my spare time. Therefore, I can only invest a limited amount of time, especially when it comes to support requests. The following principle always applies to support requests: No logs, no support. [Please follow this link to the SPT FAQ to find your logs](https://hub.sp-tarkov.com/faq-question/64-where-can-i-find-my-log-files/).

## For nerds

Please note that all of the following information may be incomplete or misinterpreted on my part, especially my knowledge or interpretation of how SPT simulates the flea market.

### How does the mod calculate the flea prices?

The SPT-server only has one price for the flea price for each item. To create offers, the SPT server takes this price and generates random offers using a price range. By default, this is 80%-120% (SPT 3.11.x). The mod takes this "average" flea price and the set minimum of the price range and calculates the lowest expected price of the item on the flea market. 

Using the "FP-100 filter absorber" as an example: 
332.400 * 0,8 = ~277.000

This means you always see the value that gives you "a 100% chance of selling" on SPT's virtual flea market.

The "Include flea tax" option then deducts the estimated fee for listing the item on the flea market. Tarkov's own method is used for this. This should actually be very accurate, but as always, it's a little more complicated in reality. That's why I've marked the function in the mod as "Experimental", because I can't guarantee that it will always be 100% accurate. Also fees for presets and packs are not fully considered by the mod at the moment (I may integrate this in future versions).

However, you can also disable the "Include flea tax" option and only enable "Show flea tax." Then the fee will not be deducted in the tooltip, but you will see what you would likely pay in flea tax. However, the orange highlighting, which indicates which sales option yields more profit, may then be incorrect. 

For items consisting of several parts, i.e., armor or weapons, the mod currently only displays the flea price of the base item. Therefore, if the weapon is heavily modded, this has no effect on the flea price display in the tooltip, as the calculation for this would be quite complex (I may integrate this in future versions). Armor cannot be sold anyway if there are still plates in it.

## Motivation behind this mod

Me and my friends are playing SPT in coop with FIKA. Until now, we have always used a different mod from another modder to display the best prices for items. Unfortunately, I realized that this mod put a lot of strain on our very small VServer we use for the SPT-server. A lot of small requests were being sent to the SPT-server. This regularly led to quite severe lags. Our tiny little VServer was not able to keep up.

Since I had wanted to create mods for SPT for quite some time, I took this as an opportunity to write a new plugin for us with the premise to put as little pressure on the SPT-server as possible. Now that we have been using it for a while, I have decided to make it publicly available.