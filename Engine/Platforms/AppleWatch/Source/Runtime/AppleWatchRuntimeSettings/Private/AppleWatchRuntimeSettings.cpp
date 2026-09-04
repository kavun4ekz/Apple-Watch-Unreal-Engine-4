// Copyright Epic Games, Inc. All Rights Reserved.

#include "AppleWatchRuntimeSettings.h"
#include "HAL/FileManager.h"
#include "Misc/Paths.h"

UAppleWatchRuntimeSettings::UAppleWatchRuntimeSettings(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	BundleDisplayName = TEXT("UE4 Watch Game");
	BundleName = TEXT("MyUE4WatchGame");
	BundleIdentifier = TEXT("com.YourCompany.GameNameNoSpaces.watchkitapp");
	VersionInfo = TEXT("1.0.0");
	MinimumWatchOSVersion = EAppleWatchVersion::WatchOS_13;
	bUseRSync = true;
	bBuildAsFramework = false;
	bGeneratedSYMFile = true;
	bGeneratedSYMBundle = true;
	bGenerateCrashReportSymbols = false;
	bShipForBitcode = false;
	bAutomaticSigning = false;
	bSupportsMetal = true;
	bSupportsMetalMRT = false;
	bEnableRemoteNotificationsSupport = false;
	bEnableBackgroundFetch = false;
	bSupportsFileSharing = false;
	AdditionalPlistData = TEXT("");
	AdditionalLinkerFlags = TEXT("");
	AdditionalShippingLinkerFlags = TEXT("");
	bDisableForceInline = false;
	bDisableHTTPS = false;
}

#if WITH_EDITOR
void UAppleWatchRuntimeSettings::PostInitProperties()
{
	Super::PostInitProperties();

	if (!RemoteServerName.IsEmpty() && !RSyncUsername.IsEmpty())
	{
		SSHPrivateKeyLocation = TEXT("");

		const FString DefaultKeyFilename = TEXT("RemoteToolChainPrivate.key");
		const FString RelativeFilePathLocation = FPaths::Combine(TEXT("SSHKeys"), *RemoteServerName, *RSyncUsername, *DefaultKeyFilename);
		const FString AppDataPath = FPlatformMisc::GetEnvironmentVariable(TEXT("APPDATA"));

		TArray<FString> PossibleKeyLocations;
		PossibleKeyLocations.Add(FPaths::Combine(*FPaths::ProjectDir(), TEXT("Restricted"), TEXT("NotForLicensees"), TEXT("Build"), *RelativeFilePathLocation));
		PossibleKeyLocations.Add(FPaths::Combine(*FPaths::ProjectDir(), TEXT("Restricted"), TEXT("NoRedist"), TEXT("Build"), *RelativeFilePathLocation));
		PossibleKeyLocations.Add(FPaths::Combine(*FPaths::ProjectDir(), TEXT("Build"), *RelativeFilePathLocation));
		PossibleKeyLocations.Add(FPaths::Combine(*FPaths::EngineDir(), TEXT("Restricted"), TEXT("NotForLicensees"), TEXT("Build"), TEXT("NotForLicensees"), *RelativeFilePathLocation));
		PossibleKeyLocations.Add(FPaths::Combine(*FPaths::EngineDir(), TEXT("Restricted"), TEXT("NoRedist"), TEXT("Build"), *RelativeFilePathLocation));
		PossibleKeyLocations.Add(FPaths::Combine(*FPaths::EngineDir(), TEXT("Build"), *RelativeFilePathLocation));
		PossibleKeyLocations.Add(FPaths::Combine(*AppDataPath, TEXT("Unreal Engine"), TEXT("UnrealBuildTool"), *RelativeFilePathLocation));

		for (const FString& NextLocation : PossibleKeyLocations)
		{
			if (IFileManager::Get().FileSize(*NextLocation) > 0)
			{
				SSHPrivateKeyLocation = NextLocation;
				break;
			}
		}
	}
}
#endif
