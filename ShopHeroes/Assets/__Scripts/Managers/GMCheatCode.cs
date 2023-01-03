using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMCheatCode : SingletonMono<GMCheatCode>
{
    [HideInInspector]
    public bool isActivate = false;
    string gmCode = "";
    void OnGUI()
    {
        if (isActivate)
        {
            GUI.BeginGroup(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 200, 400, 200));
            GUI.Box(new Rect(0, 0, 400, 200), "GM");

            GUIStyle bb = GUI.skin.textField;
            bb.fontSize = 40; //当然，这是字体颜色

            gmCode = GUI.TextField(new Rect(10, 20, 380, 100), gmCode, bb);
            if (GUI.Button(new Rect(10, 120, 380, 70), "Click me"))
            {
                Logger.log("Click me");
                sendGMCodeToServer();
                isActivate = false;
            }
            GUI.EndGroup();
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.F1))
            {
                isActivate = true;
            }
        }
    }

    public void sendGMCodeToServer()
    {

        if (!string.IsNullOrEmpty(gmCode))
        {
            if (gmCode.StartsWith("!"))
            {
                var msgtype = gmCode.Split(':')[0];
                var value = gmCode.Split(':')[1];
                switch (msgtype)
                {
                    case "!test_1":
                        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.UnLockDrawing, "", 101, 0, 1));
                        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.DrawingUpLv, "", 101, 0, 1));
                        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.NEWITEM_MSG, new queueItem(ReceiveInfoUIType.ActivateDrawing, "", 101, 0, 1));
                        EventController.inst.TriggerEvent(GameEventType.ReceiveEvent.GO_ON);
                        break;
                    case "!buff":
                        {
                            if (string.IsNullOrEmpty(value)) return;
                            string[] v = value.Split(',');
                            if (v.Length == 2)
                            {
                                EventController.inst.TriggerEvent(GameEventType.CombatEvent.COMBAT_ADDBUFF_TEST, int.Parse(v[0]), int.Parse(v[1]));
                            }
                        }
                        break;
                    case "!skill":
                        {
                            if (string.IsNullOrEmpty(value)) return;
                            string[] v = value.Split(',');
                            if (v.Length == 2)
                            {
                                EventController.inst.TriggerEvent(GameEventType.CombatEvent.COMBAT_USESKILL_TEST, int.Parse(v[0]), int.Parse(v[1]));
                            }
                        }
                        break;
                    case "!test001":
                        {
                            System.Action aaaa = testcabck;
                            List<int> ids = new List<int>();
                            ids.Add(12074);
                            ids.Add(12083);
                            ids.Add(12102);
                            EventController.inst.TriggerEvent(GameEventType.UseAdvancedEquip, aaaa, ids,LanguageManager.inst.GetValueByKey("会被用于这次制作，仍要继续吗？"));
                        }
                        break;
                }
                return;
            }

            NetworkEvent.SendRequest(new NetworkRequestWrapper()
            {
                req = new Request_Gm_Command()
                {
                    command = gmCode
                }
            });
        }
    }

    void testcabck()
    {
        Logger.log("C测试完成");
    }
}
