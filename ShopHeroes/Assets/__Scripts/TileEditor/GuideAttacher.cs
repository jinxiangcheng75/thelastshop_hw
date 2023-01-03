using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideAttacher : MonoBehaviour
{
    public SpriteRenderer pb_content;

    private void Awake()
    {
        gameObject.name = "我是修复进度条";
    }
    public void setSchedule(int curSchedule, int max)
    {
        float curX = curSchedule / (float)max;
        if (curX > 1)
            curX = 1;
        pb_content.transform.localScale = new Vector3(curX, 1, 1);
    }
}
