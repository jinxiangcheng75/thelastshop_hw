using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Text;

public class AddressablesManager : TSingleton<AddressablesManager>
{
    LoadingView _loadingview;
    Action<bool,bool> UpdateEndCallBack = null;
    //更新加载完成
    private void UpdateLoadEnd(bool hasupdate,bool hasDownload)
    {
        PlayerPrefs.DeleteKey("needUpdateAssets");
        // FGUI.inst.hideLoading();
        UpdateEndCallBack?.Invoke(hasupdate, hasDownload);
    }
    //进入更新
    public void startUpdate(Action<bool,bool> endcallback)
    {

        // Addressables.LoadAssetsAsync<TextAsset>("Assets/GameAssets/lua/class.lua.txt", (t) =>
        // {
        Debug.Log("AddressableConfig.addressableRuntimePath = " + AddressableConfig.addressableRuntimePath);
        // });
        UpdateEndCallBack = endcallback;
        //清理资源(界面，图集，场景模型等)

        //显示loading进度条
        //   _loadingview = GUIManager.OpenView<LoadingView>();
        needUpdateAssets = false;
        if(!needUpdateAssets)
        {
          //  CheckUpdate();
        }
        //检查更新 catalog
        UpdateCatalog();

    }

    private bool needUpdateAssets = false;

    public async void CheckUpdate()
    {
        var inithandle = Addressables.InitializeAsync();

        await inithandle.Task;
        Logger.log("初始化完成addressable");
        var handle = Addressables.CheckForCatalogUpdates(false);  //下载目录
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            FGUI.inst.updateProgressText(LanguageManager.inst.GetValueByKey("检查更新文件..."));
            Logger.log("检查更新 20");
            List<string> catalogs = handle.Result;
            if (catalogs != null && catalogs.Count > 0)
            {
                Logger.log("检查更新40");
                var updateHandle = Addressables.UpdateCatalogs(catalogs, false);
                await updateHandle.Task;
                Logger.log("检查更新100");
                if (updateHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    needUpdateAssets = true;
                }
                else
                {
                    Debug.LogError("下载资源目录失败，一秒后重试");
                    //更新失败 一秒后继续更新
                    GameTimer.inst.AddTimer(1, 1, () =>
                    {
                        CheckUpdate();
                    });
                    return;
                }
            }
            else
            {
                needUpdateAssets = false;
                //UpdateLoadEnd(true, false);
                return;
            }
        }
    }

    private async void UpdateCatalog()
    {
        Logger.log("初始化addressable");
        var inithandle = Addressables.InitializeAsync();

        await inithandle.Task;
        Logger.log("初始化完成addressable");
        var handle = Addressables.CheckForCatalogUpdates(false);  //下载目录
        await handle.Task;
        
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            FGUI.inst.updateProgressText(LanguageManager.inst.GetValueByKey("检查更新文件..."));
            Logger.log("检查更新 20");
            List<string> catalogs = handle.Result;
            if (catalogs != null && catalogs.Count > 0)
            {
                Logger.log("检查更新40");
                var updateHandle = Addressables.UpdateCatalogs(catalogs, false);
                await updateHandle.Task;
                Logger.log("检查更新100");
                if (updateHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    needUpdateAssets = true;
                }
                else
                {
                    Debug.LogError("下载资源目录失败，一秒后重试");
                    //更新失败 一秒后继续更新
                    GameTimer.inst.AddTimer(1, 1, () =>
                    {
                        UpdateCatalog();
                    });
                    return;
                }
            }
            else
            {
                FGUI.inst.SetLoginBGVisible(true);
                UpdateLoadEnd(true, false);
                return;
            }
        }
        FGUI.inst.SetLoginBGVisible(true);
        if (needUpdateAssets)
        {
            PlayerPrefs.SetInt("needUpdateAssets", 1);
            GameTimer.inst.StartCoroutine(DownAssetImpl());
        }
        else
        {
            if (PlayerPrefs.HasKey("needUpdateAssets"))
            {
                PlayerPrefs.SetInt("needUpdateAssets", 1);
                GameTimer.inst.StartCoroutine(DownAssetImpl());
                return;
            }
            UpdateLoadEnd(false,false);
        }

    }

    public IEnumerator DownLoginAsset()
    {
        yield return new WaitForSeconds(1f);

        yield return null;

        AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync("loginAs");

        yield return getDownloadSize;
        long downloadSize = getDownloadSize.Result;
        Addressables.Release(getDownloadSize);

        Logger.log($"更新：需要更新登录缓存资源 资源大小= {downloadSize}");

        if (downloadSize > 0)
        {
            Addressables.ClearDependencyCacheAsync("loginAs");
            yield return new WaitForSeconds(0.2f);
            FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("下载更新..."), 0.8f, 0);

            AsyncOperationHandle downloadshandle = Addressables.DownloadDependenciesAsync("loginAs");
            while (!downloadshandle.IsDone)
            {
                if (downloadshandle.PercentComplete > 0)
                {
                    float percent = (0.4f + downloadshandle.PercentComplete * 0.1f) * 2f;
                    Debug.Log($"登录已经下载：{(int)(downloadSize * percent)}/{downloadSize}");

                    FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("下载更新...") + $"{(int)(percent * 100)}%", percent, 0.02f);
                }
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
            Addressables.Release(downloadshandle);
        }

        FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("下载更新...100%"), 1, 0.2f);

        FGUI.inst.RefreshLoginBG();
        yield return new WaitForSeconds(0.2f);
        //结束登陆资源更新
    }

    public IEnumerator DownAssetImpl()
    {
        yield return new WaitForSeconds(1f);
        //yield return Addressables.InitializeAsync();

        Logger.log("本地资源——0");
        yield return null;
        AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync("preload");  //更新资源添加“preload”标签
        // while (!handle.IsDone)
        // {
        //     Logger.log("检查本地资源——" + handle.PercentComplete.ToString("N2"));
        //     yield return null;
        // }
        yield return getDownloadSize;
        long downloadSize = getDownloadSize.Result;
        Addressables.Release(getDownloadSize);
        Logger.log($"更新：需要更新缓存资源资源,大小= {downloadSize}");
        if (downloadSize > 0)
        {
            Addressables.ClearDependencyCacheAsync("preload");
            yield return new WaitForSeconds(0.2f);
            FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("下载更新..."), 0, 0);

            AsyncOperationHandle downloadshandle = Addressables.DownloadDependenciesAsync("preload");
            while (!downloadshandle.IsDone)
            {
                if (downloadshandle.PercentComplete > 0)
                {
                    float percent = (downloadshandle.PercentComplete - 0.75f) * 4f;
                    Debug.Log($"已经下载：{(int)(downloadSize * percent)}/{downloadSize}");

                    FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("下载更新...") + $"{(int)(percent * 100)}%", percent, 0.02f);
                }
                yield return null;
            }

            FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("下载更新...100%"), 1f, 0.2f);
            yield return new WaitForSeconds(0.2f);
            Addressables.Release(downloadshandle);
        }

        //GameTimer.inst.StartCoroutine(DownLoginAsset());

        yield return new WaitForSeconds(0.2f);
        //结束更新
        UpdateLoadEnd(true,true);
    }
    ///////////////////////////////////////////////////////////////////////////////////////



}
