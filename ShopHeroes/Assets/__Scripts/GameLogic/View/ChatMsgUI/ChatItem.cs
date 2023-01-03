using System.Collections;
using System.Collections.Generic;
using Mosframe;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChatItem : UIBehaviour, IDynamicScrollViewItem
{
    public ChatListItem selfItem;
    public ChatListItem OtherItem;
    public ChatListItem sysMsgTem;
    public int index;

    protected override void Start()
    {
        selfItem.button.onClick.AddListener(onSelfBtnClick);
        OtherItem.button.onClick.AddListener(onOtherBtnClick);
    }

    public void onUpdateItem(int _index)
    {
        index = _index;
    }
    int chattime = -1;
    int systemchattime = -1;
    ChatData _data;
    public void SetData(int currindex, ChatData data)
    {
        _data = data;
        if (currindex == 0 || currindex == 2)
        {
            systemchattime = -1;
            ChatListItem item;
            if (data.isSelf)
            {
                item = selfItem;
            }
            else
            {
                item = OtherItem;
            }
            item.gameObject.name = data.index + "_" + currindex;
            item.timeText.text = TimeUtils.pasttimeSpanStrip((int)GameTimer.inst.serverNow - data.chatTime);

            if (item.headGraphicSystem != null && item.headGraphicSystem.transform.parent == item.headIconParent)
            {
                item.headGraphicSystem.transform.SetParent(FGUI.inst.heroGraphicCacheParent);
                item.headGraphicSystem.transform.localPosition = Vector3.zero;
                item.headGraphicSystem = null;
            }
            item.headGraphicSystem = ChatDataProxy.inst.getGraphicDressByIndexAndChannel(data.index, currindex);
            if (item.headGraphicSystem != null)
            {
                item.headGraphicSystem.transform.SetParent(item.headIconParent);
                item.headGraphicSystem.transform.localScale = Vector3.one * 0.25f;
                item.headGraphicSystem.transform.localPosition = Vector3.down * 156f;
                item.headGraphicSystem.SetDirection(data.isSelf ? RoleDirectionType.Left : RoleDirectionType.Right);
            }
            else
            {
                Logger.error("输出 item没有拿到graphicDressUp");
            }

            if (chattime == data.index) return;
            chattime = data.index;
            item.nameText.text = LanguageManager.inst.GetValueByKey(data.nickName);
            item.msgText.text = data.content;
            item.levelText.text = data.level.ToString();
            //item.headicon.SetSprite("__common_1", data.gender == 1 ? "zhuui_touxiangnan" : "zhuui_touxiangnv");

            
            //if (item.headGraphicSystem == null)
            //{
            //    CharacterManager.inst.GetCharacter<GraphicDressUpSystem>(CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), SpineUtils.RoleDressToHeadDressIdList(data.roleDress), (EGender)data.gender, callback: (system) =>
            //    {
            //        item.headGraphicSystem = system;
            //        system.transform.SetParent(item.headIconParent);
            //        system.transform.localScale = Vector3.one * 0.25f;
            //        system.transform.localPosition = Vector3.down * 156f;
            //        system.SetDirection(data.isSelf ? RoleDirectionType.Left : RoleDirectionType.Right);
            //    });
            //}
            //else
            //{
            //    CharacterManager.inst.ReSetCharacter(item.headGraphicSystem, CharacterManager.inst.GetPeopleShapeNudeSpinePath((EGender)data.gender), SpineUtils.RoleDressToHeadDressIdList(data.roleDress), (EGender)data.gender);
            //    item.headGraphicSystem.SetDirection(data.isSelf ? RoleDirectionType.Left : RoleDirectionType.Right);
            //}

            selfItem.gameObject.SetActive(data.isSelf);
            OtherItem.gameObject.SetActive(!data.isSelf);
            sysMsgTem.gameObject.SetActive(false);
        }
        else
        {
            chattime = -1;
            sysMsgTem.timeText.text = TimeUtils.pasttimeSpanStrip((int)GameTimer.inst.serverNow - data.chatTime);
            if (systemchattime == data.chatTime) return;
            systemchattime = data.chatTime;

            selfItem.gameObject.SetActive(false);
            OtherItem.gameObject.SetActive(false);
            sysMsgTem.gameObject.SetActive(true);
            //sysMsgTem.msgText.text = data.content;
            if (data.content != null)
            {
                string[] info = data.content.Split('|');
                if (info.Length > 0)
                {
                    int.TryParse(info[0], out int msgType);

                    if (msgType == 1)
                    {
                        sysMsgTem.msgText.text = LanguageManager.inst.GetValueByKey("{0}加入公会", LanguageManager.inst.GetValueByKey(data.nickName));
                    }
                    else if (msgType == 2)
                    {
                        sysMsgTem.msgText.text = LanguageManager.inst.GetValueByKey("{0}退出公会", LanguageManager.inst.GetValueByKey(data.nickName));
                    }
                    else if (msgType == 3)
                    {
                        if (info.Length > 2)
                        {
                            if (info[1] == UserDataProxy.inst.playerData.userUid)
                            {
                                sysMsgTem.msgText.text = LanguageManager.inst.GetValueByKey("{0}帮助了我", LanguageManager.inst.GetValueByKey(data.nickName));
                            }
                            else
                            {
                                sysMsgTem.msgText.text = LanguageManager.inst.GetValueByKey("{0}帮助了{1}", LanguageManager.inst.GetValueByKey(data.nickName), LanguageManager.inst.GetValueByKey(info[2]));
                            }
                        }
                        else
                        {
                            sysMsgTem.gameObject.SetActive(false);
                        }
                    }
                    else if (msgType == 4)
                    {
                        if (info.Length > 2)
                        {
                            int.TryParse(info[1], out int buildId);
                            var buildCfg = BuildingConfigManager.inst.GetConfig(buildId);
                            if (buildCfg != null)
                                sysMsgTem.msgText.text = LanguageManager.inst.GetValueByKey("{0}投资了{1}{2}新币", LanguageManager.inst.GetValueByKey(data.nickName), LanguageManager.inst.GetValueByKey(buildCfg.name), info[2]);
                            else
                            {
                                sysMsgTem.gameObject.SetActive(false);
                            }
                        }
                        else
                        {
                            sysMsgTem.gameObject.SetActive(false);
                        }
                    }
                    else if (msgType == 5)
                    {
                        if (info.Length > 2)
                        {
                            int.TryParse(info[1], out int equipId);
                            var equipCfg = EquipConfigManager.inst.GetEquipInfoConfig(equipId);
                            if (equipCfg != null)
                            {
                                sysMsgTem.msgText.text = LanguageManager.inst.GetValueByKey("{0}向黑市商人售出{1}装备获得{2}新币", LanguageManager.inst.GetValueByKey(data.nickName), LanguageManager.inst.GetValueByKey(equipCfg.quality_name), info[2]);
                            }
                            else
                            {
                                sysMsgTem.gameObject.SetActive(false);
                            }
                        }
                        else
                        {
                            sysMsgTem.gameObject.SetActive(false);
                        }
                    }
                    else if (msgType == 6)
                    {
                        if (info.Length > 1)
                        {
                            int.TryParse(info[1], out int equipDrawingId);
                            var equipCfg = EquipConfigManager.inst.GetEquipDrawingsCfg(equipDrawingId);
                            if (equipCfg != null)
                            {
                                sysMsgTem.msgText.text = LanguageManager.inst.GetValueByKey("{0}精通了{1}图纸", LanguageManager.inst.GetValueByKey(data.nickName), LanguageManager.inst.GetValueByKey(equipCfg.name));
                            }
                            else
                            {
                                sysMsgTem.gameObject.SetActive(false);
                            }
                        }
                        else
                        {
                            sysMsgTem.gameObject.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                sysMsgTem.gameObject.SetActive(false);
            }
        }
    }


    void onSelfBtnClick()
    {
        //TODO 暂无自己的点击
    }

    void onOtherBtnClick()
    {
        EventController.inst.TriggerEvent(GameEventType.SocialEvent.REQUEST_OTHERUSERDATA, _data.userId);
    }

}
