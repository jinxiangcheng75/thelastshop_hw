using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainLinePrefabItem : MonoBehaviour
{
    public GUIIcon icon;
    public Text descText;

    public void setData(int itemId, long count)
    {
        var cfg = ItemconfigManager.inst.GetConfig(itemId);
        if (cfg == null)
        {
            Logger.error("在item表中没有找到id是" + itemId + "的数据");
            return;
        }

        icon.SetSprite(cfg.atlas, cfg.icon);
        descText.text = LanguageManager.inst.GetValueByKey(cfg.name) + "x" + count;

        gameObject.SetActive(true);
        var transRect = transform.GetComponent<RectTransform>();
        transRect.DOAnchorPos3DY(transRect.anchoredPosition.y + 300, 0.8f).OnComplete(() =>
         {
             icon.iconImage.DOFade(0, 0.35f);
             descText.DOFade(0, 0.35f).OnComplete(() =>
              {
                  Destroy(gameObject);
              });
         });
    }
}
