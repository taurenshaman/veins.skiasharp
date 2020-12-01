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

    public static SKImage CropSurfaceToImage(SKSurface skSurface, int imageCanvasSize, int destSize, SKPaint paint) {
      SKImage skImage = null;
      var srcImage = skSurface.Snapshot();
      using (var newSurface = SKSurface.Create( new SKImageInfo( destSize, destSize ) )) {
        newSurface.Canvas.DrawImage( srcImage, SKRect.Create( 0, 0, destSize, destSize ), SKRect.Create( 0, 0, destSize, destSize ), paint );
        newSurface.Canvas.Flush();
        skImage = newSurface.Snapshot();
      }
      return skImage;
    }

    public static SKImage ScaleSurfaceToImage(SKSurface skSurface, int imageCanvasSize, int destSize, SKPaint paint) {
      SKImage skImage = null;
      var canvas = skSurface.Canvas;
      // scale
      if (destSize == imageCanvasSize)
        skImage = skSurface.Snapshot();
      else {
        // 先缩放；然后绘制到小图上面，即相当于Crop了
        canvas.Scale( 1.0f * destSize / imageCanvasSize );
        var srcImage = skSurface.Snapshot();

        using (var newSurface = SKSurface.Create( new SKImageInfo( destSize, destSize ) )) {
          newSurface.Canvas.DrawImage( srcImage, SKRect.Create( 0, 0, imageCanvasSize, imageCanvasSize ), SKRect.Create( 0, 0, destSize, destSize ), paint );
          newSurface.Canvas.Flush();
          skImage = newSurface.Snapshot();
        }
      } // scale
      return skImage;
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

    public static void CircleStroke(SKCanvas canvas, SKPaint paint, SKColor strokeColor, float centerX, float centerY, float radius) {
      paint.Style = SKPaintStyle.Stroke;
      paint.Color = strokeColor;
      canvas.DrawCircle( centerX, centerY, radius, paint );
    }

    public static void CircleFill(SKCanvas canvas, SKPaint paint, SKColor fillColor, float centerX, float centerY, float radius) {
      paint.Style = SKPaintStyle.Fill;
      paint.Color = fillColor;
      canvas.DrawCircle( centerX, centerY, radius, paint );
    }

    public static void PathStroke(SKCanvas canvas, SKPaint paint, SKColor strokeColor, SKPath path) {
      paint.Style = SKPaintStyle.Stroke;
      paint.Color = strokeColor;
      canvas.DrawPath( path, paint );
    }

    public static void PathFill(SKCanvas canvas, SKPaint paint, SKColor fillColor, SKPath path) {
      paint.Style = SKPaintStyle.Fill;
      paint.Color = fillColor;
      canvas.DrawPath( path, paint );
    }

    public static void DrawTriangle(SKCanvas canvas, SKPaint paint, SKPoint p1, SKPoint p2, SKPoint p3) {
      var path = new SKPath { FillType = SKPathFillType.EvenOdd };
      path.MoveTo( p1 );
      path.LineTo( p2 );
      path.LineTo( p3 );
      path.LineTo( p1 );
      path.Close();

      canvas.DrawPath( path, paint );
    }

    public static void DrawSimplePath(SKCanvas canvas, SKPaint paint, List<SKPoint> points, bool closePath, SKPathFillType pathFillType) {
      if (points == null || points.Count == 0)// < 4
        return;

      var path = new SKPath() { FillType = pathFillType };
      bool start = true;
      for (int i = 0; i < points.Count; i++) {
        if (start) {
          path.MoveTo( points[i] );
          start = false;
        }
        else
          path.LineTo( points[i] );
      }

      if (closePath)
        path.Close();

      canvas.DrawPath( path, paint );
    }

    public static void DrawSplineCurve(SKCanvas canvas, SKPaint paint, List<SKPoint> points, bool closePath, SKPathFillType pathFillType) {
      if (points == null || points.Count == 0)// < 4
        return;

      var path = new SKPath() { FillType = pathFillType };
      path.MoveTo( points[0] );
      Common.SplineUtility.CreateSpline( path, points.ToArray() );

      if (closePath)
        path.Close();

      canvas.DrawPath( path, paint );
    }

    //public static void DrawPoint(SKCanvas canvas, SKPaint paint, SKPoint point, float raidus) {
    //  canvas.DrawCircle( point, raidus, paint );
    //}

    public static void DrawPoints(SKCanvas canvas, SKPaint paint, List<SKPoint> points) {
      if (points == null || points.Count == 0)// < 4
        return;

      for (int i = 0; i < points.Count; i++) {
        canvas.DrawPoint( points[i], paint );
      }
    }

    public static SKColor CreateRGBColor(byte red, byte green, byte blue) {
      SKColor color = new SKColor(red, green, blue);
      return color;
    }

    public static SKColor CreateRGBAColor(byte red, byte green, byte blue, byte alpha) {
      SKColor color = new SKColor( red, green, blue, alpha );
      return color;
    }

  }

}
