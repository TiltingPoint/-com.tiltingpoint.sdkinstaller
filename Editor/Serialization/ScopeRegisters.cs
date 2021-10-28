// <copyright file="ScopeRegisters.cs" company="Tilting Point Media LLC">
// Copyright (c) Tilting Point Media LLC. All rights reserved.
// </copyright>

using System;
using System.Linq;

namespace TiltingPoint.Installer.Editor.Serialization
{
    /// <summary>
    /// Representations of scoped registries in manifest.json file (Unity Package Manager).
    /// </summary>
    [Serializable]
    internal class ScopeRegisters
    {
        /// <summary>
        /// Array of registries.
        /// </summary>
        public Scope[] scopedRegistries = new Scope[0];

        /// <summary>
        /// Check if current array of registries contains registry and all of scopes.
        /// </summary>
        /// <param name="registryUrl">Registry url.</param>
        /// <param name="scopes">Scopes collections.</param>
        /// <returns>True if registry is present and contains all scopes.</returns>
        public bool ContainRegistry(string registryUrl, string[] scopes) =>
            (from item in scopedRegistries
             where item.url == registryUrl
             select scopes.All(scope => item.scopes.Contains(scope))).FirstOrDefault();

        /// <summary>
        /// Add registry to current registries array.
        /// </summary>
        /// <param name="registryName">Registry name.</param>
        /// <param name="registryUrl">Registry url.</param>
        /// <param name="scopes">Collection of scopes.</param>
        /// <returns>Returns true if registry od any of scopes were added.</returns>
        public bool SafeAddRegistry(string registryName, string registryUrl, string[] scopes)
        {
            foreach (var item in scopedRegistries)
            {
                if (item.url == registryUrl)
                {
                    return item.AddScopes(scopes);
                }
            }

            var scope = new Scope {name = registryName, scopes = scopes ?? new string[0], url = registryUrl};
            scopedRegistries = scopedRegistries.Append(scope).ToArray();
            return true;
        }
    }
}
