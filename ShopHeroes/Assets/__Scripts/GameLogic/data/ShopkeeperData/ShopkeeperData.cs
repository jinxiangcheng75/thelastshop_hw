using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopkeeperData
{
    public string atlasName;
    public string iconName;
    public string highLightName;
    public uint typeStr;

    public ShopkeeperData(string atlasName, string iconName, string highLightName, uint typeStr)
    {
        this.atlasName = atlasName;
        this.iconName = iconName;
        this.highLightName = highLightName;
        this.typeStr = typeStr;
    }
}

