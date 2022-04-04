// Copyright (c) Tilting Point Media LLC. All rights reserved.

using TiltingPoint.Installer.Editor.Data;
using TiltingPoint.Installer.Editor.Tools.UI;
using UnityEditor;

namespace TiltingPoint.Installer.Editor.Pages
{
    internal class AddExternalDependencyManager : AbstractPage, IInstallerPage
    {
        private const string HeaderText =
            "Google External Dependency Manager.";

        private const string DescriptionText =
            "To make evrything work correctly add Google External Dependency Manager into yout project." +
            "Latest version can be found here:";

        private const string LatestVersionLink = "https://developers.google.com/unity/archive#external_dependency_manager_for_unity";

        public bool IsCompleted => true;

        public void DrawGui(InstallationConfig config)
        {
            ShowHeader(HeaderText);

            EditorGUILayout.Space(10);
            ShowText(DescriptionText);
            EditorGUILayout.Space(10);

            UiLinks.ShowUrlLabel("External Dependency Manager for Unity", LatestVersionLink);
        }

        public void BeforeOpenPage(InstallationConfig config)
        {
            // empty
        }

        public void LeavePage(InstallationConfig config)
        {
            // empty
        }
    }
}
