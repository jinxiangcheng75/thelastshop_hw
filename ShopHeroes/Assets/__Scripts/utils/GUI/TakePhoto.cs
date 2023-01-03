using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class takedata
{
    public int key;
    public int sex;
    public List<int> dressIds;
    public List<int> equips;

    public takedata(int _key, int _sex, List<int> _dressIds, List<int> _equips)
    {
        key = _key;
        sex = _sex;
        dressIds = _dressIds;
        equips = _equips;
    }

}
public class TakePhoto : SingletonMono<TakePhoto>
{
    public Camera targetCamera;
    public Transform photoPoint;
    public DressUpSystem dressUpClr;
    private bool taking = false;
    private Dictionary<int, Texture2D> texture2dlist = new Dictionary<int, Texture2D>();
    private List<takedata> takesList = new List<takedata>();
    public override void init()
    {
        base.init();
        this.gameObject.SetActive(false);
    }
    public void RemovePhoto(int key)
    {

    }
    public void TakeToPhoto(int key, int gender, List<int> dressIds, List<int> equips)
    {
        // if (texture2dlist.ContainsKey(key))
        // {
        //     return;
        // }

        if (taking)
        {
            takesList.Add(new takedata(key, gender, dressIds, equips));
            return;
        }
        this.gameObject.SetActive(true);
        taking = true;
        StartCoroutine(startTake(key, gender, dressIds, equips));
    }

    IEnumerator startTake(int key, int gender, List<int> dressIds, List<int> equips)
    {
        yield return null;
        if (dressUpClr == null)
        {
            CharacterManager.inst.GetCharacterByHero<DressUpSystem>((EGender)gender, equips, dressIds, callback: (dressUpSystem) =>
            {
                dressUpClr = dressUpSystem;
                var go = dressUpSystem.gameObject;
                go.transform.SetParent(photoPoint);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                //dressUpClr.SetSortingAndOrderLayer("map_Actor", 0);
            });
        }
        else
        {
            dressUpClr.SetAnimationSpeed(0);
            CharacterManager.inst.ReSetCharacterByHero(dressUpClr, (EGender)gender, equips, dressIds);
        }
        while (dressUpClr == null || dressUpClr.isInDressing)
        {
            yield return null;
        }
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return new WaitForEndOfFrame();

        Texture2D texture = Helper.capture(targetCamera, 512, 512);
        if (texture != null)
        {
            if (texture2dlist.ContainsKey(key))
            {
                texture2dlist[key] = texture;
            }
            else
                texture2dlist.Add(key, texture);
        }
        yield return null;
        if (takesList.Count > 0)
        {
            var data = takesList[0];
            takesList.Remove(data);
            //taking = false;
            // TakeToPhoto(data.key, data.sex, data.equips);
            yield return startTake(data.key, data.sex, data.dressIds, data.equips);
        }
        else
        {
            taking = false;
            this.gameObject.SetActive(false);
        }
    }
    public Texture2D GetTexture2D(int key)
    {
        Texture2D texture = null;
        texture2dlist.TryGetValue(key, out texture);
        return texture;
    }
    public void clearTexture2d()
    {
        foreach (Texture2D texture in texture2dlist.Values)
        {
            //销毁
            Destroy(texture);
        }
        texture2dlist.Clear();
    }

    void OnDisable()
    {
        if (dressUpClr != null)
        {
            Destroy(dressUpClr.gameObject);
            dressUpClr = null;
        }
    }
}
