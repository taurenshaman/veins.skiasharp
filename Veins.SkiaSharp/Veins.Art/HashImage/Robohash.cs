using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SkiaSharp;
using System.IO;

namespace Veins.Art {
  // Ref: https://github.com/e1ven/Robohash
  // License: [MIT License](https://github.com/e1ven/Robohash/blob/master/LICENSE.txt)
  public class Robohash {
    public static readonly string[] CategorySets = new string[5] { "set1", "set2", "set3", "set4", "set5" };
    public static readonly string[] ColorSets = new string[10] { "blue", "brown", "green", "grey", "orange", "pink", "purple", "red", "white", "yellow" };
    public static readonly string[] BackgroundSets = new string[2] { "bg1", "bg2" };

    string text;
    string format = "png";

    const int ImageCanvasSize = 1024;

    string hexDigest;
    List<uint> hashes;
    // Jerin: 2020-10:
    // Original hashCount is 11, but there are some error while converting to int.
    // Length of sha512 result is 128, and createHashs means 128/hashCount, so I changed hashCount from 11 to 16.
    // The result is also OK. So I think hashCount = 16 works well.
    int hashCount = 11;
    int iter = 4;
    string resourceDir;
    List<string> sets;
    List<string> bgSets;
    List<string> colorsInSet1;

    public static string GetRandomSet() {
      int index = Common.Utility.CreateRandom( 0, CategorySets.Length );
      return CategorySets[index];
    }
    public static string GetRandomColor() {
      int index = Common.Utility.CreateRandom( 0, ColorSets.Length );
      return ColorSets[index];
    }
    public static string GetRandomBackground() {
      int index = Common.Utility.CreateRandom( 0, BackgroundSets.Length );
      return BackgroundSets[index];
    }

    // def __init__(self,string,hashcount=11,ignoreext = True):
    public Robohash(string text, int hashCount = 16, bool ignoreExt = true) {
      if (ignoreExt)
        this.removeExts( text );
      this.hashCount = hashCount;
      this.resourceDir = AppDomain.CurrentDomain.BaseDirectory + "\\robohash\\";
      this.hexDigest = Security.CryptographyHelper.SHA512EncryptToHex( this.text, System.Text.Encoding.UTF8 );
      /*
        #Start this at 4, so earlier is reserved
        #0 = Color
        #1 = Set
        #2 = bgset
        #3 = BG
       */
      this.iter = 4;
      this.hashes = this.createHashs( this.hashCount );
      this.sets = this.listDirsNames( this.resourceDir + "sets" );
      this.bgSets = this.listDirsNames( this.resourceDir + "backgrounds" );
      this.colorsInSet1 = this.listDirsNames( this.resourceDir + "sets\\set1" );
      this.format = "png";
    }

    public Stream Assemble(string roboset, string color, string format, string bgset, int size = 256) {
      if (size < 256)
        _ = 256;
      else if (size > ImageCanvasSize)
        size = ImageCanvasSize;
      // roboset
      if (roboset == "any") {
        // roboset = self.sets[self.hasharray[1] % len(self.sets) ]
        int index = (int)( this.hashes[1] % this.sets.Count );
        _ = this.sets[index];
      }
      else if (!this.sets.Contains( roboset )) {
        _ = this.sets[0];
      }

      /*
        # Only set1 is setup to be color-seletable. The others don't have enough pieces in various colors.
        # This could/should probably be expanded at some point..
        # Right now, this feature is almost never used. ( It was < 44 requests this year, out of 78M reqs )
       */
      if (roboset == "set1") {
        if (!string.IsNullOrEmpty( color ) && this.colorsInSet1.Contains( color ))
          roboset = "set1\\" + color;
        else {
          //randomcolor = self.colors[self.hasharray[0] % len(self.colors) ]
          int index = (int)( this.hashes[0] % this.colorsInSet1.Count );
          roboset = "set1\\" + this.colorsInSet1[index];
        }
      } // roboset == "set1"

      // If they specified a background, ensure it's legal, then give it to them.
      if (!bgSets.Contains( bgset ) || bgset == "any") {
        //bgset = self.bgsets[self.hasharray[2] % len( self.bgsets )]
        int index = (int)( this.hashes[2] % this.bgSets.Count );
        bgset = this.bgSets[index];
      }

      // If we set a format based on extension earlier, use that. Otherwise, PNG.
      if (string.IsNullOrEmpty( format ))
        format = this.format;

      /*
        # Each directory in our set represents one piece of the Robot, such as the eyes, nose, mouth, etc.

        # Each directory is named with two numbers - The number before the # is the sort order.
        # This ensures that they always go in the same order when choosing pieces, regardless of OS.

        # The second number is the order in which to apply the pieces.
        # For instance, the head has to go down BEFORE the eyes, or the eyes would be hidden.

        # First, we'll get a list of parts of our robot.
       */
      var roboparts = getImagesPaths( this.resourceDir + "sets\\" + roboset );
      // Now that we've sorted them by the first number, we need to sort each sub-category by the second.
      roboparts = roboparts
        .OrderBy( s => s.Split( '#' )[1] )
        .ToList();

      Stream stream = null;
      SKImage skImage = null;
      SKPaint paint = createDefaultPaint();
      try {
        using (var skSurface = SKSurface.Create( new SKImageInfo( ImageCanvasSize, ImageCanvasSize ) )) {
          var canvas = skSurface.Canvas;
          canvas.Clear( SKColors.Transparent ); //set background color
          // draw background
          if (!string.IsNullOrEmpty( bgset )) {
            string bgDirPath = this.resourceDir + "backgrounds\\" + bgset;
            List<string> legalBackgrounds = new List<string>();
            var backgrounds = System.IO.Directory.GetFiles( bgDirPath );
            var sortedBackgrounds = backgrounds.OrderBy( s => s );
            foreach (string bg in sortedBackgrounds) {
              string bgName = bg.Substring( bgDirPath.Length );
              if (!bgName.StartsWith( "." )) // skip files like .git, .xxx
                legalBackgrounds.Add( bg );
            }
            // Use some of our hash bits to choose which file
            int index = (int)( this.hashes[3] % legalBackgrounds.Count );
            string bgPartPath = legalBackgrounds[index];
            drawToCanvas( canvas, bgPartPath, paint );
          }
          foreach (var partImage in roboparts) {
            drawToCanvas( canvas, partImage, paint );
          }
          canvas.Flush();

          // scale
          if (size == ImageCanvasSize)
            skImage = skSurface.Snapshot();
          else {
            // 先缩放；然后绘制到小图上面，即相当于Crop了
            canvas.Scale( 1.0f * size / ImageCanvasSize );
            var srcImage = skSurface.Snapshot();

            //skImage = SKImage.Create( new SKImageInfo( size, size ) );
            //srcImage.ScalePixels( skImage.PeekPixels(), SKFilterQuality.High );

            using (var newSurface = SKSurface.Create( new SKImageInfo( size, size ) )) {
              newSurface.Canvas.DrawImage( srcImage, SKRect.Create( 0, 0, ImageCanvasSize, ImageCanvasSize ), SKRect.Create( 0, 0, size, size ), paint );
              newSurface.Canvas.Flush();
              skImage = newSurface.Snapshot();
            }
            
          } // scale
        }
        // encode
        SKData skData = encodeImageToSKData( skImage, format );
        
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

    SKPaint createDefaultPaint() {
      var paint = new SKPaint();
      paint.IsAntialias = true;
      paint.FilterQuality = SKFilterQuality.High;
      return paint;
    }

    void drawToCanvas(SKCanvas canvas, string imgPath, SKPaint paint = null) {
      var skBitmap = SKBitmap.Decode( imgPath );
      canvas.DrawBitmap( skBitmap, SKRect.Create( 0, 0, skBitmap.Width, skBitmap.Height ), SKRect.Create( 0, 0, ImageCanvasSize, ImageCanvasSize ), paint );
      skBitmap.Dispose();
    }

    SKData encodeImageToSKData(SKImage skImage, string format, int quality = 90) {
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

    /// <summary>
    /// get format, text
    /// </summary>
    /// <param name="text"></param>
    void removeExts(string text) {
      string lower = text.ToLower();
      if (lower.EndsWith( ".png" ) || lower.EndsWith( ".gif" ) || lower.EndsWith( ".jpg" ) || lower.EndsWith( ".bmp" ) ||
        lower.EndsWith( ".jpeg" ) || lower.EndsWith( ".ppm" ) || lower.EndsWith( ".datauri" )) {
        int index = lower.LastIndexOf( "." );
        string format = lower.Substring( index + 1 );
        if (format.ToLower() == "jpg")
          format = "jpeg";
        this.format = format;
        this.text = lower.Substring( 0, index );
      }
      else {
        this.format = "png";
        this.text = lower;
      }
    }

    List<uint> createHashs(int count = 11) {
      List<uint> hashsList = new List<uint>();
      for (int i = 0; i < count; i++) {
        // Get 1/numblocks of the hash
        int blocksize = (int)( 1.0f * this.hexDigest.Length / count );
        int currentstart = ( 1 + i ) * blocksize - blocksize;
        //int currentend = ( 1 + i ) * blocksize;
        // int(self.hexdigest[currentstart:currentend],16)
        string hex = this.hexDigest.Substring( currentstart, blocksize );
        uint hex16 = uint.Parse( hex, System.Globalization.NumberStyles.HexNumber );
        hashsList.Add( hex16 );
      }
      hashsList.AddRange( hashsList );
      return hashsList;
    }

    List<string> listDirsNames(string dirPath) {
      List<string> dirs = new List<string>();
      var pathList = System.IO.Directory.EnumerateDirectories( dirPath );
      foreach (string path in pathList) {
        dirs.Add( path.Remove( 0, dirPath.Length ) );
      }
      return dirs;
    }

    List<string> getImagesPaths(string dirPath) {
      List<string> directories = new List<string>();
      var pathList = System.IO.Directory.EnumerateDirectories( dirPath );
      foreach (string path in pathList) {
        string dirName = path.Substring( dirPath.Length );
        if (!dirName.StartsWith( "." )) // skip directories like .git, .xxx
          directories.Add( path );
      }

      List<string> files = new List<string>();
      foreach (string dir in directories) {
        var filesPaths = System.IO.Directory.GetFiles( dir );
        var sorted = filesPaths.OrderBy( s => s );
        // Use some of our hash bits to choose which file
        int index = (int)( this.hashes[this.iter] % filesPaths.Length );
        files.Add( sorted.ElementAt( index ) );
        this.iter++;
      }
      return files;
    }

  }

}
