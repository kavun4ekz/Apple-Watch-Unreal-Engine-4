// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;

public class AppleWatchRuntimeSettings : ModuleRules
{
	public AppleWatchRuntimeSettings(ReadOnlyTargetRules Target) : base(Target)
	{
		BinariesSubFolder = "AppleWatch";

		PrivateDependencyModuleNames.AddRange(
			new string[]
			{
				"Core",
				"CoreUObject",
				"Engine"
			}
		);
	}
}
