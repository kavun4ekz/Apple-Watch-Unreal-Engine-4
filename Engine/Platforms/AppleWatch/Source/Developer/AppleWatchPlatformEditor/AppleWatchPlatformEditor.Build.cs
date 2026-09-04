// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;

public class AppleWatchPlatformEditor : ModuleRules
{
	public AppleWatchPlatformEditor(ReadOnlyTargetRules Target) : base(Target)
	{
		BinariesSubFolder = "AppleWatch";

		PrivateDependencyModuleNames.AddRange(
			new string[]
			{
				"Core",
				"CoreUObject",
				"Engine",
				"Slate",
				"SlateCore",
				"EditorStyle",
				"PropertyEditor",
				"AppleWatchRuntimeSettings",
				"MaterialShaderQualitySettings",
				"RenderCore"
			}
		);

		PrivateIncludePathModuleNames.Add("Settings");
	}
}
