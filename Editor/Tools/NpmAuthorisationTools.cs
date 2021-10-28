using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace TiltingPoint.Installer.Editor.Tools
{
    internal static class NpmAuthorisationTools
    {
        [Serializable]
        private class BaseRequestBody
        {
            public string _id;
            public string name;
            public string password;
            public string type;
            public string[] roles;
            public string date;
            public string ok;

            public string ToJson() => JsonUtility.ToJson(this);
        }

        [Serializable]
        private class ResponseBody
        {
            public string ok;
            public string token;
        }

        private static string currentUserName;
        private static string currentUserPass;
        private static string currentRegistryUrl;

        private static Action<string> onSuccessCallback;
        private static Action<string> onErrorCallback;
        private static Action<string> onInfoCallback;

        private static UnityWebRequest currentRequest;
        private static int retryCount = 0;

        /// <summary>
        /// Fetches registry user access token.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="userPassword">User password.</param>
        /// <param name="registryUrl">Registry URL.</param>
        /// <param name="onSuccess">On success callback.</param>
        /// <param name="onError">On error callback.</param>
        public static void GetToken(string userName, string userPassword, string registryUrl, Action<string> onSuccess,
                                    Action<string> onError, Action<string> OnInfoMessage)
        {
            if (currentRequest != null)
            {
                onErrorCallback?.Invoke("Request is in process. Pls, wait...");
                return;
            }

            onSuccessCallback = onSuccess;
            onErrorCallback = onError;
            onInfoCallback = OnInfoMessage;

            if (string.IsNullOrEmpty(userName))
            {
                OnError("User name in empty.");
                return;
            }

            if (string.IsNullOrEmpty(userPassword))
            {
                OnError("Password in empty.");
                return;
            }

            if (string.IsNullOrEmpty(registryUrl))
            {
                OnError("Url in empty.");
                return;
            }

            currentRegistryUrl = $"{registryUrl}/-/user/org.couchdb.user:{currentUserName}".Replace("//-", "/-");

            currentUserName = userName;
            currentUserPass = userPassword;

            retryCount = 0;

            Step1();
        }

        private static void Step1()
        {
            onInfoCallback?.Invoke("Step 1/3. First request...");

            var uri = new Uri(currentRegistryUrl);
            var body = new BaseRequestBody()
                       {
                           _id = $"org.couchdb.user:{currentUserName}",
                           name = currentUserName,
                           password = currentUserPass,
                           type = "user",
                           roles = new string[0],
                           date = DateTime.UtcNow.ToString("s") + "Z",
                       }.ToJson();

            currentRequest = UnityWebRequest.Put(uri, body);
            SetRequestHeaders().SendWebRequest().completed += OnResponse;
        }

        private static void Step2()
        {
            onInfoCallback?.Invoke("Step 2/3. Info request...");

            var uri = new Uri($"{currentRegistryUrl}?write=true");
            currentRequest = UnityWebRequest.Get(uri);
            SetRequestHeaders().SendWebRequest().completed += OnResponse;
        }

        private static void Step3()
        {
            onInfoCallback?.Invoke("Step 3/3. Authorisation request...");

            var uri =
                new Uri($"{currentRegistryUrl}/-rev/undefined");
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{currentUserName}:{currentUserPass}"));
            var body = new BaseRequestBody()
                       {
                           _id = $"org.couchdb.user:{currentUserName}",
                           name = currentUserName,
                           password = currentUserPass,
                           type = "user",
                           roles = new string[0],
                           date = DateTime.UtcNow.ToString("s") + "Z",
                           ok = $"you are authenticated as '{currentUserName}'",
                       }.ToJson();

            currentRequest = UnityWebRequest.Put(uri, body);
            SetRequestHeaders(credentials).SendWebRequest().completed += OnResponse;
        }

        private static void OnResponse(AsyncOperation obj)
        {
            switch (currentRequest.responseCode)
            {
                // expected as first response.
                case 409 when currentRequest.downloadHandler.text.Contains("user registration disabled"):
                {
                    if (retryCount > 1)
                    {
                        OnError("Can not find user with this User Name and Password.");
                        return;
                    }

                    retryCount++;

                    Step2();
                    return;
                }

                // expected as second response.
                case 200 when currentRequest.downloadHandler.text.Contains("you are authenticated as"):
                {
                    Step3();
                    return;
                }

                // expected as final response.
                case 201 when currentRequest.downloadHandler.text.Contains("you are authenticated as"):
                {
                    var response = TryParse<ResponseBody>(currentRequest.downloadHandler.text);
                    if (response == null || string.IsNullOrEmpty(response.token))
                    {
                        OnError($"Error while try to parse response \"{currentRequest.downloadHandler.text}\"");
                        return;
                    }

                    currentRequest?.Dispose();
                    currentRequest = null;

                    Debug.Log("Token: " + response.token);
                    onInfoCallback?.Invoke("Done. Authorisation: successful.");
                    onSuccessCallback?.Invoke(response.token);
                    return;
                }
            }

            var message = $"Response: {currentRequest.responseCode}. Message: {currentRequest.error ?? "empty"}";
            OnError($"Unhandled response: {message}");
        }

        private static void OnError(string message = null, bool cleanAll = true)
        {
            message = message ?? $"Error: {currentRequest.error} Message: {currentRequest.downloadHandler.text}";
            var callback = onErrorCallback;
            if (cleanAll)
            {
                CleanAll();
            }

            Debug.LogError(message);
            callback?.Invoke(message);
        }

        private static UnityWebRequest SetRequestHeaders(string authorisationValue = null)
        {
            if (currentRequest == null)
            {
                return null;
            }

            currentRequest.SetRequestHeader("npm-command", "adduser");
            currentRequest.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(authorisationValue))
            {
                currentRequest.SetRequestHeader("Authorization", $"Basic {authorisationValue}");
            }

            currentRequest.SetRequestHeader("Accept", "*/*");
            currentRequest.SetRequestHeader("Accept-Encoding", "gzip, deflate");
            return currentRequest;
        }

        private static void CleanAll()
        {
            currentUserName = null;
            currentUserPass = null;
            currentRegistryUrl = null;
            onSuccessCallback = null;
            onErrorCallback = null;
            currentRequest = null;
        }

        private static T TryParse<T>(string json, T defaultValue = default)
        {
            try
            {
                return JsonUtility.FromJson<T>(json);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
