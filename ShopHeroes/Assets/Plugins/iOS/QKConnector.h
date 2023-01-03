
//

#import <Foundation/Foundation.h>
#import <JySDK/JySDKManager.h>

@interface QKConnector : NSObject<KAcountDelegate>
-(void) QKInit;
-(void) LoginQKSDK;
-(void) QKLogin;
-(void) ReLogin;
-(void) QKLoginAsGuest;
-(void) QKLogoutSdk:(NSInteger) type;
-(NSString *) QKUserID;
-(NSString *) QKUserToken;
-(void) QKUserName;
-(void) QKUpdateRole:(NSString *)roleId;
-(BOOL) IsRealName;
-(void)updateRole:(NSString *)info;
@end
