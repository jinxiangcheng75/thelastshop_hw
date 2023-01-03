using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

//商店装饰(店内装饰和墙面装饰)
public class DecorateEntity : Entity
{

    private GameObject decorateGo;
    private ActorPosCrl actorPosCrl;

    private List<SpriteRenderer> renderObjs = new List<SpriteRenderer>();
    public override void create()
    {
        base.create();
        currFurniture = FurnitureConfigManager.inst.getConfig(id);
        //currFurniture.type_1 == 4 墙饰\ ==5 室内摆件
        if (currFurniture == null)
        {
            Logger.error($"无法创建装饰，找不到对应{id}装饰配置！");
            Destroy(this.gameObject, 1);
            return;
        }
        ManagerBinder.inst.Asset.loadPrefabAsync(currFurniture.prefabnew, transform, OnLoaded);
    }

    private void OnLoaded(GameObject obj)
    {
        if (obj != null)
        {
            decorateGo = obj;
            decorateGo.transform.localPosition = Vector3.zero;

            var eventTriggerListener = decorateGo.GetComponent<InputEventListener>();
            if (eventTriggerListener != null)
            {
                eventTriggerListener.MouseUp += MouseUp;
                eventTriggerListener.MouseDown += MouseDown;
                eventTriggerListener.MouseDrag += Drag;
                eventTriggerListener.OnClick += MouseClick;
                eventTriggerListener.MousePointBlank += MousePointBlank;
            }
            //改变方向
            setDir(dir);

            foreach (Transform item in decorateGo.transform)
            {
                var render = item.GetComponent<SpriteRenderer>();
                if (render != null) renderObjs.Add(render);
            }

            updateSortingOrder();
        }
    }

    private void updateSortingOrder()
    {
        if (currFurniture != null)
        {
            if (currFurniture.type_1 == 4)
            {
                //墙饰 不沾地面格子

                for (int i = 0; i < renderObjs.Count; i++)
                {
                    var obj = renderObjs[i].gameObject;
                    renderObjs[i].sortingLayerName = "map_floor";
                    renderObjs[i].sortingOrder = MapUtils.GetTileMapOrder(obj.transform.position.y, obj.transform.position.x, currSize.x, currSize.y);
                }
            }
            else if (currFurniture.type_1 == 5)
            {
                //和家具逻辑一样。
                for (int i = 0; i < renderObjs.Count; i++)
                {
                    var obj = renderObjs[i].gameObject;

                    renderObjs[i].sortingOrder = MapUtils.GetTileMapOrder(obj.transform.position.y, obj.transform.position.x, currSize.x, currSize.y);
                }
            }
        }
    }
    private void setDir(int _dir)
    {
        dir = _dir;
        if (dir == 0)
        {
            currSize.x = currFurniture.width;
            currSize.y = currFurniture.height;
        }
        else
        {
            currSize.x = currFurniture.height;
            currSize.y = currFurniture.width;
        }
        if (decorateGo != null)
            decorateGo.transform.localScale = new Vector3(dir == 0 ? 1 : -1, 1, 1);
    }
}
