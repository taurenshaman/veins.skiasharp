using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Veins.Art.Tools {
  public static class ProcessingSkiaSharp {

    public static void noFill(SKPaint paint) {
      paint.Style = SKPaintStyle.Stroke;
    }

    public static void fill(SKPaint paint, byte r, byte g, byte b, byte a = 0) {
      paint.Style = SKPaintStyle.Fill;
      paint.Color = new SKColor( r, g, b, a );
    }

    public static void fill(SKPaint paint, SKColor color) {
      paint.Style = SKPaintStyle.Fill;
      paint.Color = color;
    }

    public static void noStroke(SKPaint paint) {
      paint.Style = SKPaintStyle.Fill;
    }

    public static void stroke(SKPaint paint, byte r, byte g, byte b, byte a = 0) {
      paint.Style = SKPaintStyle.Stroke;
      paint.Color = new SKColor( r, g, b, a );
    }

    public static void stroke(SKPaint paint, SKColor color) {
      paint.Style = SKPaintStyle.Stroke;
      paint.Color = color;
    }

    public static void strokeAndFill(SKPaint paint, SKColor color) {
      paint.Style = SKPaintStyle.StrokeAndFill;
      paint.Color = color;
    }

    public static void strokeAndFill(SKPaint paint, byte r, byte g, byte b, byte a = 0) {
      paint.Style = SKPaintStyle.StrokeAndFill;
      paint.Color = new SKColor( r, g, b, a );
    }

    public static double radians(double degrees) {
      if (degrees < 0)
        degrees = 0;
      else if (degrees > 360)
        degrees = 360;
      return Math.PI * degrees / 180;
    }


		public static void curveVertices(SKCanvas canvas, SKPaint paint, SKPath path, List<SKPoint> points, float curveTightness) {
      int numVerts = points.Count;
      float s = 1 - curveTightness;
    }


	}

}
