// <copyright file="NpmRegistryData.cs" company="Tilting Point Media LLC">
// Copyright (c) Tilting Point Media LLC. All rights reserved.
// </copyright>

using System;
using UnityEngine;

namespace TiltingPoint.Installer.Editor.Data
{
    /// <summary>
    /// Represents data for configuring UPM (.upmconfig.toml file).
    /// </summary>
    [Serializable]
    internal class NpmRegistryData
    {
        [SerializeField]
        private string registry;

        [SerializeField]
        private string scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpmRegistryData"/> class.
        /// </summary>
        /// <param name="registryValue">Registry URL.</param>
        /// <param name="scopeValue">Registry scope.</param>
        public NpmRegistryData(string registryValue, string scopeValue)
        {
            registry = registryValue;
            scope = scopeValue;
        }

        /// <summary>
        /// Gets registry URL.
        /// </summary>
        /// <remarks>
        /// Example: "https://npm.pkg.github.com/".
        /// </remarks>
        public string Registry => registry;

        /// <summary>
        /// Gets registry scope.
        /// </summary>
        /// <remarks>
        /// Example: "@TiltingPoint".
        /// </remarks>
        public string Scope => scope;
    }
}
