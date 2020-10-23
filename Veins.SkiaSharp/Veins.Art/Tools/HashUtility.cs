using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veins.Art.Tools {
  public class HashUtility {
    /// <summary>
    /// 将文本进行SHA512EncryptToHex处理，再转换成uint[]
    /// </summary>
    /// <param name="text"></param>
    /// <param name="hashCount"></param>
    /// <returns></returns>
    public static List<uint> CreateHashs(string text, int hashCount = 16) {
      string sha512 = Security.CryptographyHelper.SHA512EncryptToHex( text, System.Text.Encoding.UTF8 );
      List<uint> hashsList = CreateHashsOfSHA512( sha512, hashCount );
      return hashsList;
    }

    /// <summary>
    /// 将Hex字符串(SHA512字符串)转换成uint[]
    /// </summary>
    /// <param name="sha512">sha512 string</param>
    /// <param name="hashCount"></param>
    /// <returns></returns>
    public static List<uint> CreateHashsOfSHA512(string sha512, int hashCount = 16) {
      List<uint> hashsList = new List<uint>();
      if(string.IsNullOrEmpty( sha512 )) {
        hashsList.Add( 0u );
        return hashsList;
      }

      for (int i = 0; i < hashCount; i++) {
        // Get 1/numblocks of the hash
        int blocksize = (int)( 1.0f * sha512.Length / hashCount );
        int currentstart = ( 1 + i ) * blocksize - blocksize;
        //int currentend = ( 1 + i ) * blocksize;
        // int(self.hexdigest[currentstart:currentend],16)
        string hex = sha512.Substring( currentstart, blocksize );
        uint hex16 = uint.Parse( hex, System.Globalization.NumberStyles.HexNumber );
        hashsList.Add( hex16 );
      }
      hashsList.AddRange( hashsList );
      return hashsList;
    }

  }

}
