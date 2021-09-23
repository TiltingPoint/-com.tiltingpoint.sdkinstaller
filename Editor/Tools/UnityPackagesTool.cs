// <copyright file="UnityPackagesTool.cs" company="Tilting Point Media LLC">
// Copyright (c) Tilting Point Media LLC. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace TiltingPoint.Installer.Editor.Tools
{
    /// <summary>
    /// This class provides functionality to manage packages dependencies of projects.
    /// </summary>
    internal class UnityPackagesTool
    {
        /// <summary>
        /// Simple class to contain few requests.
        /// </summary>
        /// <remarks> Due to UPM limitations, only one active List, Add or Remove request should exist.
        /// </remarks>
        private class PackageState
        {
            public PackageInfo Present;
            public AddRequest AddRequest;
            public SearchRequest SearchRequest;
            public RemoveRequest RemoveRequest;

            public bool HasActiveRequest => AddRequest?.IsCompleted == false
                                            || SearchRequest?.IsCompleted == false
                                            || RemoveRequest?.IsCompleted == false;
        }

        private Dictionary<string, PackageState> packagesInformation;
        private ListRequest listRequest;
        private SearchRequest searchAllRequest;

        /// <summary>
        /// Gets a value indicating whether indicates if there is any active request.
        /// </summary>
        /// <remarks>Check it to ensure that new Add or Remove request can be initiated.
        /// </remarks>
        public bool IsBusy => listRequest?.IsCompleted == false
                              || searchAllRequest?.IsCompleted == false
                              || packagesInformation?.Values.Any(x => x.HasActiveRequest) == true;

        /// <summary>
        /// Updates information for packages.
        /// </summary>
        /// <param name="packagesCollection">Collection of packages identificators(names).</param>
        /// <param name="force">Force information update for all packages.</param>
        /// <remarks>If parameter <param name="force"></param> is set to false, only packages that do not have
        /// current information will be updated.</remarks>
        public void UpdatePackagesInformation(IEnumerable<string> packagesCollection, bool force = false)
        {
            if (force || packagesInformation == null)
            {
                InitPackagesCollection(packagesCollection);
                RequestLocalListUpdate();
                FetchPackagesInfo(packagesInformation.Keys);
                return;
            }

            UpdateListResults();
            var requestLocalListUpdate = false;
            foreach (var item in packagesInformation.Values)
            {
                if (item.RemoveRequest?.IsCompleted == true)
                {
                    item.RemoveRequest = null;
                    requestLocalListUpdate = true;
                }

                if (item.AddRequest?.IsCompleted == true)
                {
                    item.AddRequest = null;
                    requestLocalListUpdate = true;
                }
            }

            if (requestLocalListUpdate)
            {
                RequestLocalListUpdate();
            }
        }

        /// <summary>
        /// Search in all repositories for all available packages.
        /// </summary>
        /// <param name="callback">Callback action.</param>
        public void SearchAllPackages(Action<PackageInfo[]> callback)
        {
            void UpdateSearchAllResults()
            {
                if (searchAllRequest != null && searchAllRequest.IsCompleted == false)
                {
                    return;
                }

                EditorApplication.update -= UpdateSearchAllResults;
                searchAllRequest = null;
                callback?.Invoke(searchAllRequest?.Result);
            }

            if (IsBusy)
            {
                return;
            }

            searchAllRequest = Client.SearchAll(false);
            EditorApplication.update += UpdateSearchAllResults;
        }

        /// <summary>
        /// Adds package to project dependencies.
        /// </summary>
        /// <param name="id">Package id (name).
        /// Example: "com.tiltingpoint.goodlifeinstaller".</param>
        public void AddPackage(string id)
        {
            var state = GetPackageState(id);
            if (state != null)
            {
                state.AddRequest = Client.Add(id);
            }
        }

        /// <summary>
        /// Removes package from project dependencies.
        /// </summary>
        /// <param name="id">Package id (name).
        /// Example: "com.tiltingpoint.goodlifeinstaller".
        /// </param>
        public void RemovePackage(string id)
        {
            var state = GetPackageState(id);
            if (state != null)
            {
                state.RemoveRequest = Client.Remove(id);
            }
        }

        /// <summary>
        /// Returns package versions.
        /// </summary>
        /// <param name="id">
        /// Package id (name).
        /// Example: "com.tiltingpoint.goodlifeinstaller".
        /// </param>
        /// <returns>
        /// Returns null for local package if package is not present in project.
        /// Returns null for remote package if can not get information about package.
        /// </returns>
        /// <remarks>
        /// Unity has package caching. If UPM can not get information about package from registry and
        /// package is present in cache, result will contain versions from cache.
        /// </remarks>
        public (string Local, string Remote) GetPackageVersions(string id)
        {
            var state = GetPackageState(id);
            return state == null ? (null, null) : (state.Present?.version, state.SearchRequest?.Result?[0]?.version);
        }

        /// <summary>
        /// Clears current packages information.
        /// </summary>
        public void Clear()
        {
            packagesInformation?.Clear();
            packagesInformation = null;
        }

        /// <summary>
        /// Checks if package is present in project.
        /// </summary>
        /// <param name="id">Package id (name).
        /// Example: "com.tiltingpoint.goodlifeinstaller".
        /// </param>
        /// <returns>True if package is present.</returns>
        private bool IsPackagePresent(string id) =>
            GetPackageState(id)?.Present != null;

        private void InitPackagesCollection(IEnumerable<string> packagesCollection)
        {
            packagesInformation = new Dictionary<string, PackageState>(10);

            if (packagesCollection == null)
            {
                return;
            }

            foreach (var item in packagesCollection)
            {
                packagesInformation.Add(item, new PackageState());
            }
        }

        private void FetchPackagesInfo(IEnumerable<string> ids)
        {
            foreach (var item in ids)
            {
                FetchPackageInfo(item);
            }
        }

        private void FetchPackageInfo(string id)
        {
            var state = GetPackageState(id);
            if (state != null)
            {
                state.SearchRequest = Client.Search(id);
            }
        }

        private void UpdateListResults()
        {
            if (listRequest == null || !listRequest.IsCompleted)
            {
                return;
            }

            if (listRequest.Status == StatusCode.Failure)
            {
                listRequest = null;
                return;
            }

            foreach (var item in packagesInformation)
            {
                var state = GetPackageState(item.Key);
                if (state == null)
                {
                    continue;
                }

                state.Present = listRequest.Result.FirstOrDefault(x => x.name == item.Key);
            }

            listRequest = null;
        }

        private PackageState GetPackageState(string id)
        {
            if (packagesInformation == null)
            {
                return null;
            }

            PackageState state;
            if (!packagesInformation.TryGetValue(id, out state))
            {
                return null;
            }

            if (state == null)
            {
                packagesInformation[id] = state = new PackageState();
            }

            return state;
        }

        private void RequestLocalListUpdate()
        {
            if (listRequest != null && !listRequest.IsCompleted)
            {
                return;
            }

            listRequest = Client.List(true);
        }
    }
}
