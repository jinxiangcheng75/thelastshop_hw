using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
// public class AtlasLoadItem
// {
//     string atlasName;
//     Action callback;
//     public bool loading = false;

//     public AtlasLoadItem(string name, System.Action _callback)
//     {
//         LoadAtlas(name, _callback);
//     }
//     public void LoadAtlas(string name, System.Action _callback)
//     {
//         atlasName = name;
//         callback += _callback;
//         LoadAtlasAsync();
//     }

//     public void AddListenerCallBack(System.Action _callback)
//     {
//         callback += _callback;
//     }
//     void LoadAtlasAsync()
//     {
//         loading = true;
//         ManagerBinder.inst.Asset.loadSpriteAtlasAsync(atlasName, loadAtlasEnd);
//     }
//     void loadAtlasEnd()
//     {
//         if (callback != null)
//             callback.Invoke();

//         callback = null;
//         loading = false;
//     }
// }
public static class GUIHelper
{
    public static bool isPointerOnUI()
    {
        if (EventSystem.current == null) return false;
#if UNITY_EDITOR      

        return EventSystem.current.IsPointerOverGameObject();
#else
        return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        //return IsPointerOverGameObject();
#endif
    }

    public static bool IsPointerOverGameObject(Vector3 mousepos)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.pressPosition = mousepos;
        eventData.position = mousepos;

        List<RaycastResult> list = new List<RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, list);

        return list.Count > 0;
    }

    public static void showQualiyIcon(RectTransform qualiybgTF, int qualiy, float dsize = 200f)
    {
        foreach (Transform tf in qualiybgTF.transform)
        {
            GameObject.Destroy(tf.gameObject);
        }
        if (qualiy == 4 || qualiy == StaticConstants.SuperEquipBaseQuality + 4)
        {
            qualiybgTF.sizeDelta = Vector2.one * 80f;
            var newgo = GameObject.Instantiate(Resources.Load<GameObject>("qualityicon_zi"), Vector3.zero, Quaternion.identity, qualiybgTF.transform);
            newgo.transform.localPosition = Vector3.zero;
            newgo.GetComponent<RectTransform>().sizeDelta = Vector2.one * dsize;
        }
        else if (qualiy == 5 || qualiy == StaticConstants.SuperEquipBaseQuality + 5)
        {
            qualiybgTF.sizeDelta = Vector2.one * 80f;
            var newgo = GameObject.Instantiate(Resources.Load<GameObject>("qualityicon_h"), Vector3.zero, Quaternion.identity, qualiybgTF.transform);
            newgo.transform.localPosition = Vector3.zero;
            newgo.GetComponent<RectTransform>().sizeDelta = Vector2.one * dsize;
        }
        else
        {
            qualiybgTF.sizeDelta = Vector2.one * 160;
        }
    }
    public static bool CheckHitItem(Vector3 mousePos, int layer)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, 1 << layer))
        {
            if (hit.collider != null)
            {
                return true;
            }
        }
        return false;
    }


    public static void setRandererSortinglayer(Transform target, string LayerID, int orderinLayer)
    {
        Renderer[] particles = target.GetComponentsInChildren<Renderer>(true);
        int sortingLayerID = SortingLayer.NameToID(LayerID);
        foreach (Renderer part in particles)
        {
            part.sortingLayerID = sortingLayerID;
            part.sortingOrder = orderinLayer;
        }
    }

    public static int CalculateLengthOfText(string message, Text tex)
    {
        int totalLength = 0;
        Font myFont = tex.font;
        myFont.RequestCharactersInTexture(message, tex.fontSize, tex.fontStyle);
        CharacterInfo characterInfo = new CharacterInfo();
        char[] arr = message.ToCharArray();
        foreach (char c in arr)
        {
            myFont.GetCharacterInfo(c, out characterInfo, tex.fontSize);
            totalLength += characterInfo.advance;
        }
        return totalLength;
    }

    // public static Dictionary<string, AtlasLoadItem> atlasLoadQueue = new Dictionary<string, AtlasLoadItem>();
    public static void LoadAtlasAsync(string atlasName, System.Action<string, AsyncOperationHandle> callback)
    {
        // if (atlasLoadQueue.ContainsKey(atlasName))
        // {
        //     if (atlasLoadQueue[atlasName].loading)
        //     {
        //         atlasLoadQueue[atlasName].AddListenerCallBack(callback);
        //     }
        //     else
        //     {
        //         atlasLoadQueue[atlasName].LoadAtlas(atlasName, callback);
        //     }
        //     return;
        // }
        // else
        // {
        //     atlasLoadQueue.Add(atlasName, new AtlasLoadItem(atlasName, callback));
        // }
        //  AtlasAssetHandler.inst.LoadAtlasAsync(atlasName, callback);
    }


    /// <summary>
    /// 将数值转为 字符串  999,999,999
    /// </summary>
    /// <param name="money"></param>
    /// <returns></returns>
    public static string GetMoneyStr(int money)
    {
        return string.Format("{0:N0}", money);
    }

    /// <summary>
    /// RGB转成16进制
    /// </summary>
    /// <param name="color">颜色</param>
    static public string GetHexColorByColor(Color color)
    {
        int colorToInt = 0;
        colorToInt |= Mathf.RoundToInt(color.r * 255f) << 24;
        colorToInt |= Mathf.RoundToInt(color.g * 255f) << 16;
        colorToInt |= Mathf.RoundToInt(color.b * 255f) << 8;
        colorToInt |= Mathf.RoundToInt(color.a * 255f);
        return "#" + colorToInt.ToString("X8");
    }

    /// <summary>
    /// HexColor to Color(字符窜转颜色值)
    /// </summary>
    /// <param name="hexColor"></param>
    /// <returns></returns>
    public static Color GetColorByColorHex(string hexColor)
    {
        hexColor = hexColor.Replace("#", string.Empty);
        byte r = 255, g = 255, b = 255, a = 255;
        if (hexColor.Length > 1)
            r = byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        if (hexColor.Length > 3)
            g = byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        if (hexColor.Length > 5)
            b = byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        if (hexColor.Length > 7)
            a = byte.Parse(hexColor.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        Color tempColor = new Color32(r, g, b, a);
        return tempColor;
    }

    private static Material grayMat;
    //加描边
    public static Material GetOutlineMat()
    {
        Material mater = Resources.Load<Material>("Sprite_OutLineMat");
        return mater;
    }

    //加场景描边
    public static Material GetSceneOutlineMat()
    {
        Material mater = Resources.Load<Material>("Sprite_OutLineMat");
        return mater;
    }

    //变灰色
    private static Material GetGrayMat()
    {
        if (grayMat == null)
        {
            Shader shader = Shader.Find("Custom/UI-Gray");
            if (shader == null)
            {
                Logger.log("null");
                return null;
            }
            Material mat = new Material(shader);
            grayMat = mat;
        }
        return grayMat;
    }

    public static string NOTNEEDCLEARMAT = "alreadyGrayMat";
    public static void SetUIGray(Transform uiroot, bool gray)
    {
        //图片
        var imageList = uiroot.GetComponentsInChildren<Image>(true);
        foreach (var item in imageList)
        {
            if (gray)
            {
                item.material = GetGrayMat();
                item.SetMaterialDirty();
            }
            else
            {
                item.material = null;
            }
        }

        //文字
        var textList = uiroot.GetComponentsInChildren<Text>(true);
        foreach (var item in textList)
        {
            if (gray)
            {
                item.material = GetGrayMat();
                item.SetMaterialDirty();
            }
            else
            {
                item.material = null;
            }
        }
    }

    public static void SetSingleUIGray(Transform tf, bool gray)
    {
        var img = tf.GetComponent<Image>();
        var text = tf.GetComponent<Text>();

        if (gray)
        {
            if (img != null)
            {
                img.material = GetGrayMat();
                img.SetMaterialDirty();
            }

            if (text != null)
            {
                text.material = GetGrayMat();
                text.SetMaterialDirty();
            }
        }
        else
        {
            if (img != null) img.material = null;
            if (text != null) text.material = null;
        }
    }

    /// <param name="grayProgress">置灰程度0-1  0为最灰</param>
    public static void SetUIGrayColor(this Image image, float grayProgress) 
    {
        float H, S, V;

        Color.RGBToHSV(image.color, out H, out S, out V);
        Color color = Color.HSVToRGB(H, S, grayProgress);
        image.color = new Color(color.r, color.g, color.b, image.color.a);
    }


    /// <param name="grayProgress">置灰程度0-1  0为最灰</param>
    /// <param name="alpha">透明度0-1</param>
    /// <param name="textNeedChg">text是否跟着置灰</param>
    public static void SetUIGrayColor(Transform root, float grayProgress, float alpha = 1, bool textNeedChg = true)
    {
        var graphics = root.GetComponentsInChildren<Graphic>(true);
        float H, S, V;

        foreach (var item in graphics)
        {
            if (item.GetComponent<Text>() != null && !textNeedChg) continue;
            Color.RGBToHSV(item.color, out H, out S, out V);
            Color color = Color.HSVToRGB(H, S, grayProgress);
            item.color = new Color(color.r, color.g, color.b, alpha);
        }
        //2d 精灵图片
        var spriteRenderers = root.GetComponentsInChildren<SpriteRenderer>();
        foreach (var renderer in spriteRenderers)
        {
            Color.RGBToHSV(renderer.color, out H, out S, out V);
            Color color = Color.HSVToRGB(H, S, grayProgress);
            renderer.color = new Color(color.r, color.g, color.b, alpha);
        }
    }

    //获取里程碑对应icon
    public static void SetMilestonesIconText(progressItemInfo info, ref GUIIcon icon, ref Text valueTx)
    {
        if (icon != null)
        {
            GUIHelper.SetUIGray(icon.transform, false);
            switch (info.type)
            {
                case 1:     //耗材减少
                case 2:     //减少消耗(特殊材料)
                    itemConfig cfg = ItemconfigManager.inst.GetConfig(info.reward_id);
                    icon.SetSprite(cfg.atlas, cfg.icon);
                    break;
                case 3:     //减少消耗(装备)
                    EquipConfig equipcfg = EquipConfigManager.inst.GetEquipInfoConfig(info.reward_id);
                    icon.SetSprite(equipcfg.equipDrawingsConfig.atlas, equipcfg.equipDrawingsConfig.icon);
                    break;
                case 4:     //时间减少
                    icon.SetSprite("__common_1", "zhizuo_daojishi");
                    break;
                case 5:     //售价提高	
                    icon.SetSprite("__common_1", "zhuejiemian_meiyuan");
                    break;
                case 6:     //技能
                    break;
                case 7:     //解锁新图纸
                    EquipDrawingsConfig drawingcfg = EquipConfigManager.inst.GetEquipDrawingsCfg(info.reward_id);
                    icon.SetSprite(drawingcfg.atlas, drawingcfg.icon);
                    break;
                case 8:     //品质提升
                    GUIHelper.SetUIGray(icon.transform, true);
                    icon.SetSprite("StaticIcon", "cangku_pinzhi1");
                    break;
            }
        }
        if (valueTx != null)
        {
            string valuestr = "";
            switch (info.type)
            {
                case 1:     //耗材减少
                case 2:     //减少消耗(特殊材料)
                    {
                        valuestr = "-" + info.reward_value.ToString();
                    }
                    break;
                case 3:     //减少消耗(装备)
                    {
                        valuestr = "-" + info.reward_value.ToString();
                    }
                    break;
                case 4:     //时间减少
                    {
                        valuestr = $"-{info.reward_value}%";
                    }
                    break;
                case 5:     //售价提高	
                    {
                        valuestr = $"x{info.reward_value}%";
                    }
                    break;
                case 6:     //技能
                            //????????????????????
                    break;
                case 7:     //解锁新图纸
                    {
                        valuestr = "";
                    }
                    break;
                case 8:     //品质提升
                    {
                        valuestr = $"x{info.reward_value}";
                    }
                    break;
            }
            valueTx.text = valuestr;
        }
    }

    public static Transform FindChild(Transform parent, string name)
    {
        Transform tf = parent.Find(name);
        if (tf != null)
        {
            return tf;
        }
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform target = FindChild(parent.GetChild(i), name);
            if (target != null)
            {
                return target;
            }
        }
        return null;
    }

    public static Vector2 GetFGuiCameraUIPointByWorldPos(Vector3 worldPos)

    {
        Vector3 pos = Vector3.zero;
        pos = FGUI.inst.uiCamera.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(FGUI.inst.uiRootTF.GetComponent<RectTransform>(), pos, FGUI.inst.uiCamera, out Vector2 v2);

        return v2;
    }

}
