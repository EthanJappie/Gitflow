// Type: DaVinci.SecurityUtil
// Assembly: DaVinci, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Documents and Settings\qt94320\Desktop\DaVinci Decrypt DFE CS CFS\DaVinci.exe

using System;
using System.Security.Cryptography;
using System.Text;

namespace BMW.FS.Indexing.WebServices
{
  public class SecurityUtil
  {
    private RSACryptoServiceProvider rsaObj = (RSACryptoServiceProvider) null;

    public SecurityUtil(string strKey)
    {
      this.rsaObj = new RSACryptoServiceProvider(4096, new CspParameters()
      {
        //Flags = CspProviderFlags.UseDefaultKeyContainer
          Flags = CspProviderFlags.UseMachineKeyStore
      });
      if (string.IsNullOrEmpty(strKey))
        return;
      this.rsaObj.FromXmlString(strKey);
    }

    public string EncryptData(string strDataToEncrypt)
    {
      try
      {
        if (this.rsaObj != null)
          return Convert.ToBase64String(this.rsaObj.Encrypt(Encoding.UTF8.GetBytes(strDataToEncrypt), false));
      }
      catch (Exception ex)
      {
        throw;
      }
      return string.Empty;
    }

    public string DecryptData(string strDataToEncrypt)
    {
      try
      {
        if (this.rsaObj != null)
          return Encoding.UTF8.GetString(this.rsaObj.Decrypt(Convert.FromBase64String(strDataToEncrypt), false));
      }
      catch (Exception ex)
      {
        throw;
      }
      return string.Empty;
    }


    private static byte[] key = new byte[8] { 16, 5, 44, 36, 5, 116, 37, 228 };
    private static byte[] iv = new byte[8] { 5, 4, 43, 124, 35, 61, 74, 80 };

    public static string SimpleEncrypt(string text)
    {
        SymmetricAlgorithm algorithm = DES.Create();
        ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
        byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
        byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
        return Convert.ToBase64String(outputBuffer);
    }

    public static string SimpleDecrypt(string text)
    {
        SymmetricAlgorithm algorithm = DES.Create();
        ICryptoTransform transform = algorithm.CreateDecryptor(key, iv);
        byte[] inputbuffer = Convert.FromBase64String(text);
        byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
        return Encoding.Unicode.GetString(outputBuffer);
    }
  }
}
