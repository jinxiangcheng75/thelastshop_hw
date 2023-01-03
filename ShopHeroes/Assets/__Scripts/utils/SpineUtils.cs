using Spine;
using Spine.Unity;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public static class SpineUtils
{
    public static List<int> RoleDressToUintList(RoleDress roleDress)
    {
        List<int> cfgIds = new List<int>();

        if (roleDress == null) return cfgIds;

        cfgIds.Add(roleDress.modelColor);
        cfgIds.Add(roleDress.face);
        cfgIds.Add(roleDress.faceColor);
        cfgIds.Add(roleDress.hair);
        cfgIds.Add(roleDress.hairColor);
        cfgIds.Add(roleDress.eyesColor);


        cfgIds.Add(roleDress.weapon);
        cfgIds.Add(roleDress.upper);
        cfgIds.Add(roleDress.lower);
        cfgIds.Add(roleDress.shoes);
        cfgIds.Add(roleDress.headHat);

        cfgIds.RemoveAll(t => t == 0);
        return cfgIds;
    }

    public static List<int> RoleDressToHeadDressIdList(RoleDress roleDress)
    {
        List<int> cfgIds = new List<int>();

        if (roleDress == null) return cfgIds;

        cfgIds.Add(roleDress.modelColor);
        cfgIds.Add(roleDress.face);
        cfgIds.Add(roleDress.faceColor);
        cfgIds.Add(roleDress.hair);
        cfgIds.Add(roleDress.hairColor);
        cfgIds.Add(roleDress.eyesColor);

        cfgIds.Add(roleDress.upper);
        cfgIds.Add(roleDress.headHat);

        cfgIds.RemoveAll(t => t == 0);
        return cfgIds;
    }

    public static int GetDressIdByType_2(this RoleDress roleDress, int type_02)
    {

        int dressId = 0;

        if (type_02 < (int)FacadeType.max) //外观
        {
            switch ((FacadeType)type_02)
            {
                case FacadeType.ModelColor: dressId = roleDress.eyesColor; break;
                case FacadeType.Hair: dressId = roleDress.hair; break;
                case FacadeType.HairColor: dressId = roleDress.hairColor; break;
                case FacadeType.Face: dressId = roleDress.face; break;
                case FacadeType.FaceColor: dressId = roleDress.faceColor; break;
                case FacadeType.EyesColor: dressId = roleDress.eyesColor; break;
            }
        }
        else  //时装
        {
            switch ((FashionType)type_02)
            {
                case FashionType.Clothe: dressId = roleDress.upper; break;
                case FashionType.Pants: dressId = roleDress.lower; break;
                case FashionType.HeadHat: dressId = roleDress.headHat; break;
                case FashionType.Shoes: dressId = roleDress.shoes; break;
                case FashionType.Weapon: dressId = roleDress.weapon; break;
            }
        }

        return dressId;
    }

    public static string GetPlaceWeaponSlotName(EGender gender, EquipSubType type, out string placeWeaponAniName)
    {
        string slotname = "";
        placeWeaponAniName = "";

        int weapon_type = 0; //0 盾牌 1 轻、小武器  2 重、大武器

        switch (type)
        {
            //利刃和轻武器别在腰间
            case EquipSubType.sharp:
            case EquipSubType.handgun:
                weapon_type = 1;
                break;
            //其余放背后-、-
            case EquipSubType.blunt:
            case EquipSubType.bow:
            case EquipSubType.rifle:
            case EquipSubType.heavy:
            case EquipSubType.Biochemical:
            case EquipSubType.bomb:
                weapon_type = 2;
                break;
            case EquipSubType.shield: //盾牌
                slotname = gender == EGender.Male ? StaticConstants.man_back_shield_slotName : StaticConstants.woman_back_shield_slotName;
                placeWeaponAniName = "putshield";
                break;
            default:
                Logger.error("非武器类型");
                break;
        }

        if (weapon_type != 0)
        {
            switch (gender)
            {
                case EGender.Male:
                    slotname = weapon_type == 1 ? StaticConstants.man_back_one_slotName : StaticConstants.man_back_two_slotName;
                    break;
                case EGender.Female:
                    slotname = weapon_type == 1 ? StaticConstants.woman_back_one_slotName : StaticConstants.woman_back_two_slotName;
                    break;
            }

            placeWeaponAniName = weapon_type == 1 ? "putgunone" : "putguntwo";
        }

        return slotname;
    }


    public static bool MyEquals(this RoleDress dress1, RoleDress dress2)
    {
        bool result = true;

        if (dress1 != null && dress2 != null)
        {
            if (dress1.modelColor != dress2.modelColor) result = false;
            if (dress1.face != dress2.face) result = false;
            if (dress1.faceColor != dress2.faceColor) result = false;
            if (dress1.hair != dress2.hair) result = false;
            if (dress1.hairColor != dress2.hairColor) result = false;
            if (dress1.eyesColor != dress2.eyesColor) result = false;
            if (dress1.weapon != dress2.weapon) result = false;
            if (dress1.upper != dress2.upper) result = false;
            if (dress1.lower != dress2.lower) result = false;
            if (dress1.shoes != dress2.shoes) result = false;
            if (dress1.headHat != dress2.headHat) result = false;
        }
        else
        {
            result = false;
        }


        return result;
    }


    #region 同步更新Asset


    static bool SkeletonDataAssetIsValid(SkeletonDataAsset asset)
    {
        return asset != null && asset.GetSkeletonData(quiet: true) != null;
    }

    static void ReloadSkeletonDataAsset(SkeletonDataAsset skeletonDataAsset)
    {
        if (skeletonDataAsset != null)
        {
            foreach (AtlasAssetBase aa in skeletonDataAsset.atlasAssets)
            {
                if (aa != null) aa.Clear();
            }
            skeletonDataAsset.Clear();
        }
        skeletonDataAsset.GetSkeletonData(true);
    }

    static void ReinitializeComponent(SkeletonRenderer component)
    {
        if (component == null) return;
        if (!SkeletonDataAssetIsValid(component.SkeletonDataAsset)) return;

        var stateComponent = component as IAnimationStateComponent;
        Spine.AnimationState oldAnimationState = null;
        if (stateComponent != null)
        {
            oldAnimationState = stateComponent.AnimationState;
        }

        component.Initialize(true); // implicitly clears any subscribers

        if (oldAnimationState != null)
        {
            stateComponent.AnimationState.AssignEventSubscribersFrom(oldAnimationState);
        }

#if BUILT_IN_SPRITE_MASK_COMPONENT
			SpineMaskUtilities.EditorAssignSpriteMaskMaterials(component);
#endif
        component.LateUpdate();
    }

    static void ReloadSkeletonDataAssetAndComponent(SkeletonRenderer component)
    {
        if (component == null) return;
        ReloadSkeletonDataAsset(component.skeletonDataAsset);
        ReinitializeComponent(component);
    }

    public static void ReinitializeComponent(SkeletonGraphic component)
    {
        if (component == null) return;
        if (!SkeletonDataAssetIsValid(component.SkeletonDataAsset)) return;
        component.Initialize(true);
        component.LateUpdate();
    }

    public static void ReloadSkeletonDataAssetAndComponent(SkeletonGraphic component)
    {
        if (component == null) return;
        ReloadSkeletonDataAsset(component.skeletonDataAsset);
        // Reinitialize.
        ReinitializeComponent(component);
    }


    //同步重新加载SkeletonDataAsset
    public static void ReLoadSkeletonDataAssetSync(this DressUpBase system, SkeletonDataAsset asset)
    {
        if (system as DressUpSystem)
        {
            system.SkeletonDataAsset = asset;
            //SpineEditorUtilities.ReloadSkeletonDataAssetAndComponent((system as DressUpSystem).SkeletonRenderer);
            //ReloadSkeletonDataAssetAndComponent((system as DressUpSystem).SkeletonRenderer);
            (system as DressUpSystem).SkeletonRenderer.Initialize(true);
        }
        else
        {
            system.SkeletonDataAsset = asset;
            //SpineEditorUtilities.ReloadSkeletonDataAssetAndComponent((system as GraphicDressUpSystem).SkeletonGraphic);
            //ReloadSkeletonDataAssetAndComponent((system as GraphicDressUpSystem).SkeletonGraphic);
            (system as GraphicDressUpSystem).SkeletonGraphic.Initialize(true);
        }
    }

    #endregion



    #region Spine合批相关 废弃

    static List<AtlasRegion> GetRegions(Atlas atlas)
    {
        FieldInfo regionsField = typeof(Atlas).GetField("regions", BindingFlags.NonPublic | BindingFlags.Instance);
        return (List<AtlasRegion>)regionsField.GetValue(atlas);
    }

    public static SkeletonDataAsset MergeMaterials(TextAsset nudeSkeletonJSON, List<SkeletonDataAsset> assetList, float scale)
    {

        int maxAtlasSize = 2048;
        int padding = 2;
        TextureFormat textureFormat = TextureFormat.RGBA32;
        bool mipmaps = false;

        List<AtlasPage> atlasPages = new List<AtlasPage>();
        List<AtlasRegion> originalRegions = new List<AtlasRegion>();
        List<Texture2D> texturesToPack = new List<Texture2D>();
        Dictionary<AtlasRegion, Vector2> repackedRegionsInfoDic = new Dictionary<AtlasRegion, Vector2>();
        Dictionary<int, List<AtlasRegion>> atlasRegionByTexture2d = new Dictionary<int, List<AtlasRegion>>();

        for (int i = 0; i < assetList.Count; i++)
        {
            var atlasAsset = assetList[i].atlasAssets[0] as SpineAtlasAsset;
            if (atlasAsset.MaterialCount != 1)
            {
                Logger.error("The atlasAsset MatrialCount not 1");
            }
            else
            {
                originalRegions.AddRange(GetRegions(atlasAsset.GetAtlas()));
            }
        }

        foreach (var item in originalRegions)
        {
            if (!atlasPages.Contains(item.page))
            {
                atlasPages.Add(item.page);
                Texture2D srcTexture = (item.page.rendererObject as Material).mainTexture as Texture2D;
                Texture2D tempTexture = srcTexture;// new Texture2D(srcTexture.width, srcTexture.height);
                //tempTexture.name = item.page.GetHashCode().ToString();
                //Graphics.CopyTexture(srcTexture, 0, 0, 0, 0, srcTexture.width, srcTexture.height, tempTexture, 0, 0, 0, 0);

                texturesToPack.Add(tempTexture);

                atlasRegionByTexture2d[tempTexture.GetInstanceID()] = new List<AtlasRegion>();
                atlasRegionByTexture2d[tempTexture.GetInstanceID()].Add(item);
            }
            else
            {
                atlasRegionByTexture2d[((item.page.rendererObject as Material).mainTexture as Texture2D).GetInstanceID()].Add(item);
            }
        }

        Texture2D newTexture = new Texture2D(maxAtlasSize, maxAtlasSize, textureFormat, mipmaps);


        newTexture.mipMapBias = -0.5f;//AtlasUtilities.DefaultMipmapBias;
        newTexture.name = "MyTexture";
        Rect[] rects = newTexture.PackTextures(texturesToPack.ToArray(), padding, maxAtlasSize);



        for (int i = 0; i < rects.Length; i++)
        {
            var rect = rects[i];

            var list = atlasRegionByTexture2d[texturesToPack[i].GetInstanceID()];
            float x = newTexture.width * rect.x;
            float y = newTexture.height - (newTexture.height * rect.y + newTexture.height * rect.height);

            for (int k = 0; k < list.Count; k++)
            {
                repackedRegionsInfoDic[list[k]] = new Vector2(list[k].x + x, list[k].y + y);
            }
        }

        Shader shader = Shader.Find("Spine/Skeleton");
        Material newMaterial = new Material(shader);
        newMaterial.name = "TempMaterial";
        newMaterial.mainTexture = newTexture;

        ////序列化为atlas.txt
        StringWriter stringWriter = new StringWriter();
        stringWriter.WriteLine();
        stringWriter.WriteLine(newTexture.name);
        stringWriter.WriteLine("size: " + newTexture.width + "," + newTexture.height);
        stringWriter.WriteLine("format: " + Format.RGBA8888.ToString());
        stringWriter.WriteLine("filter: " + TextureFilter.Linear.ToString() + "," + TextureFilter.Linear.ToString());
        stringWriter.WriteLine("repeat: " + "none");
        foreach (var item in originalRegions)
        {
            stringWriter.WriteLine(item.name);
            stringWriter.WriteLine("  rotate: " + item.rotate.ToString().ToLower());
            var v2 = repackedRegionsInfoDic[item];
            stringWriter.WriteLine("  xy: " + v2.x + ", " + v2.y);
            stringWriter.WriteLine("  size: " + item.width + ", " + item.height);
            stringWriter.WriteLine("  orig: " + item.originalWidth + ", " + item.originalHeight);
            stringWriter.WriteLine("  offset: " + item.offsetX + ", " + item.offsetY);
            stringWriter.WriteLine("  index: " + item.index);
        }
        string textAssetContent = stringWriter.ToString();
        stringWriter.Close();

        //var bytes = newTexture.EncodeToPNG();
        //var file = File.Open(Application.dataPath + "/MyTexture.png", FileMode.Create);
        //var binary = new BinaryWriter(file);
        //binary.Write(bytes);
        //file.Close();

        //File.WriteAllText(Application.dataPath + "/" + "My.atlas.txt", textAssetContent);
        //AssetDatabase.CreateAsset(newMaterial, "Assets/MyMaterial.mat");

        SpineAtlasAsset spineAtlasAsset = SpineAtlasAsset.CreateRuntimeInstance(new TextAsset(textAssetContent), new Material[] { newMaterial }, true);
        SkeletonDataAsset result = SkeletonDataAsset.CreateRuntimeInstance(nudeSkeletonJSON, spineAtlasAsset, true, scale);

        //AssetDatabase.CreateAsset(result, "Assets/MySkeletonDataAsset.asset");
        //AssetDatabase.Refresh();

        return result;
    }
    #endregion
}
