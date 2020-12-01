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
  // Ref: https://github.com/aaronpenne/generative_art/blob/master/butterflies/butterflies.pyde
  public class AaronPenneButterfly {
    const int ImageCanvasSize = 1000;
    int w = ImageCanvasSize,
      h = ImageCanvasSize;


    int random_seed = 0;
    /// <summary>
    /// Parameters for draw speed
    /// </summary>
    const int frame_rate = 1;

    // Background Color
    SKColor backgroundColor = SKColor.FromHsv( 0, 0, 100 );
    List<List<SKColor>> pal = Common.ColorPalettes.AaronPenneBugs;

    List<string> availableLines = new List<string>() {
      "none", "some", "outer", "all"
    };

    SKPaint paint;

    public AaronPenneButterfly(int width, int height) {
      w = width;
      h = height;
      // # Sets color space to Hue Saturation Brightness with max values of HSB respectively
      // colorMode(HSB, 360, 100, 100, 100)
      SKColorFilter colorMode = SKColorFilter.CreateBlendMode( SKColor.FromHsv( 360, 100, 100, 100 ), SKBlendMode.Hue );

      paint = new SKPaint {
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
        StrokeJoin = SKStrokeJoin.Round,
        BlendMode = SKBlendMode.Multiply,
        //ColorFilter = colorMode
      };
    }

    public Stream Assemble() {
      // # Set the number of frames per second to display
      // frameRate( frame_rate )

      //SKCanvas canvas = new SKCanvas()
      SKPath path = new SKPath();
      Stream stream = null;
      SKImage skImage = null;
      try {
        using (var skSurface = SKSurface.Create( new SKImageInfo( w, h ) )) {
          var canvas = skSurface.Canvas;
          // # Sets color space to Hue Saturation Brightness with max values of HSB respectively
          // colorMode(HSB, 360, 100, 100, 100)

          //background(0, 0, 100)
          canvas.Clear( SKColors.White );
          // rectMode(CORNER)
          
          // draw ...
          draw( canvas );

          canvas.Flush();

          //skImage = skSurface.Snapshot();
          SKPaint paintConvert = SkiaSharpUtility.CreateDefaultPaint();
          //skImage = SkiaSharpUtility.ScaleSurfaceToImage( skSurface, ImageCanvasSize, size, paintConvert );
          skImage = skSurface.Snapshot();
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

    void draw(SKCanvas canvas) {
      int frameCount = Utility.CreateRandom( 1, 499 );
      int random_seed = (int)( frameCount * 1000000 / ( DateTime.Now.Second + 1 ) );
      random_seed = getSeed( random_seed );
      //helper.set_seed(random_seed)

      //blendMode(BLEND)
      paint.BlendMode = SKBlendMode.Multiply;

      // translate(width / 2, height / 2)
      canvas.Translate( w / 2f, h / 2f );
      canvas.Save();
      int palette_idx = RandomUtility.CreateRandom( 0, pal.Count );
      var palette = pal[palette_idx];

      //range_upper_angles = [x for x in range( int( random( 0, 20 ) ), int( random( 50, 80 ) ), int( random( 7, 20 ) ) )]
      //range_lower_angles = [x for x in range( int( random( 0, 20 ) ), int( random( 50, 80 ) ), int( random( 7, 20 ) ) )]
      // class range(start, stop[, step])
      // 0: start/min, 1: end/max, 2: step
      int[] upperSettings = new int[3] { RandomUtility.CreateRandom( 0, 20 ), RandomUtility.CreateRandom( 50, 80 ), RandomUtility.CreateRandom( 7, 20 ) };
      List<int> range_upper_angles = new List<int>();
      for(int i = upperSettings[0]; i < upperSettings[1]; i += upperSettings[2]) {
        range_upper_angles.Add( i );
      }
      int[] lowerSettings = new int[3] { RandomUtility.CreateRandom( 0, 20 ), RandomUtility.CreateRandom( 50, 80 ), RandomUtility.CreateRandom( 7, 20 ) };
      List<int> range_lower_angles = new List<int>();
      for (int i = lowerSettings[0]; i < lowerSettings[1]; i += lowerSettings[2]) {
        range_lower_angles.Add( i );
      }

      //range_upper_radii = [width * 0.2, width * 0.45]
      //range_lower_radii = [width * 0.2, width * 0.3]
      //SKPoint range_upper_radii = new SKPoint( w * 0.2f, w * 0.45f );
      //SKPoint range_lower_radii = new SKPoint( w * 0.2f, w * 0.3f );

      int num_layers = 10;
      string lines = RandomUtility.GetRandomElement<string>( availableLines );

      // curveTightness(random(-2, 0.6))

      DrawUpperWings( canvas, palette, num_layers, lines, range_upper_angles );

      var lastPalette = DrawLowerWings( canvas, palette, lines, range_lower_angles );

      var lastUpperAngle = range_upper_angles[range_upper_angles.Count - 1];
      DrawAntennaeAndBody( canvas, lastUpperAngle, lastPalette );
    }

    void DrawUpperWings(SKCanvas canvas, List<SKColor> palette, int num_layers, string lines, List<int> range_upper_angles) {
      SKPoint origin = new SKPoint( 0, 0 );
      List<byte> random2Bytes = new List<byte>() { 0, 60 };

      //List<List<SKPoint>> upper_wing = new List<List<SKPoint>>();
      //for (int i = 0; i < num_layers; i++) {
      //  //List<SKPoint> points
      //  //upper_wing.Add
      //} // for (int i = 0; i < num_layers

      for (int i = 0; i< 7; i++) {
        switch (lines) {
          case "none":
            // noStroke()
            ProcessingSkiaSharp.noStroke( paint );
            break;
          case "all":
            // stroke(0, 0, 0, 60)
            ProcessingSkiaSharp.stroke(paint, 0, 0, 0, 60 );
            break;
          case "outer":
            // stroke(0, 0, 0, 60)
            ProcessingSkiaSharp.stroke( paint, 0, 0, 0, 60 );
            break;
          case "some":
            // stroke(0, 0, 0, helper.random_list_value([0, 60]))
            byte randomByte = RandomUtility.GetRandomElement( random2Bytes );
            ProcessingSkiaSharp.stroke( paint, 0, 0, 0, randomByte );
            break;
        }
        //p = palette[int(random(0, len(palette)))]
        if (i == 3 || i == 6) {
          // fill(0, 0, 100, 100)
          ProcessingSkiaSharp.fill( paint, 0, 0, 100, 100 );
        }
        else {
          var p = RandomUtility.GetRandomElement<SKColor>( palette );
          //fill(p[0], p[1], p[2], 20)
          ProcessingSkiaSharp.fill( paint, p.Red, p.Green, p.Blue, 20 );
        }

        List<SKPoint> wing = new List<SKPoint>() { origin };
        foreach (int angle in range_upper_angles) {
          // circle_points_list(random(0, width * 0.01), random(0, height * 0.01), random( width * 0.2, width * 0.4 ), radians( random( angle - 7, angle ) )
          float randomAngle = RandomUtility.CreateRandom( angle - 7, angle );
          var cp = circle_points_list( RandomUtility.CreateRandom( 0, w * 0.01f ),
            RandomUtility.CreateRandom( 0, h * 0.01f ),
            RandomUtility.CreateRandom( w * 0.2f, w * 0.4f ),
            (float)ProcessingSkiaSharp.radians( randomAngle ) );
          wing.Add( cp );
        }
        draw_wings( canvas, wing, true );
      } // for (int i = 0; i< 7

      for (int i = 0; i < 7; i++) {
        switch (lines) {
          case "none":
            // noStroke()
            ProcessingSkiaSharp.noStroke( paint );
            break;
          case "all":
            // stroke(0, 0, 0, 60)
            ProcessingSkiaSharp.stroke( paint, 0, 0, 0, 60 );
            break;
          case "outer":
            // noStroke()
            ProcessingSkiaSharp.noStroke( paint );
            break;
          case "some":
            // stroke(0, 0, 0, helper.random_list_value([0, 60]))
            byte randomByte = RandomUtility.GetRandomElement( random2Bytes );
            ProcessingSkiaSharp.stroke( paint, 0, 0, 0, randomByte );
            break;
        }
        //p = palette[int(random(0, len(palette)))]
        if (i == 3 || i == 6) {
          // fill(0, 0, 100, 100)
          ProcessingSkiaSharp.fill( paint, 0, 0, 100, 100 );
        }
        else {
          var p = RandomUtility.GetRandomElement<SKColor>( palette );
          //fill(p[0], p[1], p[2], 20)
          ProcessingSkiaSharp.fill( paint, p.Red, p.Green, p.Blue, 20 );
        }

        List<SKPoint> wing = new List<SKPoint>() { origin };
        foreach (int angle in range_upper_angles) {
          // circle_points_list(random(0, width * 0.01), random(0, height * 0.01), random( width * 0.2, width * 0.4 ), radians( random( angle - 7, angle ) )
          float randomAngle = RandomUtility.CreateRandom( angle - 7, angle );
          var cp = circle_points_list( RandomUtility.CreateRandom( 0, w * 0.01f ),
            RandomUtility.CreateRandom( 0, h * 0.01f ),
            RandomUtility.CreateRandom( w * 0.1f, w * 0.2f ),
            (float)ProcessingSkiaSharp.radians( randomAngle ) );
          wing.Add( cp );
        }
        draw_wings( canvas, wing, true );
      } // for (int i = 0; i< 7
    }

    SKColor DrawLowerWings(SKCanvas canvas, List<SKColor> palette, string lines, List<int> range_lower_angles) {
      SKPoint origin = new SKPoint( 0, 0 );
      List<byte> random2Bytes = new List<byte>() { 0, 60 };
      SKColor p = SKColors.Transparent;

      for (int i = 0; i < 11; i++) {
        switch (lines) {
          case "none":
            // noStroke()
            ProcessingSkiaSharp.noStroke( paint );
            break;
          case "all":
            // stroke(0, 0, 0, 60)
            ProcessingSkiaSharp.stroke( paint, 0, 0, 0, 60 );
            break;
          case "outer":
            // stroke(0, 0, 0, 60)
            ProcessingSkiaSharp.stroke( paint, 0, 0, 0, 60 );
            break;
          case "some":
            // stroke(0, 0, 0, helper.random_list_value([0, 60]))
            byte randomByte = RandomUtility.GetRandomElement( random2Bytes );
            ProcessingSkiaSharp.stroke( paint, 0, 0, 0, randomByte );
            break;
        }
        //p = palette[int(random(0, len(palette)))]
        p = RandomUtility.GetRandomElement<SKColor>( palette );
        if (i == 3 || i == 6) {
          // fill(0, 0, 100, 100)
          ProcessingSkiaSharp.fill( paint, 0, 0, 100, 100 );
        }
        else {
          //fill(p[0], p[1], p[2], 20)
          ProcessingSkiaSharp.fill( paint, p.Red, p.Green, p.Blue, 20 );
        }

        List<SKPoint> wing = new List<SKPoint>() { origin };
        foreach (int angle in range_lower_angles) {
          // circle_points_list(random(0, width * 0.01), random(0, height * 0.01), random( width * 0.15, width * 0.3 ), radians( random( angle - 7, angle ) )))
          float randomAngle = RandomUtility.CreateRandom( angle - 7, angle );
          var cp = circle_points_list( RandomUtility.CreateRandom( 0, w * 0.01f ),
            RandomUtility.CreateRandom( 0, h * 0.01f ),
            RandomUtility.CreateRandom( w * 0.15f, w * 0.3f ),
            (float)ProcessingSkiaSharp.radians( randomAngle ) );
          wing.Add( cp );
        }
        float tmpRangle = RandomUtility.CreateRandom( 73, 87 );
        var tmpCp = circle_points_list( RandomUtility.CreateRandom( 0, w * 0.01f ),
          RandomUtility.CreateRandom( 0, h * 0.01f ),
          RandomUtility.CreateRandom( w * 0.15f, w * 0.3f ),
          (float)ProcessingSkiaSharp.radians( tmpRangle ) );
        wing.Add( tmpCp );

        draw_wings( canvas, wing );
      } // for (int i = 0; i< 11

      for (int i = 0; i < 11; i++) {
        switch (lines) {
          case "none":
            // noStroke()
            ProcessingSkiaSharp.noStroke( paint );
            break;
          case "all":
            // stroke(0, 0, 0, 60)
            ProcessingSkiaSharp.stroke( paint, 0, 0, 0, 60 );
            break;
          case "outer":
            // noStroke()
            ProcessingSkiaSharp.noStroke( paint );
            break;
          case "some":
            // stroke(0, 0, 0, helper.random_list_value([0, 60]))
            byte randomByte = RandomUtility.GetRandomElement( random2Bytes );
            ProcessingSkiaSharp.stroke( paint, 0, 0, 0, randomByte );
            break;
        }
        //p = palette[int(random(0, len(palette)))]
        p = RandomUtility.GetRandomElement<SKColor>( palette );
        if (i == 3 || i == 6) {
          // fill(0, 0, 100, 100)
          ProcessingSkiaSharp.fill( paint, 0, 0, 100, 100 );
        }
        else {
          //fill(p[0], p[1], p[2], 20)
          ProcessingSkiaSharp.fill( paint, p.Red, p.Green, p.Blue, 20 );
        }

        List<SKPoint> wing = new List<SKPoint>() { origin };
        foreach (int angle in range_lower_angles) {
          // circle_points_list(random(0, width * 0.01), random(0, height * 0.01), random( width * 0.05, width * 0.2 ), radians( random( angle - 7, angle ) )))
          float randomAngle = RandomUtility.CreateRandom( angle - 7, angle );
          var cp = circle_points_list( RandomUtility.CreateRandom( 0, w * 0.01f ),
            RandomUtility.CreateRandom( 0, h * 0.01f ),
            RandomUtility.CreateRandom( w * 0.05f, w * 0.2f ),
            (float)ProcessingSkiaSharp.radians( randomAngle ) );
          wing.Add( cp );
        }
        float tmpRangle = RandomUtility.CreateRandom( 73, 87 );
        var tmpCp = circle_points_list( RandomUtility.CreateRandom( 0, w * 0.01f ),
          RandomUtility.CreateRandom( 0, h * 0.01f ),
          RandomUtility.CreateRandom( w * 0.05f, w * 0.2f ),
          (float)ProcessingSkiaSharp.radians( tmpRangle ) );
        wing.Add( tmpCp );

        draw_wings( canvas, wing );
      } // for (int i = 0; i< 11

      return p;
    }

    void draw_wings(SKCanvas canvas, List<SKPoint> wing, bool upper_wing= false) {
      List<SKBlendMode> modes = new List<SKBlendMode>() {
        SKBlendMode.ColorBurn, SKBlendMode.Multiply, SKBlendMode.Difference, SKBlendMode.Darken
      };
      SKBlendMode style = RandomUtility.GetRandomElement<SKBlendMode>( modes );
      //canvas.ResetMatrix();
      int canvasCacheTag = canvas.Save();
      if (upper_wing)
        canvas.Scale( 1, -1 );

      paint.BlendMode = style;
      //draw_curve_filled( wing )
      SkiaSharpUtility.DrawSplineCurve( canvas, paint, wing, true, SKPathFillType.Winding );

      paint.BlendMode = SKBlendMode.Multiply;// BLEND
      canvas.Scale( -1, 1 );

      paint.BlendMode = style;
      // draw_curve_filled(wing)
      SkiaSharpUtility.DrawSplineCurve( canvas, paint, wing, true, SKPathFillType.Winding );

      paint.BlendMode = SKBlendMode.Multiply;// BLEND
      //canvas.ResetMatrix();
      canvas.RestoreToCount( canvasCacheTag );
    }

    void DrawAntennaeAndBody(SKCanvas canvas, int lastUpperAngle, SKColor palette) {
      paint.StrokeWidth = w * 0.002f;

      var body = get_16_points( -w * 0.015f, -h * 0.15f,
                         w * 0.03f, h * 0.5f );
      //curveTightness(0)

      // Body
      // x: Clear, DstATop, DstOver
      paint.BlendMode = SKBlendMode.SrcOver;
      //fill( 0, 0, 100 )
      //noStroke()
      //ProcessingSkiaSharp.fill( paint, SKColor.FromHsv( 0, 0, 100 ) ); 
      ProcessingSkiaSharp.strokeAndFill( paint, new SKColor( 255, 255, 255, 255 ) );
      draw_16_points( canvas, body );
      // test orgin point
      //canvas.DrawCircle( new SKPoint( 0, 0 ), 5, paint ); // OK

      // Antennae
      paint.BlendMode = SKBlendMode.Multiply;
      List<SKPoint> antennae = new List<SKPoint>();
      int max = RandomUtility.CreateRandom( 3, 8 );
      for(int i =0; i< max; i++) {
        float x = body[0].X;
        float y = body[0].Y;
        float r = RandomUtility.CreateRandom( h * 0.1f, h * 0.3f );
        //a = random(range_upper_angles[-1] * 1.2, 80)
        float a = RandomUtility.CreateRandom( lastUpperAngle * 1.2f, 80 );
        double radians = ProcessingSkiaSharp.radians( a );
        var pt = circle_points( x, y, r, (float)radians );
        antennae.Add( pt );
      }

      List<float> curve_tightness = new List<float>();
      foreach(var a in antennae) {
        curve_tightness.Add( RandomUtility.CreateRandom( -2, 0.8f ) );
      }

      //pushStyle()
      //pushMatrix()
      //canvas.ResetMatrix();
      int canvasCacheTag = canvas.Save();

      float randomDY = RandomUtility.CreateRandom( h * 0.24f, h * 0.26f );
      canvas.Translate( 0, -randomDY );
      ProcessingSkiaSharp.noFill(paint);
      //strokeWeight(width * 0.001)
      //paint.StrokeWidth = w * 0.001f;
      //stroke( p[0], p[1], 25 )palette
      //ProcessingSkiaSharp.stroke( paint, palette.Red, palette.Green, 25 );
      ProcessingSkiaSharp.stroke( paint, new SKColor( palette.Red, palette.Green, 25 ) );
      //int alpha = RandomUtility.CreateRandom( 80, 230 );
      //ProcessingSkiaSharp.stroke( paint, palette.Red, palette.Green, palette.Blue, (byte)alpha );

      canvas.Scale( 1, -1 );
      drawAntennae( canvas, body, antennae );

      canvas.Scale( -1, 1 );
      drawAntennae( canvas, body, antennae );

      //canvas.ResetMatrix();
      canvas.RestoreToCount( canvasCacheTag );
    }

    void drawAntennae(SKCanvas canvas, List<SKPoint> body, List<SKPoint> antennae) {
      List<SKPoint> points = new List<SKPoint>();
      points.Add( body[2] );
      points.Add( body[2] );
      for (int i = 0; i < antennae.Count; i++) {
        var a = antennae[i];
        //curveTightness(curve_tightness[i])
        //curveVertex(x, y)
        points.Add( a );
      }
      SkiaSharpUtility.DrawSplineCurve( canvas, paint, points, false, SKPathFillType.Winding );
      
    }

    List<SKPoint> get_16_points(float x, float y, float w, float h) {
      float squeeze = RandomUtility.CreateRandom( -w * 0.2f, w * 0.2f );
      List<SKPoint> points = new List<SKPoint>();
      points.Add( new SKPoint( x, y ) );
      points.Add( new SKPoint( x + w * 0.25f, y ) );
      points.Add( new SKPoint( x + w * 0.5f, y - h * 0.05f ) );
      points.Add( new SKPoint( x + w * 0.75f, y ) );

      points.Add( new SKPoint( x + w, y ) );
      points.Add( new SKPoint( x + w, y + h * 0.25f ) );
      points.Add( new SKPoint( x + w + squeeze, y + h * 0.5f ) );
      points.Add( new SKPoint( x + w, y + h * 0.75f ) );

      points.Add( new SKPoint( x + w, y + h ) );
      points.Add( new SKPoint( x + w * 0.75f, y + h ) );
      points.Add( new SKPoint( x + w * 0.5f, y + h ) );
      points.Add( new SKPoint( x + w * 0.25f, y + h ) );

      points.Add( new SKPoint( x, y + h ) );
      points.Add( new SKPoint( x, y + h * 0.75f ) );
      points.Add( new SKPoint( x - squeeze, y + h * 0.5f ) );
      points.Add( new SKPoint( x, y + h * 0.25f ) );

      points.RemoveAt( 12 );
      points.RemoveAt( 8 );
      points.RemoveAt( 4 );
      points.RemoveAt( 0 );

      points.Add( new SKPoint( x + w * 0.25f, y ) );

      return points;
    }

    void draw_16_points(SKCanvas canvas, List<SKPoint> points) {
      if (points == null || points.Count == 0)
        return;
      //var blendModeCache = paint.BlendMode;
      //paint.BlendMode = SKBlendMode.DstOver;
      SkiaSharpUtility.DrawSplineCurve( canvas, paint, points, true, SKPathFillType.EvenOdd );
      //SkiaSharpUtility.DrawPoints( canvas, paint, points );
      //paint.BlendMode = blendModeCache;
    }

    void draw_curve_filled(SKCanvas canvas, List<SKPoint> points) {
      if (points == null)
        return;
      SkiaSharpUtility.DrawSplineCurve( canvas, paint, points, false, SKPathFillType.Winding );
    }

    //void cvp(SKCanvas canvas, float x, float y) {
    //  //curveVertex(x, y)

    //}

    SKPoint circle_points(float origin_x, float origin_y, float r = 50, float a = 0) {
      //'''Returns cartesian coordinates given a circle origin, radius, and angle'''
      double x = origin_x + ( r * Math.Cos( a ) );
      double y = origin_y + ( r * Math.Sin( a ) );
      return new SKPoint( (float)x, (float)y );
    }

    SKPoint circle_points_list(float origin_x, float origin_y, float r = 50, float a = 0) {
      //'''Returns cartesian coordinates given a circle origin, radius, and angle'''
      double x = origin_x + ( r * Math.Cos( a ) );
      double y = origin_y + ( r * Math.Sin( a ) );
      return new SKPoint( (float)x, (float)y );
    }
    

    int getSeed(int random_seed = 0) {
      int max_seed = 1000000;
      if (random_seed == 0)
        return RandomUtility.CreateRandom( 0, max_seed );

      if (random_seed < max_seed)
        return random_seed;
      else
        return random_seed % max_seed;
    }

  }

}
