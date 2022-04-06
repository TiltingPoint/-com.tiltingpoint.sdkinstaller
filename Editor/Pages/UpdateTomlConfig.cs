// Copyright (c) Tilting Point Media LLC. All rights reserved.

using System.Text.RegularExpressions;
using TiltingPoint.Installer.Editor.Data;
using TiltingPoint.Installer.Editor.Tools;
using TiltingPoint.Installer.Editor.Tools.UI;
using UnityEditor;
using UnityEngine;
using AuthorisationTool = TiltingPoint.Installer.Editor.Tools.NpmAuthorisationTools;

// ReSharper disable Unity.PerformanceCriticalCodeInvocation
namespace TiltingPoint.Installer.Editor.Pages
{
    /// <summary>
    /// This page will help to update .upmconfig.toml file.
    /// </summary>
    internal class UpdateTomlConfig : AbstractPage, IInstallerPage
    {
        private const string HeaderText = "Add access data.";

        private const string ReadMoreAboutTomlPageUrl = "https://docs.unity.cn/Manual/upm-config.html#upmconfigUser";
        private const string ReadMoreAboutTomlPageUrlText = "Read more about .upmconfig.toml.";

        private const string ShortDescriptionText =
            "To let Unity access to our packages UPM must be configured.\n" +
            "To do that installer will do authorisation on our registy and add authorisation data to Unity Package Manager config (.upmconfig.toml).";

        private const string TomlDescriptionHeader = "What is '.upmconfig.toml' file?";

        private const string TomlDescriptionText =
            "Unity stores your token information for each scoped registry that requires " +
            "authentication in the '.upmconfig.toml' user configuration file using the npmAuth " +
            "configuration schema. Once you save this information to the configuration file, " +
            "Package Manager will provide your authentication information on every request made " +
            "to each registry in the file.";

        private const string WhatIsAuthorisationProcessHeader = "What is authorisation procces?";

        private const string WhatIsAuthorisationProcessText =
            "Basicaly it is sequence of messages with our registry. " +
            "Your authorisation data will be checked and access token will be created." +
            "This token will be used by UPM (Unity Package Manager) to access registry.";


        private const string UserNameRegexPatern = "^[\\(\\)\\-!'*~\\._a-z0-9]+$";
        private const string UserPasswordRegexPatern = "^[_a-zA-Z0-9]{6,}$";

        private const string UpdateTomlButtonText = "Update .upmconfig.toml";
        private const string GetAuthorisationButtonText = "Get Authorisation data";

        private bool isUserNameValid;
        private bool isUserPasswordValid;
        private bool isUserTokenValid;

        private Regex userNameRegex;
        private Regex userPasswordRegex;

        private string userName = string.Empty;
        private string userPassword = string.Empty;
        private string userToken = string.Empty;
        private string authorisationMessages = $"Enter your data and press '{GetAuthorisationButtonText}' button.";

        private bool showTomlDescription;
        private bool showAuthorisationDescription;
        private bool isRegistryAdded;

        /// <inheritdoc cref="IInstallerPage"/>
        public bool IsCompleted => isUserNameValid && isUserPasswordValid && isRegistryAdded;

        /// <inheritdoc cref="IInstallerPage"/>
        public void BeforeOpenPage(InstallationConfig config)
        {
            if (isUserPasswordValid && isUserPasswordValid && isUserTokenValid)
            {
                isRegistryAdded =
                    TomlTools
                        .IsRegistriesPresent(config.TiltingPointRegistry.RegistryUrl, userToken);
            }

            isUserNameValid = CheckUserName(userName);
            isUserPasswordValid = CheckUserPassword(userPassword);
        }

        /// <inheritdoc cref="IInstallerPage"/>
        public void LeavePage(InstallationConfig config)
        {
            // empty
        }

        /// <inheritdoc cref="IInstallerPage"/>
        public void DrawGui(InstallationConfig config)
        {
            ShowHeader(HeaderText);

            EditorGUILayout.Space(10);
            ShowText(ShortDescriptionText);

            GUILayout.Space(10);
            ShowAuthorisationInfo();
            ShowTomlFileInfo();

            UiGroups.BeginBoxGroup();
            ShowUserNameField();
            ShowUserPasswordField();
            GUILayout.Space(4);
            ShowAuthoriseButton(config.TiltingPointRegistry.RegistryUrl);

            UiGroups.EndBoxGroup();

            ShowAuthorisationMessage();

            UiGroups.BeginBoxGroup();
            ShowUserTokenField();
            GUILayout.Space(4);
            ShowUpdateTomlButton(config.TiltingPointRegistry.RegistryUrl, config.TiltingPointRegistry.Scopes[0]);

            UiGroups.EndBoxGroup();
        }

        private void ShowTomlFileInfo()
        {
            showTomlDescription = UiGroups.BeginFoldoutBoxGroup(showTomlDescription, TomlDescriptionHeader);
            if (showTomlDescription)
            {
                GUILayout.Space(10);
                ShowText(TomlDescriptionText);
                UiLinks.ShowUrlLabel(ReadMoreAboutTomlPageUrlText, ReadMoreAboutTomlPageUrl);
            }

            UiGroups.EndFoldoutBoxGroup();
        }

        private void ShowAuthorisationInfo()
        {
            showAuthorisationDescription =
                UiGroups.BeginFoldoutBoxGroup(showAuthorisationDescription, WhatIsAuthorisationProcessHeader);
            if (showAuthorisationDescription)
            {
                GUILayout.Space(10);
                ShowText(WhatIsAuthorisationProcessText);
            }

            UiGroups.EndFoldoutBoxGroup();
        }

        private void ShowAuthorisationMessage()
        {
            UiGroups.BeginFoldoutBoxGroup(true, "Info:");
            GUILayout.Space(10);
            ShowText(authorisationMessages);
            UiGroups.EndFoldoutBoxGroup();
        }

        private void ShowAuthoriseButton(string registry)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(!isUserPasswordValid || !isUserPasswordValid);

            if (GUILayout.Button(GetAuthorisationButtonText, GUILayout.Width(160), GUILayout.Height(40))
                && isUserPasswordValid
                && isUserPasswordValid)
            {
                userToken = null;
                authorisationMessages = null;
                AuthorisationTool.GetToken(userName, userPassword, registry,
                                           OnAuthorized,
                                           OnAuthorisationError,
                                           OnInfoUpdate);
            }

            GUILayout.FlexibleSpace();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        private void OnInfoUpdate(string message)
        {
            authorisationMessages = message;
        }

        private void OnAuthorized(string token)
        {
            userToken = token;
            isUserTokenValid = !string.IsNullOrEmpty(userToken) && userToken.Length > 10;
            if (isUserTokenValid)
            {
                authorisationMessages = $"Done. Press '{UpdateTomlButtonText}' button.";
            }
        }

        private void OnAuthorisationError(string error)
        {
            authorisationMessages = error;
            isUserTokenValid = false;
        }

        private void ShowUpdateTomlButton(string registry, string scope)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginDisabledGroup(!isUserPasswordValid || !isUserPasswordValid);

            if (GUILayout.Button(UpdateTomlButtonText, GUILayout.Width(160), GUILayout.Height(40))
                && isUserPasswordValid
                && isUserPasswordValid)
            {
                var (isSuccess, error) = TomlTools.AddRegistries(registry, userToken);
                isRegistryAdded = isSuccess;
                if (isSuccess)
                {
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError(error);
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        private void ShowUserNameField()
        {
            var input = ShowInputField("Enter your User Name: ", userName, isUserNameValid);
            if (input == userName)
            {
                return;
            }

            userName = input;
            isUserNameValid = CheckUserName(userName);
        }

        private void ShowUserPasswordField()
        {
            var input = ShowInputField("Enter your Password: ", userPassword, isUserPasswordValid);
            if (input == userPassword)
            {
                return;
            }

            userPassword = input;
            isUserPasswordValid = CheckUserPassword(input);
        }

        private void ShowUserTokenField()
        {
            GUILayout.Label("Access Token:");
            var defaultColor = GUI.backgroundColor;
            GUI.backgroundColor = isUserTokenValid ? Color.green : defaultColor;
            GUILayout.TextArea(userToken);
            GUI.backgroundColor = defaultColor;
        }

        private string ShowInputField(string label, string value, bool isValid)
        {
            GUILayout.Label(label);
            var defaultColor = GUI.backgroundColor;
            GUI.backgroundColor = isValid ? Color.green : Color.red;
            var input = GUILayout.TextField(value);
            GUI.backgroundColor = defaultColor;
            return input;
        }

        private bool CheckUserPassword(string passwordString)
        {
            if (string.IsNullOrEmpty(passwordString))
            {
                return false;
            }

            userPasswordRegex = userPasswordRegex ?? new Regex(UserPasswordRegexPatern);
            return userPasswordRegex.IsMatch(passwordString);
        }

        private bool CheckUserName(string userNameString)
        {
            if (string.IsNullOrEmpty(userNameString))
            {
                return false;
            }

            userNameRegex = userNameRegex ?? new Regex(UserNameRegexPatern);
            return userNameRegex.IsMatch(userNameString);
        }
    }
}
