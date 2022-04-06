// Copyright (c) Tilting Point Media LLC. All rights reserved.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TiltingPoint.Installer.Editor.Serialization;
using UnityEngine;

namespace TiltingPoint.Installer.Editor.Tools
{
    internal static class UnityPackageManifestTool
    {
        /// <summary>
        /// Checks if unity packages manifest.json file contains registries.
        /// </summary>
        /// <param name="registriesToCheck">Collection of registries to check.</param>
        /// <returns>Collections of check results.</returns>
        /// <remarks>Use this method to ensure that registry is present in file.
        /// If some of scopes of registry is not present in file, result for this registry will be false.</remarks>
        public static List<(string Id, bool IsPresent)> IsRegistriesPresentInManifest(
            IEnumerable<(string Name, string Registry, string[] Scope)> registriesToCheck)
        {
            var manifestPath = Path.Combine(Application.dataPath, "..", "Packages", "manifest.json");
            if (!File.Exists(manifestPath))
            {
                return registriesToCheck.Select(x => (x.Name, false)).ToList();
            }

            var content = File.ReadAllLines(manifestPath).ToList();
            if (content.Count < 2)
            {
                return registriesToCheck.Select(x => (x.Name, false)).ToList();
            }

            var (registriesBeginLine, registriesEndLine) = FindRegistryScope(content);
            if (registriesBeginLine == -1 || registriesEndLine == -1 || (registriesBeginLine == registriesEndLine))
            {
                return registriesToCheck.Select(x => (x.Name, false)).ToList();
            }

            var presentRegistries = ExtractRegisters(content, registriesBeginLine, registriesEndLine);
            if (presentRegistries == null)
            {
                return registriesToCheck.Select(x => (x.Name, false)).ToList();
            }

            return registriesToCheck
                   .Select(x => (x.Name, presentRegistries.ContainRegistry(x.Registry.TrimEnd('/'), x.Scope)))
                   .ToList();
        }

        /// <summary>
        /// Add registries into unity packages manifest.json file.
        /// </summary>
        /// <param name="registriesToAdd">Collection of registries to add.</param>
        /// <returns>Information about add operation result.</returns>
        /// <remarks>Use this method to add registries or scopes into manifest.json file.
        /// </remarks>
        public static (bool IsSuccessful, string ErrorMessage) AddRegistriesToManifest(
            IEnumerable<(string Name, string Registry, string[] Scope)> registriesToAdd)
        {
            var manifestPath = Path.Combine(Application.dataPath, "..", "Packages", "manifest.json");
            if (!File.Exists(manifestPath))
            {
                return (false, $"Can not find Unity manifest.json! Path: {manifestPath}");
            }

            var content = File.ReadAllLines(manifestPath).ToList();
            if (content.Count < 2)
            {
                return (
                    false, $"Can read Unity manifest.json! Pls, check if manifest.json correct! Path: {manifestPath}");
            }

            var (registriesBeginLine, registriesEndLine) = FindOrAddRegistriesSection(content);
            if (registriesBeginLine == -1 || registriesEndLine == -1)
            {
                var errorMessage =
                    $"Can not detect/add 'scopedRegistries' into Unity manifest.json! Pls, check if manifest.json correct! Path: {manifestPath}";
                return (false, errorMessage);
            }

            var presentRegistries = ExtractRegisters(content, registriesBeginLine, registriesEndLine);
            if (presentRegistries == null)
            {
                var errorMessage =
                    $"Can not parse 'scopedRegistries' into Unity manifest.json! Pls, check if manifest.json correct! Path: {manifestPath}";
                return (false, errorMessage);
            }

            var isChanged = false;
            foreach (var item in registriesToAdd)
            {
                isChanged |= presentRegistries.SafeAddRegistry(item.Name, item.Registry, item.Scope);
            }

            if (!isChanged)
            {
                return (true, null);
            }

            var newRegistries = RegistersToTextLines(presentRegistries);

            content.RemoveRange(registriesBeginLine, registriesEndLine - registriesBeginLine + 1);
            content.InsertRange(registriesBeginLine, newRegistries);

            if (!content[registriesBeginLine + newRegistries.Count].StartsWith("}"))
            {
                content[registriesBeginLine + newRegistries.Count - 1] += ",";
            }

            File.WriteAllLines(manifestPath, content);
            return (true, null);
        }

        private static (int BeginLineIndex, int EndLineIndex) FindOrAddRegistriesSection(List<string> content)
        {
            var (registriesBeginLine, registriesEndLine) = FindRegistryScope(content);
            if (registriesBeginLine == -1)
            {
                (registriesBeginLine, registriesEndLine) = AddScopedRegistries(content);
            }

            if (registriesBeginLine == -1 || registriesEndLine == -1)
            {
                return (-1, -1);
            }

            return (registriesBeginLine, registriesEndLine);
        }

        private static ScopeRegisters ExtractRegisters(List<string> content, int startIndex, int endIndex)
        {
            if (startIndex == endIndex || startIndex == endIndex + 1)
            {
                return new ScopeRegisters();
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("{");
            for (var i = startIndex; i < endIndex; i++)
            {
                stringBuilder.AppendLine(content[i]);
            }

            stringBuilder.AppendLine(content[endIndex].Replace(",", string.Empty));
            stringBuilder.AppendLine("}");
            var text = stringBuilder.ToString();
            var result = JsonUtility.FromJson<ScopeRegisters>(text);
            return result;
        }

        private static List<string> RegistersToTextLines(ScopeRegisters registers)
        {
            var text = JsonUtility.ToJson(registers, true);
            var lines = text.Split('\n');
            var result = lines.Skip(1).Take(lines.Length - 2).Select(ReduceIndentSpaces).ToList();
            return result;
        }

        private static string ReduceIndentSpaces(string line)
        {
            var spaces = 0;
            for (var i = 0; i < line.Length; i++)
            {
                if (line[i] != ' ') break;
                spaces = i + 1;
            }

            return line.Substring(Mathf.FloorToInt(spaces / 2f));
        }

        private static (int BeginLineIndex, int EndLineIndex) AddScopedRegistries(List<string> content)
        {
            var text = new[] { "  \"scopedRegistries\": []" };
            var lineForInsert = -1;
            for (var i = content.Count() - 1; i >= 1; i--)
            {
                if (!content[i].Contains("}"))
                {
                    continue;
                }

                lineForInsert = i;
                break;
            }

            if (lineForInsert == -1)
            {
                Debug.LogError("Can not find right place in Unity manifest.json to insert registers scope.");
                return (-1, -1);
            }

            content.InsertRange(lineForInsert, text);
            if (lineForInsert > 1 && !content[lineForInsert - 1].Contains("},"))
            {
                content[lineForInsert - 1] = content[lineForInsert - 1].Replace("}", "},");
            }

            return (lineForInsert, lineForInsert + text.Length - 1);
        }

        private static (int BeginLineIndex, int EndLineIndex) FindRegistryScope(List<string> content)
        {
            var startLine = -1;
            var endLine = -1;
            for (var i = 0; i < content.Count(); i++)
            {
                if (!content[i].Contains("\"scopedRegistries\": ["))
                {
                    continue;
                }

                startLine = i;
                break;
            }

            if (startLine == -1)
            {
                return (startLine, endLine);
            }

            if (content[startLine].Contains("]"))
            {
                return (startLine, startLine);
            }

            endLine = FindScopeEnd(content, startLine);
            return (startLine, endLine);
        }

        private static int FindScopeEnd(List<string> strings, int startLineIndex)
        {
            var scopes = 1;
            for (var i = startLineIndex + 1; i < strings.Count(); i++)
            {
                scopes -= strings[i].Count(x => x == ']');
                scopes += strings[i].Count(x => x == '[');
                if (scopes == 0)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
