using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Veins.Art.Tools {
  public static class SkiaSharpUtility {

    public static SKPaint CreateDefaultPaint() {
      var paint = new SKPaint();
      paint.IsAntialias = true;
      paint.FilterQuality = SKFilterQuality.High;
      return paint;
    }

    public static SKData EncodeImageToSKData(SKImage skImage, string format, int quality = 90) {
      SKEncodedImageFormat skFormat = SKEncodedImageFormat.Png;
      switch (format) {
        case "jpg":
          skFormat = SKEncodedImageFormat.Jpeg;
          break;
        case "jpeg":
          skFormat = SKEncodedImageFormat.Jpeg;
          break;
        case "gif":
          skFormat = SKEncodedImageFormat.Gif;
          break;
        case "bmp":
          skFormat = SKEncodedImageFormat.Bmp;
          break;
        case "webp":
          skFormat = SKEncodedImageFormat.Webp;
          break;
        default:
          skFormat = SKEncodedImageFormat.Png;
          break;
      }
      SKData skData = skImage.Encode( skFormat, quality );
      return skData;
    }

    public static void DrawImageToCanvas(SKCanvas canvas, string imgPath, SKRect dest, SKPaint paint = null) {
      var skBitmap = SKBitmap.Decode( imgPath );
      canvas.DrawBitmap( skBitmap, SKRect.Create( 0, 0, skBitmap.Width, skBitmap.Height ), dest, paint );
      skBitmap.Dispose();
    }

  }

}
