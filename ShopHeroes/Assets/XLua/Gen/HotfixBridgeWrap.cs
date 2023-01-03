#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class HotfixBridgeWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(HotfixBridge);
			Utils.BeginObjectRegister(type, L, translator, 0, 41, 0, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "_init", _m__init);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Release", _m_Release);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "TriggerLuaEvent", _m_TriggerLuaEvent);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ChangeState", _m_ChangeState);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetWindow", _m_GetWindow);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetWindowByViewId", _m_GetWindowByViewId);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ShowView", _m_ShowView);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OpenView", _m_OpenView);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "HideView", _m_HideView);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "CloseView", _m_CloseView);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AllShowingView", _m_AllShowingView);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetCurrWindow", _m_GetCurrWindow);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "CallBackMainView", _m_CallBackMainView);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "CallBackAndChangeMainView", _m_CallBackAndChangeMainView);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "CallClearAllView", _m_CallClearAllView);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetCurrWindowViewID", _m_GetCurrWindowViewID);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetViewIsShowingByViewID", _m_GetViewIsShowingByViewID);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Showing", _m_Showing);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetDirectPurchaseDataById", _m_GetDirectPurchaseDataById);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "HasBuyLevelGrowth", _m_HasBuyLevelGrowth);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetVipRemainTime", _m_GetVipRemainTime);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetTriggerIsTrig", _m_GetTriggerIsTrig);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetTriggerData", _m_GetTriggerData);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetActivity_WorkerGameFlag", _m_GetActivity_WorkerGameFlag);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetActivity_WorkerGameEquipCanAddRateByDrawingId", _m_GetActivity_WorkerGameEquipCanAddRateByDrawingId);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetActivity_WorkerGameEquipMakeIntegralByDrawingId", _m_GetActivity_WorkerGameEquipMakeIntegralByDrawingId);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetActivity_WorkerGameCoinCount", _m_GetActivity_WorkerGameCoinCount);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetActivity_WorkerGameStr", _m_GetActivity_WorkerGameStr);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetActivity_GoldenCityFlag", _m_GetActivity_GoldenCityFlag);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetActivity_GoldenCityCanRewardCount", _m_GetActivity_GoldenCityCanRewardCount);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetActivity_GoldenCityCurScoreLv", _m_GetActivity_GoldenCityCurScoreLv);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "HaveTimeLimitActivitySelfScore", _m_HaveTimeLimitActivitySelfScore);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetLuxuryBuff", _m_GetLuxuryBuff);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetRefugeData", _m_GetRefugeData);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetShopkeeperCanMoveToCounter", _m_GetShopkeeperCanMoveToCounter);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetShopkeeperExist", _m_GetShopkeeperExist);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetShopkeeperIsMoving", _m_GetShopkeeperIsMoving);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetRuinsBattleData", _m_GetRuinsBattleData);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnMessageSuccess", _m_OnMessageSuccess);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnMessageFailed", _m_OnMessageFailed);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OncallLuaGlobalHeartbeatEvent", _m_OncallLuaGlobalHeartbeatEvent);
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					HotfixBridge gen_ret = new HotfixBridge();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to HotfixBridge constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m__init(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked._init(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Release(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.Release(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_TriggerLuaEvent(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _eventtype = LuaAPI.lua_tostring(L, 2);
                    object[] _param = translator.GetParams<object>(L, 3);
                    
                    gen_to_be_invoked.TriggerLuaEvent( _eventtype, _param );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ChangeState(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    IStateTransition _transition = (IStateTransition)translator.GetObject(L, 2, typeof(IStateTransition));
                    
                    gen_to_be_invoked.ChangeState( _transition );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetWindow(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    System.Type _csType = (System.Type)translator.GetObject(L, 2, typeof(System.Type));
                    bool _needNew = LuaAPI.lua_toboolean(L, 3);
                    
                        uiWindow gen_ret = gen_to_be_invoked.GetWindow( _csType, _needNew );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetWindowByViewId(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _viewid = LuaAPI.lua_tostring(L, 2);
                    
                        uiWindow gen_ret = gen_to_be_invoked.GetWindowByViewId( _viewid );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ShowView(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    object _obj = translator.GetObject(L, 2, typeof(object));
                    System.Action<object> _callback = translator.GetDelegate<System.Action<object>>(L, 3);
                    
                    gen_to_be_invoked.ShowView( _obj, _callback );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OpenView(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    System.Type _csType = (System.Type)translator.GetObject(L, 2, typeof(System.Type));
                    System.Action<object> _callback = translator.GetDelegate<System.Action<object>>(L, 3);
                    
                        uiWindow gen_ret = gen_to_be_invoked.OpenView( _csType, _callback );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HideView(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    System.Type _csType = (System.Type)translator.GetObject(L, 2, typeof(System.Type));
                    
                    gen_to_be_invoked.HideView( _csType );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CloseView(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    System.Type _viewid = (System.Type)translator.GetObject(L, 2, typeof(System.Type));
                    
                    gen_to_be_invoked.CloseView( _viewid );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AllShowingView(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        System.Collections.Generic.List<uiWindow> gen_ret = gen_to_be_invoked.AllShowingView(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetCurrWindow(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        uiWindow gen_ret = gen_to_be_invoked.GetCurrWindow(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CallBackMainView(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.CallBackMainView(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CallBackAndChangeMainView(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.CallBackAndChangeMainView(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CallClearAllView(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.CallClearAllView(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetCurrWindowViewID(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        string gen_ret = gen_to_be_invoked.GetCurrWindowViewID(  );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetViewIsShowingByViewID(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _viewID = LuaAPI.lua_tostring(L, 2);
                    
                        bool gen_ret = gen_to_be_invoked.GetViewIsShowingByViewID( _viewID );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Showing(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    System.Type _csType = (System.Type)translator.GetObject(L, 2, typeof(System.Type));
                    
                        bool gen_ret = gen_to_be_invoked.Showing( _csType );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetDirectPurchaseDataById(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _id = LuaAPI.xlua_tointeger(L, 2);
                    DirectPurchaseData _data;
                    
                        bool gen_ret = gen_to_be_invoked.GetDirectPurchaseDataById( _id, out _data );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    translator.Push(L, _data);
                        
                    
                    
                    
                    return 2;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HasBuyLevelGrowth(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        bool gen_ret = gen_to_be_invoked.HasBuyLevelGrowth(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetVipRemainTime(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        int gen_ret = gen_to_be_invoked.GetVipRemainTime(  );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetTriggerIsTrig(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _furnId = LuaAPI.xlua_tointeger(L, 2);
                    
                        bool gen_ret = gen_to_be_invoked.GetTriggerIsTrig( _furnId );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetTriggerData(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        TriggerData gen_ret = gen_to_be_invoked.GetTriggerData(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetActivity_WorkerGameFlag(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        bool gen_ret = gen_to_be_invoked.GetActivity_WorkerGameFlag(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetActivity_WorkerGameEquipCanAddRateByDrawingId(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _equipDrawingId = LuaAPI.xlua_tointeger(L, 2);
                    
                        bool gen_ret = gen_to_be_invoked.GetActivity_WorkerGameEquipCanAddRateByDrawingId( _equipDrawingId );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetActivity_WorkerGameEquipMakeIntegralByDrawingId(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _equipDrawingId = LuaAPI.xlua_tointeger(L, 2);
                    
                        int gen_ret = gen_to_be_invoked.GetActivity_WorkerGameEquipMakeIntegralByDrawingId( _equipDrawingId );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetActivity_WorkerGameCoinCount(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        long gen_ret = gen_to_be_invoked.GetActivity_WorkerGameCoinCount(  );
                        LuaAPI.lua_pushint64(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetActivity_WorkerGameStr(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    EOperatingActivityStringType _strType;translator.Get(L, 2, out _strType);
                    
                        string gen_ret = gen_to_be_invoked.GetActivity_WorkerGameStr( _strType );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetActivity_GoldenCityFlag(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        bool gen_ret = gen_to_be_invoked.GetActivity_GoldenCityFlag(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetActivity_GoldenCityCanRewardCount(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        int gen_ret = gen_to_be_invoked.GetActivity_GoldenCityCanRewardCount(  );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetActivity_GoldenCityCurScoreLv(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        int gen_ret = gen_to_be_invoked.GetActivity_GoldenCityCurScoreLv(  );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HaveTimeLimitActivitySelfScore(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        bool gen_ret = gen_to_be_invoked.HaveTimeLimitActivitySelfScore(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetLuxuryBuff(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _subType = LuaAPI.xlua_tointeger(L, 2);
                    
                        int gen_ret = gen_to_be_invoked.GetLuxuryBuff( _subType );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetRefugeData(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        DirectPurchaseData gen_ret = gen_to_be_invoked.GetRefugeData(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetShopkeeperCanMoveToCounter(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        bool gen_ret = gen_to_be_invoked.GetShopkeeperCanMoveToCounter(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetShopkeeperExist(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        bool gen_ret = gen_to_be_invoked.GetShopkeeperExist(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetShopkeeperIsMoving(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        bool gen_ret = gen_to_be_invoked.GetShopkeeperIsMoving(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetRuinsBattleData(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                        TriggerData gen_ret = gen_to_be_invoked.GetRuinsBattleData(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnMessageSuccess(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _msg = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.OnMessageSuccess( _msg );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnMessageFailed(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    object _obj = translator.GetObject(L, 2, typeof(object));
                    int _code = LuaAPI.xlua_tointeger(L, 3);
                    
                    gen_to_be_invoked.OnMessageFailed( _obj, _code );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OncallLuaGlobalHeartbeatEvent(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                HotfixBridge gen_to_be_invoked = (HotfixBridge)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _msg = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.OncallLuaGlobalHeartbeatEvent( _msg );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
