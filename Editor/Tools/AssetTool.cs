// Copyright (c) Tilting Point Media LLC. All rights reserved.

using System.Collections.Generic;
using UnityEditor;

namespace TiltingPoint.Installer.Editor.Tools
{
    /// <summary>
    /// Class for work with assets.
    /// </summary>
    public static class AssetTool
    {
        /// <summary>
        /// Load assets.
        /// </summary>
        /// <param name="paths">Path to asset.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <typeparam name="T">Asset type.</typeparam>
        /// <returns>Loaded asset or default value.</returns>
        public static T LoadAsset<T>(IEnumerable<string> paths, T defaultValue = default) where T : UnityEngine.Object
        {
            if (paths == null)
            {
                return defaultValue;
            }

            foreach (var item in paths)
            {
                var result = AssetDatabase.LoadAssetAtPath<T>(item);
                if (result)
                {
                    return result;
                }
            }

            return defaultValue;
        }
    }
}
