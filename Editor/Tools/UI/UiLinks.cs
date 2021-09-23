// <copyright file="UiLinks.cs" company="Tilting Point Media LLC">
// Copyright (c) Tilting Point Media LLC. All rights reserved.
// </copyright>

using UnityEngine;

namespace TiltingPoint.Installer.Editor.Tools.UI
{
    /// <summary>
    /// This class contains some simplifications for showing links in UI.
    /// </summary>
    internal static class UiLinks
    {
        /// <summary>
        /// Default link stile.
        /// </summary>
        private static readonly GUIStyle LinkStyle = new GUIStyle(GUI.skin.label)
                                                     {
                                                         normal = { textColor = Color.cyan },
                                                         hover = { textColor = Color.cyan },
                                                         active = { textColor = Color.cyan * 0.9f },
                                                     };

        /// <summary>
        /// Displays clickable URL.
        /// </summary>
        /// <param name="text">Text that will be displayed.</param>
        /// <param name="url">Link that will be opened on click.</param>
        internal static void ShowUrlLabel(string text, string url)
        {
            if (!GUILayout.Button(text, LinkStyle))
            {
                return;
            }

            try
            {
                Application.OpenURL(url);
            }
            catch
            {
                // ignored exception
            }
        }

        /// <summary>
        /// Displays clickable URL that moves to mail app.
        /// </summary>
        /// <param name="text">Text that will be displayed.</param>
        /// <param name="email">Emails that will be used as mail target.</param>
        internal static void ShowMailToLabel(string text, string email) => ShowUrlLabel(text, $"mailto:{email}");
    }
}
