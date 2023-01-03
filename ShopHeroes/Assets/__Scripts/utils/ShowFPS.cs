using UnityEngine;
using System.Collections;

public class ShowFPS : SingletonMono<ShowFPS>
{
    public float updateInterval = 0.5F;
    private double lastInterval;
    private int frames = 0;
    public float fps;
    GUIStyle fontStyle;
    void Start()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
        fontStyle = new GUIStyle();
        fontStyle.normal.background = null;
        fontStyle.normal.textColor = Color.yellow;
        fontStyle.fontSize = 80;
    }
#if UNITY_EDITOR
    void OnGUI()
    {
        //GUI.color = Color.yellow;
        GUILayout.Label(" " + fps.ToString("f2"), fontStyle);
        GUILayout.Label("h = " + Screen.height, fontStyle);
    }
#endif
    void Update()
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            fps = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;
        }
    }
}
