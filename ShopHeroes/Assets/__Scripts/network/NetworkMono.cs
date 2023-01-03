/*using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class NetworkMono : MonoBehaviour {
    INetworkReceivedHandler mProxy;
    // Use this for initialization
    void Awake () {
        DontDestroyOnLoad(this.gameObject);    
    }

    public void setReceivedHandler (INetworkReceivedHandler receivedHandler) {
        mProxy = receivedHandler;
    }
    
    // Update is called once per frame
    void LateUpdate () {
        mProxy.handleReceived();
    }
}
*/