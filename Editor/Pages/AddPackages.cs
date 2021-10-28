// <copyright file="AddPackages.cs" company="Tilting Point Media LLC">
// Copyright (c) Tilting Point Media LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using TiltingPoint.Installer.Editor.Data;
using TiltingPoint.Installer.Editor.Tools;
using TiltingPoint.Installer.Editor.Tools.UI;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace TiltingPoint.Installer.Editor.Pages
{
    internal class AddPackages : AbstractPage, IInstallerPage
    {
        private const string HeaderText = "Add packages";

        private const string ShortDescription =
            "It is last step. Please select what do you want to import.";

        private const float VersionTextWidth = 50;
        private const float ButtonWidth = 60;

        private readonly UnityPackagesTool packagesTool;

        private bool isGooglePackageAdded;
        private bool isAnyTiltingPointPackageAdded;
        private Vector2 packagesListScrollPosition;
        private List<string> packagesList;

        public bool IsCompleted => false;

        public AddPackages()
        {
            packagesTool = new UnityPackagesTool();
            packagesList = new List<string>();
        }

        public void BeforeOpenPage(InstallationConfig config)
        {
            if (config == null)
            {
                return;
            }

            packagesList = config.TiltingPointPackages.Union(config.OtherDependencies).ToList();

            packagesTool.Clear();
            packagesTool.UpdatePackagesInformation(packagesList);
        }

        public void LeavePage(InstallationConfig config)
        {
            // empty
        }

        public void DrawGui(InstallationConfig config)
        {
            packagesTool.UpdatePackagesInformation(packagesList);

            ShowHeader(HeaderText);
            ShowText(ShortDescription);

            var isPackageToolBusy = packagesTool.IsBusy;

            Space(10);
            ShowFetchPackagesButton(config, isPackageToolBusy);

            Space(10);
            ShowText("Titling Point Packages:");
            ShowPackagesList(packagesList, packagesTool, isPackageToolBusy);
        }

        private void ShowFetchPackagesButton(InstallationConfig config, bool isPackageToolBusy)
        {
            GUILayout.BeginHorizontal();
            ShowText("Fetch Titling Point packages list:");
            if (ShowButton("Fetch packages list", 200 - 16, !isPackageToolBusy))
            {
                packagesTool.SearchAllPackages(x => OnSearchAllPackagesComplete(x, config.TiltingPointRegistry.Scopes));
            }

            HorizontalSpace(16);
            GUILayout.EndHorizontal();
        }

        private void OnSearchAllPackagesComplete(PackageInfo[] result, IEnumerable<string> scopes)
        {
            if (result == null || result.Length == 0)
            {
                return;
            }

            packagesList = result.Where(x => scopes.Any(scope => x.name.StartsWith(scope)))
                                 .Select(x => x.name)
                                 .Where(x => !packagesList.Contains(x))
                                 .OrderBy(x => x)
                                 .ToList();
        }

        private void ShowPackagesList(IEnumerable<string> packages, UnityPackagesTool tool, bool isBusy)
        {
            UiGroups.BeginBoxGroup();
            packagesListScrollPosition = EditorGUILayout.BeginScrollView(
                                                                         packagesListScrollPosition,
                                                                         false,
                                                                         false,
                                                                         GUILayout.ExpandHeight(true),
                                                                         GUILayout.ExpandWidth(true));

            var packagesArray = packages.ToArray();
            ShowPackageListHeader(isBusy, packagesArray, tool);
            DrawLine();

            foreach (var item in packagesArray)
            {
                ShowPackageInList(item, tool, isBusy);
            }

            EditorGUILayout.EndScrollView();
            UiGroups.EndBoxGroup();
        }

        private void ShowPackageListHeader(bool isBusy, IEnumerable<string> packages, UnityPackagesTool tool)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Local", GUILayout.Width(VersionTextWidth));
            GUILayout.Label("Remote", GUILayout.Width(VersionTextWidth));
            GUILayout.Label(" Package name");
            GUILayout.FlexibleSpace();
            GUILayout.Label(" Actions", GUILayout.Width(ButtonWidth));
            GUILayout.Label(isBusy ? "Updating..." : " ", GUILayout.Width(ButtonWidth));
            if (ShowButton("Refresh", ButtonWidth, !isBusy))
            {
                tool.UpdatePackagesInformation(packages, true);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ShowPackageInList(string id, UnityPackagesTool tool, bool ignoreActions)
        {
            EditorGUILayout.BeginHorizontal();

            var (localVersion, remoteVersion) = tool.GetPackageVersions(id);

            var canAdd = string.IsNullOrEmpty(localVersion);
            var canUpdate = !string.IsNullOrEmpty(localVersion)
                            && !string.IsNullOrEmpty(remoteVersion)
                            && localVersion != remoteVersion;
            var canRemove = !string.IsNullOrEmpty(localVersion);

            if (string.IsNullOrEmpty(localVersion))
            {
                localVersion = " -.-.-";
            }

            if (string.IsNullOrEmpty(remoteVersion))
            {
                remoteVersion = " -.-.-";
            }

            GUILayout.Label(localVersion, GUILayout.Width(VersionTextWidth));
            GUILayout.Label(remoteVersion, GUILayout.Width(VersionTextWidth));
            GUILayout.Label(id, GUILayout.ExpandWidth(false));

            GUILayout.FlexibleSpace();

            if (ShowButton("Add", ButtonWidth, canAdd) && !ignoreActions)
            {
                tool.AddPackage(id);
            }

            if (ShowButton("Update", ButtonWidth, canUpdate) && !ignoreActions)
            {
                tool.AddPackage(id);
            }

            if (ShowButton("Remove", ButtonWidth, canRemove) && !ignoreActions)
            {
                tool.RemovePackage(id);
            }

            EditorGUILayout.EndHorizontal();
        }

        private bool ShowButton(string text, float width, bool enabled)
        {
            EditorGUI.BeginDisabledGroup(!enabled);
            var result = GUILayout.Button(text, GUILayout.Width(width));
            EditorGUI.EndDisabledGroup();
            return result && enabled;
        }
    }
}
