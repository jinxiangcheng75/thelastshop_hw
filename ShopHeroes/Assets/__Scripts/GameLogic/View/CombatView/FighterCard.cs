using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Spine;

public class FighterCard : MonoBehaviour
{
    public Transform buffsTF;
    public GUIIcon buffIcon;
    public Transform heroIcon;
    public RawImage animHeroIcon;
    public Slider hpSlider;
    public Slider angerSlider;
    public Text hpText;
    public Text angerText;
    public Text nameText;
    public GUIIcon professionIconBgIcon;
    public GUIIcon professionIcon;
    public int fighterkey;
    public Text levelText;

    public Image iconBG;
    public Image iconFrame;
    public Image lvBg;
    public List<Sprite> iconBGs;
    public List<Sprite> iconFrames;
    public List<Sprite> lvBG;
    public Animator _angerAnim;

    public bool canuserAnger = false;

    public Transform NoneMaskTF;
    public Transform LevelTf;
    [Header("动画")]
    public float barMin = -450f;//65f;
    public float barMax = -160f;//320f;
    public SkeletonGraphic _angerBar;
    public SkeletonGraphic _SkillVfx;
    public RectTransform barFill;

    public RectTransform DeathTextTF;
    public void setAngerBarValue(float value)
    {
        //value = Mathf.Min(barMax, Mathf.Max(0, value));
        var _value = barMin + (barMax - barMin) * value;
        barFill.DOAnchorPosY(_value, 0.2f);
        if (_angerBar.AnimationState != null)
        {
            var trackEntry = _angerBar.AnimationState.SetAnimation(0, "energy_up", false);
            trackEntry.Complete += animComplete;
        }
    }
    void animComplete(TrackEntry track)
    {
        if (_angerBar.AnimationState != null)
        {
            _angerBar.AnimationState.SetAnimation(0, "energy_idle", true);
            track.Complete -= animComplete;
        }
    }
    void Start()
    {
        _SkillVfx.gameObject.SetActive(false);
        // if (_angerAnim == null)
        // {
        //     _angerAnim = GetComponent<Animator>();
        //     _angerAnim.speed = GameSettingManager.combatPlaySpeed;
        // }
    }
    void OnEnable()
    {
        canuserAnger = false;
        // _angerAnim.Play("nvqi_0");

    }


    public void showHeroHeadIcon(int sex, List<int> dresslist, List<int> equips)
    {
        //创建ui形象
        CharacterManager.inst.GetCharacterByHero<GraphicDressUpSystem>((EGender)sex, equips, dresslist, 0.14f, callback: (system) =>
        {
            system.transform.SetParent(heroIcon.transform);
            system.transform.localScale = Vector3.one * 0.5f;
            system.transform.localPosition = Vector3.down * 185f + Vector3.right * 38.7f;
            system.SetDirection(RoleDirectionType.Right);
        });
    }
}
