using UnityEngine;
using System.Collections;

public class MonoFixedUpdateAttacher : MonoBehaviour {
    System.Action mUpdateCallback;
    public void setUpdateCallback (System.Action updateCallback) {
        mUpdateCallback = updateCallback;
    }
    void FixedUpdate () {
        mUpdateCallback?.Invoke();        
    }
}
