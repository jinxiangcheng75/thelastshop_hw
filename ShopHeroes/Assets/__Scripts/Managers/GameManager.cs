using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
    public static string userAgent;
    private void Start()
    {
        int ecoMode = SaveManager.inst.GetInt("EcoMode", false);
        Application.targetFrameRate = ecoMode == 1 ? 35 : 60;
    }
}
