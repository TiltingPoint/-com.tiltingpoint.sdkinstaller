// <copyright file="IInstallerPage.cs" company="Tilting Point Media LLC">
// Copyright (c) Tilting Point Media LLC. All rights reserved.
// </copyright>

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
