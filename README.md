# UE4 Apple Watch Platform Extension

This package adds an experimental Apple Watch / watchOS platform extension to an Unreal Engine 4 source checkout.

It provides:

- `AppleWatch` as an UnrealBuildTool target platform.
- A watchOS toolchain path based on UE4's existing Apple/iOS remote Mac build flow.
- A separate **Project Settings > Platforms > Apple Watch** settings page.
- Apple Watch-specific config files, icon assets, device profile, plist generation, and deploy hooks.
- Remote build support from Windows to macOS using Xcode and the watchOS SDK.

This is intended for source-built UE4 forks. It is not an official Epic Games or Apple platform integration.

## Requirements

Windows build machine:

- Source-built Unreal Engine 4 checkout.
- Visual Studio 2019 toolchain or the toolchain required by your UE4 branch.
- Remote build SSH access to a Mac.

Remote Mac:

- macOS with Xcode installed.
- Xcode 26.3 selected with `xcode-select`.
- watchOS SDK 26.2 installed, for example `WatchOS26.2.sdk`.
- Apple Developer account configured in Xcode.

Device testing:

- iPhone paired with an Apple Watch.
- Developer Mode enabled on the Apple Watch.
- The iPhone/Watch visible in Xcode's Devices and Simulators window.

## Installation

Copy this package's `Engine` folder over the root of your UE4 checkout.

Example target layout:

```text
YourUE4Root/
  Engine/
    Platforms/
      AppleWatch/
    Source/
      Programs/
        UnrealBuildTool/
          Modes/
            BuildMode.cs
            CleanMode.cs
          ToolChain/
            RemoteMac.cs
```

The `Engine/Platforms/AppleWatch` directory contains the platform extension itself. The three UnrealBuildTool files are included because Apple Watch remote build settings need to be read from the Apple Watch settings section instead of the iOS settings section.

After copying the files, regenerate project files:

```bat
Engine\Build\BatchFiles\GenerateProjectFiles.bat -NoIntelliSense
```

Then rebuild UnrealBuildTool or build the editor:

```bat
Engine\Build\BatchFiles\Build.bat UE4Editor Win64 Development
```

You can validate the platform registration with:

```bat
Engine\Binaries\DotNET\UnrealBuildTool.exe -Mode=ValidatePlatforms -Platforms=AppleWatch
```

Expected output:

```text
##PlatformValidate: AppleWatch VALID
```

## Configure Apple Watch Project Settings

Open your project in the editor and go to:

```text
Edit > Project Settings > Platforms > Apple Watch
```

Configure at least:

- Remote Mac host or IP.
- Remote Mac username.
- SSH private key override path.
- Bundle identifier.
- Apple Team ID.
- Automatic signing or manual signing certificate/provisioning profile.

The settings are stored in your project's `Config/DefaultEngine.ini` under:

```ini
[/Script/AppleWatchRuntimeSettings.AppleWatchRuntimeSettings]
RemoteServerName=192.168.1.50
RSyncUsername=macusername
SSHPrivateKeyOverridePath=C:/Path/To/RemoteToolChainPrivate.key
BundleIdentifier=com.yourcompany.yourgame.watchkitapp
BundleDisplayName=Your Game
BundleName=YourGameWatch
VersionInfo=1.0.0
MinimumWatchOSVersion=WatchOS_13
bUseRSync=True
bAutomaticSigning=True
TeamID=YOURTEAMID
```

## Configure watchOS SDK Version

This extension was prepared for a remote Mac with watchOS SDK 26.2.

Merge this template:

```text
Engine/Platforms/AppleWatch/Build/AppleWatch/BuildConfiguration.RemoteMac.example.xml
```

into:

```text
Engine/Saved/UnrealBuildTool/BuildConfiguration.xml
```

or into your user-level UnrealBuildTool config.

The required setting is:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Configuration xmlns="https://www.unrealengine.com/BuildConfiguration">
	<IOSToolChain>
		<IOSSDKVersion>26.2</IOSSDKVersion>
	</IOSToolChain>
</Configuration>
```

The XML section is still named `IOSToolChain` because the Apple Watch toolchain reuses UE4's existing iOS remote Apple toolchain infrastructure.

On the Mac, verify that Xcode can see the SDK:

```bash
sudo xcode-select -s /Applications/Xcode.app/Contents/Developer
xcodebuild -version
ls /Applications/Xcode.app/Contents/Developer/Platforms/WatchOS.platform/Developer/SDKs
```

You should see `WatchOS26.2.sdk`.

## Build A Project

Development build example:

```bat
Engine\Build\BatchFiles\RunUAT.bat BuildCookRun ^
-project="C:\Path\To\YourGame\YourGame.uproject" ^
-platform=AppleWatch ^
-clientconfig=Development ^
-cook ^
-build ^
-stage ^
-pak ^
-archive ^
-archivedirectory="C:\AppleWatchBuilds\YourGame"
```

Shipping build example:

```bat
Engine\Build\BatchFiles\RunUAT.bat BuildCookRun ^
-project="C:\Path\To\YourGame\YourGame.uproject" ^
-platform=AppleWatch ^
-clientconfig=Shipping ^
-cook ^
-build ^
-stage ^
-pak ^
-archive ^
-archivedirectory="C:\AppleWatchBuilds\YourGame" ^
-distribution
```

Check the archive directory and your project's staged build output for the generated `.ipa` or `.app` bundle.

## Install On Apple Watch

Apple Watch apps are installed through the paired iPhone using Xcode.

1. Pair your Apple Watch with an iPhone.
2. Connect the iPhone to the Mac.
3. Open Xcode.
4. Open `Window > Devices and Simulators`.
5. Select the connected iPhone.
6. Make sure the paired Apple Watch appears in Xcode.
7. Enable Developer Mode on the Apple Watch if Xcode asks for it.
8. Under `Installed Apps`, click `+`.
9. Select the generated `.ipa`.

If Xcode cannot see the Apple Watch, fix device pairing first. The build cannot be installed to the physical watch until Xcode sees the paired iPhone and watch.

## Troubleshooting

`Remote compiling requires a server name`

Make sure `RemoteServerName` is set under:

```ini
[/Script/AppleWatchRuntimeSettings.AppleWatchRuntimeSettings]
```

not only under the iOS settings section.

`Invalid SDK WatchOS26.2.sdk`

The selected Xcode does not have the requested SDK. Check the selected Xcode path and installed watchOS SDKs on the Mac.

`Permission denied publickey`

Regenerate or reinstall the UE4 remote build SSH key:

```bat
Engine\Build\BatchFiles\MakeAndInstallSSHKey.bat
```

`No signing certificate`

Open Xcode on the Mac, add your Apple ID, select your Apple Developer team, and make sure the iPhone/Watch are registered for development signing.

## Notes

This extension is a source-level enablement layer. Real production watchOS projects may still require additional work around app-extension structure, companion iPhone apps, provisioning profiles, entitlements, icons, and runtime feature pruning.

Use Development builds first. Move to Shipping/distribution only after remote build, signing, and device installation work reliably.
