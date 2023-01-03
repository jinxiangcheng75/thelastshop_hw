using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMapInfo
{
    public Transform sceneCentreTF;  //场景中心点，一般用作全屏特效释放
    public Dictionary<int, Transform> attackerSites = new Dictionary<int, Transform>();
    public Dictionary<int, Transform> adversarySites = new Dictionary<int, Transform>();

    public Transform skillBlackPlane;
    public Transform GetSiteTF(int site, bool isAdversary)
    {
        Transform siteTf = null;
        if (isAdversary)
        {
            adversarySites.TryGetValue(site, out siteTf);
            return siteTf;
        }
        attackerSites.TryGetValue(site, out siteTf);
        return siteTf;
    }
    //场景加载完成后获取地图数据
    public void initMapInfo()
    {
        //战斗中
        if (ManagerBinder.inst.mGameState == kGameState.Battle)
        {
            var sitesObj = GameObject.Find("Sites");
            if (sitesObj != null)
            {
                sceneCentreTF = sitesObj.transform.Find("centre");

                var attackertf = sitesObj.transform.Find("attacker");
                foreach (Transform tf in attackertf)
                {

                    int index = int.Parse(tf.name);
                    attackerSites[index] = tf;
                }
                var adversarytf = sitesObj.transform.Find("adversary");
                foreach (Transform tf in adversarytf)
                {
                    int index = int.Parse(tf.name);
                    adversarySites[index] = tf;
                }

                skillBlackPlane = GameObject.Find("blackplane").transform;
                if (skillBlackPlane != null)
                {
                    skillBlackPlane.gameObject.SetActive(false);
                }
            }

            Transform camerastartTF = GameObject.Find("CameraStartPoint").transform;
            if (camerastartTF != null)
            {
                Camera.main.transform.position = camerastartTF.position;
                if (FGUI.inst.isLandscape)
                {
                    float currvalue = (float)FGUI.inst.GameDesignSceneSize.x / (float)FGUI.inst.GameDesignSceneSize.y;
                    float designvalue = (float)Screen.height * currvalue;
                    Camera.main.orthographicSize = StaticConstants.combatCameraOrthographicLSize * designvalue / (float)Screen.width;
                }
                else
                {
                    float currvalue = (float)FGUI.inst.GameDesignSceneSize.y / (float)FGUI.inst.GameDesignSceneSize.x;
                    float designvalue = (float)Screen.width * currvalue;
                    Camera.main.orthographicSize = StaticConstants.combatCameraOrthographicSize * (float)Screen.height / designvalue;
                }
            }
            else
            {
                Camera.main.transform.position = Vector3.zero;
                Camera.main.orthographicSize = 5;
            }
        }
    }
}
