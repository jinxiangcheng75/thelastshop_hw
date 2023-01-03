using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//顾客闲逛后去柜台结算
public class ShopperRambleState : StateBase
{
    Shopper _shopper;
    int queueErrorTimer;

    public ShopperRambleState(Shopper shopper)
    {
        _shopper = shopper;
    }

    public override int getState()
    {
        return (int)MachineShopperState.ramble;
    }

    //观赏的权重
    readonly int[] look_weights = AITalkProbConfigManager.inst.GetWeights(StaticConstants.noneWeightBase, StaticConstants.haveWeightBase); //{ 10, 20, 20 };


    //有货的权重 
    readonly int[] have_weights = AITalkProbConfigManager.inst.GetWeights(StaticConstants.haveWeightBase, StaticConstants.notHaveWeightBase); //{ 10, 5, 15 };
    //没货的权重 
    readonly int[] notHave_weights = AITalkProbConfigManager.inst.GetWeights(StaticConstants.notHaveWeightBase, StaticConstants.petWeightBase); //{ 10 };


    //宠物的权重
    readonly int[] pet_weighits = AITalkProbConfigManager.inst.GetWeights(StaticConstants.petWeightBase, StaticConstants.shopkeeperWeightBase);//{ 10, 10 };

    //没有目标货架的状态
    readonly List<ShopperRambleType> notHaveTargetShelfStates = new List<ShopperRambleType> { ShopperRambleType.look_1, ShopperRambleType.look_2, ShopperRambleType.notHave_ramble_1 };

    void getRambleType()
    {
        if (_shopper.shopperData.data.shopperAIType != (int)EShopperAIType.None)
        {
            _shopper.rambleType = (ShopperRambleType)(_shopper.shopperData.data.shopperAIType);

            //#if UNITY_EDITOR
            //            Logger.error("顾客进入了 逛商店 的状态 " + " 后端给到的行为是" + (int)_shopper.rambleType);
            //#endif

        }
        else
        {
            var shelfEquips = ItemBagProxy.inst.GetOnShelfEquipItems();
            var equip = shelfEquips.Find(t => t.equipid == _shopper.shopperData.data.targetEquipId); //shelfEquips.Find(t => t.itemUid == _shopper.shopperData.data.targetEquipUid); //Uid改为id

            bool shopHaveThisEquip = equip != null; //商店是否有这件装备（id）

            bool haveDecor = UserDataProxy.inst.shopData.decorList.FindAll(t => t.state != (int)EDesignState.InStore).Count > 0; //有无装饰

            bool havePet = PetDataProxy.inst.GetNotStorePetDatas().Count > 0 && !_shopper.shopperData.isCacheRamble;//有无宠物 (看宠物有刷能量的隐患，故若为本地缓存，不将观看宠物纳入行为随机池中)

            List<int> weightList = new List<int>();

            if (haveDecor) weightList.AddRange(look_weights);
            weightList.AddRange(shopHaveThisEquip ? have_weights : notHave_weights);
            if (havePet) weightList.AddRange(pet_weighits);

            int[] weights = weightList.ToArray();

            int index = Helper.getRandomValuefromweights(weights);

            int weightBase = StaticConstants.noneWeightBase;
            int count = 0;

            if (haveDecor)
            {
                if (index >= look_weights.Length)
                {
                    int length = shopHaveThisEquip ? look_weights.Length + have_weights.Length : look_weights.Length + notHave_weights.Length;

                    if (index < length)
                    {
                        weightBase = shopHaveThisEquip ? StaticConstants.haveWeightBase : StaticConstants.notHaveWeightBase;
                        count = look_weights.Length;
                    }
                    else
                    {
                        weightBase = StaticConstants.petWeightBase;
                        count = length;
                    }
                }
            }
            else
            {
                int length = shopHaveThisEquip ? have_weights.Length : notHave_weights.Length;

                if (index < length)
                {
                    weightBase = shopHaveThisEquip ? StaticConstants.haveWeightBase : StaticConstants.notHaveWeightBase;
                    count = 0;
                }
                else
                {
                    weightBase = StaticConstants.petWeightBase;
                    count = length;
                }
            }

            _shopper.rambleType = (ShopperRambleType)(weightBase + index - count + 1);

            //#if UNITY_EDITOR
            //            Logger.error("顾客uid: " + _shopper.shopperData.data.shopperUid +  "   进入了 逛商店 的状态 " + "商店有无自己要的装备 :" + shopHaveThisEquip + "     店内有无装饰 : " + haveDecor + "   店内有无宠物 :" + havePet + "   随机到的行为是" + (int)_shopper.rambleType);
            //#endif

        }
    }

    public override void onEnter(StateInfo info)
    {
        _shopper.rambleTalkeCount = 0;
        _shopper.rambleTargetShelfUid = -1;
        _shopper.rambleTargetEquipUid = "";
        _shopper.rambleTargetEquipId = 0;
        _shopper.rambleCanMovePathNodeNum = 0;

        getRambleType();
        setRambleParams();

        switch (_shopper.rambleType)
        {
            case ShopperRambleType.look_1: look_1(); break;
            case ShopperRambleType.look_2: look_2(); break;
            case ShopperRambleType.look_3: look_3(); break;


            case ShopperRambleType.have_ramble_1: have_ramble_1(); break;
            case ShopperRambleType.have_ramble_2: have_ramble_2(); break;
            case ShopperRambleType.have_ramble_3: have_ramble_3(); break;

            case ShopperRambleType.notHave_ramble_1: notHave_ramble_1(); break;


            case ShopperRambleType.pet_ramble_1: pet_ramble_1(); break;
            case ShopperRambleType.pet_ramble_2: pet_ramble_2(); break;

            default:
                break;
        }

    }

    public override void onUpdate()
    {

    }

    public override void onExit()
    {
        _shopper.rambleTalkeCount = 0;
        if (queueErrorTimer != 0)
        {
            GameTimer.inst.RemoveTimer(queueErrorTimer);
            queueErrorTimer = 0;
        }
    }


    //顾客随机选择[1~3]个装饰参观顺序以装饰物的，参观顺序随机。
    void look_1()
    {
        List<int> decors = UserDataProxy.inst.GetRanDecorUids(Random.Range(1, 4));
        List<Shopper_PathData> pathDatas = new List<Shopper_PathData>();

        for (int i = 0; i < decors.Count; i++)
        {
            pathDatas.Add(new Shopper_PathData(1, decors[i]));
        }

        _shopper.EstablishedLine(pathDatas, stepCallback: _shopper.MoveToPathNodeBehavior, endCallback: shopperRambleEndComplete);
    }


    //随机选择1个室内装饰，直接走到该装饰处完成参观后离开。
    void look_2()
    {
        int decorUid = UserDataProxy.inst.GetOneDecorUid();

        List<Shopper_PathData> pathDatas = new List<Shopper_PathData>();
        pathDatas.Add(new Shopper_PathData(1, decorUid));

        _shopper.EstablishedLine(pathDatas, stepCallback: _shopper.MoveToPathNodeBehavior, endCallback: shopperRambleEndComplete);


    }

    //货架有顾客购买的装备时，随机选择1~2室内装饰物，先参观装饰物，最后去往货架选择道具后购买产品。
    void look_3()
    {
        List<int> decors = UserDataProxy.inst.GetRanDecorUids(Random.Range(1, 2));
        List<Shopper_PathData> pathDatas = new List<Shopper_PathData>();

        for (int i = 0; i < decors.Count; i++)
        {
            pathDatas.Add(new Shopper_PathData(1, decors[i]));
        }

        int shelfUid = GetTargetShelfByEquipId(_shopper.shopperData.data.targetEquipId); //GetTargetShelfByEquipUid(_shopper.shopperData.data.targetEquipUid);
        if (shelfUid != -1)
        {
            pathDatas.Add(new Shopper_PathData(0, shelfUid));
            _shopper.rambleTargetShelfUid = shelfUid;
        }

        _shopper.EstablishedLine(pathDatas, stepCallback: _shopper.MoveToPathNodeBehavior, endCallback: shopperRambleEndComplete);
    }

    //直接走到自己需要购买的货架边弹出aitalk 弹出aitalk的概率为100，对应2类对话。
    void have_ramble_1()
    {
        List<Shopper_PathData> pathDatas = new List<Shopper_PathData>();
        int shelfUid = GetTargetShelfByEquipId(_shopper.shopperData.data.targetEquipId); //GetTargetShelfByEquipUid(_shopper.shopperData.data.targetEquipUid);
        if (shelfUid != -1)
        {
            pathDatas.Add(new Shopper_PathData(0, shelfUid));
            _shopper.rambleTargetShelfUid = shelfUid;
        }

        _shopper.EstablishedLine(pathDatas, stepCallback: _shopper.MoveToPathNodeBehavior, endCallback: shopperRambleEndComplete);
    }

    //随机选择1个室内装饰，并走到自己的目标货架，选择完毕后到柜台。途中每到1处弹出一次aitalk 弹出aitalk的概率为100，对应11类对话。（主要为游戏内的引导内容）
    void have_ramble_2()
    {
        List<Shopper_PathData> pathDatas = new List<Shopper_PathData>();

        int decorUid = UserDataProxy.inst.GetOneDecorUid();
        if (decorUid != -1) pathDatas.Add(new Shopper_PathData(1, decorUid));

        int shelfUid = GetTargetShelfByEquipId(_shopper.shopperData.data.targetEquipId); //GetTargetShelfByEquipUid(_shopper.shopperData.data.targetEquipUid);
        if (shelfUid != -1)
        {
            pathDatas.Add(new Shopper_PathData(0, shelfUid));
            _shopper.rambleTargetShelfUid = shelfUid;
        }

        _shopper.EstablishedLine(pathDatas, stepCallback: _shopper.MoveToPathNodeBehavior, endCallback: shopperRambleEndComplete);
    }

    //随机选择1~3个货架闲逛，当发现有空柜台时弹aitalk，只出现1次，最终走到自己的需要购买的道具柜台。
    void have_ramble_3()
    {

        int shelfUid = GetTargetShelfByEquipId(_shopper.shopperData.data.targetEquipId); //GetTargetShelfByEquipUid(_shopper.shopperData.data.targetEquipUid);

        List<int> furnitures = new List<int>();
        furnitures.AddRange(UserDataProxy.inst.GetRanShelfUids(Random.Range(1, 4), new List<int>() { shelfUid }));
        furnitures.Add(shelfUid);
        _shopper.rambleTargetShelfUid = shelfUid;

        List<Shopper_PathData> pathDatas = new List<Shopper_PathData>();
        for (int i = 0; i < furnitures.Count; i++)
        {
            pathDatas.Add(new Shopper_PathData(0, furnitures[i]));
        }

        _shopper.EstablishedLine(pathDatas, stepCallback: _shopper.MoveToPathNodeBehavior, endCallback: shopperRambleEndComplete);

    }

    //随机选择多个柜台（1~3个随机)闲逛，当逛到最后一个货架时（没有自己需要的货），吐槽并离开
    void notHave_ramble_1()
    {

        _shopper.shopperData.data.targetEquipUid = "";
        _shopper.shopperData.data.targetEquipId = 0;

        List<int> furnitures = new List<int>();
        furnitures.AddRange(UserDataProxy.inst.GetRanShelfUids(Random.Range(1, 4)));

        List<Shopper_PathData> pathDatas = new List<Shopper_PathData>();
        for (int i = 0; i < furnitures.Count; i++)
        {
            pathDatas.Add(new Shopper_PathData(0, furnitures[i]));
        }

        _shopper.EstablishedLine(pathDatas, stepCallback: _shopper.MoveToPathNodeBehavior, endCallback: shopperRambleEndComplete);
    }

    //直接走向宠物并观赏，观赏时弹出aitalk，而后购买自己需要的装备。只在观赏宠物时弹aitalk
    void pet_ramble_1()
    {
        List<Shopper_PathData> pathDatas = new List<Shopper_PathData>();

        int petUid = UserDataProxy.inst.GetOnePetUid();
        if (petUid != -1) pathDatas.Add(new Shopper_PathData(2, petUid));

        int shelfUid = GetTargetShelfByEquipId(_shopper.shopperData.data.targetEquipId); //GetTargetShelfByEquipUid(_shopper.shopperData.data.targetEquipUid);
        if (shelfUid != -1)
        {
            pathDatas.Add(new Shopper_PathData(0, shelfUid));
            _shopper.rambleTargetShelfUid = shelfUid;
        }

        _shopper.EstablishedLine(pathDatas, stepCallback: _shopper.MoveToPathNodeBehavior, endCallback: shopperRambleEndComplete);

    }

    //条件：店铺内有1宠物+1装饰。顾客先观赏宠物+装饰各1次，然后去到自己购买的货架购买。宠物处会出现1次aitalk
    void pet_ramble_2()
    {
        List<Shopper_PathData> pathDatas = new List<Shopper_PathData>();

        int petUid = UserDataProxy.inst.GetOnePetUid();
        if (petUid != -1) pathDatas.Add(new Shopper_PathData(2, petUid));

        int decorUid = UserDataProxy.inst.GetOneDecorUid();
        if (decorUid != -1) pathDatas.Add(new Shopper_PathData(1, decorUid));

        int shelfUid = GetTargetShelfByEquipId(_shopper.shopperData.data.targetEquipId); //GetTargetShelfByEquipUid(_shopper.shopperData.data.targetEquipUid);
        if (shelfUid != -1)
        {
            pathDatas.Add(new Shopper_PathData(0, shelfUid));
            _shopper.rambleTargetShelfUid = shelfUid;
        }

        _shopper.EstablishedLine(pathDatas, stepCallback: _shopper.MoveToPathNodeBehavior, endCallback: shopperRambleEndComplete);
    }

    int GetTargetShelfByEquipUid(string targetEquipUid)
    {
        int result = -1;

        var list = UserDataProxy.inst.GetShelfUidsByEquipUid(targetEquipUid);

        for (int i = 0; i < list.Count; i++)
        {
            int shelfUid = list[i];

            Vector3Int freePos = IndoorMap.inst.GetFurnitureFrontPos(shelfUid);

            if (freePos != Vector3Int.zero) //找到可观赏位置返回
            {
                result = shelfUid;
                break;
            }

        }

        return result;
    }

    int GetTargetShelfByEquipId(int targetEquipId)
    {

        int result = -1;
        var list = UserDataProxy.inst.GetShelfUidsByEquipId(targetEquipId);

        for (int i = 0; i < list.Count; i++)
        {
            int shelfUid = list[i];

            Vector3Int freePos = IndoorMap.inst.GetFurnitureFrontPos(shelfUid);

            if (freePos != Vector3Int.zero) //找到可观赏位置返回
            {
                result = shelfUid;
                break;
            }

        }

        return result;
    }

    void shopperRambleEndComplete()
    {
        //if (_shopper.moveEndTalkType != TalkConditionType.none && Helper.randomResult(_shopper.moveEndTalkPoint))
        //{
        //    _shopper.Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetRandomTalk(_shopper.shopperData.data.shopperId, _shopper.shopperData.data.shopperLevel, (int)_shopper.moveEndTalkType)));
        //}

        string equipUid = _shopper.rambleTargetEquipUid;
        int equipId = _shopper.rambleTargetEquipId == 0 ? _shopper.shopperData.data.targetEquipId : _shopper.rambleTargetEquipId;

        if (_shopper.rambleTargetShelfUid == -1 && !notHaveTargetShelfStates.Contains(_shopper.rambleType)) //目标地点到不了
        {
            _shopper.rambleTalkeCountLimit = -1;
            _shopper.Talk(LanguageManager.inst.GetValueByKey(AITalkConfigManager.inst.GetRandomTalk(_shopper.shopperData.data.shopperId, _shopper.shopperData.data.shopperLevel, (int)TalkConditionType.shopper_cantMoveToTargetShelf)));

            if (_shopper.rambleCanMovePathNodeNum != 0)
            {
                if (Helper.randomResult((int)WorldParConfigManager.inst.GetConfig(1106).parameters))
                {
                    equipUid = _shopper.shopperData.data.targetEquipUid;
                    equipId = _shopper.shopperData.data.targetEquipId;
                }
            }
            //else { }//整条路到不了 直接走人

        }

        EventController.inst.TriggerEvent<int, string, int>(GameEventType.ShopperEvent.SHOPPER_BUY_CONFIRM, _shopper.shopperUid, /*equipUid*/string.Empty, equipId);

        //容错添加 2s内未收到后端消息让其离开。
        shopperQueueError();
    }

    void shopperQueueError()
    {
        queueErrorTimer = GameTimer.inst.AddTimer(2, 1, () =>
        {
            queueErrorTimer = 0;
            if (_shopper.GetCurState() != MachineShopperState.leave)
            {
                _shopper.SetState(MachineShopperState.leave);
            }
        });
    }

    void setRambleParams()
    {
        var cfg = AITalkProbConfigManager.inst.GetConfig((int)_shopper.rambleType);

        if (cfg != null)
        {
            set(cfg.furniture1_probability, cfg.furniture2_probability, cfg.pets_probability, cfg.end_probability, cfg.talk_type, cfg.aitalk_times);
        }
        else
        {
            Logger.error("没有该行为对应的说话概率,行为id : " + (int)_shopper.rambleType);
            set();
        }
    }

    void set(int pointByDecor = 100, int pointByShelf = 100, int pointByPet = 100, int endPoint = 0, int endTalkType = 0, int limit = -1)
    {
        _shopper.rambleTalkPointByDecor = pointByDecor;
        _shopper.rambleTalkPointByShelf = pointByShelf;
        _shopper.rambleTalkPointByPet = pointByPet;
        _shopper.moveEndTalkPoint = endPoint;
        _shopper.moveEndTalkType = (TalkConditionType)endTalkType;
        _shopper.rambleTalkeCountLimit = limit;
    }

}
