using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FindTargetFormScenes : MonoBehaviour
{
    [SerializeField]
    private Transform _RotBG;
    public Transform _trangetTf;
    private Camera _camera;
    private Button _button;

    bool isVisible = true;

    RectTransform guiroot;
    void Start()
    {
        _camera = Camera.main;
        _button = GetComponent<Button>();
        if (_button != null)
        {
            _button.onClick.AddListener(() =>
            {
                if (GuideManager.inst.isInTriggerGuide) return;
                if (_trangetTf != null && D2DragCamera.inst != null)
                {
                    D2DragCamera.inst.LookToPosition(_trangetTf.position, false, 0);
                }
            });
        }
        guiroot = FGUI.inst.uiRootTF.GetComponent<RectTransform>();
        show(false);
        StartCoroutine(TrakerGuider());
    }
    private IEnumerator TrakerGuider()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            SetGuid();
        }
    }
    private Vector3 GetNode2(Vector3 pos, Vector3 startpos, float v)
    {
        pos = new Vector3(pos.x, pos.y, 0);
        Vector3 ab = pos - startpos;
        Vector3 am = ab * (Mathf.Abs(startpos.y - v) / Mathf.Abs(pos.y - startpos.y));
        Vector3 om = startpos + am;
        return om;
    }

    private Vector3 GetNode(Vector3 pos, Vector3 startpos, float v)
    {
        pos = new Vector3(pos.x, pos.y, 0);
        Vector3 ab = pos - startpos;
        Vector3 am = ab * (Mathf.Abs(startpos.x - v) / Mathf.Abs(pos.x - startpos.x));
        Vector3 om = startpos + am;
        return om;
    }

    private void SetGuid()
    {
        if (_trangetTf != null)
        {
            Vector3 pos = _camera.WorldToScreenPoint(_trangetTf.position);
            if (GuideManager.inst.isInTriggerGuide || (GuideDataProxy.inst != null && GuideDataProxy.inst.CurInfo != null && !GuideDataProxy.inst.CurInfo.isAllOver)) { show(false); return; }
            if (IndoorMapEditSys.inst != null && IndoorMapEditSys.inst.shopDesignMode == DesignMode.LookPetHouse)
            {
                show(false);
                return;
            }
            if (pos.x >= 0 && pos.x <= Screen.width && pos.y >= 0 && pos.y <= Screen.height) { show(false); return; }
            Logger.log("目标位置：" + pos.ToString());
            show(true);
            var startpos = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

            var dir = pos - startpos;
            float angle = Vector3.SignedAngle(Vector3.down, Vector3.Normalize(dir), Vector3.forward);
            _RotBG.localEulerAngles = Vector3.forward * angle;

            float sereenangle = (float)Screen.height / (float)Screen.width;
            var va = System.Math.Abs(dir.y / dir.x);
            if (va <= sereenangle)
            {
                var length = _RotBG.GetComponent<RectTransform>().sizeDelta.x;
                if (pos.x < 0)
                {
                    Vector3 _pos = GetNode(pos, startpos, length * 0.5f);
                    Vector2 localPoint;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(guiroot, _pos, FGUI.inst.uiCamera, out localPoint))
                    {
                        transform.localPosition = localPoint;
                    }

                }
                else
                {
                    Vector3 _pos = GetNode(pos, startpos, Screen.width - length * 0.5f);
                    Vector2 localPoint;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(guiroot, _pos, FGUI.inst.uiCamera, out localPoint))
                    {
                        transform.localPosition = localPoint;
                    }
                }
            }
            else
            {
                var length = _RotBG.GetComponent<RectTransform>().sizeDelta.x;
                if (pos.y < 0)
                {
                    Vector3 _pos = GetNode2(pos, startpos, length * 0.5f);
                    Vector2 localPoint;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(guiroot, _pos, FGUI.inst.uiCamera, out localPoint))
                    {
                        transform.localPosition = localPoint;
                    }
                }
                else
                {
                    Vector3 _pos = GetNode2(pos, startpos, Screen.height - length * 0.5f - 180);
                    Vector2 localPoint;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(guiroot, _pos, FGUI.inst.uiCamera, out localPoint))
                    {
                        transform.localPosition = localPoint;
                    }
                }
            }
        }
        else
        {
            if (IndoorMap.inst != null)
            {
                var counterdata = UserDataProxy.inst.GetCounter();

                var _trange = IndoorMap.inst.GetFunitures(counterdata.uid);
                if (_trange != null)
                    _trangetTf = _trange.PopUIRoot;
            }
        }
    }


    void show(bool visible)
    {
        if (isVisible == visible) return;
        isVisible = visible;
        foreach (Transform tf in transform)
        {
            tf.gameObject.SetActive(visible);
        }
        _button.interactable = visible;
    }
}
