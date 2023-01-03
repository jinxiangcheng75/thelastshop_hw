using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EquipIconItemComp : MonoBehaviour
{
    public Image bgIcon;
    public Image equipIcon;

    public void SetSprite(int id)
    {
        //设置装备Img 暂时先隐藏
        equipIcon.color = new Color(0, 0, 0, 0);

        //根据喜恶更改颜色

        // if (HeroDataProxy.inst.GetCurHero().data.heroitemconfig.favorite_id == id)
        // {
        //     bgIcon.color = Color.yellow;
        // }
        // else if (HeroDataProxy.inst.GetCurHero().data.heroitemconfig.hate_id.Contains((int)id))
        // {
        //     bgIcon.color = Color.grey;
        // }
        // else
        // {
        //     bgIcon.color = Color.white;
        // }
    }


}
