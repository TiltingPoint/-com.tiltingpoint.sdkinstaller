// <copyright file="Scope.cs" company="Tilting Point Media LLC">
// Copyright (c) Tilting Point Media LLC. All rights reserved.
// </copyright>

using System;
using System.Linq;

namespace TiltingPoint.Installer.Editor.Serialization
{
    /// <summary>
    /// Representations of scoped registry in manifest.json file (Unity Package Manager).
    /// </summary>
    [Serializable]
    internal class Scope
    {
        /// <summary>
        /// Registry name.
        /// </summary>
        public string name;

        /// <summary>
        /// registry url.
        /// </summary>
        public string url;

        /// <summary>
        /// Array of scopes.
        /// </summary>
        public string[] scopes = new string[0];

        /// <summary>
        /// Add scopes to array of scopes.
        /// </summary>
        /// <param name="newScopes">Collections of scopes to add.</param>
        /// <returns>Returns true if any of scopes was added.</returns>
        public bool AddScopes(string[] newScopes)
        {
            var isChanged = false;
            foreach (var item in newScopes)
            {
                if (scopes.Contains(item))
                {
                    continue;
                }

                scopes = scopes.Append(item).ToArray();
                isChanged = true;
            }

            return isChanged;
        }
    }
}
