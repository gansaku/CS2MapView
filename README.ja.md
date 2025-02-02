# CS2MapView
[![en](https://img.shields.io/badge/lang-en-red.svg)](https://github.com/gansaku/CS2MapView/blob/master/README.md)
[![ja](https://img.shields.io/badge/lang-ja-jp.svg)](https://github.com/gansaku/CS2MapView/blob/master/README.ja.md)

Cities:Skylines IIの地図を描画するプログラムです。
[CSLMapView](https://steamcommunity.com/sharedfiles/filedetails/?id=845665815) みたいなものです。

## 動作環境
* Windows 64bit
* [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0 ".NET 8 Desktop Runtime")

## インストール
1. Paradox mods で CS2MapView.Exporter MODを導入してください。
2. [Github](https://github.com/gansaku/CS2MapView/releases/)からビューアをダウンロードして、任意の場所に展開してください。CSLMapViewのようにビューアを同梱したりはしていません。

 
## 使い方
1. ゲーム開始後、modのオプション画面を開き、「エクスポート」ボタンを押すとcs2mapview.exeで読み込めるマップファイルが出力されます。出力先はオプション画面で指定できます。
2. cs2mapview.exeを起動し、エクスポートしたファイルを開いてください。 MODからcs2mapview.exeを起動する機能はありません。

## 操作方法
* Mouse Wheel : 拡大縮小
* Ctrl + Wheel : 回転
* Left Drag : スクロール

## その他
* CSLMapViewのファイル読み込みが可能です（交通機関のルートなど一部機能を除く）