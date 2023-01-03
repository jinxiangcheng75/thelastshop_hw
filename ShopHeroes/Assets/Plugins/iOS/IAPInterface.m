//
//  IApinterface.m
//  UnityFramework
//
//  Created by 赵福山 on 2022/6/9.
//

#import <Foundation/Foundation.h>
#import "IAPInterface.h"
#import "AppDelegate.h"
#import <WebKit/WebKit.h>
#include "IAPManager.h"
#include "QKConnector.h"
#include "CSJViewController.h"
#include "LocalPush.h"

#define TargetObject "GameSdk"       /*自行设置需要通知的U3D对象*/
#define TargetMethod "PlatformCallUnity"     /*通知对象的方法*/

#ifndef TargetObject
#error 需要设置发送对象
#endif
#ifndef TargetMethod
#error 需要设置处理方法
#endif

#define Send_Message(methodName,msg)     UnitySendMessage(TargetObject, methodName, msg) /*向U3D发送消息*/

@implementation IAPInterface

void TestMsg(){
    //NSLog(@"我的测试消息");
}

AppDelegate *appDelegate = nil;

IAPManager *iapManager = nil;

QKConnector *qKConnector = nil;

CSJViewController *cSJViewController = nil;

static const NSString *userAgent;

LocalPush *localPushController = nil;

//必须。返回平台类型
NSInteger PlatformType()
{
        return  4;
}

NSString* GetUUIDByKeyChain(){
    return @"";
}
//初始化
void InitIAPManager()
{
    getUserAgent();
    
    //未拿到UA回调之前 不往下继续执行
    while (userAgent == nil) {
        [[NSRunLoop currentRunLoop] runMode:NSDefaultRunLoopMode beforeDate:[NSDate distantFuture]];
    }
    
    
    [UIApplication sharedApplication].statusBarHidden = YES;
    //appsflyer
    appDelegate = [[AppDelegate alloc] init];
    [appDelegate startAppsflyerSDK];
    
    [appDelegate sendGameEvent:@"Before_InitSDK" :NULL];
    
    //充值
    iapManager = [[IAPManager alloc] init];
    [iapManager attachObserver];
    
    //[iapManager requestProductData:@"com.tbscjh.lastshop.099\tcom.tbscjh.lastshop.1499\tcom.tbscjh.lastshop.1999"];
    
    //初始化LocalPush
    localPushController = [[LocalPush alloc] init];
    [localPushController application];
    removeAppIconBadge();
    
    //初始化广告
    cSJViewController = [[CSJViewController alloc] init];
    [cSJViewController InitCSJSDK];
    //[ABUAdSDKManager setLoglevel:ABUAdSDKLogLevelDebug language:ABUAdSDKLogLanguageCH];
    //ironSourceViewController = [[IronSourceViewController alloc] init];
    //[ironSourceViewController InitIroSourceSDK];

    //初始化QKGameSDK  登陆
    qKConnector = [[QKConnector alloc] init];
    [qKConnector QKInit];

}

///打点
///unity游戏事件打点
void sendGameEvent(void *eventname, void *eventvalue)
{
    if(appDelegate)
    {
        
        NSString * _eventname = [NSString stringWithUTF8String:eventname];
        NSString * _value = [NSString stringWithUTF8String:eventvalue];
        
        //NSLog(@"游戏事件打点  %@", _eventname);
        
        [appDelegate sendGameEvent:_eventname :@{@"value":_value}];
    }
}
//
// 是否启用充值
bool IsProductAvailable()
{
    return [iapManager CanMakePayment];
}
// 获得商品信息。
void RequestProductInfo(void *p){
    NSString *list = [NSString stringWithUTF8String:p];
    //NSLog(@"productKey:%@", list);
    [iapManager requestProductData:list];
}

//登陆
void login(){
    //NSLog(@"调用登陆");
    [qKConnector LoginQKSDK];
}

//重新登陆
void reLogin()
{
    [qKConnector ReLogin];
}

//退出登录
void logout()
{
    //NSLog(@"调用登出");
    [qKConnector QKLogoutSdk:3];
    removeAllNotification();
}

bool isSDKLogined()
{
    return [JySDKManager isLogined];
}

//只设置游戏userid
void updateRoleInfo(void *p)
{
    if(p)
    {
        NSString *info = [NSString stringWithUTF8String:p];
        [qKConnector QKUpdateRole:info];
        [UIApplication sharedApplication].statusBarHidden = YES;
    }
    //ShowAppReview();
}
//判断用户是否实名认证
bool isRealName(){
    return [qKConnector IsRealName];
}
//
// 购买商品
void BuyProduct(void *p){
    [iapManager buyRequest:[NSString stringWithUTF8String:p]];
}

void SaveSceneQrientation(const char *p)
{
    [iapManager saveSceneOrientation:p];
}

//=========================================广告
//广告是否准备好
bool isRewardedVideoAvailable()
{
    //if(ironSourceViewController)
    //{
    //    return [ironSourceViewController IsRewardedVideoAvailable];
    //}
    if (cSJViewController) {
        return [cSJViewController IsRewardedVideoAvailable];
    }
    return false;
}
//播放广告
void playRewardedVideo()
{
    //if(ironSourceViewController)
    //{
    //    [ironSourceViewController PlayRewardedVideo];
    //}
    if (cSJViewController) {
        [cSJViewController PlayRewardedVideo];
    }
}

bool isGuest()
{
    return [JySDKManager isGuest];
}

void showUserCenter()
{
    [JySDKManager showUserCenter];
}

bool ShowAppReview()
{
    //NSLog(@"调用五星好评界面");
    [SKStoreReviewController requestReview];
    return true;
}

//skip to app-store update app   APPLE_APP_ID @"1626975093"
void toAppUpdateUrl()
{
    [[UIApplication sharedApplication] openURL:[NSURL URLWithString:@"https://apps.apple.com/cn/app/%E7%94%9F%E5%AD%98%E5%87%A0%E4%BD%95-%E6%98%8E%E6%97%A5%E5%95%86%E5%BA%97/id1626975093"] options:@{} completionHandler:nil];
}

bool isLogined()
{
    return [JySDKManager isLogined];
}

void hideFloatBtn()
{
    [JySDKManager hideFloatMenuBtn];
}

void showFloatBtn()
{
    [JySDKManager showFloatMenuBtnWithIsLeft:true andWithCenterY:150];
}

void getUserAgent()
{
    __block WKWebView *webView = [[WKWebView alloc] initWithFrame:CGRectZero];
      [webView evaluateJavaScript:@"navigator.userAgent" completionHandler:^(NSString *result, NSError * error) {
          webView = nil; /// 延迟webview释放，否则无法获取result
          if (!error && result && [result isKindOfClass:[NSString class]]) {
              userAgent = result;
              //NSLog(@"get the UA: %@",userAgent);
              Send_Message("SetUserAgent",[userAgent UTF8String]);
          }
          else
          {
              userAgent = @"";
              //NSLog(@"Can't get the userAgent !!!");
          }
      }];
}

//=========================================推送
void addLocalNotice(void* title, void* subTitle, void* body, void* badge, double secs, void* identifier)
{
    //NSLog(@"@@@@@@@@@@@@@@addLocalNotice");
    if (localPushController) {
        
        NSString * _title = [NSString stringWithUTF8String:title];
        NSString * _subTitle = [NSString stringWithUTF8String:subTitle];
        NSString * _body = [NSString stringWithUTF8String:body];
        NSNumber * _badge = [NSNumber numberWithInt:badge];
        NSString * _identifier = [NSString stringWithUTF8String:identifier];
        
        [localPushController addLocalNotice:_title:_subTitle:_body:_badge:secs:_identifier];

    }
}

bool checkHaveOneNotificationWithID(void* identifier)
{
    if (localPushController)
    {
        NSString * _identifier = [NSString stringWithUTF8String:identifier];
        return [localPushController checkHaveOneNotificationWithID:_identifier];
    }
    return false;
}

void removeOneNotificationWithID(void* noticeId)
{
    if (localPushController) {
        NSString * _noticeId = [NSString stringWithUTF8String:noticeId];
        [localPushController removeOneNotificationWithID:_noticeId];
    }
}

void removeAllNotification()
{
    if (localPushController) {
        [localPushController removeAllNotification];
    }
}

//清除角标
void removeAppIconBadge()
{
    [UIApplication sharedApplication].applicationIconBadgeNumber = -1;
}

bool checkUserNotificationEnable()
{
    
    if (localPushController) {
        return [localPushController checkUserNotificationEnable];
    }
    
    return false;
}


@end
