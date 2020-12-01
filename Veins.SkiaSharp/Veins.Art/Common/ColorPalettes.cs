using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Veins.Art.Common {
  public static class ColorPalettes {

    // Ref: https://github.com/aaronpenne/generative_art/blob/43bf8facade5820db95c309a33f5e04dfe5074d0/libraries/site-packages/bug_palette.py
    public static readonly List<SKColor> BugGreenOrange = new List<SKColor>() {
      SKColor.FromHsv(26, 79, 78), // orange
      SKColor.FromHsv(107, 52, 31), // green
      SKColor.FromHsv(74, 62, 51), // weirdgreen
      SKColor.FromHsv(197, 37, 38), // grayblue
      SKColor.FromHsv(39, 52, 87) // yellow
    };
    public static readonly List<SKColor> BugGreenBrown = new List<SKColor>() {
      SKColor.FromHsv(44, 53, 38), // brownish
      SKColor.FromHsv(79, 40, 65), // lightgreen
      SKColor.FromHsv(83, 66, 38), // green
      SKColor.FromHsv(43, 42, 84), // tan
      SKColor.FromHsv(38, 65, 70), // tanbrown
      SKColor.FromHsv(78, 69, 19) // darkgreen
    }; 
    public static readonly List<SKColor> BugRedOrange = new List<SKColor>() {
      SKColor.FromHsv(14, 79, 93), // orange
      SKColor.FromHsv(5, 100, 55), // red
      SKColor.FromHsv(359, 100, 35), // darkred
      SKColor.FromHsv(37, 38, 87) // tan
    };
    // 根据bright salmon的搜索结果（https://en.wikipedia.org/wiki/Salmon_(color)）和python代码[348.7, 50.4, 94.9],  # bright salmon判定：属于HSV数据
    public static readonly List<SKColor> Kbo = new List<SKColor>() {
      SKColor.FromHsv(348.7f, 50.4f, 94.9f), // bright salmon
      SKColor.FromHsv(306.6f, 40.8f, 96.1f), // bright pink
      SKColor.FromHsv(45.7f, 78.8f, 94.1f), // yellow
      SKColor.FromHsv(16.3f, 28.6f, 96.1f), // salmon
      SKColor.FromHsv(358.7f, 75.6f, 94.9f) // red
    };
    public static readonly List<SKColor> Zenburn = new List<SKColor>() {
      SKColor.FromHsv(60, 7, 86), // #dcdccc cream
      SKColor.FromHsv(0, 28, 80), // #cc9393 pink
      SKColor.FromHsv(180, 9, 69), // #9fafaf blue gray
      SKColor.FromHsv(0, 13, 74), // #bca3a3 mauve
      SKColor.FromHsv(24, 31, 100), // #ffcfaf peach
      SKColor.FromHsv(150, 22, 56) // #709080 green
    };
    public static readonly List<SKColor> BugRedBlueGreen = new List<SKColor>() {
      SKColor.FromHsv(60, 23, 95), // cream
      SKColor.FromHsv(5, 75, 69), // red
      SKColor.FromHsv(97, 55, 65), // green
      SKColor.FromHsv(221, 70, 42), // dark blue
      SKColor.FromHsv(162, 100, 58), // aqua?
      SKColor.FromHsv(263, 39, 41), // purple
      SKColor.FromHsv(31, 76, 91) // orange
    };
    public static readonly List<SKColor> BugPurpleBlueBlack = new List<SKColor>() {
      SKColor.FromHsv(234, 46, 67), // light purple
      SKColor.FromHsv(164, 29, 81), // light blue
      SKColor.FromHsv(229, 53, 25), // basically black
      SKColor.FromHsv(188, 49, 76), // blue
      SKColor.FromHsv(240, 37, 69) // purple
    };
    public static readonly List<SKColor> AlbrechtDurer = new List<SKColor>() {
      SKColor.FromHsv(92, 23, 45), // green
      SKColor.FromHsv(79, 21, 65), // green
      SKColor.FromHsv(33, 32, 55), // brown
      SKColor.FromHsv(26, 12, 84), // tan
      SKColor.FromHsv(25, 9, 95) // light tan
    };
    public static readonly List<SKColor> GraysBrowns = new List<SKColor>() {
      SKColor.FromHsv(0, 0, 10), // 
      SKColor.FromHsv(0, 0, 30), // 
      SKColor.FromHsv(0, 0, 50), // 
      SKColor.FromHsv(0, 0, 70), // 
      SKColor.FromHsv(0, 0, 90), // 
      SKColor.FromHsv(30, 54, 45), // 
      SKColor.FromHsv(184, 10, 65) // 
    };
    public static readonly List<SKColor> MonarchButterfly = new List<SKColor>() {
      SKColor.FromHsv(21, 99, 76), // dark orange
      SKColor.FromHsv(260, 33, 11), // black
      SKColor.FromHsv(36, 98, 99), // orange
      SKColor.FromHsv(31, 96, 96), // dark orange
      SKColor.FromHsv(37, 17, 96) // cream
    };

    // Jerin
    public static readonly List<SKColor> Rainbow = new List<SKColor>() {
      SKColors.Red, SKColors.Orange, SKColors.Yellow, SKColors.Green, SKColors.Aqua, SKColors.Blue, SKColors.Purple
    };
    // https://wenku.baidu.com/view/5f5bcdd233d4b14e852468d6.html
    public static readonly List<SKColor> BlueSet = new List<SKColor>() {
      new SKColor(0, 0, 255),
      new SKColor(176, 224, 230),
      new SKColor(65, 105, 225),
      new SKColor(106, 90, 205),
      new SKColor(135, 206, 235),
      new SKColor(0, 255, 255),
      new SKColor(8, 46, 84)
    };
    public static readonly List<SKColor> PurpleSet = new List<SKColor>() {
      new SKColor(160, 32, 240),
      new SKColor(138, 43, 226),
      new SKColor(160, 102, 211),
      new SKColor(153, 51, 250),
      new SKColor(218, 112, 214),
      new SKColor(221, 160, 221)
    };

    public static readonly List<List<SKColor>> AaronPenneBugs = new List<List<SKColor>>() {
      BugGreenOrange, BugGreenBrown, BugRedOrange, Kbo, Zenburn,
      BugRedBlueGreen, BugPurpleBlueBlack, AlbrechtDurer, GraysBrowns, MonarchButterfly,
      // Jerin
      Rainbow, BlueSet, PurpleSet
    };
  }

}
