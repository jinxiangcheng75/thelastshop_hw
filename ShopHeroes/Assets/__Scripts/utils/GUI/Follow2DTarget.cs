using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow2DTarget : MonoBehaviour
{
    public Transform target;
    private Camera maincamera;
    // Update is called once per frame
    void Start()
    {
        if (target == null)
        {
            Destroy(this.gameObject);
        }
        if (maincamera == null)
        {
            maincamera = Camera.main;
        }
        if (target == null) return;
        Vector3 pos = target.position * 1000f;//maincamera.WorldToScreenPoint(target.position);
                                              // Vector2 localPoint;
                                              // if (RectTransformUtility.ScreenPointToLocalPointInRectangle(FGUI.inst.hudPlanel, pos, FGUI.inst.uiCamera, out localPoint))
                                              // {
                                              //     transform.localPosition = localPoint;
                                              // }

        transform.localPosition = pos;
    }
    void LateUpdate()
    {
        if (target == null)
        {
            Destroy(this.gameObject);
        }
    }
}
