// Copyright (c) Tilting Point Media LLC. All rights reserved.

using UnityEditor;
using UnityEngine;

namespace TiltingPoint.Installer.Editor.Pages
{
    /// <summary>
    /// This class contains some simplifications for pages.
    /// </summary>
    public class AbstractPage
    {
        /// <summary>
        /// Cached header style.
        /// </summary>
        private static readonly GUIStyle HeaderStyle =
            new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontSize = 24};


        /// <summary>
        /// Displays header text.
        /// </summary>
        /// <param name="text">Text that will be displayed.</param>
        protected static void ShowHeader(string text)
        {
            GUILayout.Space(20);
            GUILayout.Label(text, HeaderStyle);
            GUILayout.Space(10);
        }

        /// <summary>
        /// Draws line as part of UI content.
        /// </summary>
        protected static void DrawLine() => EditorGUILayout.LabelField(" ", GUI.skin.horizontalSlider);

        /// <summary>
        /// Shows text that can be wrapped.
        /// </summary>
        /// <param name="text">Text.</param>
        protected static void ShowText(string text) => GUILayout.Label(text, EditorStyles.wordWrappedLabel);

        /// <summary>
        /// Add space.
        /// </summary>
        /// <param name="pixels">Space amount.</param>
        protected static void Space(int pixels) => GUILayout.Space(pixels);

        /// <summary>
        /// Add empty space for horizontal layout.
        /// </summary>
        /// <param name="pixels">Space amount.</param>
        protected static void HorizontalSpace(int pixels) => GUILayout.Label(" ", GUILayout.Width(pixels));
    }
}
