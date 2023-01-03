using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureBoxInfoChanceItem : MonoBehaviour
{
    public List<Text> allText;
    int basicNum = 1600;
    public void setData(int boxGroupId)
    {
        for (int i = 0; i < allText.Count; i++)
        {
            int index = i;
            allText[index].text = (WorldParConfigManager.inst.GetConfig(basicNum + ((boxGroupId - 50001) * 5) + index).parameters / 100.0f) + "%";
        }
    }
}
