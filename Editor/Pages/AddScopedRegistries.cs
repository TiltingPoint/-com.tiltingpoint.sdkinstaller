// <copyright file="AddScopedRegistries.cs" company="Tilting Point Media LLC">
// Copyright (c) Tilting Point Media LLC. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using TiltingPoint.Installer.Editor.Data;
using TiltingPoint.Installer.Editor.Tools;
using TiltingPoint.Installer.Editor.Tools.UI;
using UnityEditor;
using UnityEngine;

namespace TiltingPoint.Installer.Editor.Pages
{
    internal class AddScopedRegistries : AbstractPage, IInstallerPage
    {
        private const string HeaderText = "Add Registries";

        private const string ReadMoreAboutScopedRegistriesUrl = "https://docs.unity3d.com/Manual/upm-scoped.html";
        private const string ReadMoreAboutScopedRegistriesUrlText = "Read more about Scoped Registries";

        private const string ShortDescriptionText =
            "To let Unity know where it can find our packages we will add Tilting Point and " +
            "Google scoped registries to the project.";

        private const string ScopedRegistriesDescriptionHeader = "What is scoped registries for?";

        private const string ScopedRegistriesDescriptionText =
            "Scoped Registries allow Unity to communicate the location of any custom package " +
            "registry server to the Package Manager so that the user has access to several " +
            "collections of packages at the same time.";

        private bool isRegistryAdded;
        private bool showScopedRegistryDescription;
        private Dictionary<string, bool> dependenciesState = new Dictionary<string, bool>(0);

        public bool IsCompleted => isRegistryAdded;

        public void BeforeOpenPage(InstallationConfig config)
        {
            UpdateState(config);
        }

        public void LeavePage(InstallationConfig config)
        {
        }

        public void DrawGui(InstallationConfig config)
        {
            if (dependenciesState == null)
            {
                dependenciesState = GetRegistriesState(GetAllRegistries(config));
                isRegistryAdded = dependenciesState.Values.All(x => x);
            }

            ShowHeader(HeaderText);
            GUILayout.Space(10);
            ShowText(ShortDescriptionText);

            showScopedRegistryDescription =
                UiGroups.BeginFoldoutBoxGroup(showScopedRegistryDescription, ScopedRegistriesDescriptionHeader);
            if (showScopedRegistryDescription)
            {
                GUILayout.Space(10);
                ShowText(ScopedRegistriesDescriptionText);
                UiLinks.ShowUrlLabel(ReadMoreAboutScopedRegistriesUrlText, ReadMoreAboutScopedRegistriesUrl);
            }

            UiGroups.EndFoldoutBoxGroup();

            GUILayout.Space(20);

            ShowRegistriesList();

            GUILayout.Space(20);

            ShowAddRegistriesButton(config);
        }

        private void ShowRegistriesList()
        {
            UiGroups.BeginBoxGroup();

            GUILayout.Label("  Present       Registry");
            DrawLine();
            foreach (var item in dependenciesState)
            {
                GUILayout.Label($"      ( {(item.Value ? "+" : "-")} )         {item.Key}");
            }

            UiGroups.EndBoxGroup();
        }

        private void ShowAddRegistriesButton(InstallationConfig config)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Add registries", GUILayout.Width(160), GUILayout.Height(40)))
            {
                AddRegistries(config);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void AddRegistries(InstallationConfig config)
        {
            var (isSuccess, errorMessage) =
                UnityPackageManifestTool.AddRegistriesToManifest(GetAllRegistries(config)
                                                                     .Select(x => (x.RegistryName, x.RegistryUrl,
                                                                                 x.Scopes)));

            isRegistryAdded = isSuccess;
            if (!isRegistryAdded)
            {
                Debug.LogError(errorMessage);
                return;
            }

            AssetDatabase.Refresh();
            UpdateState(config);
        }

        private void UpdateState(InstallationConfig config)
        {
            dependenciesState = GetRegistriesState(GetAllRegistries(config));
            isRegistryAdded = dependenciesState.Values.All(x => x);
        }

        private (string Name, string Registry, string[] Scope) GetRegistryData(UnityRegistryData registryData) =>
            (registryData.RegistryName, registryData.RegistryUrl, registryData.Scopes);

        private Dictionary<string, bool> GetRegistriesState(IEnumerable<UnityRegistryData> registries) =>
            UnityPackageManifestTool
                .IsRegistriesPresentInManifest(registries.Select(GetRegistryData))
                .ToDictionary(x => x.Id, y => y.IsPresent);

        private IEnumerable<UnityRegistryData> GetAllRegistries(InstallationConfig config) =>
            new[] {config.TiltingPointRegistry}.Union(config.OtherRegistries);

        private void DrawLine() => EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
    }
}
