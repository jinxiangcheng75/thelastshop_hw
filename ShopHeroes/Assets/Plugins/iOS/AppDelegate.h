//
//  AppDelegate.h
//  UnityFramework
//
//  Created by 唐雪艳 on 2022/6/18.
//
#import <AppsFlyerLib/AppsFlyerLib.h>

@interface AppDelegate : UIResponder <UIApplicationDelegate>
-(void) startAppsflyerSDK;
-(void)sendGameEvent:(NSString *) _eventname :(NSDictionary * _Nullable)_values;

@end
