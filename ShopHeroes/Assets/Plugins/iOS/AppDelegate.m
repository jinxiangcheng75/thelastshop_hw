//
//  AppDelegate.m
//  UnityFramework
//
#import "AppDelegate.h"
#import <AppsFlyerLib/AppsFlyerLib.h>

#define DF_DEV_KEY @"42i6LEuJ34TLfkNWWyLN4B"
#define APPLE_APP_ID @"1626975093"


@interface AppDelegate()
@end

@implementation AppDelegate

-(void) startAppsflyerSDK
{
    [[AppsFlyerLib shared] setAppsFlyerDevKey:DF_DEV_KEY];
    [[AppsFlyerLib shared] setAppleAppID:APPLE_APP_ID];
    
    [[AppsFlyerLib shared] start];
}

-(void)sendGameEventWithEvnetName:(NSString *)_eventname :(NSDictionary<NSString *,id> * _Nullable)_eventValue
{
    [[AppsFlyerLib shared] logEventWithEventName:_eventname eventValues:_eventValue completionHandler:nil];
}

-(void)sendGameEvent:(NSString *) _eventname :(NSDictionary * _Nullable)_values
{
    //NSLog(@"发送事件到APPSFLAYER EventName = %@",_eventname);
    [[AppsFlyerLib shared] logEvent:_eventname withValues:_values];
}

@end
