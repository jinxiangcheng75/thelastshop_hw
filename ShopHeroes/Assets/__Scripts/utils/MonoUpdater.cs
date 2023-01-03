using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class MonoUpdater : MonoBehaviour {
    IMonoUpdater mUpdater;
    void Awake () {
        DontDestroyOnLoad(this.gameObject);    
    }

    void Start () {
        if(mUpdater == null) {
            this.enabled = false;
            throw new System.Exception("[MonoUpdater] updater not set !!!");
        }    
    }
    // Use this for initialization
    public void setUpdater (IMonoUpdater upd) {
        mUpdater = upd;
    }

    void LateUpdate () {
        if (mUpdater == null) return;
        mUpdater.onUpdate();
    }
}
public interface IMonoUpdater {
    void onUpdate ();
}