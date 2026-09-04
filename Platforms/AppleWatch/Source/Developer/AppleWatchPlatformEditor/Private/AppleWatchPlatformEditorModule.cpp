// Copyright Epic Games, Inc. All Rights Reserved.

#include "AppleWatchRuntimeSettings.h"
#include "ISettingsModule.h"
#include "ISettingsSection.h"
#include "MaterialShaderQualitySettings.h"
#include "Modules/ModuleManager.h"
#include "ShaderPlatformQualitySettings.h"

#define LOCTEXT_NAMESPACE "FAppleWatchPlatformEditorModule"

class FAppleWatchPlatformEditorModule : public IModuleInterface
{
public:
	virtual void StartupModule() override
	{
		if (ISettingsModule* SettingsModule = FModuleManager::GetModulePtr<ISettingsModule>("Settings"))
		{
			SettingsModule->RegisterSettings("Project", "Platforms", "AppleWatch",
				LOCTEXT("RuntimeSettingsName", "Apple Watch"),
				LOCTEXT("RuntimeSettingsDescription", "Settings and resources for the Apple Watch platform"),
				GetMutableDefault<UAppleWatchRuntimeSettings>()
			);

			static FName NAME_SF_METAL_APPLEWATCH(TEXT("SF_METAL_APPLEWATCH"));
			UShaderPlatformQualitySettings* AppleWatchMaterialQualitySettings = UMaterialShaderQualitySettings::Get()->GetShaderPlatformQualitySettings(NAME_SF_METAL_APPLEWATCH);
			SettingsModule->RegisterSettings("Project", "Platforms", "AppleWatchMetalQuality",
				LOCTEXT("AppleWatchMetalQualitySettingsName", "Apple Watch Material Quality"),
				LOCTEXT("AppleWatchMetalQualitySettingsDescription", "Settings for Apple Watch material quality"),
				AppleWatchMaterialQualitySettings
			);
		}
	}

	virtual void ShutdownModule() override
	{
		if (ISettingsModule* SettingsModule = FModuleManager::GetModulePtr<ISettingsModule>("Settings"))
		{
			SettingsModule->UnregisterSettings("Project", "Platforms", "AppleWatch");
			SettingsModule->UnregisterSettings("Project", "Platforms", "AppleWatchMetalQuality");
		}
	}
};

IMPLEMENT_MODULE(FAppleWatchPlatformEditorModule, AppleWatchPlatformEditor);

#undef LOCTEXT_NAMESPACE
