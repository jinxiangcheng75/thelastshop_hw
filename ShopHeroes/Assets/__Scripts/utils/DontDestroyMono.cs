using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class DontDestroyMono : MonoBehaviour {

    void Awake () {
        DontDestroyOnLoad(this.gameObject);
    }
}
