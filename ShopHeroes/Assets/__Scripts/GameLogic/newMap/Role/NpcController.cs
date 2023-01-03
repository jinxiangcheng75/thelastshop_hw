using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : IndoorRole
{
    public bool promptIsShow;
    public int npcId;
    protected override void Init()
    {
        base.Init();
        gameObject.name = "我是npc";
    }

    public void setData(int modelId, float size, Vector3Int npcPos, bool needShowPrompt, RoleDirectionType dir)
    {
        //actorAttacher = gameObject.GetComponent<ActorAttacher>();
        promptIsShow = !needShowPrompt;
        if (Attacher != null)
        {
            //Attacher.mCol.enabled = true;
            SetBubbleClickHandler(clickPopup);
            Attacher.spRoot.position -= new Vector3(0, 0, 10);
            //Attacher.GetComponent<PolygonCollider2D>().enabled = true;
        }

        CharacterManager.inst.GetCharacterByModel<DressUpSystem>(modelId, size, callback: (dress) =>
        {
            _character = dress;
            SetCellPos(npcPos);
            _character.transform.SetParent(_attacher.actorParent, false);
            GuideDataProxy.inst.CurInfo.isCreatFinish = true;
            GuideManager.inst.GuideManager_OnCheckGuide(K_Guide_End.CreatNpcFinish, 0);
            UpdateSortingOrder();
            dress.SetDirection(dir);
            dress.Play("idle_1",true);
        });
    }

    private void clickPopup()
    {
        AudioManager.inst.PlaySound(62);
        EventController.inst.TriggerEvent(GameEventType.GuideEvent.FINGERACTIVEFALSE);
        Attacher.HidePopup(() =>
        {
            GuideDataProxy.inst.CurInfo.isClickTarget = true;
            GuideManager.inst.GuideManager_OnCheckGuide(K_Guide_End.ClickTarget, 0);
        });
    }

    int timerid = 0;
    public void ChangeRoleDirection(RoleDirectionType directioNType)
    {
        if (Character == null)
        {
            if (timerid > 0)
            {
                GameTimer.inst.RemoveTimer(timerid);
                timerid = 0;
            }

            timerid = GameTimer.inst.AddTimer(0.2f, () =>
             {
                 if (Character != null)
                 {
                     Character.SetDirection(directioNType);
                     GameTimer.inst.RemoveTimer(timerid);
                     timerid = 0;
                 }
             });
        }
        else
        {
            Character.SetDirection(directioNType);
        }
    }
}
