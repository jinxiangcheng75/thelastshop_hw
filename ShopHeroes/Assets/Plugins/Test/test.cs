using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    void Start()
    {
        Social.localUser.Authenticate(ProcessAuthentication);
    }

    public void ProcessAuthentication(bool success)
    {

        if (success)
            Debug.Log("Authenticated+" + Social.localUser.userName);
        else
            Debug.Log("Failed to authenticate");
    }
}
