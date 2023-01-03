using UnityEngine;
using System.Collections;

public class MonoUpdateAttacher : MonoBehaviour {
    System.Action mUpdateCallback;
    // Use this for initialization
    void Start () {}
    public void setUpdateCallback(System.Action updateCallback) {
        mUpdateCallback = updateCallback;
    }
    // Update is called once per frame
    void Update () {
        mUpdateCallback?.Invoke();
    }
}