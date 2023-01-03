//
//  IAPManager.m
//  UnityFramework
//
//  Created by 赵福山 on 2022/6/8.
//

#import <Foundation/Foundation.h>
#import "IAPManager.h"
#import <AppsFlyerLib/AppsFlyerLib.h>

#define TargetObject "GameSdk"       /*自行设置需要通知的U3D对象*/
#define TargetMethod "PlatformCallUnity"     /*通知对象的方法*/

#define CallUnity(methodName,msg)     UnitySendMessage(TargetObject, methodName, msg) /*向U3D发送消息*/

@implementation IAPManager

-(void) attachObserver{
    //NSLog(@"attachObserver");
    [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
    
    //if([self CanMakePayment])
    //{
        //NSLog(@"支持内购");
    //}
    //else
    //{
    //    NSLog(@"不支持内购");
    //}
}

-(BOOL)CanMakePayment{
    return [SKPaymentQueue canMakePayments];
}

-(void)requestProductData:(NSString *)ProductIdentifiers{
    NSArray *idArray = [ProductIdentifiers componentsSeparatedByString:@"\t"];
    NSSet *idSet = [NSSet setWithArray:idArray];
    [self sendRequest:idSet];
}

-(void)sendRequest:(NSSet *)idset{
    SKProductsRequest *request = [[SKProductsRequest alloc] initWithProductIdentifiers:idset];
    request.delegate = self;
    [request start];
}

///// 获取请求结果，把商品加入到自己的商品列表
/// @param request 请求
/// @param response 返回结果
-(void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(nonnull SKProductsResponse *)response{
    NSArray *products = response.products;
    if(products.count > 0)
    {
        if (self.productDict == nil) {
            self.productDict = [NSMutableDictionary dictionaryWithCapacity:products.count];
        }
    }
    for(SKProduct *product in products){
        [self.productDict setObject:product forKey:product.productIdentifier];
        CallUnity("ShowProductList", [[self productInfo:product] UTF8String]);
//        NSLog(@"Invalid product:%@ , des: %@ ,Price: %@, \n productinfo: %@",product.productIdentifier, product.description,[self getLocalePrice:product], [self productInfo:product]);
    }
    
    //for(NSString *invalidProductId in response.invalidProductIdentifiers)
    //{
    //    NSLog(@"Invalid product id:%@", invalidProductId);
    //}
    
}

-(NSString *)getLocalePrice:(SKProduct *)product{
    if(product){
        NSNumberFormatter * formatter = [[NSNumberFormatter alloc] init];
        [formatter setFormatterBehavior:NSNumberFormatterBehavior10_4];
        [formatter setNumberStyle:NSNumberFormatterCurrencyStyle];
        [formatter setLocale:product.priceLocale];
        return [formatter stringFromNumber:product.price];
    }
    return @"";
}

//购买商品
-(void)buyRequest:(NSString *)productIdentifier{
    
    //NSLog(@"购买商品  %@", productIdentifier);
    
    SKPayment *skpayment = [SKPayment paymentWithProductIdentifier:productIdentifier];
    
    [[SKPaymentQueue defaultQueue] addPayment:skpayment]; //加到请求队列
}

//商品数据
//结构 com.tbscjh.lastshop.1499   小额金条1     购买后可获得60金条    ¥6.00
-(NSString *) productInfo:(SKProduct *) product{
    //NSLog(@"productInfo");
    NSArray *infos = [NSArray arrayWithObjects:product.productIdentifier, product.localizedTitle,product.localizedDescription, [self getLocalePrice:product], nil];
    return [infos componentsJoinedByString:@"\t"];
}

-(NSString *) transactionInfo:(SKPaymentTransaction *) transaction{
    return [self encode:(uint8_t *)transaction.transactionReceipt.bytes  :transaction.transactionReceipt.length];
}


- (NSString *)encode:(const uint8_t *)input :(NSInteger)length {
    static char table[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
    NSMutableData *data = [NSMutableData dataWithLength:((length + 2) / 3) * 4];
    uint8_t *output = (uint8_t *)data.mutableBytes;
    for (NSInteger i = 0; i < length; i += 3) {
        NSInteger value = 0;
        for (NSInteger j = i; j < (i + 3); j++) {
            value <<= 8;
            if (j < length) {
                value |= (0xFF & input[j]);
            }
        }
        NSInteger index = (i / 3) * 4;
        output[index + 0] = table[(value >> 18) & 0x3F];
        output[index + 1] = table[(value >> 12) & 0x3F];
        output[index + 2] = (i + 1) < length ? table[(value >> 6) & 0x3F] : '=';
        output[index + 3] = (i + 2) < length ? table[(value >> 0) & 0x3F] : '=';
    }
    return [[NSString alloc] initWithData:data encoding:NSASCIIStringEncoding];
}

-(void) provideContent:(SKPaymentTransaction *)transaction{
    //NSLog(@"provideContent");
    CallUnity("OnPayFinish", [[self transactionInfo:transaction] UTF8String]);
}

-(void) paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray<SKPaymentTransaction *> *)transactions
{
    //NSLog(@"paymentQueue");
    for(SKPaymentTransaction *transaction in transactions){
        switch (transaction.transactionState) {
            case SKPaymentTransactionStatePurchased:
                [self completeTransaction:transaction];
                break;
            case SKPaymentTransactionStateFailed:
                [self failedTransaction:transaction];
                break;
            case SKPaymentTransactionStateRestored:
                [self restoreTransaction:transaction];
                break;
            default:
            {
                //NSLog(@"state:%ld",(long)transaction.transactionState);
            }
                break;
        }
    }
}

//根据商品ID查找商品
- (SKProduct *)getSKPaymentByProductID:(NSString *)productID
{
    SKProduct *product = self.productDict[productID];
    return product;
}

//完成
-(void) completeTransaction:(SKPaymentTransaction *)transaction{
    //NSLog(@"Completetransaction:%@",transaction.transactionIdentifier);
    [self provideContent:transaction];
    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
    //上报appflyer
    SKProduct *product = self.productDict[transaction.payment.productIdentifier];
    if(product!=nil)
    {
        NSString* currencyCode = [product.priceLocale objectForKey:NSLocaleCurrencyCode];
        NSDecimalNumber* price = product.price;
        [[AppsFlyerLib shared] logEvent:@"af_purchase" withValues:@{
            @"af_content_id":product.productIdentifier,
            @"af_content_type":product.localizedTitle,
            @"af_currency":currencyCode,
            @"af_quantity":@"1",
            @"af_revenue":[NSString stringWithFormat:@"%@",price]}];
    }
}



//失败
-(void) failedTransaction:(SKPaymentTransaction *)transaction
{
    //NSLog(@"failedTransaction:%@",transaction.transactionIdentifier);
    
    if(transaction.error.code != SKErrorPaymentCancelled)
    {
       //NSLog(@"!Cancelled");
       CallUnity("OnPayFinish", "error");
    }
    else
    {
        //NSLog(@"Cancelled");
        CallUnity("OnPayFinish", "cancel");
    }

    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
}

//
-(void) restoreTransaction:(SKPaymentTransaction *)transaction
{
    // 恢复购买  订阅使用。消耗商品不会使用
    CallUnity("OnPayFinish", "error");
    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
}

-(void) saveSceneOrientation:(const char *)v
{
    NSString *nstr = [NSString stringWithUTF8String:v];
    //NSLog(@"saveDefaultData:%@",nstr);
    if([nstr isEqual:@"L"])
    {
        [[NSUserDefaults standardUserDefaults] setInteger:1 forKey:@"pingmu"];
        NSLog(@"saveDefaultData:%@",nstr);
    }
    else
    {
        [[NSUserDefaults standardUserDefaults] setInteger:0 forKey:@"pingmu"];
        NSLog(@"saveDefaultData:%@",nstr);
    }

}
@end
