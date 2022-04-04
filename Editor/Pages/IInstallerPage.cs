// Copyright (c) Tilting Point Media LLC. All rights reserved.

using TiltingPoint.Installer.Editor.Data;

namespace TiltingPoint.Installer.Editor.Pages
{
    internal interface IInstallerPage
    {
        bool IsCompleted { get; }

        void DrawGui(InstallationConfig config);

        void BeforeOpenPage(InstallationConfig config);

        void LeavePage(InstallationConfig config);
    }
}
