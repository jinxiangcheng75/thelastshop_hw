//
//  IAPManager.h.h
//  UnityFramework
//
//  Created by 赵福山 on 2022/6/8.
//
#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

@interface IAPManager : NSObject<SKProductsRequestDelegate, SKPaymentTransactionObserver>{
    SKProduct *proUpgradeProduct;
    SKProductsRequest *productsRequest;

}

/// 商品字典
@property(nonatomic,strong)NSMutableDictionary *productDict;

-(NSString *)getLocalePrice:(SKProduct *)product;

-(void)attachObserver;
-(BOOL)CanMakePayment;
-(void)requestProductData:(NSString *)ProductIdentifiers;
-(void)buyRequest:(NSString *)productIdentifier;
-(void)saveSceneOrientation:(const char *)v;
@end
