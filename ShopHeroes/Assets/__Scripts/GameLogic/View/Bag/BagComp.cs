using UnityEngine;
using UnityEngine.UI;
using Mosframe;
public class BagComp : MonoBehaviour
{
    public Button bgBtn;
    public Button closeBtn;
    public Animator uiAnimator;

    //仓库分类
    public ToggleGroupEX toggleGroup;
    public Toggle equipToggle;
    public Toggle runeToggle;
    public Toggle exceptionToggle;
    public DynamicScrollView loopListView;

    public Text bagLimitText;
    public Button sortordButton;

    [Header("动画")]
    public GameObject maskObj;

}
