using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System;

public class PlatformBuildEditor
{
    static void addToPatchList(string filePath, kAssetType type, IList<PatchFileItem> assetList)
    {
        byte[] bts = null;
        FileUtils.loadFile(filePath, out bts);
        var item = new PatchFileItem()
        {
            fileName = filePath,
            type = type,
            hash = FileUtils.GetBytesMD5(bts),
            size = bts.Length
        };
        assetList.Add(item);
    }

    [MenuItem("Build/Pack Configs-->NEW")]
    static void PackConfigs()
    {
        ClearPersistentData();

        if (Directory.Exists(ResPathUtility.getstreamingAssetsPath(false) + "cfgs/"))
        {
            DirectoryInfo di = new DirectoryInfo(ResPathUtility.getstreamingAssetsPath(false) + "cfgs/");
            di.Delete(true);
        }
        Directory.CreateDirectory(ResPathUtility.getstreamingAssetsPath(false) + "cfgs/");
        string configFolder = "Assets/Configs/";
        string[] assetPaths = Directory.GetFiles(configFolder, "*.csv", SearchOption.AllDirectories);
        EditorUtility.DisplayProgressBar("执行中", "查找配置文件", 0);
        var setting = AddressableAssetSettingsDefaultObject.Settings;

        LocalCsvCatalog csvCatalog = new LocalCsvCatalog();
        for (int i = 0; i < assetPaths.Length; i++)
        {
            string assetPath = assetPaths[i];
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            string laststr = "";
            if (asset != null)
            {
                EditorUtility.DisplayProgressBar("执行中", $"文件{asset.name},替换文本中的[]", (float)i / (float)(assetPaths.Length - 1));
                laststr = asset.text.Replace('[', '{');
                laststr = laststr.Replace(']', '}');
                laststr = laststr.Replace("\r\n", "\n");
                try
                {
                    FileStream stream = new FileStream(assetPath, FileMode.OpenOrCreate);
                    byte[] data = Encoding.UTF8.GetBytes(laststr);
                    stream.SetLength(0);
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                    stream.Close();
                    // 保存到streamingAssets
                    string md5 = FileUtils.GetBytesMD5(data);
                    string tpath = ResPathUtility.getstreamingAssetsPath(false) + $"cfgs/{asset.name}{md5}.csv";

                    csvCatalog.csvFileList.Add(asset.name, new CsvCfgCatalog(asset.name, md5));

                    // fileMd5list.Add(asset.name, md5);
                    FileStream stream2 = new FileStream(tpath, FileMode.OpenOrCreate);
                    stream2.SetLength(0);
                    stream2.Write(data, 0, data.Length);
                    stream2.Flush();
                    stream2.Close();

                }
                catch (Exception ex)
                {
                    EditorUtility.ClearProgressBar();
                    Debug.LogError("========配置文件出现错误=======");
                    Debug.LogError(ex);
                    Debug.LogError("========错误文件为：" + asset);
                    return;
                }
                AssetDatabase.Refresh();
            }
            //生成文件名对应 MD5文件
            string CsvVersionPath = ResPathUtility.getstreamingAssetsPath(false) + "cfgs/CsvCatalog.txt";
            SaveManager.Save<LocalCsvCatalog>(csvCatalog, CsvVersionPath);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
        Debug.Log("========配置文件整理完成=======");
    }


    [MenuItem("Tools/ClearPlayerPrefs")]
    static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        ClearPersistentData();
    }

    //[MenuItem("")]
    static void ClearPersistentData()
    {
        string path = ResPathUtility.getpersistentDataPath(false);
        if (Directory.Exists(path))
        {
            DirectoryInfo di = new DirectoryInfo(path);
            di.Delete(true);
        }
        else
        {
            Directory.CreateDirectory(path);
        }
    }
}