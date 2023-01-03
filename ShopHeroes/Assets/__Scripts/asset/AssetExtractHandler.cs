using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public interface IAssetExtractHandler {
    System.Object handle (UnityWebRequest uwr);
}

public class TextAssetExtractHandler : IAssetExtractHandler {
    public object handle (UnityWebRequest uwr) {
        return uwr.downloadHandler.text;
    }
}

public class BinaryAssetExtractHandler : IAssetExtractHandler {
    public object handle (UnityWebRequest uwr) {
        return uwr.downloadHandler.data;
    }
}

public class AssetExtractHandlerFactory : AbstractHandlerFactory<IAssetExtractHandler, kAssetExtractType> {
    protected override int getTypeIndex (kAssetExtractType type) {
        return (int)type;
    }

    protected override int getTypeNum () {
        return (int)(kAssetExtractType.Binary) + 1;
    }
}
