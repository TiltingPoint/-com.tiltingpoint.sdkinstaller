# TP SDK Installer

Installer will help you to get access to Tilting Point packages registry and configure your project.

### Setup
1) Add `com.tiltingpoint.sdkinstaller` package to your project

    #### With Unity Package manager (Unity 2019+):

    * open `Main Menu -> Windows -> PackageManager`;
    * press `+` (top left corner);
    * select `Add package from git URL`;
    * past `https://github.com/ValentynKhylchukTiltingPoint/com.tiltingpoint.sdkinstaller.test1.git`;
    * done.

    #### Manually to manifest.json:
    Open `<project path>/Packages/manifest.json` and add packages to dependencies scope.
    
        "dependencies": [
          "com.tiltingpoint.sdkinstaller": "https://github.com/TiltingPoint/com.tiltingpoint.sdkinstaller.git",
        ]

### Use
Open installer window `Main Menu -> TiltingPoint -> SDK Installer -> Show SDK Installer`;
