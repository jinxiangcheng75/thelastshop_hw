using Spine.Unity;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using System.Collections.Generic;
using Spine;

public class SpineExportEditor
{

    [MenuItem("Spine/RenameSkeletonData")]
    public static void RenameSkeletonData()
    {
        //英雄 男
        renameSkeletonData("hero/manItems/", "SkeletonData", "man");
        //英雄 女
        renameSkeletonData("hero/womanItems/", "SkeletonData", "woman");
        //店主 男
        renameSkeletonData("shopowner/manItems/", "SkeletonData");
        //店主 女
        renameSkeletonData("shopowner/womanItems/", "SkeletonData");
        //人形通用
        renameSkeletonData("people_shape/", "SkeletonData");

        //非人形
        renameSkeletonData("monster/", "Monster");

        //宠物
        renameSkeletonData("pets", "Pet");

        //礼包小人
        renameSkeletonData("giftRoles","GiftRole_");

        //其他模型
        renameSkeletonData("modelEntities", "SkeletonData");

        AssetDatabase.Refresh();

        Debug.Log("RenameSkeletonData over");
    }


    static void renameSkeletonData(string relativePath, string skeletonDataName, string ex = "")
    {
        string path = StaticConstants.spinePath + relativePath;
        string[] assetPaths = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);

        var setting = AddressableAssetSettingsDefaultObject.Settings;

        for (int i = 0; i < assetPaths.Length; i++)
        {
            string assetPath = assetPaths[i];

            SkeletonDataAsset asset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(assetPath);
            var strs = assetPath.Replace("\\", @"/").Split('/');
            string newName = skeletonDataName + strs[strs.Length - 2];//SkeletonData_id号


            if (asset != null)
            {
                EditorUtility.DisplayProgressBar("等一下下下~~~", "正在读取asset文件更新group信息...", (float)i / assetPaths.Length);

                AssetDatabase.RenameAsset(assetPath, newName);
                var entry = AddressableAssetsEditor.GetAddressableAssetEntry(asset);

                if (entry == null)
                {
                    var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));
                    entry = setting.CreateOrMoveEntry(guid, setting.FindGroup("characters"), readOnly: false, postEvent: false);
                    setting.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
                }

                if (entry != null)
                {
                    entry.address = newName + ex;
                    entry.SetLabel("Spine", true);
                    Debug.Log(entry.AssetPath);
                }
                else
                {
                    Debug.Log("error : " + assetPath);
                }
            }
        }

        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Spine/Texture2dOpenIsReadable")]
    public static void OpenTexture2DReadAndWrite()
    {
        OpenTexture2DReadAndWriteByPath(StaticConstants.spinePath + "hero/");//英雄
        OpenTexture2DReadAndWriteByPath(StaticConstants.spinePath + "shopowner/");//店主
        OpenTexture2DReadAndWriteByPath(StaticConstants.spinePath + "people_shape/");//人形通用
        OpenTexture2DReadAndWriteByPath(StaticConstants.spinePath + "monster/");//怪物
        OpenTexture2DReadAndWriteByPath(StaticConstants.spinePath + "pets/");//宠物

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        Debug.Log("All Texture2D Read/Write Enabled is on");
    }

    private static void OpenTexture2DReadAndWriteByPath(string path)
    {
        string[] pngPaths = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);

        for (int i = 0; i < pngPaths.Length; i++)
        {
            string pngPath = pngPaths[i];

            EditorUtility.DisplayProgressBar("等一下下下~~~", "正在打开png文件的读写设置...", (float)i / pngPaths.Length);

            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(pngPath);

            ///-------------
            //ti.isReadable = false;
            //AssetDatabase.ImportAsset(pngPath);
            ///------------

            if (!ti.isReadable)
            {
                Debug.Log(pngPath);
                ti.isReadable = true;
                AssetDatabase.ImportAsset(pngPath);
            }
        }
    }

    [MenuItem("Spine/CheckTexture2dCompressFormat")]
    public static void CheckTexture2dCompressFormat()
    {
        CheckTexture2dCompressFormatByPath(StaticConstants.spinePath + "hero/");//英雄
        CheckTexture2dCompressFormatByPath(StaticConstants.spinePath + "shopowner/");//店主
        CheckTexture2dCompressFormatByPath(StaticConstants.spinePath + "people_shape/");//人形通用
        CheckTexture2dCompressFormatByPath(StaticConstants.spinePath + "monster/");//怪物
        CheckTexture2dCompressFormatByPath(StaticConstants.spinePath + "pets/");//宠物

        EditorUtility.ClearProgressBar();
        //AssetDatabase.Refresh();
        Debug.Log("All Texture2DCompressFormat Check out! ");
    }

    private static void CheckTexture2dCompressFormatByPath(string path) 
    {
        string[] pngPaths = Directory.GetFiles(path, "*.png", SearchOption.AllDirectories);

        for (int i = 0; i < pngPaths.Length; i++)
        {
            string pngPath = pngPaths[i];

            EditorUtility.DisplayProgressBar("等一下下下~~~", "正在检测png文件的压缩格式...", (float)i / pngPaths.Length);

            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(pngPath);

            string format = "Default";

#if UNITY_IOS
    format = "iOS";
#elif UNITY_ANDROID
            format = "Android";
#endif

            var tx2dFormat = ti.GetAutomaticFormat(format);

            ///-------------
            //ti.isReadable = false;
            //AssetDatabase.ImportAsset(pngPath);
            ///------------
            if (tx2dFormat != TextureImporterFormat.RGBA32)
            {
                Logger.error("此路径的图片压缩格式非RGBA32  ： " + pngPath);
            }
        }
    }

    [MenuItem("Spine/CreateDataAssetJsonBySlotDatas")]
    public static void CreateDataAssetJsonBySlotDatas() //根据dataAsset存储slotsData数据
    {
        string path = StaticConstants.spinePath;
        string[] assetPaths = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);

        List<object> depList = new List<object>();

        for (int i = 0; i < assetPaths.Length; i++)
        {
            string assetPath = assetPaths[i];

            SkeletonDataAsset asset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(assetPath);

            if (asset != null)
            {
                EditorUtility.DisplayProgressBar("等一下下~~", "正在读取asset文件生成对应json词条...", (float)i / assetPaths.Length);

                var entry = AddressableAssetsEditor.GetAddressableAssetEntry(asset);

                if (entry == null)
                {
                    Debug.LogError("没有该addressable资源 ： 路径： " + assetPath);
                }
                else
                {
                    Debug.Log(entry.address);

                    SkeletonData skeletonData = asset.GetSkeletonData(true);

                    var depItem = new Dictionary<string, System.Object>();

                    var slotsData = new Dictionary<int, Dictionary<string, string>>();

                    depItem.Add("address", entry.address);
                    depItem.Add("assetScale", asset.scale);
                    depItem.Add("slotsData", slotsData);

                    foreach (var item in skeletonData.Slots)
                    {
                        var slotData = new Dictionary<string, string>();
                        slotsData.Add(item.Index, slotData);

                        slotData.Add("name", item.Name);
                        slotData.Add("attachmentName", item.AttachmentName.IfNullThenEmpty());
                    }

                    depList.Add(depItem);
                }

            }

        }

        string content = MiniJSON.Json.Serialize(depList);
        string savePath = StaticConstants.spinePath + "spineDependency.json";
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        FileUtils.saveTxtFileUTF8(savePath, content);

        Object json = AssetDatabase.LoadAssetAtPath<Object>(savePath);
        AddressableAssetEntry jsonEntry = AddressableAssetsEditor.GetAddressableAssetEntry(json);

        if (jsonEntry == null)
        {
            var guid = AssetDatabase.AssetPathToGUID(savePath);
            var setting = AddressableAssetSettingsDefaultObject.Settings;
            jsonEntry = setting.CreateOrMoveEntry(guid, setting.FindGroup("characters"), readOnly: false, postEvent: false);
            setting.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, jsonEntry, true);
            jsonEntry.address = "spineDependency";
        }
        else
        {
            jsonEntry.address = "spineDependency";
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        Debug.Log("Create DataAssetJsonBySlotDatas is over  dataAsset count is " + depList.Count);
    }


    [MenuItem("Spine/CreateNeedCacheSpineAssetAddressesJson")]
    public static void CreateNeedCacheSpineAssetAddressesJson()
    {
        string[] addresses = { "SkeletonDataman", "SkeletonDatawoman" };

        string content = MiniJSON.Json.Serialize(addresses);
        string savePath = StaticConstants.spinePath + "spineNeedCacheAddresses.json";
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        FileUtils.saveTxtFileUTF8(savePath, content);

        Object json = AssetDatabase.LoadAssetAtPath<Object>(savePath);
        AddressableAssetEntry jsonEntry = AddressableAssetsEditor.GetAddressableAssetEntry(json);

        if (jsonEntry == null)
        {
            var guid = AssetDatabase.AssetPathToGUID(savePath);
            var setting = AddressableAssetSettingsDefaultObject.Settings;
            jsonEntry = setting.CreateOrMoveEntry(guid, setting.FindGroup("characters"), readOnly: false, postEvent: false);
            setting.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, jsonEntry, true);
            jsonEntry.address = "spineNeedCacheAddresses";
        }
        else
        {
            jsonEntry.address = "spineNeedCacheAddresses";
        }

        AssetDatabase.Refresh();
        Debug.Log("CreateNeedCacheSpineAssetAddressesJson over");

    }



    //[MenuItem("Spine/DelOldSpineFiles(切勿点击 导入资源时排重使用)")]
    public static void DelAllOldSpineFiles()
    {


        string manpath = StaticConstants.spinePath + "CurExportIds/hero_man.txt";
        string womanpath = StaticConstants.spinePath + "CurExportIds/hero_woman.txt";

        var text = AssetDatabase.LoadAssetAtPath<TextAsset>(manpath);

        string[] man_delFiles = text.text.Split('\n');

        text = AssetDatabase.LoadAssetAtPath<TextAsset>(womanpath);

        string[] woman_delFiles = text.text.Split('\n');


        //英雄 男
        DelOldSpineFilesByPath("hero/manItems/", man_delFiles);
        //英雄 女
        DelOldSpineFilesByPath("hero/womanItems/", woman_delFiles);

        AssetDatabase.Refresh();
        Debug.Log("all old Files is Clear");
    }


    static void DelOldSpineFilesByPath(string rootPath, string[] delFiles)
    {
        string path = StaticConstants.spinePath + rootPath;
        string[] dirPaths = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);

        for (int i = 0; i < dirPaths.Length; i++)
        {
            string dirPath = dirPaths[i];

            var strs = dirPath.Replace("\\", @"/").Split('/');
            string dirName = strs[strs.Length - 1];


            for (int k = 0; k < delFiles.Length; k++)
            {
                if (delFiles[k].Trim() == dirName.Trim())
                {
                    AssetDatabase.DeleteAsset(dirPath);
                    Debug.Log("Del File Path :" + dirPath);
                }
            }

        }
    }

    [MenuItem("Spine/HotfixRenameSkeletonData")]
    public static void HotfixRenameSkeletonData()
    {
        //英雄 男
        hotfixRenameSkeletonData("hero/manItems/", "SkeletonData", "man");
        //英雄 女
        hotfixRenameSkeletonData("hero/womanItems/", "SkeletonData", "woman");
        //店主 男
        hotfixRenameSkeletonData("shopowner/manItems/", "SkeletonData");
        //店主 女
        hotfixRenameSkeletonData("shopowner/womanItems/", "SkeletonData");
        //人形通用
        hotfixRenameSkeletonData("people_shape/", "SkeletonData");

        //非人形
        hotfixRenameSkeletonData("monster/", "Monster");

        //宠物
        hotfixRenameSkeletonData("pets", "Pet");

        //礼包小人
        hotfixRenameSkeletonData("giftRoles", "GiftRole_");

        //其他模型
        hotfixRenameSkeletonData("modelEntities", "SkeletonData");

        AssetDatabase.Refresh();

        Debug.Log("RenameSkeletonData over");
    }


    static void hotfixRenameSkeletonData(string relativePath, string skeletonDataName, string ex = "")
    {
        string path = StaticConstants.spinePath + relativePath;
        string[] assetPaths = Directory.GetFiles(path, "*.asset", SearchOption.AllDirectories);

        var setting = AddressableAssetSettingsDefaultObject.Settings;

        for (int i = 0; i < assetPaths.Length; i++)
        {
            string assetPath = assetPaths[i];

            SkeletonDataAsset asset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(assetPath);
            var strs = assetPath.Replace("\\", @"/").Split('/');
            string newName = skeletonDataName + strs[strs.Length - 2];//SkeletonData_id号


            if (asset != null)
            {
                EditorUtility.DisplayProgressBar("等一下下下~~~", "正在读取asset文件更新group信息...", (float)i / assetPaths.Length);

                AssetDatabase.RenameAsset(assetPath, newName);
                var entry = AddressableAssetsEditor.GetAddressableAssetEntry(asset);

                if (entry == null)
                {
                    var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));
                    entry = setting.CreateOrMoveEntry(guid, setting.FindGroup("RemoteAssets"), readOnly: false, postEvent: false);
                    setting.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);

                    if (entry != null)
                    {
                        entry.address = newName + ex;
                        entry.SetLabel("Spine", true);
                        Debug.Log("hotfixRenameSkeletonData  --" + entry.AssetPath);
                    }
                    else
                    {
                        Debug.Log("hotfixRenameSkeletonData error : " + assetPath);
                    }
                }
            }
        }

        EditorUtility.ClearProgressBar();
    }

}
