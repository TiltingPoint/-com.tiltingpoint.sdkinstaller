// <copyright file="UserLanding.cs" company="Tilting Point Media LLC">
// Copyright (c) Tilting Point Media LLC. All rights reserved.
// </copyright>

using TiltingPoint.Installer.Editor.Data;
using TiltingPoint.Installer.Editor.Tools;
using TiltingPoint.Installer.Editor.Tools.UI;
using UnityEditor;
using UnityEngine;

namespace TiltingPoint.Installer.Editor.Pages
{
    /// <summary>
    /// First page that should be showed to users.
    /// </summary>
    internal class UserLanding : AbstractPage, IInstallerPage
    {
        private const string InstallationGuideUrl =
            "https://sites.google.com/tiltingpoint.com/tpsdk";

        private const string InstallationGuideUrlText =
            "TiltingPoint SDK Installation guide";

        private const string DescriptionText =
            "Welcome to Tilting Point Package Installer.\n\n" +
            "We will help you to add Tilting Point packages into your project with few clicks.\n\n" +
            "To get access to Tilting Pont registry we will go through next steps:\n" +
            "  - Get access to Tilting Point packages registry;\n" +
            "  - Add access credentials to UPM config;\n" +
            "  - Add registries data;\n" +
            "  - Add any Tilting Point Package you want.\n" +
            "  - Add External Dependency Manager for Unity.\n\n" +
            "If you are hardcore user and want to do all by yourself, read ManualInstruction in Documentation or" +
            " visit our installations guide page:";

        private readonly string[] logoFilePaths =
        {
            "Assets/TiltingPoint/SdkInstaller/Editor/Editor Resources/Logo.png",
            "Assets/TiltingPointSDK/SdkInstaller/Editor/Editor Resources/Logo.png",
            "Packages/com.tiltingpoint.sdkinstaller/Editor/Editor Resources/Logo.png",
        };

        private readonly GUIContent logoContent;
        private readonly GUIStyle logoStyle;

        public bool IsCompleted => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLanding"/> class.
        /// </summary>
        public UserLanding()
        {
            logoContent = new GUIContent {image = AssetTool.LoadAsset(logoFilePaths, new Texture2D(100, 100))};
            logoStyle = new GUIStyle {imagePosition = ImagePosition.ImageAbove, alignment = TextAnchor.MiddleCenter};
        }

        public void BeforeOpenPage(InstallationConfig config)
        {
            // empty
        }

        public void LeavePage(InstallationConfig config)
        {
            // empty
        }

        public void DrawGui(InstallationConfig config)
        {
            ShowLogo(logoContent, logoStyle);
            ShowText(DescriptionText);
            UiLinks.ShowUrlLabel(InstallationGuideUrlText, InstallationGuideUrl);
        }

        private static void ShowLogo(GUIContent logo, GUIStyle style)
        {
            var width = EditorGUIUtility.currentViewWidth * 0.8f;
            var height = Mathf.Clamp(width * (logo.image.width / (float) logo.image.height), 5, 200);

            EditorGUILayout.BeginHorizontal(GUILayout.Height(height));
            GUILayout.FlexibleSpace();
            GUILayout.Box(logo, style, GUILayout.Width(width), GUILayout.Height(height));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
        }
    }
}
