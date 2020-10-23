using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using Veins.Art.Common;
using Veins.Art.Tools;

namespace Veins.Art.Livings {
  // Ref: https://github.com/erdavids/Birds-of-a-Feather
  public class GeometricBird {
    const int ImageCanvasSize = 1000;
    const int w = ImageCanvasSize,
      h = ImageCanvasSize;
    // The birds will draw inside this rectangle
    const float grid_x_pixels = w * 0.8f,
      grid_y_pixels = h * 0.8f;

    // Background Color
    SKColor bc = SKColors.White;
    List<SKColor> colors = new List<SKColor>();

    const int feet_length = 40;
    const int body_height = 100;
    const int line_thickness = 7;

    const double body_fill_chance = .3;
    const double head_fill_chance = .3;
    const double tail_chance = .3;
    const double arc_chance = .4;

    const int min_shape_lines = 1;
    const int max_shape_lines = 5;

    float head_x = 0;
    float head_y = 0;
    const int head_size = 90;
    const int head_radius = head_size / 2;

    SKPaint paint;

    public GeometricBird() {
      colors.Add( SkiaSharpUtility.CreateRGBColor( 189, 208, 196 ) );
      colors.Add( SkiaSharpUtility.CreateRGBColor( 154, 183, 211 ) );
      colors.Add( SkiaSharpUtility.CreateRGBColor( 245, 210, 211 ) );
      colors.Add( SkiaSharpUtility.CreateRGBColor( 247, 225, 211 ) );
      colors.Add( SkiaSharpUtility.CreateRGBColor( 223, 204, 241 ) );

      paint = new SKPaint {
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
        StrokeJoin = SKStrokeJoin.Round,
        StrokeWidth = line_thickness
      };
    }

    public Stream Assemble(int size = 256) {
      //SKCanvas canvas = new SKCanvas()
      SKPath path = new SKPath();
      Stream stream = null;
      SKImage skImage = null;
      try {
        using (var skSurface = SKSurface.Create( new SKImageInfo( w, h ) )) {
          var canvas = skSurface.Canvas;
          canvas.Clear( bc ); //set background color
          // draw ...
          draw( canvas );

          canvas.Flush();

          //skImage = skSurface.Snapshot();
          SKPaint paintConvert = SkiaSharpUtility.CreateDefaultPaint();
          skImage = SkiaSharpUtility.CropSurfaceToImage( skSurface, ImageCanvasSize, size, paintConvert );
        }
        // encode
        SKData skData = SkiaSharpUtility.EncodeImageToSKData( skImage, "png" );

        stream = skData.AsStream();
      }
      catch (Exception ex) {
        Console.WriteLine( ex.Message );
      }
      finally {
        if (skImage != null)
          skImage.Dispose();
        paint.Dispose();
      }
      return stream;
    }

    void draw_lines(SKCanvas canvas, List<SKPoint> point_list, SKColor color) {
      var p1 = RandomUtility.PopRandomElement( point_list );
      var p2 = RandomUtility.PopRandomElement( point_list );
      var p3 = RandomUtility.PopRandomElement( point_list );

      int lines = RandomUtility.CreateRandom( min_shape_lines, max_shape_lines );

      float first_x_adj = 1;
      if (p3.X - p1.X == 0)
        first_x_adj = 1;
      else
        first_x_adj = ( p3.X - p1.X ) / Math.Abs( p3.X - p1.X );

      float first_y_adj = 1;
      if (p3.Y - p1.Y == 0)
        first_y_adj = 1;
      else
        first_y_adj = ( p3.Y - p1.Y ) / Math.Abs( p3.Y - p1.Y );

      double first_x_sep = Math.Sqrt( Math.Pow( p1.X - p3.X, 2 ) ) / lines * first_x_adj;
      double first_y_sep = Math.Sqrt( Math.Pow( p1.Y - p3.Y, 2 ) ) / lines * first_y_adj;

      float second_x_adj = 1;
      if (p3.X - p2.X == 0)
        second_x_adj = 1;
      else
        second_x_adj = ( p3.X - p2.X ) / Math.Abs( p3.X - p2.X );

      float second_y_adj = 1;
      if (p3.Y - p2.Y == 0)
        second_y_adj = 1;
      else
        second_y_adj = ( p3.Y - p2.Y ) / Math.Abs( p3.Y - p2.Y );

      double second_x_sep = Math.Sqrt( Math.Pow( p2.X - p3.X, 2 ) ) / lines * second_x_adj;
      double second_y_sep = Math.Sqrt( Math.Pow( p2.Y - p3.Y, 2 ) ) / lines * second_y_adj;

      paint.Color = color;
      for (int i = 0; i < lines; i++) {
        float x1 = (float)( p1.X + first_x_sep * i );
        float y1 = (float)( p1.Y + first_y_sep * i );
        float x2 = (float)( p2.X + second_x_sep * i );
        float y2 = (float)( p2.Y + second_y_sep * i );
        canvas.DrawLine( x1, y1, x2, y2, paint );
      }
    }

    void draw_bird_legs(SKCanvas canvas, float x, float y) {
      paint.StrokeCap = SKStrokeCap.Round;
      paint.Color = SkiaSharpUtility.CreateRGBColor( 0, 0, 0 );

      canvas.DrawLine( x - feet_length, y, x + feet_length, y, paint );
      canvas.DrawLine( x - feet_length / 3.0f, y, x - feet_length / 3.0f - feet_length / 2.0f, y - feet_length, paint );
      canvas.DrawLine( x + feet_length / 3.0f, y, x + feet_length / 3.0f - feet_length / 2.0f, y - feet_length, paint );
    }

    SKPoint draw_bird_body(SKCanvas canvas, float x, float y, SKColor pc, SKColor bc, SKColor dc) {
      float body_bottom = y - feet_length / 2.0f;

      SKPoint body_one = new SKPoint( x - feet_length * 2.0f, body_bottom );
      SKPoint body_two = new SKPoint( x + feet_length * 1.5f, body_bottom );
      SKPoint body_three = new SKPoint( x + feet_length * 2.1f, body_bottom - body_height );
      SKPoint body_four = new SKPoint(x, body_bottom - body_height * 1.3f );

      SKPoint left_midpoint = new SKPoint( ( body_four.X + body_one.X ) / 2, ( body_four.Y + body_one.Y ) / 2 );
      SKPoint top_midpoint = new SKPoint( ( body_four.X + body_three.X ) / 2, ( body_four.Y + body_three.Y ) / 2);
      SKPoint right_midpoint = new SKPoint( ( body_two.X + body_three.X ) / 2, ( body_two.Y + body_three.Y ) / 2 );
      SKPoint bottom_midpoint = new SKPoint( ( body_one.X + body_two.X ) / 2, ( body_one.Y + body_two.Y ) / 2 );

      //SKPoint true_midpoint = new SKPoint( ( left_midpoint.X + right_midpoint.X ) / 2, ( left_midpoint.Y + right_midpoint.Y ) / 2 );

      List<SKPoint> body_points = new List<SKPoint>() { body_one, body_three, body_four, left_midpoint, top_midpoint, bottom_midpoint };

      var pathBody = new SKPath { FillType = SKPathFillType.EvenOdd };
      pathBody.MoveTo( body_one );
      pathBody.LineTo( body_two );
      pathBody.LineTo( body_three );
      pathBody.LineTo( body_four );
      pathBody.Close();

      SkiaSharpUtility.PathStroke( canvas, paint, dc, pathBody );
      SkiaSharpUtility.PathFill( canvas, paint, bc, pathBody );

      int range = RandomUtility.CreateRandom( 1, 4 );
      for(int i = 0; i < range; i++) {
        var point_one = RandomUtility.GetRandomElement( body_points );
        var point_two = RandomUtility.GetRandomElement( body_points );
        var point_three = RandomUtility.GetRandomElement( body_points );
        //var point_four = RandomUtility.GetRandomElement( body_points );

        var path = new SKPath { FillType = SKPathFillType.EvenOdd };
        path.MoveTo( point_one );
        path.LineTo( point_two );
        path.LineTo( point_three );
        path.Close();

        SkiaSharpUtility.PathStroke( canvas, paint, dc, path );
        SkiaSharpUtility.PathFill( canvas, paint, pc, path );

        if (RandomUtility.CreateRandom() < 0.5) {
          draw_lines( canvas, body_points, dc );
        }
      } // for
      head_x = x + feet_length;
      head_y = body_bottom - body_height * 1.1f;

      return body_one;
    }

    void draw_bird_tail(SKCanvas canvas, SKPoint body_one, SKColor pc, SKColor dc) {
      if (RandomUtility.CreateRandom() < tail_chance) {
        int var_width = RandomUtility.CreateRandom( 15, 30 );
        int var_x = RandomUtility.CreateRandom( -25, -15 );
        int var_y = RandomUtility.CreateRandom( -50, -30 );
        if (RandomUtility.CreateRandom() < 0.3) {
          var_y *= -1;
        }

        var path = new SKPath { FillType = SKPathFillType.EvenOdd };
        path.MoveTo( body_one.X, body_one.Y );
        path.LineTo( body_one.X + var_width, body_one.Y );
        path.LineTo( body_one.X + var_width + var_x, body_one.Y + var_y );
        path.LineTo( body_one.X + var_x, body_one.Y + var_y );
        path.Close();

        SkiaSharpUtility.PathStroke( canvas, paint, dc, path );
        SkiaSharpUtility.PathFill( canvas, paint, pc, path );
      }
    }

    void draw_bird_beak(SKCanvas canvas, SKColor pc, SKColor bc, SKColor dc) {
      int y_variance = RandomUtility.CreateRandom( 10, 40 );
      int length_variance = RandomUtility.CreateRandom( 50, 100 );
      var p1 = new SKPoint( head_x, head_y - y_variance );
      var p2 = new SKPoint( head_x, head_y + y_variance );
      var p3 = new SKPoint( head_x + length_variance, head_y );

      paint.Style = SKPaintStyle.Stroke;
      paint.Color = dc;
      SkiaSharpUtility.DrawTriangle( canvas, paint, p1, p2, p3 );

      paint.Style = SKPaintStyle.Fill;
      paint.Color = RandomUtility.CreateRandom() < body_fill_chance ? pc : bc;
      SkiaSharpUtility.DrawTriangle( canvas, paint, p1, p2, p3 );
    }

    void draw_bird_head(SKCanvas canvas, SKColor pc, SKColor dc) {
      paint.Style = SKPaintStyle.StrokeAndFill;
      paint.Color = SKColors.White;
      canvas.DrawCircle( head_x, head_y, head_radius, paint );

      if (RandomUtility.CreateRandom() < arc_chance) {
        paint.Style = SKPaintStyle.Fill;
        paint.Color = pc;
        // arc(head_x, head_y, head_size, head_size, random(.7, 1)*PI, 1.8*PI, PIE);
        double startAngle = RandomUtility.CreateRandom( 0.7, 1 ) * Math.PI;
        double sweepAngle = 1.8 * Math.PI;
        canvas.DrawArc( new SKRect( head_x, head_y, head_x + head_radius, head_y + head_radius ), (float)startAngle, (float)sweepAngle, true, paint );
      }

      paint.Color = dc;
      if (RandomUtility.CreateRandom() < head_fill_chance) {
        paint.Style = SKPaintStyle.StrokeAndFill;
      }
      else {
        paint.Style = SKPaintStyle.Stroke;
      }
      canvas.DrawCircle( head_x, head_y, head_radius, paint );
    }

    void draw_bird_eyes(SKCanvas canvas, SKColor dc) {
      float eye_x = head_x + head_radius / 6.0f;
      float eye_y = head_y - head_radius / 8.0f;
      //int eye_size = 25; // diameter
      float eye_radius = 12.5f;

      SkiaSharpUtility.CircleStroke( canvas, paint, dc, eye_x, eye_y, eye_radius );
      SkiaSharpUtility.CircleFill( canvas, paint, SKColors.White, eye_x, eye_y, eye_radius );

      //SkiaSharpUtility.CircleStroke( canvas, paint, SKColors.Black, eye_x, eye_y, 5 );
      SkiaSharpUtility.CircleFill( canvas, paint, SKColors.Black, eye_x, eye_y, 5 );
    }

    void drawBird(SKCanvas canvas, float x, float y, SKColor pc, SKColor bc, SKColor dc) {
      // Draw Legs
      draw_bird_legs( canvas, x, y );
      // Draw Body
      var body_one = draw_bird_body( canvas, x, y, pc, bc, dc );
      // Draw Tail
      draw_bird_tail( canvas, body_one, pc, dc );
      // Draw Beak
      draw_bird_beak( canvas, pc, bc, dc );
      // Draw Head
      draw_bird_head( canvas, pc, dc );
      // Draw Eyes
      draw_bird_eyes( canvas, dc );
    }

    void draw(SKCanvas canvas) {
      float current_x = w / 2.0f - grid_x_pixels / 2.0f + 10;
      float current_y = h / 2.0f - grid_y_pixels / 2.0f + body_height - 10;
      var pc = RandomUtility.GetRandomElement<SKColor>( colors );
      double inc = .2f * 255; // 255 * RandomUtility.CreateRandom( 0.2, 1 ); //
      var dc = SkiaSharpUtility.CreateRGBColor( (byte)( pc.Red - inc ), (byte)( pc.Green - inc ), (byte)( pc.Blue - inc ) );

      
      drawBird( canvas, current_x, current_y, pc, bc, dc);
    }

  }

}
