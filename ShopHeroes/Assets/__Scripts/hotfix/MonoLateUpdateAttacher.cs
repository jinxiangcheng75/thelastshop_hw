using UnityEngine;
using System.Collections;

public class MonoLateUpdateAttacher : MonoBehaviour {
    System.Action mUpdateCallback;
    // Use this for initialization
    void Start () { }
    public void setUpdateCallback (System.Action updateCallback) {
        mUpdateCallback = updateCallback;
    }
    // Update is called once per frame
    void LateUpdate () {
        mUpdateCallback?.Invoke();
    }
}
