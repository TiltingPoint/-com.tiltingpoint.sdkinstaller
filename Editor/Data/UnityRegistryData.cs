// <copyright file="UnityRegistryData.cs" company="Tilting Point Media LLC">
// Copyright (c) Tilting Point Media LLC. All rights reserved.
// </copyright>

using System;
using UnityEngine;

namespace TiltingPoint.Installer.Editor.Data
{
    /// <summary>
    /// Representation of UPM Registry record data for manifest.json.
    /// </summary>
    [Serializable]
    internal class UnityRegistryData
    {
        [SerializeField]
        private string registryName;

        [SerializeField]
        private string registryUrl;

        [SerializeField]
        private string[] scopes;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityRegistryData"/> class.
        /// </summary>
        /// <param name="name">Registry name.</param>
        /// <param name="url">Registry Url.</param>
        /// <param name="scopesCollection">Scopes.</param>
        public UnityRegistryData(string name, string url, string[] scopesCollection)
        {
            registryName = name;
            registryUrl = url;
            scopes = scopesCollection;
        }

        /// <summary>
        /// Gets registry name.
        /// </summary>
        /// <remarks>
        /// Name that will be displayed in projects settings.
        /// Example: "My private NPM registry".
        /// </remarks>
        public string RegistryName => registryName;

        /// <summary>
        /// Gets registry URL.
        /// </summary>
        /// <remarks>
        /// Example: "https://npm.pkg.github.com/@MyScope".</remarks>
        public string RegistryUrl => registryUrl;

        /// <summary>
        /// Gets collections of scopes.
        /// </summary>
        /// Exaple: {"com.mycompanyname", "com.mytools"}
        public string[] Scopes => scopes;
    }
}
