# AppleWatch platform extension

This platform extension adds the `AppleWatch` UBT target name and routes compilation through the existing UE4 iOS remote Mac toolchain flow.

The Windows host still needs the normal UE4 iOS remote build settings:

- `RemoteServerName`
- `RSyncUsername`
- `SSHPrivateKeyLocation`
- a macOS remote machine with Xcode 26.3 selected by `xcode-select`
- watchOS SDK 26.2 installed under the selected Xcode (`WatchOS26.2.sdk`)

Use `Build/AppleWatch/BuildConfiguration.RemoteMac.example.xml` as the UnrealBuildTool XML config template:

- copy it to `Engine/Saved/UnrealBuildTool/BuildConfiguration.xml` for this engine checkout, or
- merge its `IOSToolChain` section into your existing user-level `BuildConfiguration.xml`.

Apple Watch has its own project settings page under `Project Settings > Platforms > Apple Watch`.

The remote build host/user/key for Apple Watch come from the Apple Watch settings section, not the iOS settings section. For source-controlled defaults, put them in the project's `Config/DefaultEngine.ini`:

```ini
[/Script/AppleWatchRuntimeSettings.AppleWatchRuntimeSettings]
RemoteServerName=your-mac-host-or-ip
RSyncUsername=your-mac-user
SSHPrivateKeyOverridePath=C:/Path/To/RemoteToolChainPrivate.key
```

Example:

```powershell
Engine\Build\BatchFiles\RunUAT.bat BuildCookRun -project="C:\Path\Game.uproject" -platform=AppleWatch -clientconfig=Development -cook -build -stage -pak
```

This is a source-level platform enablement layer. Real production watchOS projects may still need project-specific app-extension structure, provisioning profiles, icons, and runtime feature pruning.
