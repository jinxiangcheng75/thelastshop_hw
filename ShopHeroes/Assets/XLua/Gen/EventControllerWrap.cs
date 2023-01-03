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
    public class EventControllerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(EventController);
			Utils.BeginObjectRegister(type, L, translator, 0, 6, 2, 2);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Cleanup", _m_Cleanup);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ContainsEvent", _m_ContainsEvent);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddListener", _m_AddListener);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RemoveListener", _m_RemoveListener);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "TriggerEvent", _m_TriggerEvent);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "TriggerEvent_Lua0", _m_TriggerEvent_Lua0);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "globalEventHandler", _g_get_globalEventHandler);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "TheRouter", _g_get_TheRouter);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "globalEventHandler", _s_set_globalEventHandler);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "TheRouter", _s_set_TheRouter);
            
			
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
					
					EventController gen_ret = new EventController();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to EventController constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Cleanup(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                EventController gen_to_be_invoked = (EventController)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.Cleanup(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ContainsEvent(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                EventController gen_to_be_invoked = (EventController)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _eventType = LuaAPI.lua_tostring(L, 2);
                    
                        bool gen_ret = gen_to_be_invoked.ContainsEvent( _eventType );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddListener(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                EventController gen_to_be_invoked = (EventController)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _eventType = LuaAPI.lua_tostring(L, 2);
                    System.Action _handler = translator.GetDelegate<System.Action>(L, 3);
                    
                    gen_to_be_invoked.AddListener( _eventType, _handler );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveListener(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                EventController gen_to_be_invoked = (EventController)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _eventType = LuaAPI.lua_tostring(L, 2);
                    System.Action _handler = translator.GetDelegate<System.Action>(L, 3);
                    
                    gen_to_be_invoked.RemoveListener( _eventType, _handler );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_TriggerEvent(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                EventController gen_to_be_invoked = (EventController)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _eventType = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.TriggerEvent( _eventType );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_TriggerEvent_Lua0(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                EventController gen_to_be_invoked = (EventController)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _eventType = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.TriggerEvent_Lua0( _eventType );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_globalEventHandler(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                EventController gen_to_be_invoked = (EventController)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.globalEventHandler);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_TheRouter(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                EventController gen_to_be_invoked = (EventController)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.TheRouter);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_globalEventHandler(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                EventController gen_to_be_invoked = (EventController)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.globalEventHandler = (GlobalEventHandler)translator.GetObject(L, 2, typeof(GlobalEventHandler));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_TheRouter(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                EventController gen_to_be_invoked = (EventController)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.TheRouter = (System.Collections.Generic.Dictionary<string, System.Delegate>)translator.GetObject(L, 2, typeof(System.Collections.Generic.Dictionary<string, System.Delegate>));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
