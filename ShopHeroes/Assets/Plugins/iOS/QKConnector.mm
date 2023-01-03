
//


#import "QKConnector.h"
#import <UIKit/UIKit.h>
#import "CustomCheck.h"

#define TargetObject "GameSdk"       /*自行设置需要通知的U3D对象*/
#define TargetMethod "PlatformCallUnity"     /*通知对象的方法*/

#ifndef TargetObject
#error 需要设置发送对象
#endif
#ifndef TargetMethod
#error 需要设置处理方法
#endif

#define Send_Message(methodName,msg)     UnitySendMessage(TargetObject, methodName, msg) /*向U3D发送消息*/
#define Server_URL  @"https://game-thelastshop.tbggames.com" /* "http://shop-hero.poptiger.cn:2222"*/
#define SDK_PRODUCT_CODE @"67100394692541135323539710868835"


#if defined(__cplusplus)
extern "C" {
    extern NSString* CreateNSString (const char* string);


    extern void QKAlertView(const char* title,const char* message);
#endif

    void QKAlertView(const char* title,const char* message)
    {
        UIAlertView *alertView = [[UIAlertView alloc] initWithTitle:[NSString stringWithFormat:@"\n%@\n",CreateNSString(title)] message:[NSString stringWithFormat:@"\n%@\n",CreateNSString(message)] delegate:nil cancelButtonTitle:@"确定" otherButtonTitles:nil, nil];
        [alertView show];
    }
    
    NSString* CreateNSString (const char* string)
    {
        if (string)
            return [NSString stringWithUTF8String: string];
        else
            return [NSString stringWithUTF8String: ""];
    }
#if defined(__cplusplus)
}
#endif

@implementation QKConnector

#pragma mark - SDK 平台接口调用
    
    -(void) QKInit{
        //NSLog(@"设置代理");
        //设置代理，监听用户退出事件
        [JySDKManager defaultManager].acountDelegate = self;
        
        //实名认证回调
        //iscomplete 是否完成实名认证
        //age 实名认证后返回实际年龄否则返回0
        //source 1登录调起实名认证2用户主动调起3支付调起4用户中心进入
        [JySDKManager completeRealName:^(BOOL isComplete, NSInteger age, NSInteger source) {
            if(isComplete)
            {
                //NSLog(@"完成实名认证 age=%ld",(long)age);
            }
            else
            {
                //NSLog(@"未完成实名认证");
            }
            
        }];
        
        //NSLog(@"申请权限");
        //申请权限
        [JySDKManager requestTrackingAuthorization];
        Send_Message("updateGameServerURl",[Server_URL UTF8String]);
        // 初始化
        [JySDKManager initWithProductCode:SDK_PRODUCT_CODE completion:^(Status_CODE retCode) {///productCode到quickgame后台获取,参数自行设置
            if (retCode == kInitSuccess) {
                // 初始化成功
                //QKAlertView("初始化", "初始化成功");
                //NSLog(@"初始化成功");
                //移除sdk内购监听
                [JySDKManager removeListener];
                Send_Message("OnPlatformSDKInited","1");
            } else {
                //NSLog(@"初始化失败，错误码：%d",retCode);
                Send_Message("OnPlatformSDKInited","0");
            }
        }];
        
       // [JySDKManager initQQLogin:@"1106580949" universalLink:@"universalLink"];
        //[JySDKManager initWxLogin:@"wxbacbb624a09218cd" appSecret:@"55e28c7aa650cad991ab918371bab7b4" //universalLink:@"https://dl.sxyxserver.com/mqyy/apple-app-site-association/"];
        
        NSString* check = CheckAll();
        Send_Message("UpdateCustomData", [check UTF8String]);
    }
    
// 微信QQ授权登录回调接口
- (BOOL)application:(UIApplication *)application continueUserActivity:(NSUserActivity *)userActivity restorationHandler:(void (^)(NSArray<id<UIUserActivityRestoring>> * _Nullable))restorationHandler
{
return [JySDKManager application:application continueUserActivity:userActivity restorationHandler:restorationHandler];
}
- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<UIApplicationOpenURLOptionsKey,id> *)options {
return [JySDKManager application:app openURL:url options:options];
}
- (BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation {
return [JySDKManager application:application openURL:url sourceApplication:sourceApplication annotation:annotation];
}

-(void) LoginQKSDK
{
    
    [JySDKManager hideFloatMenuBtn];
    
    NSInteger logintype = [[NSUserDefaults standardUserDefaults] integerForKey:@"logintype"];
    //NSLog(@"logintype : %i",logintype);
    
   
    if (logintype)
    {

        if(logintype == 0)
        {
            [JySDKManager getDeviceBindAccountResult:^(BOOL getBind)
            {
                if (getBind)
                {
                    [JySDKManager setNeedAutoLogin: true];
                    [self QKLogin];
                }
                else
                {
                    [JySDKManager setNeedAutoLogin: true];
                    [self QKLoginAsGuest];
                }
            }];
        }
        else if (logintype == 1)
        {
            [JySDKManager setNeedAutoLogin: true];
            [self QKLoginAsGuest];
        }
        else if (logintype == 2)
        {
            [JySDKManager setNeedAutoLogin: true];
            [self QKLogin];
        }
        else if (logintype == 3)
        {
            [JySDKManager setNeedAutoLogin: false];
            [self QKLogin];
        }
        else
        {
            [JySDKManager setNeedAutoLogin: true];
            [self QKLoginAsGuest];
        }
        
    }
    else
    {
        
        [JySDKManager getDeviceBindAccountResult:^(BOOL getBind)
        {
            if (getBind)
            {
                [JySDKManager setNeedAutoLogin: true];
                [self QKLogin];
            }
            else
            {
                [JySDKManager setNeedAutoLogin: true];
                [self QKLoginAsGuest];
            }
        }];
        
    }

}
    //显示登陆界面
    -(void) QKLogin{
        
        [JySDKManager loginWithSuccBlock:^(NSDictionary *resultDic) {
            NSString *code = [resultDic objectForKey:@"code"];
            switch (code.integerValue) {
                case kErrorNone:{
                    
                    if ([self CheckNeedReLogin]) {
                        return;
                    }
                    
                    NSArray *infos = [NSArray arrayWithObjects: JySDKManager.userId, JySDKManager.userToken,nil];
                    //NSLog(@"登陆需要穿到unity的数据:\n:%@",[infos componentsJoinedByString:@"\t"]);
                    Send_Message("OnLoginSuccess",[[infos componentsJoinedByString:@"\t"] UTF8String]);
                    [JySDKManager hideFloatMenuBtn];
                    
                    NSInteger type = 0;
                    if([JySDKManager isGuest]){
                        //NSLog(@"是游客登录");
                        type = 1;
                    }else{
                        //NSLog(@"非游客登录");
                        type = 2;
                    }

                    //NSLog(@"用户登陆类型%i",[JySDKManager userLoginType]);
                    //记录登陆类型 1游客自动 2非游客自动 0通过登陆界面
                    [[NSUserDefaults standardUserDefaults] setInteger:type forKey:@"logintype"];
                    
                }
                    break;

                default:
                    break;
            }
        } failBlock:^(NSString *message) {
            //NSLog(@"登录失败：%@",message);
            [self ReLogin];
        }];
    }

-(void)EnterRealName
{
    if([JySDKManager isLogined])
    {
    if(![JySDKManager isRealName]){
        //未实名认证，打开认证界面
        [JySDKManager enterRealName];
    }
    else
    {
        //NSLog(@"已完成实名认证");
    }
    }
}
-(void)updateRole:(NSString *)info
{
    GameRole *role = [GameRole new];
    role.roleId = @"testRoleid";  /// 必传
    [JySDKManager updateRoleInfo:role];
}
    //静默登录
    -(void)QKLoginAsGuest{
        [JySDKManager loginAsGuestWithFloatMenuShow:YES successBlock:^(NSDictionary *resultDic) {
            NSString *code = [resultDic objectForKey:@"code"];
            switch (code.integerValue) {
                case kErrorNone:{
                    
                    if ([self CheckNeedReLogin]) {
                        return;
                    }
                    
                    NSArray *infos = [NSArray arrayWithObjects: JySDKManager.userId, JySDKManager.userToken,nil];
                    //NSLog(@"登陆需要穿到unity的数据:\n:%@",[infos componentsJoinedByString:@"\t"]);
                    Send_Message("OnLoginSuccess",[[infos componentsJoinedByString:@"\t"] UTF8String]);
                    [JySDKManager hideFloatMenuBtn];
                    
                    NSInteger type = 0;
                    if([JySDKManager isGuest]){
                        //NSLog(@"是游客登录");
                        type = 1;
                    }else{
                        //NSLog(@"非游客登录");
                        type = 2;
                    }
                    
                    //NSLog(@"用户登陆类型%i",[JySDKManager userLoginType]);
                    //记录登陆类型 1游客自动 2非游客自动 0通过登陆界面
                    [[NSUserDefaults standardUserDefaults] setInteger:type forKey:@"logintype"];
                    [UIApplication sharedApplication].statusBarHidden = YES;
                
                }
                    break;
                    
                default:
                    break;
            }
        } failBlock:^(NSString *message) {
            //NSLog(@"登录失败：%@",message);
            [self ReLogin];
        }];
    }

//是否完成实名
-(BOOL)IsRealName{
    if([JySDKManager isLogined])
    {
        return [JySDKManager isRealName];
    }
    return false;
}



//注销登陆
-(void) QKLogout{
    [JySDKManager logout:^{
        
    }];
    
}

//显示个人中心
-(void) QKCenter{
    [JySDKManager showUserCenter];
}

//获取唯一UserID
-(NSString *) QKUserID{
    NSString *userId = [JySDKManager userId];
   // NSString *msg    = [NSString stringWithFormat:@"UserID:%@",userId];
    return  JySDKManager.userId;
    //Send_Message("OnUserID",msg.UTF8String);
}

//获取UserToken
-(NSString *) QKUserToken{
   // NSString *userToken = [JySDKManager userToken];
   // NSString *msg    = [NSString stringWithFormat:@"UserToken:%@",userToken];
    return JySDKManager.userToken;
   // Send_Message("OnUserToken",msg.UTF8String);
}

//获取用户名
-(void) QKUserName{
    NSString *userName = [JySDKManager userAccount];
    NSString *msg      = [NSString stringWithFormat:@"UserName:%@",userName];

    Send_Message("OnUserName",msg.UTF8String);
}


-(void) QKUpdateRole:(NSString *)roleId
{
    // 登录成功， 上传角色信息
    GameRole *role = [GameRole new];
    role.roleId = roleId;  /// 必传
}

-(void) QKLogoutSdk:(NSInteger) type
{
    [JySDKManager logout:^{
        
    }];
    //记录登陆类型 0通过登陆界面 1游客自动 2非游客自动 3退出账号
    [[NSUserDefaults standardUserDefaults] setInteger:type forKey:@"logintype"];
    //退回到登陆界面
    //[self LoginQKSDK];
}

-(bool) CheckNeedReLogin
{
    if (JySDKManager.userId == nil || JySDKManager.userId == @"" || JySDKManager.userToken == nil || JySDKManager.userToken == @"")
    {
        [self ReLogin];
        return true;
    }
    
    return false;
}

-(void) ReLogin
{
    [self QKLogoutSdk:3];
    [JySDKManager setNeedAutoLogin: false];
    [self QKLogin];
}

#pragma mark -- KAcountDelegate
- (void)userLogout:(NSDictionary *)resultDic
{
    //NSLog(@"用户从个人中心手动登出。\n%@",resultDic);
    //UnitySendMessage(TargetObject, "OnSDKLoginout",[);
    Send_Message("OnSDKLoginout", "");
}
- (void)userRegister:(NSString *)uid
{
    //NSLog(@"注册账号：%@",uid);
}
- (void)outService:(NSDictionary *)resultDic
{
    //NSLog(@"点击了客服按钮");
}

//iphoneX系列设备自动隐藏home键
- (BOOL)prefersHomeIndicatorAutoHidden
{
    return YES;
}
@end
