using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShelfContentCtrlComp : MonoBehaviour
{
    public GUIIcon[] img_contentTypeList;
    public ShelfGridComp[] m_nodeList;
    public GUIIcon[] img_levelShelfList;

    bool fieldInit;
    void SetContentTypes(int shelfType)
    {

        List<int> types = ShelfUpgradeConfigManager.inst.GetShelfImgTypes(shelfType);

        for (int i = 0; i < img_contentTypeList.Length; i++)
        {
            if (i < types.Count)
            {
                img_contentTypeList[i].gameObject.SetActive(true);



                EquipClassification classcfg = EquipConfigManager.inst.GetEquipTypeByID(types[i]);
                img_contentTypeList[i].SetSprite(classcfg.Atlas, classcfg.icon);

            }
            else
            {
                img_contentTypeList[i].gameObject.SetActive(false);
            }

        }
    }

    public void SetNodes(bool isShowSmallType, IndoorData.ShopDesignItem shelf)
    {
        if (!fieldInit)
        {
            for (int i = 0; i < m_nodeList.Length; i++)
            {
                m_nodeList[i].field = i + 1;
            }
            fieldInit = true;
        }

        for (int i = 0; i < m_nodeList.Length; i++)
        {
            m_nodeList[i].isShowTypes = isShowSmallType;
        }

        RefreshShelfGridItem(shelf);
        SetContentTypes(shelf.config.type_2);
    }

    public void RefreshShelfGridItem(IndoorData.ShopDesignItem shelf)
    {

        List<ShelfEquip> equipItems = shelf.equipList;
        ShelfUpgradeConfig curConfig = ShelfUpgradeConfigManager.inst.getConfigByType(shelf.config.type_2, shelf.level);


        //该格对应的Field为-1隐藏 为0锁定
        for (int i = m_nodeList.Length - 1; i >= 0; i--)
        {
            int[] field = curConfig.getFieldByLevel(i + 1);

            m_nodeList[i].SetTypes(field);

            if (field[0] == -1)
            {
                m_nodeList[i].gameObject.SetActive(false);
            }
            else if (field[0] == 0)
            {
                m_nodeList[i].Lock();
            }
            else
            {
                ShelfEquip equip = equipItems.Find(t => t.fieldId == m_nodeList[i].field);
                m_nodeList[i].SetData(equip,shelf.uid);
            }
        }
    }

}
