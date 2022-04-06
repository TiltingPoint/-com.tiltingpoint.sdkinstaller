// Copyright (c) Tilting Point Media LLC. All rights reserved.

using UnityEditor;
using UnityEngine;

namespace TiltingPoint.Installer.Editor.Tools.UI
{
    /// <summary>
    /// This class contains simplifications for UI groups organisation.
    /// </summary>
    internal static class UiGroups
    {
        /// <summary>
        /// Begins vertical group with GroupBox style.
        /// </summary>
        public static void BeginBoxGroup() => GUILayout.BeginVertical("GroupBox");

        /// <summary>
        /// Ends vertical group.
        /// </summary>
        public static void EndBoxGroup() => GUILayout.EndVertical();

        /// <summary>
        /// Begins foldout vertical group with GroupBox style.
        /// </summary>
        /// <param name="foldout">Should group be unfolded.</param>
        /// <param name="headerText">Header text.</param>
        /// <returns>True is group should be unfolded.</returns>
        public static bool BeginFoldoutBoxGroup(bool foldout, string headerText)
        {
            GUILayout.BeginVertical("GroupBox");
            return EditorGUILayout.Foldout(foldout, headerText);
        }

        /// <summary>
        /// Ends vertical group.
        /// </summary>
        public static void EndFoldoutBoxGroup() => GUILayout.EndVertical();
    }
}
