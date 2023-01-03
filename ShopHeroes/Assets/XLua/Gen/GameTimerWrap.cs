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
    public class GameTimerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(GameTimer);
			Utils.BeginObjectRegister(type, L, translator, 0, 9, 1, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "init", _m_init);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddTimer", _m_AddTimer);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddTimerFrame", _m_AddTimerFrame);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetTimer", _m_GetTimer);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "RemoveTimer", _m_RemoveTimer);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "clearAll", _m_clearAll);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "setServerTime", _m_setServerTime);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddLoopTimerComp", _m_AddLoopTimerComp);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "removeLoopTimer", _m_removeLoopTimer);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "serverNow", _g_get_serverNow);
            
			
			
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
					
					GameTimer gen_ret = new GameTimer();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to GameTimer constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_init(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GameTimer gen_to_be_invoked = (GameTimer)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.init(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddTimer(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GameTimer gen_to_be_invoked = (GameTimer)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& translator.Assignable<System.Action>(L, 3)) 
                {
                    float _rate = (float)LuaAPI.lua_tonumber(L, 2);
                    System.Action _callBack = translator.GetDelegate<System.Action>(L, 3);
                    
                        int gen_ret = gen_to_be_invoked.AddTimer( _rate, _callBack );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 5&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& translator.Assignable<System.Action>(L, 4)&& translator.Assignable<GameTimerType>(L, 5)) 
                {
                    float _rate = (float)LuaAPI.lua_tonumber(L, 2);
                    int _ticks = LuaAPI.xlua_tointeger(L, 3);
                    System.Action _callBack = translator.GetDelegate<System.Action>(L, 4);
                    GameTimerType _tickType;translator.Get(L, 5, out _tickType);
                    
                        int gen_ret = gen_to_be_invoked.AddTimer( _rate, _ticks, _callBack, _tickType );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 4&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& translator.Assignable<System.Action>(L, 4)) 
                {
                    float _rate = (float)LuaAPI.lua_tonumber(L, 2);
                    int _ticks = LuaAPI.xlua_tointeger(L, 3);
                    System.Action _callBack = translator.GetDelegate<System.Action>(L, 4);
                    
                        int gen_ret = gen_to_be_invoked.AddTimer( _rate, _ticks, _callBack );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to GameTimer.AddTimer!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddTimerFrame(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GameTimer gen_to_be_invoked = (GameTimer)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _frame = LuaAPI.xlua_tointeger(L, 2);
                    int _ticks = LuaAPI.xlua_tointeger(L, 3);
                    System.Action _callBack = translator.GetDelegate<System.Action>(L, 4);
                    
                        int gen_ret = gen_to_be_invoked.AddTimerFrame( _frame, _ticks, _callBack );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetTimer(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GameTimer gen_to_be_invoked = (GameTimer)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _timerId = LuaAPI.xlua_tointeger(L, 2);
                    
                        GameTimer.Timer gen_ret = gen_to_be_invoked.GetTimer( _timerId );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_RemoveTimer(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GameTimer gen_to_be_invoked = (GameTimer)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    int _timerId = LuaAPI.xlua_tointeger(L, 2);
                    
                    gen_to_be_invoked.RemoveTimer( _timerId );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& translator.Assignable<GameTimer.Timer>(L, 2)) 
                {
                    GameTimer.Timer _timer = (GameTimer.Timer)translator.GetObject(L, 2, typeof(GameTimer.Timer));
                    
                    gen_to_be_invoked.RemoveTimer( _timer );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to GameTimer.RemoveTimer!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_clearAll(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GameTimer gen_to_be_invoked = (GameTimer)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.clearAll(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_setServerTime(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GameTimer gen_to_be_invoked = (GameTimer)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    double _sed = LuaAPI.lua_tonumber(L, 2);
                    
                    gen_to_be_invoked.setServerTime( _sed );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddLoopTimerComp(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GameTimer gen_to_be_invoked = (GameTimer)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 5&& translator.Assignable<UnityEngine.GameObject>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& translator.Assignable<System.Action>(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)) 
                {
                    UnityEngine.GameObject _target = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
                    float _distance = (float)LuaAPI.lua_tonumber(L, 3);
                    System.Action _callfun = translator.GetDelegate<System.Action>(L, 4);
                    int _loop = LuaAPI.xlua_tointeger(L, 5);
                    
                        LoopEventcomp gen_ret = gen_to_be_invoked.AddLoopTimerComp( _target, _distance, _callfun, _loop );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 4&& translator.Assignable<UnityEngine.GameObject>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& translator.Assignable<System.Action>(L, 4)) 
                {
                    UnityEngine.GameObject _target = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
                    float _distance = (float)LuaAPI.lua_tonumber(L, 3);
                    System.Action _callfun = translator.GetDelegate<System.Action>(L, 4);
                    
                        LoopEventcomp gen_ret = gen_to_be_invoked.AddLoopTimerComp( _target, _distance, _callfun );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to GameTimer.AddLoopTimerComp!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_removeLoopTimer(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                GameTimer gen_to_be_invoked = (GameTimer)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    LoopEventcomp _comp = (LoopEventcomp)translator.GetObject(L, 2, typeof(LoopEventcomp));
                    
                    gen_to_be_invoked.removeLoopTimer( _comp );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_serverNow(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                GameTimer gen_to_be_invoked = (GameTimer)translator.FastGetCSObj(L, 1);
                LuaAPI.lua_pushnumber(L, gen_to_be_invoked.serverNow);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
		
		
		
		
    }
}
