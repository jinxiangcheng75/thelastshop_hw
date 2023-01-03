using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Text;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class CsvCfgCatalog
{
    public string fileName;
    public string fileMd5;
    public string fileFullName;
    public CsvCfgCatalog() { }
    public CsvCfgCatalog(string filenme, string filemd5)
    {
        fileName = filenme;
        fileMd5 = filemd5;
        fileFullName = filenme + filemd5;
    }
}

[Serializable]
public class LocalCsvCatalog
{
    public Dictionary<string, CsvCfgCatalog> csvFileList = new Dictionary<string, CsvCfgCatalog>();
}
public class CsvCfgCatalogMgr : TSingleton<CsvCfgCatalogMgr>
{
    Dictionary<string, CsvCfgCatalog> remoteCsvList = new Dictionary<string, CsvCfgCatalog>();//远程
    LocalCsvCatalog localCsvCatalog;
    private Action<bool> CheckAndUpdateEnd = null;

    Dictionary<string, string> csvMap = new Dictionary<string, string>();
    public void ReloadCSVFile(System.Action loadEnd)
    {
        GameTimer.inst.StartCoroutine(LoadAllCsvFiles(loadEnd));
    }
    IEnumerator LoadAllCsvFiles(System.Action loadEnd)
    {
        
        Logger.log("loadLocalCatalog: 12.2");
        yield return null;
        if (localCsvCatalog == null)
            yield return loadLocalCatalog(false);
        csvMap.Clear();
        Logger.log("loadLocalCatalog: 13");
        yield return new WaitForSeconds(0.2f);
        FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("进入壁垒..."), 0.3f, 0.2f);
        yield return new WaitForSeconds(0.2f);
        float index = 0;
        float count = localCsvCatalog.csvFileList.Count;
        Logger.log("loadLocalCatalog: 14");
        foreach (CsvCfgCatalog cfglog in localCsvCatalog.csvFileList.Values)
        {
            string configpath = ResPathUtility.getpersistentDataPath(false) + "cfgs/" + cfglog.fileFullName + ".csv";
            if (File.Exists(configpath) != false)
            {
                Logger.log("loadLocalCatalog: 15" + configpath);
                StreamReader sr = new StreamReader(configpath);
                string csv = sr.ReadToEnd();
                sr.Close();
                csvMap.Add(cfglog.fileName, csv);
            }
            else
            {

                configpath = ResPathUtility.getstreamingAssetsPath(true) + "/cfgs/" + cfglog.fileFullName + ".csv";
                Logger.log("loadLocalCatalog: 16" + configpath);
                UnityWebRequest request = UnityWebRequest.Get(configpath);
                yield return request.SendWebRequest();
                if (!request.isHttpError && !request.isNetworkError)
                {
                    csvMap.Add(cfglog.fileName, request.downloadHandler.text);
                }
            }
            Logger.log("loadLocalCatalog: 17");
            index++;
            FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("进入壁垒..."), 0.3f + (index / count) * 0.4f, 0);
            if (index % 3 == 0)
                yield return null;
        }
        loadEnd?.Invoke();
    }
    //初始化
    public override void init()
    {
        base.init();
        Helper.AddNetworkRespListener(MsgType.Response_Csv_List_Cmd, onResponseCsvList);
        Helper.AddNetworkRespListener(MsgType.Response_Csv_Load_Cmd, onResponseCsvLoad);
    }
    public void InitCsvCatalog(Action<bool> callback)
    {
        CheckAndUpdateEnd = callback;
        //加载本地配置目录 并检查list
        GameTimer.inst.StartCoroutine(loadLocalCatalog());
        //
    }
    public void ClearCsvlogCache()
    {
        string filefullpath = ResPathUtility.getpersistentDataPath(false) + "cfgs/CsvCatalog.txt";
        if(File.Exists(filefullpath) != false)
        {

        }
    }
    IEnumerator loadLocalCatalog(bool ischeck = true)
    {
        Logger.log("loadLocalCatalog: 1");
        yield return null;
        FGUI.inst.updateProgressText(LanguageManager.inst.GetValueByKey("检查更新..."));

        string currlocalappv = GameSettingManager.appVersion.Replace('.', '0');
        string lastappv = SaveManager.inst.GetString("CsvAppV");
        bool useLocal = true;
        if (string.IsNullOrEmpty(lastappv))
            useLocal = true;
        else
            useLocal = int.Parse(currlocalappv) > int.Parse(lastappv);
        //判断缓存目录
        string filefullpath = ResPathUtility.getpersistentDataPath(false) + "cfgs/CsvCatalog.txt";
        if (!useLocal && File.Exists(filefullpath) != false)
        {
            Logger.log("loadLocalCatalog: 2");

            SaveManager.Load<LocalCsvCatalog>(filefullpath, ref localCsvCatalog);
        }
        else
        {
            if(File.Exists(filefullpath) != false)
            {
                removeFile(filefullpath);
            }
            Logger.log("loadLocalCatalog: 3");
            filefullpath = ResPathUtility.getstreamingAssetsPath(true) + "/cfgs/CsvCatalog.txt";
            UnityWebRequest request = UnityWebRequest.Get(filefullpath);
            yield return request.SendWebRequest();
            if (!request.isHttpError && !request.isNetworkError)
            {
                Stream s = new MemoryStream(request.downloadHandler.data);
                BinaryFormatter bf = new BinaryFormatter();
                object o = bf.Deserialize(s);
                s.Close();
                localCsvCatalog = (LocalCsvCatalog)o;
            }
        }
        Logger.log("loadLocalCatalog: 4");
        if (localCsvCatalog == null)
        {
            throw new System.Exception("配置文件目录加载失败。检查CsvCatalog文件是否存在!");
        }
        if (ischeck)
            yield return CheckLocakCsvUpdateList();
    }


    //获得对应csv文件全名（加了MD5的文件名）
    public string GetCsvbyName(string filename)
    {
        if (!csvMap.ContainsKey(filename))
        {
            // 
            if (filename == "translate_start")
            {
                return Resources.Load<TextAsset>("translate_start").text;
            }
            else
            {
                throw new System.Exception($"配置文件目录加载失败。检查({filename})文件是否存在!");
            }
        }
        return csvMap[filename];
    }

    public bool IsContainsCsvByName(string filename) 
    {
#if UNITY_EDITOR
        return File.Exists("Assets/Configs/" + filename + ".csv");
#endif
        return csvMap.ContainsKey(filename);
    }

    //开始检查更新
#region 更新配置
    public void SaveRemoteCatalog2Local()
    {
        foreach (var rcatalog in remoteCsvList)
        {
            if (localCsvCatalog.csvFileList.ContainsKey(rcatalog.Key))
            {
                localCsvCatalog.csvFileList[rcatalog.Key].fileFullName = rcatalog.Value.fileFullName;
            }
            else
            {
                localCsvCatalog.csvFileList.Add(rcatalog.Key, rcatalog.Value);
            }
        }
        string filefullpath = ResPathUtility.getpersistentDataPath(false) + "cfgs/CsvCatalog.txt";
        SaveManager.Save<LocalCsvCatalog>(localCsvCatalog, filefullpath);
        string localappv = GameSettingManager.appVersion.Replace('.', '0');
        SaveManager.inst.SaveString("CsvAppV", localappv);
    }
    public void GetServerCSV()
    {
        loadRemoteCsvListed = false;
        //获取服务器配置列表
        NetworkEvent.SendRequest(
            new NetworkRequestWrapper()
            {
                req = new Request_Csv_List()
            }
        );
    }
    public void LoadServerCsv(string msgstr)
    {
        // FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("更新配置"), 0.3f, 0.2f);
        //下载Csv更新
        NetworkEvent.SendRequest(
            new NetworkRequestWrapper()
            {
                req = new Request_Csv_Load()
                {
                    csvFileNameList = msgstr
                }
            }
        );
    }

    List<string> inequableList = new List<string>();
    List<string> downloadCache = new List<string>();
    bool loadRemoteCsvListed = false;
    bool loadCsvFileEnd = false;
    //对比本地文件
    private IEnumerator CheckLocakCsvUpdateList()
    {
        inequableList.Clear();
        remoteCsvList.Clear();
        //请求服务器配置目录
        GetServerCSV();
        Logger.log("loadLocalCatalog: 5");
        while (!loadRemoteCsvListed)
        {
            yield return null;
        }
        Logger.log("loadLocalCatalog: 6");
        //检查差异并检查本地文件
        foreach (var remote in remoteCsvList)
        {
            if (localCsvCatalog.csvFileList.ContainsKey(remote.Key))
            {
                if (localCsvCatalog.csvFileList[remote.Key].fileFullName != remote.Value.fileFullName)
                {
                    Logger.log("检测到不一样的文件 ： " + remote.Value.fileFullName);
                    inequableList.Add(remote.Key);
                }
            }
            else
            {
                inequableList.Add(remote.Key);
            }
        }
        Logger.log("loadLocalCatalog: 7");
        yield return null;
        if (inequableList.Count <= 0)
        {
            Logger.log("loadLocalCatalog: 8");
            loadCsvFileEnd = true;
            CheckAndUpdateEnd?.Invoke(true);
            yield break;
        }
        else
        {
            Logger.log("loadLocalCatalog: 9");
            Logger.log("开始更新配置表 ");
            HttpRequestConst.SetDefaultTimeout();
            //更新配置文件
            FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("加载文件"), 0, 0);
            yield return null;
            // FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("更新配置"), 0.1f, 0.2f);
            //请求下载
            loadCsvFileEnd = false;
            // StringBuilder str = new StringBuilder();
            // str.Append(inequableList[0]);
            // for (int i = 1; i < inequableList.Count; i++)
            // {
            //     str.Append("|" + inequableList[i]);
            // }
            // LoadServerCsv(str.ToString());
            // str.Clear();

            StartLoadCsvFile(inequableList);
        }
        while (!loadCsvFileEnd)
        {
            yield return null;
        }
        Logger.log("下载配置表完成！！！", "#ff0000");
        HttpRequestConst.SetDefaultTimeout(20000);
        SaveRemoteCatalog2Local();
        PlatformManager.inst.GameHandleEventLog("Update_CfgEnd", "");
        Logger.log("loadLocalCatalog: 10");
        CheckAndUpdateEnd?.Invoke(true);
    }

    void StartLoadCsvFile(List<string> filelist)
    {
        GameTimer.inst.StopCoroutine(startLoadFile(null));
        GameTimer.inst.StartCoroutine(startLoadFile(filelist));
    }

    IEnumerator startLoadFile(List<string> filelist)
    {
        int targetnumber = filelist.Count;
        int currindex = 0;
        loadnum = 0;
        foreach (string csvfile in filelist)
        {
            yield return null;

            if (downloadCache.Contains(csvfile))
            {
                loadnum++;
                currindex++;
                continue;
            }

            LoadServerCsv(csvfile); //下载文件
            currindex += 1;
            float time = 0;
            while (loadnum != currindex)
            {
                if (loadnum == -1)
                {
                    Debug.Log("下载csv 文件失败，重新下载：" + csvfile);
                    FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("加载文件中，请耐心等待！"), 0, 0.1f);
                    FGUI.inst.setWifiObj(true, 0.38f);
                    yield return new WaitForSeconds(2f);
                    time = 0;
                    // yield return startLoadFile(filelist);
                    StartLoadCsvFile(filelist);
                    //下载失败重新下载
                    yield break;
                }
                time += Time.deltaTime;
                if (time > 35f)
                {
                    FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("加载文件中，请耐心等待！"), 0, 0.1f);
                    FGUI.inst.setWifiObj(true, 0.38f);
                    yield return new WaitForSeconds(2f);
                    time = 0;
                    Logger.log("下载csv 文件失败，重新下载：" + csvfile);
                    StartLoadCsvFile(filelist);
                    yield break;
                }
                yield return null;
            }
            Logger.log("下载csv 文件：" + csvfile);
            FGUI.inst.setWifiObj(false);
            FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("加载文件") + $"({currindex}/{targetnumber})", ((float)currindex / (float)targetnumber), 0.1f);
            yield return new WaitForSeconds(0.10f);
        }
        yield return new WaitForSeconds(0.2f);
        loadCsvFileEnd = true;
    }
    int loadnum = 0;


    void onResponseCsvList(HttpMsgRspdBase msg)
    {
        var data = (Response_Csv_List)msg;
        if (data.errorCode == (int)EErrorCode.EEC_Success)
        {

            remoteCsvList.Clear();
            if (data.csvFileMd5List.Count > 0)
            {
                foreach (var dfl in data.csvFileMd5List)
                {
                    remoteCsvList.Add(dfl.fileName, new CsvCfgCatalog(dfl.fileName, dfl.fileMd5));
                }
                loadRemoteCsvListed = true;
            }
            else
            {
                //更新end
                Debug.LogError("没有获取到服务器配置列表!!!!!!!");
                CheckAndUpdateEnd?.Invoke(false);
            }
        }
    }

    void onResponseCsvLoad(HttpMsgRspdBase msg)
    {
        var data = (Response_Csv_Load)msg;
        if (data.errorCode != (int)EErrorCode.EEC_Success)
        {
            loadnum = -1;
        }
        else if (data.csvFileContentList.Count > 0)
        {
            //GameTimer.inst.StartCoroutine(updateCSV(data.csvFileContentList));
            CsvFileUnZip(data.csvFileContentList[0]);
        }
    }


    void CsvFileUnZip(CsvFileContent csvC)
    {
        byte[] decBytes = Convert.FromBase64String(csvC.fileContent);
        byte[] fileContent = ZipHelper.UnGZip2(decBytes);
        if (fileContent != null)
        {
            if (localCsvCatalog.csvFileList.ContainsKey(csvC.fileName))
            {
                var localclog = localCsvCatalog.csvFileList[csvC.fileName];
                string tpath = ResPathUtility.getpersistentDataPath(false) + $"cfgs/{localclog.fileFullName}.csv";
                removeFile(tpath);
            }
            saveCsvFile(csvC.fileName, csvC.fileMd5, fileContent);
            if (localCsvCatalog.csvFileList.ContainsKey(csvC.fileName))
            {
                localCsvCatalog.csvFileList[csvC.fileName].fileFullName = csvC.fileName + csvC.fileMd5;
            }
            else
            {
                localCsvCatalog.csvFileList.Add(csvC.fileName, new CsvCfgCatalog(csvC.fileName, csvC.fileMd5));
            }
            downloadCache.Add(csvC.fileName);
            if(remoteCsvList.ContainsKey(csvC.fileName))
            {
                remoteCsvList[csvC.fileName].fileMd5 = csvC.fileMd5;
                remoteCsvList[csvC.fileName].fileFullName = csvC.fileName + csvC.fileMd5;
            }
            loadnum++;
        }
        else
        {
            Debug.LogError($"{csvC.fileName} 文件解压失败！！");
            loadnum = -1;
        }
    }
    IEnumerator updateCSV(List<CsvFileContent> datalist)
    {
        yield return new WaitForSeconds(0.2f);
        int index = 0;
        FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("加载文件"), 0.5f, 0.2f);
        yield return new WaitForSeconds(0.2f);
        foreach (var csvC in datalist)
        {
            index++;
            //////------------------------------------------------------------------------------------------------------------------------------
            byte[] decBytes = Convert.FromBase64String(csvC.fileContent);
            byte[] fileContent = ZipHelper.UnGZip2(decBytes);
            if (fileContent != null)
            {
                if (localCsvCatalog.csvFileList.ContainsKey(csvC.fileName))
                {
                    var localclog = localCsvCatalog.csvFileList[csvC.fileName];
                    string tpath = ResPathUtility.getpersistentDataPath(false) + $"cfgs/{localclog.fileFullName}.csv";
                    removeFile(tpath);
                }
                saveCsvFile(csvC.fileName, csvC.fileMd5, fileContent);
                if (localCsvCatalog.csvFileList.ContainsKey(csvC.fileName))
                {
                    localCsvCatalog.csvFileList[csvC.fileName].fileFullName = csvC.fileName + csvC.fileMd5;
                }
                else
                {
                    localCsvCatalog.csvFileList.Add(csvC.fileName, new CsvCfgCatalog(csvC.fileName, csvC.fileMd5));
                }
            }
            else
            {
                Debug.LogError($"{csvC.fileName} 文件解压失败！！");
            }

            FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("加载文件") + $"({index}/{datalist.Count})", ((float)index / (float)datalist.Count) * 0.5f + 0.5f, 0.1f);
            yield return new WaitForSeconds(0.10f);
        }
        SaveRemoteCatalog2Local();
        FGUI.inst.updateProglressBar(LanguageManager.inst.GetValueByKey("加载文件") + $"({datalist.Count}/{datalist.Count})", 1, 0.2f);
        yield return new WaitForSeconds(0.2f);
        loadCsvFileEnd = true;
    }

    void removeFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
    void saveCsvFile(string csvname, string md5, byte[] fileContent)
    {
        if (Directory.Exists(ResPathUtility.getpersistentDataPath(false) + "cfgs") == false)
        {
            Directory.CreateDirectory(ResPathUtility.getpersistentDataPath(false) + "cfgs");
        }
        string tpath = ResPathUtility.getpersistentDataPath(false) + $"cfgs/{csvname}{md5}.csv";
        FileUtils.saveFile(fileContent, tpath);
    }
#endregion
}
