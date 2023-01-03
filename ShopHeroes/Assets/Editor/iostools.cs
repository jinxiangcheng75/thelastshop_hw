
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetProcessor : AssetPostprocessor
{
    static string guiPrefabPath = "Assets";
    //[MenuItem("Tools/统一图片压缩格式(IOS)")]
    static void CompressSelectTexture()
    {

        string[] allPath = AssetDatabase.FindAssets("t:Texture", new string[] { guiPrefabPath });

        for (int i = 0; i < allPath.Length; i++)
        {
            string assetpath = AssetDatabase.GUIDToAssetPath(allPath[i]);
            if (assetpath.StartsWith("Assets/Spine")) continue;
            if (assetpath.IndexOf("Assets/__Scenes") >= 0) continue;
            SetTextureFormat(assetpath);
            EditorUtility.DisplayProgressBar("批量处理图片", assetpath, (float)i / allPath.Length);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        Debug.Log("All done, count: " + allPath.Length);
    }

    static void SetTextureFormat(string assetpath)
    {
        // 根据路径获得文件目录，设置图集的packagingTag
        TextureImporter textureImporter;
        try
        {
            textureImporter = (TextureImporter)AssetImporter.GetAtPath(assetpath);
        }
        catch (System.Exception)
        {

            return;
        }


        if (textureImporter == null) return;

        // if (textureImporter.textureType != TextureImporterType.Default)
        // {
        //     Debug.LogWarning("This texture is not Default Type: " + assetpath, textureImporter);
        //     return;
        // }
        // if(assetPath.FindIndex("Spine/SpineExport")<0) return;
        // bool haveAlpha = textureImporter.DoesSourceTextureHaveAlpha();

        // textureImporter.alphaIsTransparency = haveAlpha;
        // textureImporter.mipmapEnabled = false;
        //textureImporter.isReadable = false;
        //textureImporter.textureType = TextureImporterType.Default;
        //textureImporter.wrapMode = TextureWrapMode.Clamp;
        // textureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
        // textureImporter.textureCompression = TextureImporterCompression.Compressed;

        //  // Android 端单独设置
        //  TextureImporterPlatformSettings settingAndroid = new TextureImporterPlatformSettings()
        //  {
        //      name = "Android",
        //      overridden = true,
        //      format = haveAlpha ? TextureImporterFormat.ETC2_RGBA8 : TextureImporterFormat.ETC_RGB4,
        //  };
        //  textureImporter.SetPlatformTextureSettings(settingAndroid);

        // IOS端单独设置
        TextureImporterPlatformSettings settingIOS = new TextureImporterPlatformSettings()
        {
            name = "iOS",
            overridden = true,//
            format = TextureImporterFormat.ASTC_6x6,
        };
        textureImporter.SetPlatformTextureSettings(settingIOS);
        textureImporter.SaveAndReimport();

        Debug.Log("Reimport texture done: " + assetpath);
    }

    /// <summary>
    /// full path 转 asset path
    /// </summary>
    public static string GetAssetPath(string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath))
            return "";

        fullPath = fullPath.Replace("\\", "/");
        return fullPath.StartsWith("Assets/") ?
            fullPath :
            "Assets" + fullPath.Substring(Application.dataPath.Length);
    }

    public static bool IsImage(string path)
    {
        string ext = Path.GetExtension(path);
        return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".tga";
    }

}

