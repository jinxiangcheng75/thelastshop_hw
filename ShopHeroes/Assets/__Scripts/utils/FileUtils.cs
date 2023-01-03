using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

public class FileUtils
{

    public static void checkAndCreateDirectory(string path)
    {
        if (Directory.Exists(path) == false)
            Directory.CreateDirectory(path);
    }

    public static void copyFile(string src, string dest, bool overwrite = true)
    {
        try
        {
            File.Copy(src, dest, overwrite);
        }
        catch (System.Exception ex)
        {

            Debug.LogException(ex);
        }
    }

    public static void copyFolderFiles(string srcDir, string destDir, string pattern, bool overwrite = true)
    {
        FileInfo[] fis = (new DirectoryInfo(srcDir)).GetFiles(pattern, SearchOption.TopDirectoryOnly);
        foreach (var fi in fis)
        {
            string fromPath = srcDir + fi.Name;
            string toPath = destDir + fi.Name;
#if UNITY_EDITOR
            Logger.log("copyFolderFiles src:" + fromPath + " dest:" + toPath);
#endif
            copyFile(fromPath, toPath, true);
        }
    }

    public static string loadTxtFile(string path)
    {
        if (File.Exists(path) == false)
            return null;
        string str = null;
        using (StreamReader sr = new StreamReader(path))
        {
            str = sr.ReadToEnd();
            sr.Close();
        }
        return str;
    }

    public static void saveTxtFileDefault(string path, string content)
    {
        byte[] bytes = System.Text.Encoding.Default.GetBytes(content);
        saveFile(bytes, path);
    }

    public static void saveTxtFileUTF8(string path, string content)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);
        saveFile(bytes, path);
    }

    public static void saveTxtFileUnicode(string path, string content)
    {
        byte[] bytes = System.Text.Encoding.Unicode.GetBytes(content);
        saveFile(bytes, path);
    }

    public static void saveFile(byte[] bytes, string path)
    {
        saveFile(bytes, path, 0, bytes.Length);
    }
    public static void saveFile(byte[] bytes, string path, int offset, int length)
    {
        using (FileStream fs = File.Open(path, FileMode.Create))
        {
            fs.Write(bytes, offset, length);
            fs.Flush();
            fs.Close();
        }
    }

    public static void saveFileCheckDir(byte[] bytes, string path)
    {
        string dirPath = Path.GetDirectoryName(path);
        FileUtils.checkAndCreateDirectory(dirPath);
        saveFile(bytes, path);
    }

    public static void loadFile(string path, out byte[] bytes)
    {
        if (File.Exists(path) == false)
        {
            bytes = null;
            return;
        }
        FileStream fs = null;
        bytes = null;
        try
        {
            fs = File.Open(path, FileMode.Open);
            bytes = new byte[(int)fs.Length];
            fs.Read(bytes, 0, (int)fs.Length);
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            if (fs != null)
                fs.Close();
        }
    }

    public static void checkDelete(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }

    public static void checkDeleteFolder(string path, string pattern)
    {
        if (Directory.Exists(path))
        {
            string[] ss = Directory.GetFiles(path, pattern);
            string fs = null;
            for (int i = 0; i < ss.Length; i++)
            {
                fs = ss[i];
                if (fs != null)
                    checkDelete(fs);
            }
        }
    }

    public static void deleteFolderFiles(string path, string pattern)
    {
        DirectoryInfo di = new DirectoryInfo(path);
        FileInfo[] fis = null;
        if (string.IsNullOrEmpty(pattern))
            fis = di.GetFiles();
        else
            fis = di.GetFiles(pattern);

        foreach (var fi in fis)
        {
            fi.Delete();
        }
    }

    public static void checkDeleteFolderItself(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path);
        }
    }

    public static void serializeData(System.Object _obj, string _savePath)
    {
        System.Object obj = _obj;//new V[m_dict.Count];
        //System.Type tp = _obj.GetType();
        string savePath = _savePath;
        if (File.Exists(savePath))
            File.Delete(savePath);
        using (Stream s = File.Open(savePath, FileMode.Create))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(s, obj);
            s.Close();
        }
    }

    public static System.Object deserializeData(string _savePath)
    {
        string spath = _savePath;
        //System.Type tp = typeof(T);
        byte[] bytes;
        if (File.Exists(spath) == false)
            return null;
        FileUtils.loadFile(spath, out bytes);
        Stream s = new MemoryStream(bytes);
        BinaryFormatter bf = new BinaryFormatter();
        object o = bf.Deserialize(s);
        //System.Object dpList = o as List<T>;
        s.Close();
        return o;
    }

    public static System.Object deserializeDataBytes(byte[] bytes)
    {
        if (bytes == null)
            return null;
        Stream s = new MemoryStream(bytes);
        BinaryFormatter bf = new BinaryFormatter();
        object o = bf.Deserialize(s);
        //System.Object dpList = o as List<T>;
        s.Close();
        return o;
    }

    public static void serializeObjectWithEncrypto(System.Object _obj, string _savePath)
    {
        System.Object obj = _obj;//new V[m_dict.Count];
        //System.Type tp = _obj.GetType();
        string savePath = _savePath;
        if (File.Exists(savePath))
            File.Delete(savePath);
        using (Stream s = File.Open(savePath, FileMode.Create))
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = ASCIIEncoding.ASCII.GetBytes(des_key_key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(des_key_iv);
            CryptoStream cs = null;
            byte[] bytes = null;
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            byte[] srcBytes = ms.ToArray();
            bool okWrite = false;
            ms.Seek(0, SeekOrigin.Begin);
            try
            {
                cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(srcBytes, 0, srcBytes.Length);
                cs.FlushFinalBlock();
                okWrite = true;
            }
            catch (System.Exception ex)
            {

                Debug.LogException(ex);
            }
            finally
            {
                if (cs != null)
                    cs.Close();
            }

            if (okWrite)
            {
                bytes = ms.ToArray();
                s.Write(bytes, 0, bytes.Length);
            }
            ms.Close();
            s.Close();
        }
    }

    public static System.Object desrializeObjectWithDecrypto(string _savePath)
    {
        byte[] bytes = null;
        FileUtils.loadFile(_savePath, out bytes);
        if (bytes == null)
            return null;

        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        des.Key = ASCIIEncoding.ASCII.GetBytes(des_key_key);
        des.IV = ASCIIEncoding.ASCII.GetBytes(des_key_iv);
        CryptoStream cs = null;
        MemoryStream ms = new MemoryStream();
        System.Object o = null;
        try
        {
            cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();
            ms.Seek(0, SeekOrigin.Begin);
            BinaryFormatter bf = new BinaryFormatter();
            o = bf.Deserialize(ms);
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            if (cs != null)
                cs.Close();
            if (ms != null)
                ms.Close();
        }
        return o;
    }

    private static string _enc;
    public static void SetEnc(string enc)
    {
        _enc = enc;

#if UNITY_EDITOR
        Logger.log("setEnc:" + enc);
#endif

    }

    public static void SetFirstEnc(int st) {
        SetEnc(GenFirstEnc(st));
    }

    public static string GenFirstEnc (int st) {
        System.DateTime dt = TimeUtils.getDateTimeBySecs(st);
        string dateStr = null;
        if(dt.Hour >= 23) {
            //dt = new System.DateTime(lst + 3600 * 1000);
            dt = dt.AddSeconds(3600);
        }
        dateStr = dt.ToLocalTime().ToString("yyyy-MM-dd");//HH:mm:ss:ms
        char[] cts = new char[8];
        cts[0] = dateStr[1];
        cts[1] = dateStr[3];
        cts[2] = dateStr[6];
        cts[3] = dateStr[0];
        cts[4] = dateStr[4];
        cts[5] = dateStr[9];
        cts[6] = dateStr[1];
        cts[7] = dateStr[5];
        string ctsStr = new string(cts);

#if UNITY_EDITOR
        Logger.info("cts:" + ctsStr);
#endif
        return ctsStr;
    }

    private static string _bindenc;
    public static void SetBindEnc(string bindenc)
    {
        _bindenc = bindenc;
#if UNITY_EDITOR
        Logger.log("SetBindEnc:" + _bindenc);
#endif

    }

    public static string ReqEnc(string req)
    {
        return reqEncImpl(req, _enc);
    }

    public static byte[] ReqEnc(byte[] reqBytes) {
        return reqEncImpl(reqBytes, _enc);
    }

    public static string BindRoleEnc(string req)
    {
        return reqEncImpl(req, _bindenc);
    }

    private static string reqEncImpl(string req, string reqEnc)
    {
        StringBuilder ret = new StringBuilder();
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                //des.Key = ASCIIEncoding.ASCII.GetBytes(reqEnc);
                //des.IV = ASCIIEncoding.ASCII.GetBytes(des_key_iv);
                des.Key = UTF8Encoding.UTF8.GetBytes(reqEnc);
                des.IV = UTF8Encoding.UTF8.GetBytes(des_key_iv);
                byte[] bts = Encoding.UTF8.GetBytes(req);
                //MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(bts, 0, bts.Length);
                cs.FlushFinalBlock();
                byte[] msbts = ms.ToArray();
                for (int i = 0; i < msbts.Length; i++)
                {
                    ret.AppendFormat("{0:x2}", msbts[i]);
                }
                cs.Close();
                ms.Close();
            }

#if UNITY_EDITOR
            Logger.log("ReqEnc:" + ret.ToString());
            ret.ToString();
#endif

        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
        return ret.ToString();
    }

    private static byte[] reqEncImpl (byte[] reqBytes, string reqEnc) {
        StringBuilder ret = new StringBuilder();
        byte[] encBytes = null;
        try {
            using(MemoryStream ms = new MemoryStream()) {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                //des.Key = ASCIIEncoding.ASCII.GetBytes(reqEnc);
                //des.IV = ASCIIEncoding.ASCII.GetBytes(des_key_iv);
                des.Key = UTF8Encoding.UTF8.GetBytes(reqEnc);
                des.IV = UTF8Encoding.UTF8.GetBytes(des_key_iv);
                byte[] bts = reqBytes;
                //MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(bts, 0, bts.Length);
                cs.FlushFinalBlock();
                encBytes = ms.ToArray();
                cs.Close();
                ms.Close();
            }

        } catch(System.Exception e) {
            Debug.LogException(e);
        }
        return encBytes;
    }


    public static string ReqDec(string req)
    {
        return ReqDecImpl(req, _enc);
    }

    public static string BindRoleDec(string req)
    {
        return ReqDecImpl(req, _bindenc);
    }

    private static string ReqDecImpl(string req, string reqEnc)
    {
        string ret = null;

        try
        {

            MemoryStream ms = new MemoryStream();

            byte[] bts = new byte[req.Length / 2];
            for (int i = 0; i < req.Length / 2; i++)
            {
                int b = (System.Convert.ToInt32(req.Substring(i * 2, 2), 16));
                bts[i] = (byte)b;
            }

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //des.Key = ASCIIEncoding.ASCII.GetBytes(reqEnc);
            //des.IV = ASCIIEncoding.ASCII.GetBytes(des_key_iv);
            des.Key = UTF8Encoding.UTF8.GetBytes(reqEnc);
            des.IV = UTF8Encoding.UTF8.GetBytes(des_key_iv);
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(bts, 0, bts.Length);
            cs.FlushFinalBlock();
            byte[] msbts = ms.ToArray();
            ret = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            cs.Close();
            ms.Close();
        }
        catch (System.Exception e)
        {

            Debug.LogException(e);
        }

#if UNITY_EDITOR
        Logger.log("Dec:" + ret);
#endif

        return ret;
    }

    const string des_key = "asdfefswg4ewltkjerterrgfsdf";
    static string des_key_key = des_key[6].ToString() +
                                des_key[0].ToString() +
                                des_key[17].ToString() +
                                des_key[10].ToString() +
                                des_key[20].ToString() +
                                des_key[8].ToString() +
                                des_key[4].ToString() +
                                des_key[12].ToString();

    static string des_key_key2 = des_key[7].ToString() +
                            des_key[6].ToString() +
                            des_key[9].ToString() +
                            des_key[1].ToString() +
                            des_key[20].ToString() +
                            des_key[3].ToString() +
                            des_key[13].ToString() +
                            des_key[15].ToString();

    static string des_key_iv = des_key[16].ToString() +
                                des_key[10].ToString() +
                                des_key[7].ToString() +
                                des_key[1].ToString() +
                                des_key[2].ToString() +
                                des_key[18].ToString() +
                                des_key[14].ToString() +
                                des_key[21].ToString();
    static int _DkkIndex = 0;
    static string[] Dkks = new string[] {
        des_key_key,
        des_key_key2,
    };
    public static void SetDkkIndex (int idx) {
        _DkkIndex = idx;
#if UNITY_EDITOR
        Logger.log("dkkindex:" + idx);
        Logger.log("dkk0" + des_key_key);
        Logger.log("dkk1" + des_key_key2);
        Logger.log("dkiv" + des_key_iv);
#endif
        SetEnc(Dkks[_DkkIndex]);
    }

    public static void OverrideEncByDkk () {
        SetDkkIndex(_DkkIndex);
    }

    public static int GetDkkIndex () {
        return _DkkIndex;
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

            Debug.LogException(ex);
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
            Debug.LogError(ex.StackTrace);
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


    public static string GetBytesMD5(byte[] fileBytes)
    {
        byte[] bytes = fileBytes;
        //encrypt bytes
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);
        //hash str
        string hashStr = "";
        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashStr += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
        return hashStr.PadLeft(32, '0');
    }

    public static string ToHexString (byte[] bts) {
        var hexStr = "";
        for(int i = 0; i < bts.Length; i++) {
            hexStr += System.Convert.ToString(bts[i], 16).PadLeft(2, '0');
        }
        return hexStr;
    }

#if UNITY_EDITOR

    public static void CopyDirectory(string src, string dst)
    {
        string[] files = Directory.GetFiles(src);
        string[] dirs = Directory.GetDirectories(src);
        if (!Directory.Exists(dst))
            Directory.CreateDirectory(dst);
        //
        foreach (string f in files)
        {
            string tp = Path.Combine(dst, Path.GetFileName(f));
            Logger.log("ff:" + tp);
            File.Copy(f, tp, true);
        }
        foreach (string d in dirs)
        {
            string tp = Path.Combine(dst, Path.GetFileName(d));
            Logger.log("dd:" + tp);
            CopyDirectory(d, tp);
        }
    }

    public static void EnumeratorDirectoryFiles(string path, System.Action<string> processFile, string pattern = "*.*", bool isUnityAssetPath = true)
    {
        DirectoryInfo di = new DirectoryInfo(path);
        FileInfo[] fis = di.GetFiles(pattern, SearchOption.AllDirectories);
        string[] sp_param = new string[1] { "/Assets/" };
        if (fis != null)
        {

            for (int i = 0; i < fis.Length; i++)
            {
                FileInfo fi = fis[i];
                string fpath = fi.FullName.Replace("\\", "/");
                if (isUnityAssetPath)
                {
                    string[] res = fpath.Split(sp_param, System.StringSplitOptions.None);
                    if (res != null && res.Length > 1)
                    {
                        fpath = "Assets/" + res[1];
                    }
                }
                processFile(fpath);
            }
        }
    }
#endif
}