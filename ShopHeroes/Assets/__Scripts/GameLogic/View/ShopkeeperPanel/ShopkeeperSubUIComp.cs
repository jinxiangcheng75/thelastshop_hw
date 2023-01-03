using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mosframe;

public class ShopkeeperSubUIComp : MonoBehaviour
{
    public Text topText;
    public Button closeBtn;
    public Button bgCloseBtn;
    public ToggleGroupMarget group;
    public ToggleGroupMarget fashionGroup;
    //public ToggleGroupMarget sexGroup;
    public Button maleBtn;
    public Button femaleBtn;
    public GameObject oneObj;
    public GameObject twoObj;
    public GameObject exteriorObj;
    public GameObject fashionObj;
    public DynamicVScrollView scrollView;
    public DynamicVScrollView fashionScrollView;
    public Animator windowAnimator;
}
