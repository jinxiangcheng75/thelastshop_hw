using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CombatComp : MonoBehaviour
{
    public Button playBtn;
    public Button pauseBtn;
    public Button speedUpBtn;
    public GameObject speed_x1_Obj;
    public GameObject speed_x3_Obj;
    public Button exitBtn;
    public RectTransform CardsTF;
    public Text speedText;
    //public FighterCard fighterCard;
    public Text roundText;
    public FighterCard[] fighterCards;
    public GameObject ruinsObj;
    public Slider ruinsSlider;
    public Text ruinsScheduleText;
    public GUIIcon ruinsBossIcon;
    public Text ruinsWaveText;
    public RectTransform leftTf;
}
