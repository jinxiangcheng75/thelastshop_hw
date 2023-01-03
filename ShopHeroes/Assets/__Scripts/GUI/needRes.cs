using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class needRes : MonoBehaviour
{
    GUIIcon iconImage;
    Text countText;

    string iconname = "";
    string number = "";

    public void setData(string atlasname, string icon, int haveCount, int needCount, bool enough, int qu = 1)
    {
        iconImage = transform.Find("resicon").GetComponent<GUIIcon>();
        countText = transform.Find("countText").GetComponent<Text>();
        iconname = icon;
        if (needCount > 0)
        {
            countText.color = GUIHelper.GetColorByColorHex("FFFFFF");
            if (enough)
                number = "<Color=#FFFFFF>" + haveCount + "</Color>" + "/" + needCount;
            else
                number = "<Color=#ff4040>" + haveCount + "</Color>" + "/" + needCount;
        }
        else
        {
            number = haveCount.ToString();
            countText.color = enough ? GUIHelper.GetColorByColorHex("FFFFFF") : GUIHelper.GetColorByColorHex("cc2201");
        }

        countText.text = number;

        iconImage.SetSprite(atlasname, iconname, qu > 1 ? StaticConstants.qualityColor[qu - 1] : "");
    }
}
