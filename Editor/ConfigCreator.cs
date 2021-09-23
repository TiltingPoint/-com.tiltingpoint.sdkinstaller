// <copyright file="ConfigCreator.cs" company="Tilting Point Media LLC">
// Copyright (c) Tilting Point Media LLC. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Text;
using TiltingPoint.Installer.Editor.Data;
using UnityEditor;
using UnityEngine;

namespace TiltingPoint.Installer.Editor
{
    [Serializable]
    internal class ConfigCreator : EditorWindow
    {
        private const string TiltingPointRegistryName = "Tilting Point Registry";

        private const string TiltingPointRegistryUrlForUnity = "http://registry.tiltingpoint.io/";
        private const string TiltingPointRegistryScopeForUnity = "com.tiltingpoint";


        private const string GoogleGamePackageRegistryName = "Game Package Registry by Google";
        private const string GoogleGamePackageRegistryUrl = "https://unityregistry-pa.googleapis.com";
        private const string GoogleGamePackageRegistryScope = "com.google";

        private static readonly string[] TiltingPointPackages = new[]
                                                                {
                                                                    "com.tiltingpoint.ads",
                                                                    "com.tiltingpoint.amplitude",
                                                                    "com.tiltingpoint.appsflyer",
                                                                    "com.tiltingpoint.iostracking",
                                                                    "com.tiltingpoint.ops",
                                                                    "com.tiltingpoint.purchasing",
                                                                    "com.tiltingpoint.qa", "com.tiltingpoint.sampleapp",
                                                                };

        private static readonly string[] OtherPackages;

        [SerializeField]
        private InstallationConfig config;

        [MenuItem("Tools/CreateConfig")]
        private static void Init()
        {
            var window = (ConfigCreator) GetWindow(typeof(ConfigCreator));
            window.config = GenerateConfig();
            window.Show();
        }

        private static void CreateConfigFile(InstallationConfig data)
        {
            var path = EditorUtility.SaveFilePanel("Path", Application.dataPath, "configuration.json", string.Empty);
            var content = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, content, Encoding.UTF8);
        }

        private static InstallationConfig GenerateConfig() =>
            new InstallationConfig
            {
                TiltingPointRegistry = GenerateTiltingPointRegistry(),
                OtherRegistries = new[] {GenerateGoogleRegistry()},
                TiltingPointPackages = TiltingPointPackages,
                OtherDependencies = OtherPackages,
            };

        private static UnityRegistryData GenerateTiltingPointRegistry() =>
            new UnityRegistryData(
                                  TiltingPointRegistryName,
                                  TiltingPointRegistryUrlForUnity,
                                  new[] {TiltingPointRegistryScopeForUnity});

        private static UnityRegistryData GenerateGoogleRegistry() =>
            new UnityRegistryData(
                                  GoogleGamePackageRegistryName,
                                  GoogleGamePackageRegistryUrl,
                                  new[] {GoogleGamePackageRegistryScope});

        private void OnGUI()
        {
            if (config == null)
            {
                config = GenerateConfig();
            }

            var serializedObject = new SerializedObject(this);
            var stringsProperty = serializedObject.FindProperty("config");

            EditorGUILayout.PropertyField(stringsProperty, true);
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("SaveFile"))
            {
                CreateConfigFile(config);
            }
        }
    }
}
