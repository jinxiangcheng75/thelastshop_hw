using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class FollowBloodstrip : MonoBehaviour
{
    public int key;
    public Transform target;
    // public Text bloodsChangeText;
    public Slider hpBarL;
    public Slider hpBarR;
    public HealthEffect effectTF;
    // public Slider AngerBar;
    private Camera maincamera;
    private int CurrentHp;
    private int maxHp;
    private Slider currHpBar;
    public TextMeshPro effectText;

    public Sprite hp_g;
    public Sprite hp_y;
    public Sprite hp_r;
    public Animator _hpAnimator;
    public Image barSprite;
    // Start is called before the first frame update

    public Slider nvqiBar;
    public GUIIcon jobBg;
    public GUIIcon jobIcon;
    public Text lvText;

    void Start()
    {
        maincamera = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        if (maincamera == null)
        {
            maincamera = Camera.main;
        }
        if (target == null) return;
        Vector3 pos = maincamera.WorldToScreenPoint(target.position);
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(FGUI.inst.hudPlanel, pos, FGUI.inst.uiCamera, out localPoint))
        {
            transform.localPosition = localPoint;
        }
    }

    public void SetInitInfo(bool isSelf, int maxHp, int currHp, int maxAnger, int currAnger, int lv, int job)
    {
        nvqiBar.gameObject.SetActive(isSelf);
        jobBg.gameObject.SetActive(isSelf);

        hpBarR.gameObject.SetActive(true);
        hpBarL.gameObject.SetActive(true);

        hpBarR.maxValue = maxHp;
        hpBarL.maxValue = maxHp;
        hpBarR.value = currHp;
        hpBarL.value = currHp;

        CurrentHp = currHp;
        this.maxHp = maxHp;
        if (isSelf)
        {
            HeroProfessionConfigData herocfg = HeroProfessionConfigManager.inst.GetConfig(job);
            jobBg.SetSprite(StaticConstants.roleAtlasName, StaticConstants.heroTypeBgIconName[herocfg.type - 1]);
            jobIcon.SetSprite(herocfg.atlas, herocfg.ocp_icon);
            lvText.text = lv.ToString();
            nvqiBar.maxValue = maxAnger;
            nvqiBar.value = currAnger;
        }
        updateBarSprite();

    }

    private void updateBarSprite()
    {
        var v = (float)hpBarR.value / (float)hpBarR.maxValue;
        if (v > 0.66f)
        {
            if (barSprite.sprite != hp_g)
                _hpAnimator.SetTrigger("doudong");

            barSprite.sprite = hp_g;
        }
        else if (v > 0.33f)
        {
            if (barSprite.sprite != hp_y)
                _hpAnimator.SetTrigger("doudong");
            barSprite.sprite = hp_y;
        }
        else
        {
            if (barSprite.sprite != hp_r)
                _hpAnimator.SetTrigger("doudong");
            barSprite.sprite = hp_r;
        }
    }
    public void hpChange(int hp, bool ishedge, bool critical = false)
    {

        if (!ishedge)
        {
            if (hp == 0) return;
            CurrentHp += hp;
            if(CurrentHp > maxHp)
            {
                CurrentHp = maxHp;
            }

            hpBarR.value = CurrentHp;
            hpBarL.DOValue(CurrentHp, 0.5f, true).SetDelay(0.2f);
            if (effectTF != null)
            {
                var heff = GameObject.Instantiate(effectTF, effectTF.transform.parent, false);
                heff.isCurrentHp = critical;
                heff.ShowFx("", hp);
                heff.gameObject.SetActive(true);
            }
            updateBarSprite();
        }
        else
        {
            if (effectTF != null)
            {
                var heff = GameObject.Instantiate(effectTF, effectTF.transform.parent, false);
                heff.ShowFx("zhandou_weimingzhong", 0);
                heff.gameObject.SetActive(true);
            }
        }
    }

    public void showStateText(string name)
    {
        var heff = GameObject.Instantiate(effectTF, effectTF.transform.parent, false);
        heff.ShowFx(name, 0);
        heff.isCurrentHp = false;
        heff.gameObject.SetActive(true);
    }
    public void AngerChange(int anger)
    {
        nvqiBar.value = anger;
    }
}
