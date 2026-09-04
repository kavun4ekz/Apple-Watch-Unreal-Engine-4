UE4 AppleWatch platform package

Copy this folder's Engine tree over the root of a matching UE4 checkout.

Includes:
- Engine/Platforms/AppleWatch: AppleWatch platform extension, icons, configs, runtime settings, project settings editor module, UBT platform files.
- Engine/Source/Programs/UnrealBuildTool/ToolChain/RemoteMac.cs: adds AppleWatch-specific remote build settings lookup.
- Engine/Source/Programs/UnrealBuildTool/Modes/BuildMode.cs: passes target platform to RemoteMac.
- Engine/Source/Programs/UnrealBuildTool/Modes/CleanMode.cs: passes target platform to RemoteMac.

After copying:
1. Run Engine/Build/BatchFiles/GenerateProjectFiles.bat -NoIntelliSense
2. Build UnrealBuildTool.csproj or run a normal UE4Editor build.
3. In Project Settings > Platforms > Apple Watch, configure RemoteServerName, RSyncUsername, SSHPrivateKeyOverridePath, signing, bundle id, and team id.
4. For watchOS SDK 26.2, merge Engine/Platforms/AppleWatch/Build/AppleWatch/BuildConfiguration.RemoteMac.example.xml into Engine/Saved/UnrealBuildTool/BuildConfiguration.xml or the user-level UBT config.

Validated locally:
- UnrealBuildTool.csproj builds with 0 errors and 0 warnings.
- GenerateProjectFiles.bat -NoIntelliSense succeeds.
- AppleWatchRuntimeSettings and AppleWatchPlatformEditor modules build for UE4Editor Win64 Development.
- UnrealBuildTool.exe -Mode=ValidatePlatforms -Platforms=AppleWatch returns AppleWatch VALID.
