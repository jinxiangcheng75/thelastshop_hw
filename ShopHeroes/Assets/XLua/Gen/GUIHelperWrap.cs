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
    public class GUIHelperWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(GUIHelper);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 19, 1, 1);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "isPointerOnUI", _m_isPointerOnUI_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "IsPointerOverGameObject", _m_IsPointerOverGameObject_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "showQualiyIcon", _m_showQualiyIcon_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CheckHitItem", _m_CheckHitItem_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "setRandererSortinglayer", _m_setRandererSortinglayer_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "CalculateLengthOfText", _m_CalculateLengthOfText_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LoadAtlasAsync", _m_LoadAtlasAsync_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetMoneyStr", _m_GetMoneyStr_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetHexColorByColor", _m_GetHexColorByColor_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetColorByColorHex", _m_GetColorByColorHex_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetOutlineMat", _m_GetOutlineMat_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetSceneOutlineMat", _m_GetSceneOutlineMat_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetUIGray", _m_SetUIGray_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetSingleUIGray", _m_SetSingleUIGray_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetUIGrayColor", _m_SetUIGrayColor_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "SetMilestonesIconText", _m_SetMilestonesIconText_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "FindChild", _m_FindChild_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "GetFGuiCameraUIPointByWorldPos", _m_GetFGuiCameraUIPointByWorldPos_xlua_st_);
            
			
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "NOTNEEDCLEARMAT", _g_get_NOTNEEDCLEARMAT);
            
			Utils.RegisterFunc(L, Utils.CLS_SETTER_IDX, "NOTNEEDCLEARMAT", _s_set_NOTNEEDCLEARMAT);
            
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "GUIHelper does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_isPointerOnUI_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    
                        bool gen_ret = GUIHelper.isPointerOnUI(  );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_IsPointerOverGameObject_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector3 _mousepos;translator.Get(L, 1, out _mousepos);
                    
                        bool gen_ret = GUIHelper.IsPointerOverGameObject( _mousepos );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_showQualiyIcon_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& translator.Assignable<UnityEngine.RectTransform>(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    UnityEngine.RectTransform _qualiybgTF = (UnityEngine.RectTransform)translator.GetObject(L, 1, typeof(UnityEngine.RectTransform));
                    int _qualiy = LuaAPI.xlua_tointeger(L, 2);
                    float _dsize = (float)LuaAPI.lua_tonumber(L, 3);
                    
                    GUIHelper.showQualiyIcon( _qualiybgTF, _qualiy, _dsize );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& translator.Assignable<UnityEngine.RectTransform>(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    UnityEngine.RectTransform _qualiybgTF = (UnityEngine.RectTransform)translator.GetObject(L, 1, typeof(UnityEngine.RectTransform));
                    int _qualiy = LuaAPI.xlua_tointeger(L, 2);
                    
                    GUIHelper.showQualiyIcon( _qualiybgTF, _qualiy );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to GUIHelper.showQualiyIcon!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CheckHitItem_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector3 _mousePos;translator.Get(L, 1, out _mousePos);
                    int _layer = LuaAPI.xlua_tointeger(L, 2);
                    
                        bool gen_ret = GUIHelper.CheckHitItem( _mousePos, _layer );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_setRandererSortinglayer_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Transform _target = (UnityEngine.Transform)translator.GetObject(L, 1, typeof(UnityEngine.Transform));
                    string _LayerID = LuaAPI.lua_tostring(L, 2);
                    int _orderinLayer = LuaAPI.xlua_tointeger(L, 3);
                    
                    GUIHelper.setRandererSortinglayer( _target, _LayerID, _orderinLayer );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CalculateLengthOfText_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _message = LuaAPI.lua_tostring(L, 1);
                    UnityEngine.UI.Text _tex = (UnityEngine.UI.Text)translator.GetObject(L, 2, typeof(UnityEngine.UI.Text));
                    
                        int gen_ret = GUIHelper.CalculateLengthOfText( _message, _tex );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadAtlasAsync_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _atlasName = LuaAPI.lua_tostring(L, 1);
                    System.Action<string, UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle> _callback = translator.GetDelegate<System.Action<string, UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>>(L, 2);
                    
                    GUIHelper.LoadAtlasAsync( _atlasName, _callback );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetMoneyStr_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    int _money = LuaAPI.xlua_tointeger(L, 1);
                    
                        string gen_ret = GUIHelper.GetMoneyStr( _money );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetHexColorByColor_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Color _color;translator.Get(L, 1, out _color);
                    
                        string gen_ret = GUIHelper.GetHexColorByColor( _color );
                        LuaAPI.lua_pushstring(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetColorByColorHex_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    string _hexColor = LuaAPI.lua_tostring(L, 1);
                    
                        UnityEngine.Color gen_ret = GUIHelper.GetColorByColorHex( _hexColor );
                        translator.PushUnityEngineColor(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetOutlineMat_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    
                        UnityEngine.Material gen_ret = GUIHelper.GetOutlineMat(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetSceneOutlineMat_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    
                        UnityEngine.Material gen_ret = GUIHelper.GetSceneOutlineMat(  );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetUIGray_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Transform _uiroot = (UnityEngine.Transform)translator.GetObject(L, 1, typeof(UnityEngine.Transform));
                    bool _gray = LuaAPI.lua_toboolean(L, 2);
                    
                    GUIHelper.SetUIGray( _uiroot, _gray );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetSingleUIGray_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Transform _tf = (UnityEngine.Transform)translator.GetObject(L, 1, typeof(UnityEngine.Transform));
                    bool _gray = LuaAPI.lua_toboolean(L, 2);
                    
                    GUIHelper.SetSingleUIGray( _tf, _gray );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetUIGrayColor_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& translator.Assignable<UnityEngine.Transform>(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 4)) 
                {
                    UnityEngine.Transform _root = (UnityEngine.Transform)translator.GetObject(L, 1, typeof(UnityEngine.Transform));
                    float _grayProgress = (float)LuaAPI.lua_tonumber(L, 2);
                    float _alpha = (float)LuaAPI.lua_tonumber(L, 3);
                    bool _textNeedChg = LuaAPI.lua_toboolean(L, 4);
                    
                    GUIHelper.SetUIGrayColor( _root, _grayProgress, _alpha, _textNeedChg );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& translator.Assignable<UnityEngine.Transform>(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    UnityEngine.Transform _root = (UnityEngine.Transform)translator.GetObject(L, 1, typeof(UnityEngine.Transform));
                    float _grayProgress = (float)LuaAPI.lua_tonumber(L, 2);
                    float _alpha = (float)LuaAPI.lua_tonumber(L, 3);
                    
                    GUIHelper.SetUIGrayColor( _root, _grayProgress, _alpha );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 2&& translator.Assignable<UnityEngine.Transform>(L, 1)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)) 
                {
                    UnityEngine.Transform _root = (UnityEngine.Transform)translator.GetObject(L, 1, typeof(UnityEngine.Transform));
                    float _grayProgress = (float)LuaAPI.lua_tonumber(L, 2);
                    
                    GUIHelper.SetUIGrayColor( _root, _grayProgress );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to GUIHelper.SetUIGrayColor!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetMilestonesIconText_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    progressItemInfo _info = (progressItemInfo)translator.GetObject(L, 1, typeof(progressItemInfo));
                    GUIIcon _icon = (GUIIcon)translator.GetObject(L, 2, typeof(GUIIcon));
                    UnityEngine.UI.Text _valueTx = (UnityEngine.UI.Text)translator.GetObject(L, 3, typeof(UnityEngine.UI.Text));
                    
                    GUIHelper.SetMilestonesIconText( _info, ref _icon, ref _valueTx );
                    translator.Push(L, _icon);
                        
                    translator.Push(L, _valueTx);
                        
                    
                    
                    
                    return 2;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_FindChild_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Transform _parent = (UnityEngine.Transform)translator.GetObject(L, 1, typeof(UnityEngine.Transform));
                    string _name = LuaAPI.lua_tostring(L, 2);
                    
                        UnityEngine.Transform gen_ret = GUIHelper.FindChild( _parent, _name );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetFGuiCameraUIPointByWorldPos_xlua_st_(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
            
                
                {
                    UnityEngine.Vector3 _worldPos;translator.Get(L, 1, out _worldPos);
                    
                        UnityEngine.Vector2 gen_ret = GUIHelper.GetFGuiCameraUIPointByWorldPos( _worldPos );
                        translator.PushUnityEngineVector2(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_NOTNEEDCLEARMAT(RealStatePtr L)
        {
		    try {
            
			    LuaAPI.lua_pushstring(L, GUIHelper.NOTNEEDCLEARMAT);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_NOTNEEDCLEARMAT(RealStatePtr L)
        {
		    try {
                
			    GUIHelper.NOTNEEDCLEARMAT = LuaAPI.lua_tostring(L, 1);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
