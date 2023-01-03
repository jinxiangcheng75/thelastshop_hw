using System.Collections;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;

public class EncryptoUtils
{

    public const string des_key = "asdfefswg4ewltkjerterrgfsdf";
    static string des_key_key = des_key[6].ToString() +
                                des_key[0].ToString() +
                                des_key[17].ToString() +
                                des_key[10].ToString() +
                                des_key[20].ToString() +
                                des_key[8].ToString() +
                                des_key[4].ToString() +
                                des_key[12].ToString();
    static string des_key_iv = des_key[16].ToString() +
                                des_key[10].ToString() +
                                des_key[7].ToString() +
                                des_key[1].ToString() +
                                des_key[2].ToString() +
                                des_key[18].ToString() +
                                des_key[14].ToString() +
                                des_key[21].ToString();

    public static void Init()
    {

    }

    public static void XORBytes(byte[] bytes, byte[] key)
    {
        int j = 0;
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] ^= key[j++];
            if (j >= key.Length)
                j = 0;
        }
    }

    public static void serializeDataCrypto(System.Object obj, string path)
    {
        using (FileStream fs = File.Create(path))
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = ASCIIEncoding.ASCII.GetBytes(des_key_key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(des_key_iv);
            CryptoStream cs = new CryptoStream(fs, des.CreateEncryptor(), CryptoStreamMode.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(cs, obj);
            cs.Close();
            fs.Close();
        }
    }

    public static System.Object deserializeDataCrypto(string path)
    {
        if (File.Exists(path) == false)
            return null;
        System.Object obj = null;
        using (FileStream fs = File.OpenRead(path))
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = ASCIIEncoding.ASCII.GetBytes(des_key_key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(des_key_iv);
            CryptoStream cs = new CryptoStream(fs, des.CreateDecryptor(), CryptoStreamMode.Read);
            BinaryFormatter bf = new BinaryFormatter();
            obj = bf.Deserialize(cs);
            cs.Close();
            fs.Close();
        }
        return obj;
    }

    public static void decryptoDataBytes(byte[] bytes)
    {
        MemoryStream ms = new MemoryStream(bytes);
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        des.Key = ASCIIEncoding.ASCII.GetBytes(des_key_key);
        des.IV = ASCIIEncoding.ASCII.GetBytes(des_key_iv);
        CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read);
        byte[] deBytes = new byte[cs.Length];
        cs.Read(deBytes, 0, (int)cs.Length);
        cs.Close();
    }

    public static void encryptoDataBytes(byte[] bytes)
    {
        MemoryStream ms = new MemoryStream(bytes);
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        des.Key = ASCIIEncoding.ASCII.GetBytes(des_key_key);
        des.IV = ASCIIEncoding.ASCII.GetBytes(des_key_iv);
        CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
    }

    public static void encryptoStream(byte[] srcBytes, out byte[] streamBytes)
    {
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        des.Key = ASCIIEncoding.ASCII.GetBytes(des_key_key);
        des.IV = ASCIIEncoding.ASCII.GetBytes(des_key_iv);
        CryptoStream cs = null;
        byte[] bytes = null;
        MemoryStream ms = new MemoryStream();
        try
        {
            cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(srcBytes, 0, srcBytes.Length);
            cs.FlushFinalBlock();
            bytes = ms.ToArray();
        }
        catch (System.Exception ex)
        {

            Logger.logException(ex);
        }
        finally
        {
            if (cs != null)
                cs.Close();
        }
        if (bytes != null)
        {
            streamBytes = bytes;
        }
        else
        {
            streamBytes = null;
        }
    }

    public static MemoryStream decryptoStream(byte[] srcBytes, out byte[] streamBytes)
    {
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        des.Key = ASCIIEncoding.ASCII.GetBytes(des_key_key);
        des.IV = ASCIIEncoding.ASCII.GetBytes(des_key_iv);
        CryptoStream cs = null;
        byte[] bytes = null;
        MemoryStream ms = new MemoryStream();
        try
        {
            cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(srcBytes, 0, srcBytes.Length);
            cs.FlushFinalBlock();
            bytes = ms.ToArray();
        }
        catch (System.Exception ex)
        {
            Logger.error(ex.StackTrace);
        }
        finally
        {
            if (cs != null)
                cs.Close();
        }
        if (bytes != null)
        {
            streamBytes = bytes;
        }
        else
        {
            streamBytes = null;
        }
        return ms;
    }

    public static string MD5Encrypt(byte[] data)
    {
        MD5CryptoServiceProvider md5p = new MD5CryptoServiceProvider();
        byte[] hashBytes = md5p.ComputeHash(data);
        //
        string hashStr = string.Empty;
        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashStr += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
        return hashStr.PadLeft(32, '0');
    }

    public static bool IsMD5Match(byte[] data, string md5)
    {
        return MD5Encrypt(data) == md5;
    }

    public static string EncryptSHA1(string content, Encoding encode) {
        try {
            
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bts_in = encode.GetBytes(content);
            byte[] bts_out = sha1.ComputeHash(bts_in);
            sha1.Dispose();
            string res = System.BitConverter.ToString(bts_out);
            return res;

        } catch(System.Exception) {

            throw new System.Exception("SHA1 encryption error");
        }
    }
}
