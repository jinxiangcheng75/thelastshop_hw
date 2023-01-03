using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainLineTaskComp : MonoBehaviour
{
    public MainLineTaskFinger finger;
    public Text taskText;
    public Button taskBtn;
    public GUIIcon taskIcon;
    public Image taskSchedule;
    public GUIIcon bgIcon;
    public Image bgLgIcon;
    public Image circleLgIcon;
    public Image gouImg;
    public RectTransform bgRect;
    public RectTransform bgLgRect;
    public Canvas contentCanvas;
    public MainLineTaskDialog dialog;
    public MainLinePrefabItem prefab;
    public Material lg4;
    public Material lg5;
    public Transform prefabTrans;
}
