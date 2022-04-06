// Copyright (c) Tilting Point Media LLC. All rights reserved.

using System.Collections.Generic;
using UnityEditor;

namespace TiltingPoint.Installer.Editor.Tools
{
    public static class AssetTool
    {
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
