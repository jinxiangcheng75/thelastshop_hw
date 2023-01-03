# 热更说明v001

`Ctrl+Shift+V` 预览本文档

## 预览分支

checkout `ft_hotfix_v001_20210723`

用vscode 打开 `Assets/GameAssets/lua` 目录，即可编辑

Lua高亮：下载插件`sumneko.lua`

## 基本思路

1. 所有更新资源全部走`Addressables`(包括`configs.zip`)
2. 所有流程控制代码 全部用`Lua`重写
3. C#逻辑代码比如 继承自 `BaseSystem`,`IDataProxy`,`ViewBse`的代码 由`Lua`统一创建 和 管理
4. 通过检查`lua/hotfix/`目录下文件决定什么文件需要hotfix
5. C#逻辑代码通过 hotfix 更新
6. 新增模块全部使用`Lua`,使C# 和 Lua 模块并存
7. 配置，协议，数据之类的采用增量更新(为了兼容已有C#代码)


## 资源配置

1. AddressableAssetSetting 中开启`Build Remote Catalog`
2. AddressableAssetSetting 中选择 `Custom certificate handler`(目前为`HttpsCertificateHandler`)
3. `Built In Data`中 去掉勾选 `Include Resources`,`Include Build Settings`
4. 新建Group `LocalAssets`(default),`LocalLua`,`UpdateAssets`,`UpdateLua`
5. 具体Group分组根据实际情况
6. `LocalAssets`,`LocalLua`设置 Build Path为`LocalBuildPath`,Load Path为`LocalLoadPath`
7. 设置Restriction 为`Cannot Change Post Release`
8. `UpdateAssets`,`UpdateLua`设置Build Path为 `RemoteBuildPath`,Load Path为`RemoteLoadPath`
9. AddressablesProfiles.RemoteLoadPath改为 `{AssetConfig.RuntimePath}/[BuildTarget]`


## 使用

* <font color=#FF0000>首次出包</font>
1. 使用Addressables
2. PlayerSettings 设置 defines `HOTFIX_ENABLE`
3. 给所有需要更新的资源标记`Addressable`
4. 所有需要打在包里的资源放入 `LocalAssets`
5. 所有需要打在包里的Lua放入 `LocalLua`
6. `AssetConfig.RuntimePath` 为默认CDN地址
7. `NetworkConfig.Host` 为默认服务器地址
8. xlua 生成代码
9. `Clean Previous Build`
10. `Build Player Content`
11. 测试无误后
12. copy `{ProjectDir}/ServerData/[target]` 至 cdn
13. 出包
14. 打 tag 和 branch

* <font color=#00FFFF>更新</font>
1. 给需要打补丁C#类，新建**同名**lua文件，放入 `Assets/GameAssets/lua/hotfix/*.lua.txt`,系统会自动识别
2. 新建的lua文件后，除了勾选`Addressable`外，需要执行菜单`XLua/Custom/Export Lua FileList`导出lua文件列表
3. ~~将修改后的资源，lua 分别拖入 `UpdateAssets`,`UpdataLua`~~
4. 新增菜单`Addressables/99. Move ContentUpdate To Update Group`会自动把更改的资源从`Local*`Group挪动到`Update*`Group去
5. 执行 `Update Previous Build`
6. 将 `{ProjectDir}/ServerData/[target]` 目录 copy 至 cdn
7. git 打 tag 和 branch
   

* <font color=#00FF00>再次出包</font>
1. **case1**:仅修复C# bug的
2. 提升VersionCode
3. 可能需要根据版本号修改lua内容(如果有，需要同步更新线上内容)
4. 出包
5. **case2**:大修改，不兼容线上热更内容，
6. 备份原有版本的资源
7. 需要修改`AssetConfig.RuntimePath`
8. AddressableAssetSetting 中提升 `Player Override Version`
9. ~~把资源拖回Group `LocalAssets` `LocalLuaAssets`~~
10. 新增菜单`Addressables/100. Move All Update Group To Local Group 移动所有Update Group内容到Local Group`会自动把`Update*`Group的内容移动回到`Local*`Group中去
11. xlua 重新生成代码
12. Addressables重新发布`Build Player Content`
13. 提升VersionCode
14. 出包
15. git 打 tag 和 branch

> case2: 需要维护多个客户端
> 可以在开启新功能时注明“需要下载新版本才可使用新功能”的提示
> 可以通过服务端统计版本的方式逐步强制淘汰使用率低的版本

* <font color=#00FF00>新增模块</font>
1. 新增模块一般包含(UI,System,DataProxy)，三部分，全部需要在Lua中编写
2. 新UI的prefab需要挂上 `LuaBehaviour`~~(通过序列化的lua文件名来调起运行lua文件)~~
3. 新System,DataProxy,有需要就注册到Lua的`ManagerBinder`中
4. 具体可参考示例 `TestNewView.lua.txt` `TestNewSystem.lua.txt` `TestNewDataProxy.lua.txt`
5. 新增配置表参考 `TestNewConfig.lua.txt`


## 代码说明

### 注意
1. **C# 中特别长的方法请自行分割，否则hotfix的时候痛苦的是你**

### 启动
1. [CS] `GameStartHotfix` 初始化
2. [CS] 启动后`VersionManager`会自动拉取最新的`Catalog`
3. [CS] 显示`SplashUI`(仅图片，防止黑屏，独立于UI系统 也由`Addressables`更新)
4. [CS] 获取更新文件`CheckContentUpdates`
5. [CS] 判断有无`Lua`文件，如有，只下载lua文件(较小，1k~99k)
6. [CS] 把所有`Lua`文件`LoadAssetAsync`载入缓存(因为Addressable为异步加载，不方便xlua调用addLoader)
7. [CS] 启动唯一`LuaEnv`
8. [Lua] `GameStart` 初始化 `ManagerBinder.init`
9. [Lua] 切换至 `GameStatePreload`
10. [Lua] 显示`LoadingUI`
11. [Lua] `AddressableManager` 获取更新文件列表
12. [Lua] `AddressableManager` 下载 `channel_version.json` 判断有无停服更新以及服务器列表
13. [Lua] `AddressableManager` 下载资源文件，除了`Lua`
14. [Lua] 下载完毕后 changeState

### 事件
1. C#中的方法正常使用

* 已有修改
1. 已有事件主要是参数修改，通过xlua提供的泛型方法新建事件方法，然后调用，参考`snippet 1`
2. C#端事件修改一般在 Hotfix中进行，发送，接受都需要Hotfix，如果仅仅传递 基础类型，建议使用 Lua的事件
3. Lua端事件，直接使用`EventDispatcher`,Lua事件名位于`event/GameEvent`中

* 新增事件
1. 直接使用`EventDispatcher`,在`event/GameEvent`中新建事件名称
2. C#接收端代码Hotfix 添加监听和响应


```Lua
    --调用C#事件
    local csEventController = CS.EventController
    csEventControllerInst.inst:TriggerEvent(csGameEventType.BagEvent.BAG_GET_DATA)

    --snippet 1
    -- 带参数的泛型事件调用
    local funcGeneric = xlua.get_generic_method(CS.EventController, "TriggerEvent")
    local func = funcGeneric(CS.System.String)
    func(csEventControllerInst.inst, csGameEventType.UnionEvent.UNION_REQUEST_DATA, "")
```

```Lua
    --监听事件
    EventDispatcher:AddEvent(GameEvent.UI.ShowUI_Loading, onShowUI)
    --移除事件
    EventDispatcher:RemoveEvent(GameEvent.UI.ShowUI_Loading, onShowUI)
    --分发事件
    EventDispatcher:dispatchEvent(GameEvent.UI.ShowUI_Loading, ...)
```

### 配置
* 说明
1. 配置解压后默认还是放在C#端缓存`CSVParser.ConfigCaches`
2. Lua端解析配置参考`TestNewConfig.lua.txt`
3. 新增配置直接在Lua中调用

* 已有修改
1. case1: C# 端通过补丁的方式，比如 `configa.csv` 的补丁 `configa_fix.csv`(只包含索引字段 id 和 新增字段)
2. 在lua中解析,使用，参考`TestNewDataProxy.lua.txt`或 在Hotfix中使用
3. case2: 在Lua直接解析新配置，并Hotfix所有调用
4. 建议使用`case1`只在需要用到新字段的地方hotfix
5. Lua端直接修改发布即可

* 新增配置
1. 直接在Lua中解析并使用

### 网络消息
* 说明
1. C#端默认消息收发还是走原始接口
2. Lua端发送的消息，由Lua端处理

* 已有修改
1. 需要把发送消息或处理消息代码改到 Lua端
2. 接收消息需要改到Lua中时，在`MsgType`中添加对应CMD
3. Lua端`MsgType`还未使用服务端导出代码，目前仅供参考
4. 已有协议采用增量修改(协议不修改原有字段，只新增)
5. 参考`MsgType.lua.txt`,Lua Decode时先创建C#对象，然后C#对象作为参数存于Lua 对象中
6. 这样能避免修改大量调用接口，只在需要使用新增字段的地方hotfix
7. Hotfix来处理消息收发，修改接收代码适配 修改后的消息
8. `MsgType.lua.txt` 协议参数，有需要修改的再添加，不需要一次性导出所有
9. C#端新增了`HotfixNetworkPackage`作为Lua消息传递的载体

* 新增协议
1. 直接在Lua中解析并使用

```Lua
    MsgType = {
        Request_User_Login_Cmd = BASE_CMD + 100,
        Response_User_Login_Cmd = BASE_CMD + 101,
    }
```

```Lua
    --先生成C#对象再解析 Lua中的新字段
    --MsgType.lua.txt
    function Response_User_Login:Decode(jsonStr)
        --先创建 C# 对象
        local csJsonData = CS.JsonMapper.ToObject(jsonStr)
        local csLoginResp = CS.Response_User_Login()
        csLoginResp:Decode(csJsonData)
        self.csResponse = csLoginResp
        --再解析 增量 数据
        local jsonData = JsonUtils:decode(jsonStr)
        self.new_field = jsonData.new_field
    end
```

```Lua
    --仅修改发送 需要在MsgType中添加对应类，可能会统一导出
    --AccountSystem.lua.txt
    require("network/NetworkManager")

    xlua.hotfix(CS.AccountSystem, "requestLogin", function (self, account)
        print("AccountSystem:requestLogin called account:", account)
        local req = Request_User_Login:new()
        req.account = account
        NetworkManager:handleSend(req)
    end)
```

```Lua
    --仅修改接收
    --AccountSystem.lua.txt
    require("network/NetworkManager")

    xlua.hotfix(CS.AccountSystem, "addLoginResponse", function (self)
    print("AccountSystem:addLoginResponse called")
    csThis = self
    NetworkEvent:SetCallback(MsgType.Response_User_Login_Cmd, nil, 
        function (resp)
            print("Response_User_Login_Cmd onSuccess")
            --resp.csResponse 为C#端生成的消息对象 Response_User_Login
            local csResp = resp.csResponse
            --resp.new_field 为 Lua端新增的对象
            print("Response_User_Login_Cmd onSuccess newField ", resp.new_field)
            csThis:Logined(csResp)
        end,
        function (code)
            print("Response_User_Login_Cmd onFailed code:", code)
            csEventController.inst:TriggerEvent(csGameEventType.SHOWUI_MSGBOX, "登录失败!");
        end)
    end)
```

### 数据

* 已有修改
1. 方法使用hotfix修改
2. 字段考虑使用增量，就是已有的保留，新增的通过Lua赋值

* 新增字段
1. 直接在Lua中解析并使用

```Lua
    local xxxData = {}
    
    xlua.hotfix(CS.XXXDataProxy, "setXXX", function(self)
        print("Lua initComplete:", self)
        xxxData.abc = "abc"
    end)
```


### UI

* 已有修改UI
1. 通过Hotfix修改
2. 实在无法修改的，请走新增UI的方式
3. 显示

* 新增UI
1. 参考 `TestNewView.lua`,以及`TestNewUI.prefab`
2. 显示UI通过参考下面代码
3. `TestNewView.lua`继承自`ViewBase.lua`

```Lua
    --全局变量
    local csEventController = CS.EventController
    local csGameEventType = CS.GameEventType
    --显示C# UI
    CS.EventController.inst:TriggerEvent(csGameEventType.SHOWUI_SHOPSCENE)
    --隐藏C# UI
    csEventController.inst:TriggerEvent(csGameEventType.HIDEUI_SHOPSCENE)
    

    --发送显示事件
    EventDispatcher:dispatchEvent(GameEvent.UI.ShowUI_TestNew)
    --显示Lua UI
    GUIManager.inst:OpenView(Constants.ViewName.TestNew, function (view)
        print("TestNewView open callback")
    end)

    --发送隐藏事件
    EventDispatcher:dispatchEvent(GameEvent.UI.HideUI_TestNew)
    --隐藏Lua UI
    GUIManager.inst:HideView(Constants.ViewName.TestNew)
```

### System

* 已有修改System
1. 通过Hotfix修改

* 新增System
1. 参考`TestNewSystem.lua.txt`
2. 有需要就在`ManagerBinder.lua.txt`中添加初始化


### Hotfix 热更

1. 把需要热更的代码放入 `Assets/GameAssets/lua/hotfix/` 目录下，系统会自动识别
2. 需要Hotifx的C#除了添加`[Hotfix]`attribute 外，还可以在 `HotfixTags.by_property` 中添加
3. C# `HotfixBridge.cs`对应 `HotfixBridge.lua.txt`为C#向Lua发送消息统一接口
4. `XLuaEditor`中的`xluaTag`可以添加需要`LuaCallCSharp`和`CSharpCallLua`的类型
5. `XLuaEditor`中的`HotfixTags.by_property`可以添加需要Hotfix的类型


### 第三方库

1. `class`
2. `coroutine_cs`
3. `collection/Queue`
4. `collection/Stack`
5. `collection/LinkedList`
6. `utils/JsonUtils`
7. `utils/XmlUtils`


### 平台相关(Android,iOS)

1. C# `PlatformManager`
2. Lua `manager/NativeManager`
3. Lua访问调用 `CS.PlatformManager.call(string,string)`
4. Lua接收消息为 `hotfix/PlatformManager.lua.txt`


### 局限性

1. C#没有的代码 无法调用
2. xlua不能做的 也不能做
3. Shader目前还不能热更


# 最后
    
    请你根据自己的实际需求去匹配上述的热更方式，如有无法满足的情况，请即时反馈
