// <copyright file="TomlTools.cs" company="Tilting Point Media LLC">
// Copyright (c) Tilting Point Media LLC. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace TiltingPoint.Installer.Editor.Tools
{
    internal static class TomlTools
    {
        private const string DefaultFileName = ".upmconfig.toml";

        private static string FullFilePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), DefaultFileName);

        /// <summary>
        /// Reads .upmconfig.toml and checks if it contains registry.
        /// </summary>
        /// <param name="registry">Url of the registry that includes scope.
        /// Example: "https://npm.pkg.github.com/@ScopeName".</param>
        /// <param name="token">Access token.</param>
        /// <returns>True is registry is present.</returns>
        public static bool IsRegistriesPresent(string registry, string token)
        {
            var path = FullFilePath;
            if (!File.Exists(path))
            {
                return false;
            }

            var fileContent = File.ReadAllText(path);
            if (string.IsNullOrEmpty(fileContent))
            {
                return false;
            }

            var sample = GenerateTomlFileContent(registry, token);
            return fileContent.Contains(sample);
        }

        /// <summary>
        /// Adds authorization data into UPM config file (.upmconfig.toml).
        /// UPM uses this date every time it does request for packages.
        /// </summary>
        /// <param name="registry">Url of the registry that includes scope.
        /// Example: "https://npm.pkg.github.com/@ScopeName".</param>
        /// <param name="accessToken">Access Token.</param>
        /// <returns>
        /// Field isSuccess is true if add actions is successful.
        /// Field error will contains error message in a case when actions is failed.
        /// </returns>
        /// <remarks>
        /// How it should looks like in '.upmconfig.toml' file.
        /// ****
        /// [npmAuth."https://npm.pkg.github.com/@TiltingPoint"]
        /// token = "GITHUB_PERSONAL_ACCESS_TOKEN"
        /// alwaysAuth = true
        /// ****.
        /// </remarks>
        public static (bool IsSuccess, string Error) AddRegistries(string registry, string accessToken)
        {
            var path = FullFilePath;
            if (!File.Exists(path))
            {
                return WriteFile(path, GenerateTomlFileContent(registry, accessToken));
            }

            var fileContent = File.ReadAllText(path, Encoding.ASCII);
            if (fileContent.Length == 0)
            {
                return WriteFile(path, GenerateTomlFileContent(registry, accessToken));
            }

            if (IsRegistryPresent(fileContent, registry))
            {
                fileContent = RemoveRegistryFromFile(fileContent, registry);
            }

            fileContent = fileContent.TrimEnd('\n');
            if (fileContent.Length != 0)
            {
                fileContent += "\n\n";
            }

            return WriteFile(path, fileContent + GenerateTomlFileContent(registry, accessToken));
        }

        private static string RemoveRegistryFromFile(string content, string registry)
        {
            var searchLine = registry;
            var lines = content.Split(new[] {'\n'});

            var removeNextLine = false;
            for (var i = 0; i < lines.Length; i++)
            {
                if (removeNextLine)
                {
                    if (lines[i] != null && lines[i].StartsWith("[npmAuth"))
                    {
                        removeNextLine = false;
                        continue;
                    }

                    lines[i] = null;
                    continue;
                }

                if (lines[i] == null || !lines[i].Contains(searchLine))
                {
                    continue;
                }

                lines[i] = null;
                removeNextLine = true;
            }

            return string.Join("\n", lines.Where(x => x != null));
        }

        private static bool IsRegistryPresent(string fileContent, string registry) => fileContent.Contains(registry);

        private static string GenerateTomlFileContent(string registry, string token) =>
            $"[npmAuth.\"{registry}\"]\n" +
            $"token = \"{token}\"\n" +
            $"alwaysAuth = true";

        private static (bool IsSuccess, string Error) WriteFile(string path, string content)
        {
            try
            {
                File.WriteAllText(path, content, Encoding.ASCII);
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }

            return (true, null);
        }
    }
}
