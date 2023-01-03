using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUpItem : MonoBehaviour
{

    public Button selfBtn;
    public Image goldIcon;
    public GUIIcon typeIcon;
    public Text explainText;
    private PlayerUpItemData _data;
    public Image orderIcon;
    public Text orderText;

    public Animator btnAnimator;

    private void Start()
    {
        selfBtn.onClick.AddListener(SelfBtnOnClick);
    }

    public void SetData(PlayerUpItemData data, int index)
    {
        _data = data;
        gameObject.SetActive(true);

        switch (data.mainType)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 8:
                goldIcon.gameObject.SetActive(true);
                break;
            default:
                goldIcon.gameObject.SetActive(false);
                break;
        }

        switch (data.mainType)
        {
            case 0:
                orderIcon.gameObject.SetActive(true);
                orderText.text = data.mainValue + "";
                break;
            default:
                orderIcon.gameObject.SetActive(false);
                //orderText.text = "";
                break;
        }

        explainText.text = LanguageManager.inst.GetValueByKey(StaticConstants.PlayerUpitemDesc[data.mainType]);

        if (string.IsNullOrEmpty(StaticConstants.PlayerUpTypeSprites[data.mainType]))
        {
            if (data.mainType == 7) //新工匠
            {
                var workerCfg = WorkerConfigManager.inst.GetConfig(data.mainValue);
                if (workerCfg == null)
                {
                    Logger.error("工匠表中无此ID：" + data.mainValue);
                }
                string _atlasName = StaticConstants.roleHeadIconAtlasName;
                string _spriteName = workerCfg.icon;
                typeIcon.SetSprite(_atlasName, _spriteName);
                typeIcon.iconImage.rectTransform.sizeDelta = Vector2.one * 150f;
            }
        }
        else
        {
            if (data.mainType == 9 || data.mainType == 10)
            {
                var cfg = FurnitureConfigManager.inst.getConfig(data.mainValue);
                if (cfg == null)
                {
                    Logger.error("家具表中无此ID：" + data.mainValue);
                }
                else
                {
                    typeIcon.SetSprite(cfg.atlas, cfg.icon, needSetNativeSize: true);
                    explainText.text = LanguageManager.inst.GetValueByKey(cfg.name);
                }
            }
            else
            {
                typeIcon.SetSprite("PlayerUp_atlas", StaticConstants.PlayerUpTypeSprites[data.mainType]/*, needSetNativeSize: true*/);
            }
        }

        if (GameSettingManager.inst.needShowUIAnim) doAnim(index);
    }

    public void Clear()
    {
        _data = null;
        gameObject.SetActive(false);

    }

    private void SelfBtnOnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.SHOWUI_MSGBOXPLAYERUPITEMUI, _data);
    }

    void doAnim(int index)
    {
        gameObject.SetActive(false);

        GameTimer.inst.AddTimer(0.46f + 0.1f * index, 1, () =>
        {
            if (this == null) return;
            gameObject.SetActive(true);
            btnAnimator.CrossFade("show", 0f);
            btnAnimator.Update(0f);
            btnAnimator.Play("show");
        });
    }



}
