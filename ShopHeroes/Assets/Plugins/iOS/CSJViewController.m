//
//  CSJViewController.m
//  UnityFramework
//
//  Created by 唐雪艳 on 2022/8/8.
//

# if __has_include(<ABUAdSDK/ABUAdSDK.h>)
#import <ABUAdSDK/ABUAdSDK.h>
#else
#import <Ads-Mediation-CN/ABUAdSDK.h>
#endif
#import <ABUAdSDK/ABUAdSDKManager.h>

#import "CSJViewController.h"

#define TargetObject "GameSdk"       /*自行设置需要通知的U3D对象*/

#define CallUnity(methodName,msg)     UnitySendMessage(TargetObject, methodName, msg) /*向U3D发送消息*/

//广告
#define Csj_AppId @"5322600"

@interface CSJViewController () <ABURewardedVideoAdDelegate>

@property (nonatomic, strong) ABURewardedVideoAd *rewardedVideoAd;

@end

@implementation CSJViewController
- (void)dealloc {
    self.rewardedVideoAd = nil;
}

- (void)loadRewardVideoAdWithAdUnitID:(NSString *)adUnitID {
    // 广告加载
    #warning Every time the data is requested, a new one ABURewardedVideoAd needs to be initialized. Duplicate request data by the same reward screen video ad is not allowed.
    // gdt和穿山甲激励服务端校验需要赋值ABURewardedVideoModel;//􏰭如果开启对应adn服务端校验，这些参数会通过回调接口传给接入方
    //ABURewardedVideoModel *model = [[ABURewardedVideoModel alloc] init];
    //model.userId = @"123";
    //model.rewardAmount = 50;
    // 配置个性化信息
    //NSDictionary *extraDic = @{@"key1": @"value1", @"key2": @"value2", @"key3": @"value3"};
    //NSData *jsonData = [NSJSONSerialization dataWithJSONObject:extraDic options:NSJSONWritingPrettyPrinted error:nil];
    //NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    // 去除空格
    //jsonString = [jsonString stringByReplacingOccurrencesOfString:@"\r" withString:@""];
    //jsonString = [jsonString stringByReplacingOccurrencesOfString:@"\n" withString:@""];
    //jsonString = [jsonString stringByTrimmingCharactersInSet:[NSCharacterSet whitespaceAndNewlineCharacterSet]]; //去除掉首尾的空白字符和换行字符使用
    //jsonString = [jsonString stringByReplacingOccurrencesOfString:@" " withString:@""];
    //model.extra = jsonString;
    //self.rewardedVideoAd = [[ABURewardedVideoAd alloc] initWithAdUnitID:adUnitID rewardedVideoModel:model];
    self.rewardedVideoAd = [[ABURewardedVideoAd alloc] initWithAdUnitID:adUnitID ];
    self.rewardedVideoAd.delegate = self;
    //self.rewardedVideoAd.mutedIfCan = YES;//静音

    // v2700开始原生广告支持自渲染和模板类型混出，如果开发者在平台配置了对应代码位的该属性则无需设置；否则开发者需要设置getExpressAdIfCan属性来告知SDK当前广告位下配置的是否为模板类型；平台配置优先于getExpressAdIfCan设置；在SDK V2900以上激励视频客户端将无需区分模板非模板
    //self.rewardedVideoAd.getExpressAdIfCan = YES;
    
    //NSLog(@"%s", __func__);
    //该逻辑用于判断配置是否拉取成功。如果拉取成功，可直接加载广告，否则需要调用setConfigSuccessCallback，传入block并在block中调用加载广告。SDK内部会在配置拉取成功后调用传入的block
    //__weak typeof(self) weakself = self;
    //当前配置拉取成功，直接loadAdData
    if ([ABUAdSDKManager configDidLoad]) {
        //NSLog(@"%s:---当前配置拉取成功，直接加载广告", __func__);
        [self.rewardedVideoAd loadAdData];
    } else {
        //当前配置未拉取成功，在成功之后会调用该callback
        //NSLog(@"%s:---配置拉取未成功，延后加载广告", __func__);
        [ABUAdSDKManager addConfigLoadSuccessObserver:self withAction:^(id  _Nonnull observer) {
            //NSLog(@"%s: ----setConfigSuccessCallback", __func__);
            //[weakself.rewardedVideoAd loadAdData];
            [self.rewardedVideoAd loadAdData];
        }];
    }
}

//CSJ广告init
-(void) InitCSJSDK
{
    NSDictionary *didDic = @{};// !!!建议在初始化时设置；如需更改后期调用updateExtraDeviceMap
        [ABUAdSDKManager setupSDKWithAppId:Csj_AppId config:^ABUUserConfig *(ABUUserConfig *c) {
            c.logEnable = NO;
            c.extraDeviceMap = didDic;
            return c;
        }];
    
    [self loadRewardVideoAdWithAdUnitID:@"102112628"];
}

//广告是否准备好
-(BOOL) IsRewardedVideoAvailable
{
    if (self.rewardedVideoAd) {
        return self.rewardedVideoAd.isReady;
    }
    
    return false;
}

//播放广告
-(void) PlayRewardedVideo
{
    if([self IsRewardedVideoAvailable])
    {
        //NSLog(@"调用播放奖励视频");
        [self.rewardedVideoAd showAdFromRootViewController:UnityGetGLViewController()];
    }
    else
    {
        //NSLog(@"视频没有准备好！！！！！");
        //重新再拉取一下广告
        [self loadRewardVideoAdWithAdUnitID:@"102112628"];
    }
}


#pragma mark - <---ABURewardedVideoAdDelegate--->
/**
 This method is called when video ad material loaded successfully.
 */
- (void)rewardedVideoAdDidLoad:(ABURewardedVideoAd *_Nonnull)rewardedVideoAd {
    
    //NSLog(@"%s", __func__);

}

/**
 This method is called when video ad materia failed to load.
 @param error : the reason of error
 */
- (void)rewardedVideoAd:(ABURewardedVideoAd *_Nonnull)rewardedVideoAd didFailWithError:(NSError *_Nullable)error {
    //NSLog(@"广告物料加载失败 rewardedVideoAd %s:%@", __func__, error);
}

/**
 This method is called when a ABURewardedVideoAd failed to render.
 @param error : the reason of error
 Only for expressAd,hasExpressAdGot = yes
 */
- (void)rewardedVideoAdViewRenderFail:(ABURewardedVideoAd *_Nonnull)rewardedVideoAd error:(NSError *_Nullable)error {
    //NSLog(@"%s", __func__);
}

/**
 This method is called when cached successfully.
 */
- (void)rewardedVideoAdDidDownLoadVideo:(ABURewardedVideoAd *_Nonnull)rewardedVideoAd {
    //NSLog(@"%s", __func__);
}

/**
 This method is called when video ad slot has been shown.
 */
- (void)rewardedVideoAdDidVisible:(ABURewardedVideoAd *_Nonnull)rewardedVideoAd {
    //NSLog(@"%s", __func__);
    //ABURitInfo *info = [rewardedVideoAd getShowEcpmInfo];
    //NSLog(@"ecpm:%@", info.ecpm);
    //NSLog(@"platform:%@", info.adnName);
    //NSLog(@"ritID:%@", info.slotID);
    //NSLog(@"requestID:%@", info.requestID ?: @"None");
    
    //NSLog(@"getAdLoadInfoList:%@", [rewardedVideoAd getAdLoadInfoList]);
    CallUnity("OnRewardedVideoOpen",[@"" UTF8String]);
}

/**
 This method is called when video ad show API is called failed.
 */
- (void)rewardedVideoAdDidShowFailed:(ABURewardedVideoAd *_Nonnull)rewardedVideoAd error:(NSError *_Nonnull)error {
    //NSLog(@"%s", __func__);
    //NSLog(@"Ad show fail:%@", error);
    
}

/**
 This method is called when video ad is closed.
 */
- (void)rewardedVideoAdDidClose:(ABURewardedVideoAd *_Nonnull)rewardedVideoAd {
    //NSLog(@"%s", __func__);
    CallUnity("RewardedVideoClose",[@"" UTF8String]);
    [self loadRewardVideoAdWithAdUnitID:@"102112628"];
}

/**
 This method is called when video ad is clicked.
 */
- (void)rewardedVideoAdDidClick:(ABURewardedVideoAd *_Nonnull)rewardedVideoAd {
    //NSLog(@"%s", __func__);
}

- (void)rewardedVideoAdDidSkip:(ABURewardedVideoAd *)rewardedVideoAd {
    //NSLog(@"%s", __func__);
}

/**
 This method is called when video ad play completed or an error occurred.
 @param error : the reason of error
 */
- (void)rewardedVideoAdDidPlayFinish:(ABURewardedVideoAd * _Nonnull)rewardedVideoAd didFailWithError:(NSError *_Nullable)error {
    //NSLog(@"%s", __func__);
}

- (void)rewardedVideoAdServerRewardDidSucceed:(ABURewardedVideoAd *)rewardedVideoAd rewardInfo:(ABUAdapterRewardAdInfo *)rewardInfo verify:(BOOL)verify {
    //NSLog(@"%s", __func__);
    CallUnity("RewardedVideoAdRewarded",[@"" UTF8String]);
}

- (BOOL)shouldAutorotate {
    return YES;
}

- (UIInterfaceOrientationMask)supportedInterfaceOrientations {
    return UIInterfaceOrientationMaskAll;
}

@end
