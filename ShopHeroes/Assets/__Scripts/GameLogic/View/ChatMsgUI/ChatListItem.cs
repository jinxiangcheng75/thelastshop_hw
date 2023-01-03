using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChatListItem : MonoBehaviour
{
    public Transform headIconParent;
    public Text levelText;
    public Text nameText;
    public Text msgText;
    public Text timeText;
    public Button button;

    [HideInInspector]
    public GraphicDressUpSystem headGraphicSystem;

}
