using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.AddressableAssets;
public class AtlasStack
{
    public bool aoutRelease = true;
    public AssetReferenceAtlasedSprite referenceAtlas = null;
    public int hit = 0;
    public AtlasStack(string name, bool aoutrelease)
    {
        referenceAtlas = new AssetReferenceAtlasedSprite("name");
        aoutRelease = aoutrelease;
        hit = 1;
        Load();
    }
    public void Release()
    {

        referenceAtlas.ReleaseAsset();
    }
    public bool IsValid()
    {
        return referenceAtlas.IsValid();
    }

    public void Load()
    {
        referenceAtlas.LoadAssetAsync();
    }

    public Sprite GetSprite(string spritename)
    {
        return (referenceAtlas.Asset as SpriteAtlas).GetSprite(spritename);
    }
}

public class GameAtlasSpriteManager : TSingletonHotfix<GameAtlasSpriteManager>
{
    Dictionary<string, AtlasStack> atlasList = new Dictionary<string, AtlasStack>();
    public void loadAtlas(string name, bool aoutrelease = true)
    {
        if (atlasList.ContainsKey(name))
        {
            atlasList[name].hit++;
            return;
        }
        atlasList.Add(name, new AtlasStack(name, aoutrelease));
    }
    public void removeAtlas(string name)
    {
        if (atlasList.ContainsKey(name))
        {
            atlasList[name].hit--;
            //atlasList[name].Release();
        }
    }
    public bool ValidateAtlas(string name)
    {
        if (atlasList.ContainsKey(name))
        {
            return atlasList[name].IsValid();
        }
        return false;
    }

    public Sprite GetSprite(string atlasname, string spritename)
    {
        if (ValidateAtlas(atlasname))
        {
            atlasList[atlasname].hit++;
            return atlasList[atlasname].GetSprite(spritename);
        }
        Debug.LogError($"{atlasname}图集未被加载");
        return null;
    }

    public void ReleaseSprite(string atlasname)
    {
        if (ValidateAtlas(atlasname))
        {
            atlasList[atlasname].hit--;
        }
    }

    public void Clear(bool force = true)
    {
        List<string> removelist = new List<string>();
        foreach (var item in atlasList)
        {
            if (item.Value.aoutRelease)
            {
                removelist.Add(item.Key);
            }
        }

        foreach (string key in removelist)
        {
            atlasList[key].Release();
            atlasList.Remove(key);
        }

        Resources.UnloadUnusedAssets();
    }
}
