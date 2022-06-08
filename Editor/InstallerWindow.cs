// Copyright (c) Tilting Point Media LLC. All rights reserved.

using System;
using System.Collections.Generic;
using TiltingPoint.Installer.Editor.Data;
using TiltingPoint.Installer.Editor.Pages;
using TiltingPoint.Installer.Editor.Tools;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace TiltingPoint.Installer.Editor
{
    /// <summary>
    /// Installer windows will help you to configure your project and import new packages.
    /// </summary>
    public class InstallerWindow : EditorWindow
    {
        private const string TitleOfWindow = "Tilting Point SDK Installer";

        private readonly string[] configurationFilePath =
        {
            "Assets/TiltingPoint/SdkInstaller/configuration.json",
            "Assets/TiltingPointSDK/SdkInstaller/configuration.json",
            "Packages/com.tiltingpoint.sdkinstaller/Editor/Editor Resources/configuration.json",
        };

        private readonly string[] logoSmallFilePaths =
        {
            "Assets/TiltingPoint/SdkInstaller/Editor/Editor Resources/LogoSmall.png",
            "Packages/com.tiltingpoint.sdkinstaller/Editor/Editor Resources/LogoSmall.png",
        };


        private readonly Color defaultButtonColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        private readonly Color successButtonColor = new Color(0.8f, 1f, 0.8f, 1f);

        private InstallationConfig installationConfig;

        private bool openOnPackagesPage;
        private int currentPage;
        private List<IInstallerPage> pages;

        private DateTime lastUpdate = DateTime.UtcNow;

        private Vector2 scrollPosition;

        [MenuItem("TiltingPoint/SDK Installer/Show SDK Installer")]
        private static void Init() => GetWindow(typeof(InstallerWindow)).Show();

        [MenuItem("TiltingPoint/SDK Installer/Manage Packages")]
        private static void InitOnPackagesPage()
        {
            var window = (InstallerWindow) GetWindow(typeof(InstallerWindow));
            window.openOnPackagesPage = true;
            window.Show();
        }

        private void OnEnable()
        {
            titleContent.image = AssetTool.LoadAsset(logoSmallFilePaths, new Texture2D(5, 5));
            titleContent.text = TitleOfWindow;
            installationConfig = LoadInstallationParameters();
        }

        private void Update()
        {
            if ((DateTime.UtcNow - lastUpdate).TotalSeconds > 0.5f)
            {
                Repaint();
            }
        }

        private void OnGUI()
        {
            lastUpdate = DateTime.UtcNow;

            if (pages == null || pages.Count == 0)
            {
                CreatePages();
            }

            if (openOnPackagesPage)
            {
                openOnPackagesPage = false;
                var index = pages.FindIndex(x => x.GetType() == typeof(AddPackages));
                ChangePage(index == -1 ? pages.Count : index);
            }

            installationConfig = installationConfig ?? LoadInstallationParameters();

            DrawHeader(currentPage, pages.Count - 1);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            pages[currentPage].DrawGui(installationConfig);
            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            DrawButtons(pages[currentPage].IsCompleted);
        }

        private void DrawHeader(int currentStep, int allSteps)
        {
            GUILayout.Space(12);
            GUILayout.BeginHorizontal(GUILayout.Height(30));
            var style = new GUIStyle(GUI.skin.label)
                        {
                            alignment = TextAnchor.MiddleCenter, fontSize = 25, fontStyle = FontStyle.Bold,
                        };

            GUILayout.Label(
                            currentStep == 0
                                ? $"Tilting Point SDK Installer"
                                : $"Step {currentStep} of {allSteps}",
                            style);

            GUILayout.EndHorizontal();

            DrawHorizontalLine(Color.gray);
        }


        private void CreatePages() =>
            pages = new List<IInstallerPage>()
                    {
                        new UserLanding(),
                        new AddScopedRegistries(),
                        new AddPackages(),
                        new AddExternalDependencyManager(),
                    };

        private void DrawButtons(bool isCompleted)
        {
            DrawHorizontalLine(Color.gray);

            GUILayout.BeginHorizontal(GUILayout.MinHeight(30), GUILayout.MaxHeight(40));

            ShowColoredButton(currentPage == 0 ? string.Empty : "Back", defaultButtonColor, PreviousPage);

            if (currentPage + 1 == pages.Count)
            {
                ShowColoredButton("Done", new Color(0.9f, 0.9f, 0.9f, 1f), null);
            }
            else
            {
                ShowColoredButton("Next", isCompleted ? successButtonColor : defaultButtonColor, NextPage);
            }

            GUILayout.EndHorizontal();
        }

        private void DrawHorizontalLine(Color color)
        {
            EditorGUILayout.Space();
            var rect = EditorGUILayout.BeginHorizontal();
            var c = Handles.color;
            Handles.color = color;
            Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
            Handles.color = c;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private void ShowColoredButton(string text, Color bgColor, Action action)
        {
            var style = new GUIStyle(GUI.skin.button);
            var previousBgColor = GUI.backgroundColor;
            GUI.backgroundColor = bgColor;
            var buttonWasPressed = GUILayout.Button(text, style, GUILayout.MinHeight(60), GUILayout.MaxHeight(80));
            GUI.backgroundColor = previousBgColor;
            if (buttonWasPressed)
            {
                action?.Invoke();
            }
        }


        private void ChangePage(bool toNext)
        {
            var page = currentPage;
            currentPage = Mathf.Clamp(currentPage + (toNext ? 1 : -1), 0, pages.Count - 1);
            if (currentPage == page)
            {
                return;
            }

            pages[page].LeavePage(installationConfig);
            pages[currentPage].BeforeOpenPage(installationConfig);
        }

        private void ChangePage(int number)
        {
            var page = currentPage;
            currentPage = Mathf.Clamp(number, 0, pages.Count - 1);
            if (currentPage == page)
            {
                return;
            }

            pages[page].LeavePage(installationConfig);
            pages[currentPage].BeforeOpenPage(installationConfig);
        }

        private void NextPage() => ChangePage(true);

        private void PreviousPage() => ChangePage(false);

        private InstallationConfig LoadInstallationParameters()
        {
            var content = AssetTool.LoadAsset<TextAsset>(configurationFilePath);
            if (content)
            {
                return JsonUtility.FromJson<InstallationConfig>(content.text);
            }

            Debug.LogError($"Can not find default configuration file ! ");
            return null;
        }
    }
}
