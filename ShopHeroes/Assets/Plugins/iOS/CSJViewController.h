//
//  CSJViewController.h
//  Unity-iPhone
//
//  Created by 唐雪艳 on 2022/8/8.
//

# if __has_include(<ABUAdSDK/ABUAdSDK.h>)
#import <ABUAdSDK/ABUAdSDK.h>
#else
#import <Ads-Mediation-CN/ABUAdSDK.h>
#endif
#import <ABUAdSDK/ABUAdSDKManager.h>

@interface CSJViewController : UIViewController<ABURewardedVideoAdDelegate>
-(void)InitCSJSDK;
-(BOOL)IsRewardedVideoAvailable;
-(void)PlayRewardedVideo;
@end
