using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUnlockWorker : MonoBehaviour
{
    public Text nameText;
    public Text contentText;
    public Text typeText;
    public Text unlockText;
    public List<GameObject> unlockItems;
    public List<GUIIcon> unlockItemsIcon;
    public Button freeBtn;
    public Transform modelPos;
    GraphicDressUpSystem graphicDressUp;

    private void Awake()
    {
        freeBtn.onClick.AddListener(() =>
        {
            AudioManager.inst.PlaySound(62);
            GuideManager.inst.GuideManager_OnNextGuide();
        });
    }

    public void showGUnlockWorker()
    {
        AudioManager.inst.PlaySound(25);
        gameObject.SetActiveTrue();

        var cfg = GuideDataProxy.inst.CurInfo.m_curCfg;
        var workerCfg = WorkerConfigManager.inst.GetConfig(cfg.character_id);
        creatNpcModel(workerCfg.model);
        nameText.text = LanguageManager.inst.GetValueByKey(workerCfg.name);
        typeText.text = LanguageManager.inst.GetValueByKey(workerCfg.profession);
        contentText.text = LanguageManager.inst.GetValueByKey(workerCfg.desc);

        var itemDatas = workerCfg.equipment_id;

        for (int i = 0; i < unlockItems.Count; i++)
        {
            if (i >= itemDatas.Length)
            {
                unlockItems[i].SetActiveFalse();
            }
            else
            {
                unlockItems[i].SetActiveTrue();
                var equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(itemDatas[i]);
                unlockItemsIcon[i].SetSprite(equipCfg.atlas, equipCfg.icon);
            }
        }
    }

    private void creatNpcModel(int modelId)
    {
        if (graphicDressUp == null)
        {
            CharacterManager.inst.GetCharacterByModel<GraphicDressUpSystem>(modelId, callback: (dressUpSystem) =>
            {
                graphicDressUp = dressUpSystem;
                graphicDressUp.transform.SetParent(modelPos);
                graphicDressUp.transform.localScale = Vector3.one * 1.3f;
                graphicDressUp.transform.localPosition = Vector3.zero;
                graphicDressUp.Play("idle_1", true);
            });
        }
        else
        {
            CharacterManager.inst.ReSetCharacterByModel(graphicDressUp, modelId);
            graphicDressUp.Play("idle_1", true);
        }
    }

    public void hideGUnlockWorker()
    {
        gameObject.SetActiveFalse();
    }
}
