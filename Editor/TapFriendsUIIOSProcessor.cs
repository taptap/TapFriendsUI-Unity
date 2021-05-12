using System.IO;
using TapTap.Common.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace TapTap.FriendsUI.Editor
{
    public static class TapFriendsUIIOSProcessor
    {
        // 添加标签，unity导出工程后自动执行该函数
        [PostProcessBuild(105)]
        
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget != BuildTarget.iOS) return;
            // 获得工程路径
            var projPath = TapCommonCompile.GetProjPath(path);
            var proj = TapCommonCompile.ParseProjPath(projPath);
            var target = TapCommonCompile.GetUnityTarget(proj);
            
            var unityAppControllerPath = path + "/Classes/UnityAppController.mm";
            var unityAppController = new TapFileHelper(unityAppControllerPath);
            unityAppController.WriteBelow(@"#import <TapLoginSDK/TapLoginHelper.h>",@"#import <TapFriendSDK/TapFriendSDK.h>");
            unityAppController.WriteBelow(
                @"id sourceApplication = options[UIApplicationOpenURLOptionsSourceApplicationKey], annotation = options[UIApplicationOpenURLOptionsAnnotationKey];",
                @"if(url){[TapFriends handleOpenURL:url];}");
            Debug.Log("TapFriendsUI Change UnityAppController File!");

            if (TapCommonCompile.CheckTarget(target))
            {
                Debug.LogError("Unity-iPhone is NUll");
                return;
            }

            if (TapCommonCompile.HandlerIOSSetting(path,
                Application.dataPath,
                "TapFriendResource",
                "com.taptap.tds.friends.ui",
                "FriendsUI",
                new[] {"TapFriendResource.bundle"},
                target, projPath, proj))
            {
                Debug.Log("TapFriendsUI add Bundle Success!");
                return;
            }

            Debug.LogWarning("TapFriendsUI add Bundle Failed!");
        }

    }
}