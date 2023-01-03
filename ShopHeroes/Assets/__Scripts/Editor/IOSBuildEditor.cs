
#if UNITY_IOS

using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class BuildCallback : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    int IOrderedCallback.callbackOrder { get { return 0; } }

    System.DateTime startTime;

    //打包前事件
    void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
    {
        startTime = System.DateTime.Now;
        Debug.Log("开始打包 : " + startTime);
    }

    //打包后事件
    void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
    {
        System.TimeSpan buildTimeSpan = System.DateTime.Now - startTime;
        Debug.Log("打包成功，耗时 : " + buildTimeSpan);
    }

    [PostProcessBuild(1)]
    static void OnPostprocessBuild(BuildTarget target, string pathToBuildProject)
    {
        ModifyProj(pathToBuildProject);
    }

    public static void ModifyProj(string pathToBuildProject)
    {
        string _projPath = PBXProject.GetPBXProjectPath(pathToBuildProject);
        PBXProject _pbxProj = new PBXProject();

        _pbxProj.ReadFromString(File.ReadAllText(_projPath));
        string _targetGuid = _pbxProj.TargetGuidByName("UnityFramework");

        //*******************************添加framework*******************************//

        _pbxProj.AddFrameworkToProject(_targetGuid, "CoreMotion.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "AdSupport.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "Accelerate.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "AudioToolbox.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "CoreGraphics.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "CoreImage.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "CoreLocation.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "CoreMedia.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "CoreTelephony.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "CoreText.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "ImageIO.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "JavaScriptCore.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "MapKit.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "MediaPlayer.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "MobileCoreServices.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "QuartzCore.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "Security.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "StoreKit.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "SystemConfiguration.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "UIKit.framework", true);
        _pbxProj.AddFrameworkToProject(_targetGuid, "WebKit.framework", true);

        //*******************************添加tbd*******************************//

		//_pbxProj.AddFileToBuild(_targetGuid, _pbxProj.AddFile("usr/lib/libz.tbd", "libz.tbd", PBXSourceTree.Sdk));
		//_pbxProj.AddFileToBuild(_targetGuid, _pbxProj.AddFile("usr/lib/libbz2.1.0.tbd", "libbz2.1.0.tbd", PBXSourceTree.Sdk));
		//_pbxProj.AddFileToBuild(_targetGuid, _pbxProj.AddFile("usr/lib/libbz2.tbd", "libbz2.tbd", PBXSourceTree.Sdk));
		//_pbxProj.AddFileToBuild(_targetGuid, _pbxProj.AddFile("usr/lib/libc++abi.tbd", "libc++abi.tbd", PBXSourceTree.Sdk));
		//_pbxProj.AddFileToBuild(_targetGuid, _pbxProj.AddFile("usr/lib/libresolv.tbd", "libresolv.tbd", PBXSourceTree.Sdk));
		//_pbxProj.AddFileToBuild(_targetGuid, _pbxProj.AddFile("usr/lib/libc++.tbd", "libc++.tbd", PBXSourceTree.Sdk));
		//_pbxProj.AddFileToBuild(_targetGuid, _pbxProj.AddFile("usr/lib/libresolv.9.tbd", "libresolv.9.tbd", PBXSourceTree.Sdk));
		//_pbxProj.AddFileToBuild(_targetGuid, _pbxProj.AddFile("usr/lib/libsqlite3.tbd", "libsqlite3.tbd", PBXSourceTree.Sdk));
		//_pbxProj.AddFileToBuild(_targetGuid, _pbxProj.AddFile("usr/lib/libxml2.tbd", "libxml2.tbd", PBXSourceTree.Sdk));

        //*******************************设置buildsetting*******************************//
        //_pbxProj.SetBuildProperty(_targetGuid, "ENABLE_BITCODE", "NO");//bitcode  NO


        //*******************************添加苹果内置功能（内购，登录等）*******************************//
        //_targetGuid = _pbxProj.TargetGuidByName("Unity-iPhone");
        //_pbxProj.AddCapability (_targetGuid, PBXCapabilityType.InAppPurchase);//内购



        File.WriteAllText(_projPath, _pbxProj.WriteToString());
        Debug.Log("Xcode Set End...");
    }

}


#endif
