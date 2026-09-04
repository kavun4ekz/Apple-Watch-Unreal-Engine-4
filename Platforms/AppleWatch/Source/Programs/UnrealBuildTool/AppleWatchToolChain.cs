// Copyright Epic Games, Inc. All Rights Reserved.

namespace UnrealBuildTool
{
	class AppleWatchToolChainSettings : IOSToolChainSettings
	{
		public AppleWatchToolChainSettings()
			: base("WatchOS", "WatchSimulator")
		{
		}
	}

	class AppleWatchToolChain : IOSToolChain
	{
		public AppleWatchToolChain(ReadOnlyTargetRules InTarget, AppleWatchProjectSettings InProjectSettings)
			: base(InTarget, InProjectSettings, () => new AppleWatchToolChainSettings())
		{
		}

		public override string GetXcodeMinVersionParam()
		{
			return "watchos-version-min";
		}

		public override string GetArchitectureArgument(CppConfiguration Configuration, string UBTArchitecture)
		{
			// Xcode 26 watchOS device and simulator builds use arm64 on current Apple hardware.
			return " -arch arm64";
		}
	}
}
