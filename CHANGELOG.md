# tpsdk-unity-installer

##Connecting to Tilting Point’s private package registry
**TPSDK 4.+** utilizes Unity Package Manager for distribution. 
The following instructions require a minimum Unity version of **2019.3.4f1**.

You can find Unity's instructions on how to get your computer ready to go for registries [here](https://forum.unity.com/threads/npm-registry-authentication.836308/) or follow our tailored instructions below:

1. Ensure that you have been invited to access the private packages.
   - Send your GitHub user names or email addresses to your Tilting Point Tech contact in order to be invited.
   - Accept the invitation.
2. Generate a GitHub Personal Access Token.
3. Locate or create the UPM config file
   ```
   Windows: %USERPROFILE%/.upmconfig.toml (Usually %SystemDrive%\Users\<your username>/.upmconfig.toml)
   Windows (System user) : %ALLUSERSPROFILE%Unity/config/ServiceAccounts/.upmconfig.toml
   MacOS and Linux: ~/.upmconfig.toml (usually /Users/<your username>/.upmconfig.toml)
   ```
4. Add the following to that .upmconfig.toml file
   ```
   [npmAuth."https://npm.pkg.github.com/@TiltingPoint"]
   token = "{GITHUB_PERSONAL_ACCESS_TOKEN}"
   email = "{GITHUB_EMAIL_ADDRESS}"
   alwaysAuth = true
   ```

##Adding Packages
You can always find the latest versions of our packages in our GitHub Registry.

1. Add our registry to the scopedRegistries of your Unity project's manifest.json
   ```
   "scopedRegistries": [
      {
        "name": "Tilting Point Registry",
        "url": "https://npm.pkg.github.com/@TiltingPoint",
        "scopes": [
          "com.tiltingpoint"
        ]
      }
    ]
   ```
2. Add the packages requested by your Tilting Point Tech contact. For example:
   ```
   "dependencies" : {
     "com.tiltingpoint.amplitude": "4.0.0",
     ...
   ```
3. Unity will automatically import packages and dependencies added to the manifest.

##Other Requirements
The **TPSDK** utilizes several libraries on Android which require careful dependency management. 
Unfortunately Unity has no built in tool for handling this however the standard has been set via a 
Google plugin called the **External Dependency Manager** for Unity.
You can add this plugin to your project also in your manifest.json with the following settings:
   ```
   "dependencies" : {
     "com.google.external-dependency-manager": "1.2.157",
   },
   "scopedRegistries": [
     {
       "name": "Game Package Registry by Google",
       "url": "https://unityregistry-pa.googleapis.com",
       "scopes": [
         "com.google"
       ]
     }
   ]
   ```

##Unity Errors

   - Due to GitHub Packages limitations Unity may show error with next text:
   ```
   Cannot perform upm operation: Request [GET https://npm.pkg.github.com/@TiltingPoint/-/v1/search?text=com.tiltingpoint&from=0&size=250] failed with status code [405] [NotFound]Error searching for packages.
   [PackageManager] Error Request [GET https://npm.pkg.github.com/@TiltingPoint/-/v1/search?text=com.tiltingpoint&from=0&size=250] failed with status code [405]
   ```
   This errors will not have influence on your project, just ignore them. We already working on solution.
