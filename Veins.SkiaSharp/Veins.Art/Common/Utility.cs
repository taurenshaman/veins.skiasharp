using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Veins.Art.Common {
  public class Utility {
    static Random random = new Random();

    /// <summary>
    /// get avatar URL from Gravatar.com
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public static string GetGravatarUrl( string email ) {
      if ( string.IsNullOrWhiteSpace( email ) )
        return null;

      string lowerEmail = email.Trim().ToLower();
      byte[] key_bytes = System.Text.Encoding.UTF8.GetBytes( lowerEmail );
      System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();
      key_bytes = md5.ComputeHash( key_bytes );
      StringBuilder builder = new StringBuilder( key_bytes.Length * 2 );
      for (int i = 0; i < key_bytes.Length; i++) {
        builder.Append( key_bytes[i].ToString( "X2" ) );
      }
      string hash = builder.ToString();

      return "http://www.gravatar.com/avatar/" + hash.ToLower() + ".png?s=64";
    }

    /// <summary>
    /// return value in [min, max)
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int CreateRandom(int min, int max) {
      return random.Next( min, max );
    }
    /// <summary>
    /// return value in [0, 1.0)
    /// </summary>
    /// <returns></returns>
    public static double CreateRandom() {
      return random.NextDouble();
    }

  }

}
