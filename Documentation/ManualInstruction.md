# Manual installation


## 1. Add Tilting Point registry to project settings.


#### From Unity editor (Unity 2019+):
Go to `Main Menu -> Edit -> Project Settings -> Package Manager` and add registry.

* Name: `Tilting Point Registry`
* URL: `http://registry.tiltingpoint.io`
* Scopes: `com.tiltingpoint`


#### Manually to manifest.json:
Open `<project path>/Packages/manifest.json` and add

    "scopedRegistries": [
      {
        "name": "Tilting Point Registry",
        "url": "http://registry.tiltingpoint.io",
        "scopes": [
          "com.tiltingpoint"
        ]
      }
    ]

## 2. Add packages
#### From Unity editor (Unity 2019+):
Go to `Main Menu -> Windows -> Package Manager -> (top left corner) -> My Registries` and add packages you want to.

#### Manually to manifest.json:
Open `<project path>/Packages/manifest.json` and add packages to dependencies scope.

    "dependencies": [
      "com.tiltingpoint.core": "file:../../tpsdk-unity-core",
    ]

## 3. Add External Dependencies Manager
You can find it here: [External Dependencies Manager](https://developers.google.com/unity/archive#external_dependency_manager_for_unity)