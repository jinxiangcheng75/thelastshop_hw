using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class AssetInstanceAttacher : MonoBehaviour
{
    public IAssetInstanceHandler mHandler;
    public string mAssetKey;
    void OnDestroy()
    {
        if (gameObject != null)
            mHandler.handleInstanceDestroy(mAssetKey, this.gameObject);
    }
}