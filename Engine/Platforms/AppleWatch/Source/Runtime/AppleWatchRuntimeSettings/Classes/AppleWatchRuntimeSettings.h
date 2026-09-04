// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "UObject/Object.h"
#include "AppleWatchRuntimeSettings.generated.h"

UENUM()
enum class EAppleWatchVersion : uint8
{
	WatchOS_13 = 13 UMETA(DisplayName = "13.0"),
	WatchOS_14 = 14 UMETA(DisplayName = "14.0"),
	WatchOS_15 = 15 UMETA(DisplayName = "15.0"),
	WatchOS_16 = 16 UMETA(DisplayName = "16.0"),
	WatchOS_17 = 17 UMETA(DisplayName = "17.0"),
	WatchOS_18 = 18 UMETA(DisplayName = "18.0"),
	WatchOS_19 = 19 UMETA(DisplayName = "19.0")
};

UCLASS(config=Engine, defaultconfig, meta=(DisplayName="Apple Watch"))
class APPLEWATCHRUNTIMESETTINGS_API UAppleWatchRuntimeSettings : public UObject
{
public:
	GENERATED_UCLASS_BODY()

	UPROPERTY(GlobalConfig, EditAnywhere, Category = BundleInformation, meta = (DisplayName = "Bundle Display Name"))
	FString BundleDisplayName;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = BundleInformation, meta = (DisplayName = "Bundle Name"))
	FString BundleName;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = BundleInformation, meta = (DisplayName = "Bundle Identifier", ConfigHierarchyEditable))
	FString BundleIdentifier;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = BundleInformation, meta = (DisplayName = "Version"))
	FString VersionInfo;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = OSInfo, meta = (DisplayName = "Minimum watchOS Version"))
	EAppleWatchVersion MinimumWatchOSVersion;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Build, meta = (DisplayName = "Remote Mac"))
	FString RemoteServerName;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Build, meta = (DisplayName = "Use RSync for remote build"))
	bool bUseRSync;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Build, meta = (EditCondition = "bUseRSync", DisplayName = "Username on Remote Server"))
	FString RSyncUsername;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Build, meta = (DisplayName = "Remote Build Path Override"))
	FString RemoteServerOverrideBuildPath;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Build, meta = (EditCondition = "bUseRSync", DisplayName = "Override SSH Private Key"))
	FString SSHPrivateKeyOverridePath;

	UPROPERTY(VisibleAnywhere, Category = Build, meta = (DisplayName = "Found Existing SSH Private Key"))
	FString SSHPrivateKeyLocation;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Build, meta = (DisplayName = "Build project as a framework"))
	bool bBuildAsFramework;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Build, meta = (DisplayName = "Generate dSYM file"))
	bool bGeneratedSYMFile;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Build, meta = (DisplayName = "Generate dSYM bundle"))
	bool bGeneratedSYMBundle;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Build, meta = (DisplayName = "Generate .udebugsymbols file"))
	bool bGenerateCrashReportSymbols;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Build, meta = (DisplayName = "Support bitcode in Shipping"))
	bool bShipForBitcode;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Build)
	FString MobileProvision;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Build)
	FString SigningCertificate;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Build)
	bool bAutomaticSigning;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Build, meta = (DisplayName = "Apple Team ID", ConfigHierarchyEditable))
	FString TeamID;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Rendering, meta = (DisplayName = "Metal Renderer"))
	bool bSupportsMetal;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Rendering, meta = (DisplayName = "Metal MRT Renderer"))
	bool bSupportsMetalMRT;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Online)
	bool bEnableRemoteNotificationsSupport;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Online)
	bool bEnableBackgroundFetch;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = FileSystem, meta = (DisplayName = "Support File Sharing"))
	bool bSupportsFileSharing;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = ExtraData, meta = (DisplayName = "Additional Plist Data"))
	FString AdditionalPlistData;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Build, meta = (DisplayName = "Additional Non-Shipping Linker Flags", ConfigHierarchyEditable))
	FString AdditionalLinkerFlags;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Build, meta = (DisplayName = "Additional Shipping Linker Flags", ConfigHierarchyEditable))
	FString AdditionalShippingLinkerFlags;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Build, meta = (DisplayName = "Disable FORCEINLINE"))
	bool bDisableForceInline;

	UPROPERTY(GlobalConfig, EditAnywhere, Category = Online, meta = (DisplayName = "Allow web connections to non-HTTPS websites"))
	bool bDisableHTTPS;

#if WITH_EDITOR
	virtual void PostInitProperties() override;
#endif
};
