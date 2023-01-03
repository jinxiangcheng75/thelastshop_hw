using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndoorEnvironmentObjVisibleClr : SingletonMono<IndoorEnvironmentObjVisibleClr>
{
    public List<GameObject> otherGameObj;
    private Dictionary<Transform, Vector3Int> ObjCellPosintList = new Dictionary<Transform, Vector3Int>();
    void Start()
    {
        foreach (Transform child in transform)
        {
            Vector3Int cell = MapUtils.WorldPosToCellPos(child.localPosition + transform.position);
            child.name = $"{cell.x}:{cell.y}";
            ObjCellPosintList.Add(child, cell);
        }
    }

    public void setVisible(bool visible)
    {
        foreach (var go in otherGameObj)
        {
            go.SetActive(visible);
        }
        gameObject.SetActive(visible);
    }
    public void UpdateObjVisible()
    {
        StopCoroutine(checkprocess());
        this.StartCoroutine(checkprocess());
    }
    IEnumerator checkprocess()
    {
        RectInt unVisibleRect = UserDataProxy.inst.GetIndoorSize();
        unVisibleRect.xMin -= 1;
        unVisibleRect.yMin -= 7;
        unVisibleRect.xMin += StaticConstants.IndoorOffsetX;
        unVisibleRect.xMax += StaticConstants.IndoorOffsetX;
        yield return null;
        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf) continue;
            if (CheckRect(unVisibleRect, ObjCellPosintList[child]))
            {
                child.gameObject.SetActive(false);
            }
            yield return new WaitForFixedUpdate();
        }
    }
    private bool CheckRect(RectInt rectangle, Vector3Int point)
    {
        //判断点是否在区域内
        if (point.x < rectangle.xMin || point.x > rectangle.xMax || point.y < rectangle.yMin || point.y > rectangle.yMax)
            return false;
        return true;
    }
}
