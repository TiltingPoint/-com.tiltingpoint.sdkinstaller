# Manual installation


## 1. Get access to Tilting Point registry.
To get access to the registry contact DevRel team. Response will contain username and password.


## 2. Install NPM
NPM is distributing as a part of Node.js. You can download latest version of Node.js with links:
* https://nodejs.org/en/download/
* https://nodejs.org/dist/latest/


## 3. Login into registry with NPM and get token

1) Open terminal/console and execute `npm login --registry "http://registry.tiltingpoint.io/"` and use username and password from step 1.
   

2) Get authorisation token. NPM will create `.npmrc` file with authorisation token. Copy authorisation token from the file.
   
    Line with token example: `//registry.tiltingpoint.io/:_authToken=<token>`

    File location:
    * Windows: `C:\Users\<your username>/.npmrc`;
    * Windows: (Global config): `C:\Users\%username%\AppData\Roaming\npm\etc\npmrc`;
    * MacOS: `/Users/<your username>/.npmrc`;
    * Linux: `/Users/<your username>/.npmrc`;
    * or use command to get file content `npm config edit`


## 4. Edit unity package manager config file

Unity uses `.upmconfig.toml` configuration file to authorize communication with registries.

1) Locate this file or create new one
    * Windows: `C:\Users\<your username>/.upmconfig.toml)`;
    * Windows: (System user) `C:\Users\%username%\Unity\config\ServiceAccounts\.upmconfig.toml`;
    * MacOS: `/Users/<your username>/.upmconfig.toml`;
    * Linux: `/Users/<your username>/.upmconfig.toml`;
    

2) Add to file authorisation information:
   
        [npmAuth."http://registry.tiltingpoint.io"]
        token = "<token>"
        alwaysAuth = true

    If file already contains record, use empty line to separate it.


3) Save file with `ASCII` encoding.


## 5. Add Tilting Point registry to project settings.


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

## 6. Add packages
#### From Unity editor (Unity 2019+):
Go to `Main Menu -> Windows -> Package Manager -> (top left corner) -> My Registries` and add packages you want to.

#### Manually to manifest.json:
Open `<project path>/Packages/manifest.json` and add packages to dependencies scope.

    "dependencies": [
      "com.tiltingpoint.core": "file:../../tpsdk-unity-core",
    ]