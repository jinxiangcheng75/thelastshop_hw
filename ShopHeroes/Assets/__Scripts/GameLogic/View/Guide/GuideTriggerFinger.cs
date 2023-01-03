using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideTriggerFinger : MonoBehaviour
{
    public SpriteRenderer finger;
    public RectTransform fingerRect;

    private void Awake()
    {
        finger.sortingOrder = 9999;
    }

    Transform target;
    string targetOffset;
    public void ShowFinger(Transform targetTrans, string offset)
    {
        finger.sortingOrder = 9999;
        fingerRect.gameObject.SetActive(true);
        target = targetTrans;
        targetOffset = offset;

        float offsetX = 0, offsetY = 0, offsetRotateZ = 0;
        if (offset != null)
        {
            var splitStr = offset.Split('|');
            if (0 < splitStr.Length)
                offsetX = float.Parse(splitStr[0]);
            if (1 < splitStr.Length)
                offsetY = float.Parse(splitStr[1]);
            if (2 < splitStr.Length)
                offsetRotateZ = float.Parse(splitStr[2]);
        }

        Vector2 v2;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(FGUI.inst.uiRootTF.GetComponent<RectTransform>(), FGUI.inst.uiCamera.WorldToScreenPoint(targetTrans.position), FGUI.inst.uiCamera, out v2);
        var rectTrans = targetTrans.GetComponent<RectTransform>();
        float x = 0;
        float y = 0;
        float height = 0;
        if (rectTrans != null)
        {
            Vector2 pivot = rectTrans.pivot;
            if (targetTrans.localScale.x > 0)
            {
                if (pivot.x > 0.5f)
                {
                    x = -(rectTrans.rect.width * (pivot.x - 0.5f));
                }
                else if (pivot.x < 0.5f)
                {
                    x = (rectTrans.rect.width * (0.5f - pivot.x));
                }
                if (pivot.y > 0.5f)
                {
                    y = -(rectTrans.rect.height * (pivot.y - 0.5f));
                }
                else if (pivot.y > 0.5f)
                {
                    y = (rectTrans.rect.height * (0.5f - pivot.y));
                }
            }
            else
            {
                if (pivot.x < 0.5f)
                {
                    x = (rectTrans.rect.width * (pivot.x - 0.5f));
                }
                else if (pivot.x > 0.5f)
                {
                    x = -(rectTrans.rect.width * (0.5f - pivot.x));
                }
                if (pivot.y > 0.5f)
                {
                    y = -(rectTrans.rect.height * (pivot.y - 0.5f));
                }
                else if (pivot.y > 0.5f)
                {
                    y = (rectTrans.rect.height * (0.5f - pivot.y));
                }
            }

            height = rectTrans.rect.height / 2;
        }

        fingerRect.anchoredPosition = new Vector3(v2.x + x + offsetX, v2.y + y/* + height*/ + offsetY, 0);
        fingerRect.transform.eulerAngles = new Vector3(0, 0, offsetRotateZ);
    }

    public void HideFinger()
    {
        fingerRect.gameObject.SetActive(false);
        target = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null /*&& targetOffset != null*/)
        {
            ShowFinger(target, targetOffset);
        }
    }
}
