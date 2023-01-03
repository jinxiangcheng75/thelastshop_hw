using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;

public class ExplorePanelCom : MonoBehaviour
{
    public Button closeBtn;
    public List<UnionBuffItem> unionBuffItems;//公会buff items
    public DynamicScrollView scrollView;
    public Animator windowAnim;
    public GameObject maskObj;
    public ScrollRect scrollRect;
}
