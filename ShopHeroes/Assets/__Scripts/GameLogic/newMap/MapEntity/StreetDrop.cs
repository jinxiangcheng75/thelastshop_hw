using UnityEngine;
using DG.Tweening;
using TMPro;

public class StreetDrop : MonoBehaviour
{
    public InputEventListener eventListener;
    public SpriteRenderer spriteRenderer;
    public TextMeshPro countTmpTx;
    public ParticleSystem vfxSystem;
    public GameObject haloVfxObj;

    [HideInInspector]
    public StreetDropData data;

    bool isClicked;

    private void Start()
    {
        eventListener.OnClick = onClick;
        spriteRenderer.sortingLayerName = "map_Actor";
    }

    int[] randomPower_icon = { 4, 4, 1, 1 };

    public void SetData(StreetDropData _data, float spScale = 0.38f)
    {

        data = _data;
        transform.localScale = Vector3.zero;


        string atlas = _data.accessoryData.atlas;
        string icon = _data.accessoryData.icon;


        //临时更改 
        if (_data.accessoryData.itemId == 10001)
        {
            int name = 1 + Helper.getRandomValuefromweights(randomPower_icon);

            atlas = "main_atlas";
            icon = "qian" + name;
        }

        AtlasAssetHandler.inst.GetAtlasSprite(atlas, icon, (gsprite) =>
        {
            SpriteEX sex = spriteRenderer.gameObject.GetComponent<SpriteEX>() ?? spriteRenderer.gameObject.AddComponent<SpriteEX>();
            sex.mGSprite = gsprite;
            //spriteRenderer.sprite = sprite;

            //Vector3 localScale = Vector3.zero;
            //localScale.x = spScale / sprite.bounds.size.x;
            //localScale.y = spScale / sprite.bounds.size.y;
            spriteRenderer.transform.localScale = Vector3.one * spScale;


            transform.DOScale(1, 0.3f).From(0).OnComplete(() =>
            {
                transform.position = MapUtils.CellPosToWorldPos(_data.dropPos);
                //randomAngleZ();

                //transform.DOScale(0.5f, 1f).From(1).SetLoops(-1, LoopType.Yoyo).SetDelay(0.2f);
                //Logger.error("出现了掉落物");
            });
        });

        vfxSystem.Pause();
    }

    void randomAngleZ()
    {
        float z = UnityEngine.Random.Range(-90f, 90f);
        Vector3 localEulerAngles = spriteRenderer.transform.localEulerAngles;
        localEulerAngles.z = z;
        spriteRenderer.transform.localEulerAngles = localEulerAngles;
    }

    private void onClick(Vector3 mousePos)
    {
        if (isClicked) return;
        isClicked = true;

        EventController.inst.TriggerEvent<int>(GameEventType.StreetDropEvent.STREETDROP_REQUEST_CLAIMED, data.uid);
    }

    public void Clear()
    {
        //起飞 ==> 销毁

        StreetDropDataProxy.inst.DelStreetDropPosCache(data.dropPos);
        DOTween.Kill(transform);

        haloVfxObj.SetActiveFalse();
        vfxSystem?.Play();

        countTmpTx.transform.DOLocalMoveY(countTmpTx.transform.localPosition.y + 0.4f, 1.2f).OnStart(() =>
        {
            spriteRenderer.DOFade(0, 1.2f);
            countTmpTx.DOFade(0, 1.2f);
            countTmpTx.text = "+" + data.accessoryData.count.ToString("N0");
        });

        GameTimer.inst.AddTimer(1.6f, 1, aniComplete);
    }

    void aniComplete()
    {
        if (this != null)
            Destroy(gameObject);
    }

}
