using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

[DisallowMultipleComponent]
public class AssetManualAttacher : MonoBehaviour
{
    void OnDestroy()
    {
        // if (string.IsNullOrEmpty(assetKey)) return;
        // unregister(assetKey);
        if (gameObject != null)
        {
            if (Addressables.ReleaseInstance(gameObject))
            {
                //Destroy
            }
        }
    }
}
