#include <sys/stat.h>
#include <dlfcn.h>

const char *paths[] = {
    "/Applications/ALS.app"
    "/Applications/Cydia.app",
    "/Applications/FakeCarrier.app",
    "/Applications/Filza.app",
    "/Applications/FlyJB.app",
    "/Applications/IntelliScreen.app",
    "/Applications/MTerminal.app",
    "/Applications/SBSetttings.app",
    "/Applications/Snoop-itConfig.app"
    "/Applications/WinterBoard.app",
    "/Applications/blackra1n.app",
    "/Library/LaunchDaemons/com.openssh.sshd.plist"
    "/Library/LaunchDaemons/com.saurik.Cydia.Startup.plist",
    "/Library/LaunchDaemons/com.tigisoftware.filza.helper.plist",
    "/Library/LaunchDaemons/com.rpetrich.rocketbootstrapd.plist",
    "/Library/LaunchDaemons/dhpdaemon.plist",
    "/Library/LaunchDaemons/re.frida.server.plist",
    "/Library/MobileSubstrate",
    "/Library/MobileSubstrate/DynamicLibraries/LiveClock.plist",
    "/Library/MobileSubstrate/DynamicLibraries/Veency.plist",
    "/Library/MobileSubstrate/MobileSubstrate.dylib",
    "/System/Library/LaunchDaemons/com.ikey.bbot.plist",
    "/System/Library/LaunchDaemons/com.saurik.Cydia.Startup.plist",
    "/User/Applications/",
    "/bin.sh",
    "/bin/bash",
    "/etc/apt",
    "/etc/ssh/sshd_config",
    "/private/etc/apt",
    "/private/etc/apt/preferences.d/checkra1n",
    "/private/etc/apt/preferences.d/cydia",
    "/private/etc/dpkg/origins/debian",
    "/private/etc/ssh/sshd_config",
    "/private/var/lib/apt",
    "/private/var/lib/cydia",
    "/private/var/mobileLibrary/SBSettingsThemes/",
    "/private/var/stash",
    "/private/var/tmp/cydia.log",
    "/usr/bin/cycript",
    "/usr/bin/ssh",
    "/usr/lib/libcycript.dylib",
    "/usr/libexec/cydia/",
    "/usr/libexec/sftp-server",
    "/usr/libexec/ssh-keysign",
    "/usr/local/bin/cycript",
    "/usr/sbin/frida-server",
    "/usr/sbin/sshd",
    "/var/lib/cydia",
    "/var/lib/dpkg/info"
};

//__attribute__((always_inline)) long f_stat(const char * s, struct stat * stat_info) {
//    long ret = 0;
//    __asm__ volatile(
//                     "mov x0, %[s_p]\n"
//                     "mov x1, %[stat_info_p]\n"
//                     "mov x16, #338\n"
//                     "svc #0x80\n"
//                     "mov %[ret_p], x0\n"
//        : [ret_p]"=r"(ret)
//        : [s_p]"r"(s), [stat_info_p]"r"(stat_info)
//    );
//    return ret == 0 ? ret : -1;
//}

//check path
BOOL Check01() {
   
    for (int i = 0;i < sizeof(paths) / sizeof(char *);i++) {
        struct stat stat_info;
        if (0 == stat(paths[i], &stat_info)) {
//        if (0 == f_stat(paths[i], &stat_info)) {
            return YES;
        }
    }

    return NO;
    
}

//check env
BOOL Check02(){
    char* env = getenv("DYLD_INSERT_LIBRARIES");
#if DEBUG
    NSLog(@"CheckDataEnv %s", env);
#endif
    if(env){
        return YES;
    }
    return NO;
}

//check dylib
BOOL Check03(){
    if(TARGET_IPHONE_SIMULATOR)return NO;
    int ret;
    Dl_info dylib_info;
    
    int (*func_stat)(const char*, struct stat*) = stat;
    
    if((ret = dladdr(&func_stat, &dylib_info))){
        NSString* fname = [NSString stringWithUTF8String:dylib_info.dli_fname];
        
        if(![fname isEqualToString:@"/usr/lib/system/libsystem_kernel.dylib"]){
            return YES;
        }
    }
    return NO;
}

//BOOL CheckDataDL()
//{
//    int i=0;
//    char *substrate = "/Library/MobileSubstrate/MobileSubstrate.dylib";
//    while(true){
//        // hook _dyld_get_image_name方法可以绕过
//        const char *name = _dyld_get_image_name(i++);
//        if(name==NULL){
//            break;
//        }
//        if (name != NULL) {
//                    if (strcmp(name,substrate)==0) {
//                            return YES;
//                    }
//        }
//    }
//    return NO;
//}

//check fork
BOOL Check04(){
    int pid = fork();
    if(!pid){
        return NO;
    }
    if(pid >=0)
        return YES;
    return NO;
}

//check private folder
//BOOL Check05(){
//    NSString *path = @"/private/avl.txt";
//    NSFileManager *fileManager = [NSFileManager defaultManager];
//    @try {
//        NSError* error;
//        NSString *test = @"AVL was here";
//        [test writeToFile:path atomically:NO encoding:NSStringEncodingConversionAllowLossy error:&error];
//        [fileManager removeItemAtPath:path error:nil];
//        if(error==nil)
//        {
//            return YES;
//        }
//
//        return NO;
//    } @catch (NSException *exception) {
//        return NO;
//    }
//}

//check abnormal class
BOOL Check06(){
    //     查看是否有注入异常的类,比如HBPreferences 是越狱常用的类，这里无法绕过，只要多找一些特征类就可以，注意，很多反越狱插件会混淆，所以可能要通过查关键方法来识别
    NSArray *checksClass = [[NSArray alloc] initWithObjects:@"HBPreferences",nil];
    for(NSString *className in checksClass)
    {
      if (NSClassFromString(className) != NULL) {
        return YES;
      }
    }
    return NO;
}

NSString* CheckAll(){
    //检查字符串
    //对应PlatformManager.inst.s4
    NSString* ss = [NSString stringWithFormat:@"jb:%d%d%d%d%d", Check01(), Check02(), Check03(), Check04(), Check06()];
    return ss;
}


/**
 reference:

 */
