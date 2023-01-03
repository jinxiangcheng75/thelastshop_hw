//
//  LocalPush.h
//  UnityFramework
//
//  Created by 唐雪艳 on 2022/9/26.
//

#import <UserNotifications/UserNotifications.h>

@interface LocalPush : NSObject

-(BOOL)application;
-(void)addLocalNotice:(NSString *)title :(NSString *)subTitle :(NSString *)body :(NSNumber *)badge :(double)secs :(NSString *)identifier;
-(void)removeOneNotificationWithID:(NSString *)noticeId;
-(void)removeAllNotification;
-(BOOL)checkUserNotificationEnable;
-(BOOL)checkHaveOneNotificationWithID:(NSString *)noticeId;

@end
