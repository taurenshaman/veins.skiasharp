using System;
using System.Text;

namespace Veins.Art.Security {
  public class CryptographyHelper {
    /// <summary>
    /// SHA512加密。字符串编码使用UTF8。
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string SHA512Encrypt( string input ) {
      return SHA512Encrypt( input, System.Text.UTF8Encoding.UTF8 );
    }

    public static string SHA512Encrypt(string input, Encoding encoding) {
      byte[] bytes = SHA512EncryptToBytes( input, encoding );
      if (bytes == null || bytes.Length == 0)
        return string.Empty;

      StringBuilder sb = new StringBuilder();
      foreach (byte b in bytes)
        sb.Append( b.ToString( "x2" ) );

      return sb.ToString();
    }

    public static string SHA512EncryptToHex(string input, Encoding encoding) {
      byte[] bytes = SHA512EncryptToBytes( input, encoding );
      if (bytes == null || bytes.Length == 0)
        return string.Empty;

      string bytesString = BitConverter.ToString( bytes );
      string hex = bytesString.Replace( "-", string.Empty );
      return hex;
    }

    public static byte[] SHA512EncryptToBytes(string input, Encoding encoding) {
      if (input == null || input.Length == 0)
        return null;
      if (encoding == null)
        encoding = System.Text.UTF8Encoding.UTF8;

      byte[] inputBytes = encoding.GetBytes( input );
      System.Security.Cryptography.SHA512Managed sha = new System.Security.Cryptography.SHA512Managed();
      byte[] shaResults = sha.ComputeHash( inputBytes );

      sha.Clear();
      return shaResults;
    }

  }

}
