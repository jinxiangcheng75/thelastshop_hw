using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PethouseFeedTips : MonoBehaviour
{
    public InputEventListener petFeedListener;

    public int FUid = 0;

    private void Start()
    {
        petFeedListener.OnClick = onPetFeedBtnClick;

    }

    void onPetFeedBtnClick(Vector3 mousePos)
    {
        var data = UserDataProxy.inst.GetFuriture(FUid);

        if (data != null)
        {
            EventController.inst.TriggerEvent(GameEventType.ShopDesignEvent.LOOKPETHOUSE, data);
        }

    }

    public void setSortingLayer(string sortingLayerName)
    {
        var renderlist = transform.GetComponentsInChildren<SetRendererOrder>(true);
        foreach (var _render in renderlist)
        {
            _render.setLayer(sortingLayerName);
        }
    }

    public void setSortingOrder(int order)
    {
        var renderlist = transform.GetComponentsInChildren<SetRendererOrder>(true);
        foreach (var _render in renderlist)
        {
            _render.setOrder(order,false);
        }
    }






}
