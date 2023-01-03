using UnityEngine;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.GZip;
public class ZipHelper
{
    const int BUFFER_SIZE = 4096;
    //static byte[] ZipReadBuffer = new byte[BUFFER_SIZE];
    public static byte[] UnGZip2(byte[] byteArray)
    {
        GZipInputStream gzi = new GZipInputStream(new MemoryStream(byteArray));

        MemoryStream re = new MemoryStream(BUFFER_SIZE);
        int count;
        byte[] data = new byte[BUFFER_SIZE];
        while ((count = gzi.Read(data, 0, data.Length)) != 0)
        {
            re.Write(data, 0, count);
        }
        byte[] overarr = re.ToArray();
        return overarr;
    }

    public static byte[] UnGZip(byte[] dataBytes)
    {
        using (MemoryStream ms = new MemoryStream(dataBytes))
        {
            using (ZipInputStream s = new ZipInputStream(ms))
            {
                byte[] buffer = new byte[BUFFER_SIZE];
                ZipEntry theEntry;
                if ((theEntry = s.GetNextEntry()) != null)
                {
                    using (MemoryStream sms = new MemoryStream())
                    {
                        int size = 0;//file size buffer
                                     //byte[] data = new byte[size];
                        while (true)
                        {
                            size = s.Read(buffer, 0, buffer.Length);
                            if (size > 0)
                            {
                                sms.Write(buffer, 0, size);
                            }
                            else
                                break;
                        }
                        byte[] entryBytes = sms.ToArray();
                        return entryBytes;
                    }
                }
                return null;
            }
        }
    }

    public static void Decompress(byte[] dataBytes, string dir, System.Action<byte[], string, string> entryCallback, bool saveLocal)
    {
#if UNITY_EDITOR
        Logger.info("unzipProcess");
#endif
        using (MemoryStream ms = new MemoryStream(dataBytes))
        {
            //MemoryStream ms = new MemoryStream(dataBytes);
            using (ZipInputStream s = new ZipInputStream(ms))
            {
                ZipEntry theEntry;
                string folder = null;
#if UNITY_EDITOR
                folder = Application.streamingAssetsPath + "/" + dir;
#else
                folder = Application.persistentDataPath + "/" + dir;
#endif
                if (Directory.Exists(folder) == false)
                {
                    Directory.CreateDirectory(folder);
                }
                folder = folder + "/";
                byte[] buffer = new byte[BUFFER_SIZE];
                //string finalDir = null;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string dirName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    if (Directory.Exists(folder + dirName) == false)
                    {
                        Directory.CreateDirectory(folder + dirName);
                    }
                    if (fileName != string.Empty)
                    {

                        //using (FileStream streamWriter = File.Create(finalDir + theEntry.Name)) {
                        using (MemoryStream sms = new MemoryStream())
                        {
                            int size = 0;//file size buffer
                            //byte[] data = new byte[size];
                            while (true)
                            {
                                size = s.Read(buffer, 0, buffer.Length);
                                if (size > 0)
                                {
                                    sms.Write(buffer, 0, size);
                                }
                                else
                                    break;
                            }
                            byte[] entryBytes = sms.ToArray();
                            if (saveLocal)
                                FileUtils.saveFile(entryBytes, folder + fileName);

                            if (entryCallback != null)
                                entryCallback(entryBytes, folder, fileName);
                        }
                    }
                }
            }
        }
    }
    public static void Unzip(Stream zipStream, string savePath, bool overwrite = true)
    {
        using (ZipInputStream s = new ZipInputStream(zipStream))
        {
            ZipEntry entry;
            byte[] buffer = new byte[BUFFER_SIZE];
            while ((entry = s.GetNextEntry()) != null)
            {
                string entryPath = entry.Name;
                string dir = Path.GetDirectoryName(entryPath);
                string fileName = Path.GetFileName(entryPath);

                string saveDir = savePath + dir;
                if (!Directory.Exists(saveDir))
                    Directory.CreateDirectory(saveDir);

                using (FileStream fs = File.Create(saveDir + fileName))
                {
                    int size = 0;
                    while (true)
                    {
                        size = s.Read(buffer, 0, BUFFER_SIZE);
                        if (size <= 0)
                            break;
                        fs.Write(buffer, 0, size);
                    }
                    fs.Close();
                }
            }
            s.Close();
        }
    }

    public static void UnzipWithoutSave(Stream zipStream, System.Action<string, byte[]> entryCallback)
    {
        using (ZipInputStream s = new ZipInputStream(zipStream))
        {
            ZipEntry entry;
            byte[] buffer = new byte[BUFFER_SIZE];
            while ((entry = s.GetNextEntry()) != null)
            {
                string entryPath = entry.Name;
                string dir = Path.GetDirectoryName(entryPath);
                string fileName = Path.GetFileName(entryPath);

                using (MemoryStream ms = new MemoryStream())
                {
                    int size = 0;
                    while (true)
                    {
                        size = s.Read(buffer, 0, BUFFER_SIZE);
                        if (size <= 0)
                            break;
                        ms.Write(buffer, 0, size);
                    }
                    byte[] entryBytes = ms.ToArray();
                    entryCallback?.Invoke(fileName, entryBytes);
                    ms.Close();
                }
            }
            s.Close();
        }
    }

    public static void ZipFile(string folder, string savePath, string filters)
    {
        if (!Directory.Exists(folder))
        {
            Logger.error("ZipFile failed Directory not found: " + folder);
            return;
        }
        string[] fileNames = Directory.GetFiles(folder, filters, SearchOption.AllDirectories);
        using (ZipOutputStream s = new ZipOutputStream(File.Create(savePath)))
        {
            string[] splitParam = new string[1] { folder };
            s.SetLevel(9);
            byte[] buffer = new byte[4096];
            foreach (var file in fileNames)
            {
                string fullName = Path.GetFullPath(file);
                fullName = fullName.Replace("\\", "/");
                string entryName = fullName.Split(splitParam, System.StringSplitOptions.None)[1];
                ZipEntry entry = new ZipEntry(entryName);
                s.PutNextEntry(entry);
                using (FileStream fs = File.OpenRead(file))
                {
                    int wlen = 0;
                    do
                    {
                        wlen = fs.Read(buffer, 0, buffer.Length);
                        s.Write(buffer, 0, wlen);
                    } while (wlen > 0);
                    fs.Close();
                }
            }
            s.Finish();
            s.Close();
        }
    }
}
