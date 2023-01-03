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
    public class NetworkManagerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(NetworkManager);
			Utils.BeginObjectRegister(type, L, translator, 0, 8, 8, 6);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LuaCallTest", _m_LuaCallTest);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Send", _m_Send);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "sendLua", _m_sendLua);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "clear", _m_clear);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "test", _m_test);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "HeartbeatDataInit", _m_HeartbeatDataInit);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "startKeepAlive", _m_startKeepAlive);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "PauseKeepAlive", _m_PauseKeepAlive);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "isWifi", _g_get_isWifi);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "isNotReachable", _g_get_isNotReachable);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "mRequestHandlerFactory", _g_get_mRequestHandlerFactory);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "mEncodeHandler", _g_get_mEncodeHandler);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "mHotfixResponseDict", _g_get_mHotfixResponseDict);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "isOnline", _g_get_isOnline);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "_raceLampIndex", _g_get__raceLampIndex);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "KeepAlivePause", _g_get_KeepAlivePause);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "mRequestHandlerFactory", _s_set_mRequestHandlerFactory);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "mEncodeHandler", _s_set_mEncodeHandler);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "mHotfixResponseDict", _s_set_mHotfixResponseDict);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "isOnline", _s_set_isOnline);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "_raceLampIndex", _s_set__raceLampIndex);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "KeepAlivePause", _s_set_KeepAlivePause);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 2, 1, 1);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "GetNetWorkType", _m_GetNetWorkType_xlua_st_);
            
			
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "inst", _g_get_inst);
            
			Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "inst", _s_set_inst);
            
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					NetworkManager gen_ret = new NetworkManager();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to NetworkManager constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LuaCallTest(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.LuaCallTest(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Send(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _cmd = LuaAPI.xlua_tointeger(L, 2);
                    string _jsonData = LuaAPI.lua_tostring(L, 3);
                    
                    gen_to_be_invoked.Send( _cmd, _jsonData );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_sendLua(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    HotfixNetworkPackage _pkg = (HotfixNetworkPackage)translator.GetObject(L, 2, typeof(HotfixNetworkPackage));
                    
                    gen_to_be_invoked.sendLua( _pkg );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_clear(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.clear(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_test(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.test(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetNetWorkType_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    
                        UnityEngine.NetworkReachability gen_ret = NetworkManager.GetNetWorkType(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_HeartbeatDataInit(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.HeartbeatDataInit(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_startKeepAlive(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.startKeepAlive(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_PauseKeepAlive(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    bool _pause = LuaAPI.lua_toboolean(L, 2);
                    
                    gen_to_be_invoked.PauseKeepAlive( _pause );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_isWifi(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.isWifi);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_isNotReachable(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.isNotReachable);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_inst(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, NetworkManager.inst);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_mRequestHandlerFactory(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
                translator.PushAny(L, gen_to_be_invoked.mRequestHandlerFactory);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_mEncodeHandler(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
                translator.PushAny(L, gen_to_be_invoked.mEncodeHandler);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_mHotfixResponseDict(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.mHotfixResponseDict);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_isOnline(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.isOnline);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get__raceLampIndex(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked._raceLampIndex);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_KeepAlivePause(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.KeepAlivePause);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_inst(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    NetworkManager.inst = (NetworkManager)translator.GetObject(L, 1, typeof(NetworkManager));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_mRequestHandlerFactory(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.mRequestHandlerFactory = (IHandlerFactory<IPackageHandler, kRequestHandlerType>)translator.GetObject(L, 2, typeof(IHandlerFactory<IPackageHandler, kRequestHandlerType>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_mEncodeHandler(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.mEncodeHandler = (IMessageEncodeHandler)translator.GetObject(L, 2, typeof(IMessageEncodeHandler));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_mHotfixResponseDict(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.mHotfixResponseDict = (System.Collections.Generic.Dictionary<int, bool>)translator.GetObject(L, 2, typeof(System.Collections.Generic.Dictionary<int, bool>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_isOnline(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.isOnline = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set__raceLampIndex(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked._raceLampIndex = LuaAPI.xlua_tointeger(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_KeepAlivePause(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                NetworkManager gen_to_be_invoked = (NetworkManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.KeepAlivePause = LuaAPI.lua_toboolean(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
