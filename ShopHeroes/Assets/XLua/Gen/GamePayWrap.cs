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
    public class GamePayWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(GamePay);
			Utils.BeginObjectRegister(type, L, translator, 0, 12, 0, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "init", _m_init);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LoadOrderData", _m_LoadOrderData);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SaveOrderData", _m_SaveOrderData);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "FindAllProductPriceList", _m_FindAllProductPriceList);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "addProductPriceList", _m_addProductPriceList);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetProductPrice", _m_GetProductPrice);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Pay", _m_Pay);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "startCheckOrderList", _m_startCheckOrderList);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddPurchaseOrderToCheckList", _m_AddPurchaseOrderToCheckList);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "startCheckGamePayOrde", _m_startCheckGamePayOrde);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "checkGameOrde", _m_checkGameOrde);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "checkPayBalance", _m_checkPayBalance);
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 1, 1);
			
			
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "orderData", _g_get_orderData);
            
			Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "orderData", _s_set_orderData);
            
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					GamePay gen_ret = new GamePay();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to GamePay constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_init(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GamePay gen_to_be_invoked = (GamePay)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.init(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadOrderData(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GamePay gen_to_be_invoked = (GamePay)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.LoadOrderData(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SaveOrderData(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GamePay gen_to_be_invoked = (GamePay)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.SaveOrderData(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_FindAllProductPriceList(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GamePay gen_to_be_invoked = (GamePay)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _pricesid = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.FindAllProductPriceList( _pricesid );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_addProductPriceList(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GamePay gen_to_be_invoked = (GamePay)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _product = LuaAPI.lua_tostring(L, 2);
                    string _price = LuaAPI.lua_tostring(L, 3);
                    
                    gen_to_be_invoked.addProductPriceList( _product, _price );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetProductPrice(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GamePay gen_to_be_invoked = (GamePay)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _Productid = LuaAPI.lua_tostring(L, 2);
                    
                        string gen_ret = gen_to_be_invoked.GetProductPrice( _Productid );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Pay(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GamePay gen_to_be_invoked = (GamePay)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _priceId = LuaAPI.xlua_tointeger(L, 2);
                    string _productId = LuaAPI.lua_tostring(L, 3);
                    int _payActivityType = LuaAPI.xlua_tointeger(L, 4);
                    int _payActivityId = LuaAPI.xlua_tointeger(L, 5);
                    System.Action<bool> _callback = translator.GetDelegate<System.Action<bool>>(L, 6);
                    
                        bool gen_ret = gen_to_be_invoked.Pay( _priceId, _productId, _payActivityType, _payActivityId, _callback );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_startCheckOrderList(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GamePay gen_to_be_invoked = (GamePay)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.startCheckOrderList(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddPurchaseOrderToCheckList(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GamePay gen_to_be_invoked = (GamePay)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string __token = LuaAPI.lua_tostring(L, 2);
                    string __json = LuaAPI.lua_tostring(L, 3);
                    string __sign = LuaAPI.lua_tostring(L, 4);
                    
                    gen_to_be_invoked.AddPurchaseOrderToCheckList( __token, __json, __sign );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_startCheckGamePayOrde(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GamePay gen_to_be_invoked = (GamePay)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.startCheckGamePayOrde(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_checkGameOrde(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GamePay gen_to_be_invoked = (GamePay)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.checkGameOrde(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_checkPayBalance(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GamePay gen_to_be_invoked = (GamePay)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.checkPayBalance(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_orderData(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, GamePay.orderData);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_orderData(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    GamePay.orderData = (GamePaySaveData)translator.GetObject(L, 1, typeof(GamePaySaveData));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
