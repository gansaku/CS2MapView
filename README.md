# CS2MapView
[![en](https://img.shields.io/badge/lang-en-red.svg)](https://github.com/gansaku/CS2MapView/blob/master/README.md)
[![ja](https://img.shields.io/badge/lang-ja-jp.svg)](https://github.com/gansaku/CS2MapView/blob/master/README.ja.md)

This program draws maps of Cities:Skylines II like [CSLMapView](https://steamcommunity.com/sharedfiles/filedetails/?id=845665815).

## Requirement
* Windows 64bit
* [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0 ".NET 8 Desktop Runtime")

## Installation
1. Download the latest packages from [Releases](https://github.com/gansaku/CS2MapView/releases/).
2. ~~Install CS2MapView.Exporter mod in Paradox mods.~~<br>
Download and unzip the mod (CS2MapView.Exporter_x.x.x.zip) into the mod directory of the game. <br>
C:\Users\\(username)\AppData\LocalLow\Colossal Order\Cities Skylines II\Mods\CS2MapView.Exporter<br>
Please see [issues/3](https://github.com/gansaku/CS2MapView/issues/3) for more information.
3. Download and unzip the viewer (cs2apview_x.x.x.zip) to a location of your choice.

 
## Usage
1. After starting the game, open the mod's options and click the Export button to output a map file that can be read by cs2mapview.exe. The output destination can be specified on the options.
2. Start cs2mapview.exe and open the exported file. There is no function to launch cs2mapview.exe from the mod.

## Operation
* Mouse Wheel : zoom
* Ctrl + Wheel : rotate
* Left Drag : map scroll

## Miscellaneous
* CSLMapView format files (*.cslmap,*.cslmap.gz) can be loaded (except for some functions such as transportation routes)
