// Copyright (c) Tilting Point Media LLC. All rights reserved.

using TiltingPoint.Installer.Editor.Data;
using TiltingPoint.Installer.Editor.Tools.UI;
using UnityEditor;

namespace TiltingPoint.Installer.Editor.Pages
{
    internal class GetAccessToGitProject : AbstractPage, IInstallerPage
    {
        private const string HeaderText =
            "Get access to packages.";

        private const string DescriptionText =
            "Our packages are places in privat Vardaccio repository. In order to get access to it" +
            "you need to contact our team to get your login and password. " +
            "If you already have access to our packages, press Next.";

        private const string ContactEmail = "devrel@tiltingpoint.com";

        public bool IsCompleted => true;

        public void DrawGui(InstallationConfig config)
        {
            ShowHeader(HeaderText);

            EditorGUILayout.Space(10);
            ShowText(DescriptionText);
            EditorGUILayout.Space(10);

            UiLinks.ShowMailToLabel($"Mail to {ContactEmail}", ContactEmail);
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
