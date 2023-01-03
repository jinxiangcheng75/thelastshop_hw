using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetLoadTask {
    public string path;
    public kAssetType assetType;
    public AssetPipelineConfig pipelineConfig;
    public string hash;
    public string savePath;
    public kAssetLocation location;
}

public class AssetLoadGroup {
    const int RetryMax = 3;
    int id;
    List<AssetLoadTask> mTaskList;
    int mLoadIndex;
    int mRetryCount;
    public AssetLoadGroup () {
        mTaskList = new List<AssetLoadTask>();
        clear();
    }

    public int getTotal () {
        return mTaskList.Count;
    }

    public int getLoaded () {
        return mLoadIndex + 1;
    }
    
    public void add (AssetLoadTask task) {
        if (task.location == kAssetLocation.Remote) return; //跳过远端服务器
        mTaskList.Add(task);
    }
    
    public AssetLoadTask getTask() {
        if(mLoadIndex >= mTaskList.Count)
            return null;
        return mTaskList[mLoadIndex];
    }

    public void taskFailed () {
        mRetryCount++;
    }

    public void taskSuccess () {
        mLoadIndex++;
        mRetryCount = 0;
    }

    public bool canRetry () {
        return mRetryCount < RetryMax;
    }

    public void upgradeLocation () {
        AssetLoadTask task = getTask();
        if(task != null && (task.location == kAssetLocation.Streaming || task.location == kAssetLocation.Persistent))
            task.location = kAssetLocation.Remote;
    }

    public void clear () {
        mRetryCount = 0;
        mLoadIndex = 0;
        mTaskList.Clear();
    }
}
/// <summary>
/// 
/// </summary>
public class ChannelAsset {
    public int channel;//ch
    public string asset;//a
    public string server;//s
    public string patch;//p
    public string catalog;//addressable catalog
    public int catalogPath;
    public int version;//v
    public bool jump;//j
    public string jumpUrl;//j_url
}

public class PatchAsset {
    int buildVersion;
    int patchVersion;
    List<PatchAssetItem> mItems;
}

public class PatchFileItem {
    public string fileName;
    public kAssetType type;
    public string hash;
    public int size;
    public kAssetLocation location;
    public string hashedPath;
}

public class PatchAssetItem {
    public string remotePath;
    public string hash;
    public kAssetType type;
    public string savePath;
}

public enum kAssetType {
    Channel,
    Patch,
    UIDependency,
    Configs,
    Lua,
    Shader,
    Material,
    Texture,
    Prefab,
    Text,
    Binary,
    Catalog,    //addressables  catalog
    ExtraCatalog,   //addressables extra catalog
    AddressablePack,
}

public enum kAssetExtractType {
    None,
    Text,
    Binary,
}

public enum kAssetDownloadType {
    Web,
    Addressable,
    Catalog,
    ExtraCatalog,
}

public enum kAssetHandlerType {
    Channel,
    Patch,
    UIDependency,
    Configs,
    Lua,
    Addressable,
    AddressablePack,
}

public enum kAssetEncryptType {
    None,
}

public enum kAssetLocation {
    None,
    Persistent,
    Streaming,
    Remote
}

public sealed class AssetPipelineConfig {
    public kAssetExtractType extractType = kAssetExtractType.Text;
    public kAssetDownloadType requestType = kAssetDownloadType.Web;
    public kAssetHandlerType assetHandlerType;
    public kAssetEncryptType decryptType = kAssetEncryptType.None;
    public static AssetPipelineConfig ChannelCfg = new AssetPipelineConfig {
        assetHandlerType = kAssetHandlerType.Channel,
    };
    public static AssetPipelineConfig UIDepCfg = new AssetPipelineConfig {
        assetHandlerType = kAssetHandlerType.UIDependency,
    };
    public static AssetPipelineConfig PatchCfg = new AssetPipelineConfig {
        assetHandlerType = kAssetHandlerType.Patch,
    };
    public static AssetPipelineConfig ConfigsCfg = new AssetPipelineConfig {
        extractType = kAssetExtractType.Binary,
        assetHandlerType = kAssetHandlerType.Configs
    };
    public static AssetPipelineConfig LuaCfg = new AssetPipelineConfig {
        assetHandlerType = kAssetHandlerType.Lua
    };
    public static AssetPipelineConfig AddressableCfg= new AssetPipelineConfig {
        extractType = kAssetExtractType.None,
        requestType = kAssetDownloadType.Addressable,
        assetHandlerType = kAssetHandlerType.Addressable
    };
    public static AssetPipelineConfig CatalogCfg = new AssetPipelineConfig {
        extractType = kAssetExtractType.None,
        requestType = kAssetDownloadType.Catalog,
        assetHandlerType = kAssetHandlerType.Addressable
    };
    public static AssetPipelineConfig ExtraCatalogCfg = new AssetPipelineConfig {
        extractType = kAssetExtractType.None,
        requestType = kAssetDownloadType.ExtraCatalog,
        assetHandlerType = kAssetHandlerType.Addressable
    };
    public static AssetPipelineConfig AddressablePackCfg = new AssetPipelineConfig {
        extractType = kAssetExtractType.Binary,
        requestType = kAssetDownloadType.Web,
        assetHandlerType = kAssetHandlerType.AddressablePack
    };

    public static AssetPipelineConfig getConfig(kAssetType type) {
        switch(type) {
        case kAssetType.Channel:
        return ChannelCfg;
        case kAssetType.Patch:
        return PatchCfg;
        case kAssetType.Configs:
        return ConfigsCfg;
        case kAssetType.Lua:
        return LuaCfg;
        case kAssetType.UIDependency:
        return UIDepCfg;
        case kAssetType.Catalog:
        return CatalogCfg;
        case kAssetType.ExtraCatalog:
        return ExtraCatalogCfg;
        case kAssetType.AddressablePack:
        return AddressablePackCfg;
        default:
        return AddressableCfg;
        }
    }
}

/*public class UIDenpendencyItem {
    public string uiName;
    public List<string> atlasList;
}*/