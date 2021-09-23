// <copyright file="InstallationConfig.cs" company="Tilting Point Media LLC">
// Copyright (c) Tilting Point Media LLC. All rights reserved.
// </copyright>

using System;
using UnityEngine;

namespace TiltingPoint.Installer.Editor.Data
{
    /// <summary>
    /// Representations of data that requested for TiltingPoint SDK installations process.
    /// </summary>
    [Serializable]
    internal class InstallationConfig
    {
        [SerializeField]
        private UnityRegistryData tiltingPointRegistry;

        [SerializeField]
        private UnityRegistryData[] otherRegistries;

        [SerializeField]
        private string[] otherDependencies;

        [SerializeField]
        private string[] tiltingPointPackages;

        /// <summary>
        /// Gets or sets tiltingPoint packages registry data.
        /// </summary>
        public UnityRegistryData TiltingPointRegistry
        {
            get => tiltingPointRegistry;
            set => tiltingPointRegistry = value;
        }

        /// <summary>
        /// Gets or sets other package registries data.
        /// </summary>
        public UnityRegistryData[] OtherRegistries
        {
            get => otherRegistries;
            set => otherRegistries = value;
        }

        /// <summary>
        /// Gets or sets other dependencies that should be showed to user.
        /// </summary>
        public string[] OtherDependencies
        {
            get => otherDependencies;
            set => otherDependencies = value;
        }

        /// <summary>
        /// Gets or sets list of TiltingPoint packages.
        /// </summary>
        public string[] TiltingPointPackages
        {
            get => tiltingPointPackages;
            set => tiltingPointPackages = value;
        }
    }
}
