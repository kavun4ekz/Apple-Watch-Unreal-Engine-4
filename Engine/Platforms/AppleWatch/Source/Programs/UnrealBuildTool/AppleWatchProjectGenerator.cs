// Copyright Epic Games, Inc. All Rights Reserved.

using System.Collections.Generic;
using Tools.DotNETCommon;

namespace UnrealBuildTool
{
	class AppleWatchProjectGenerator : IOSProjectGenerator
	{
		public AppleWatchProjectGenerator(CommandLineArguments Arguments)
			: base(Arguments)
		{
		}

		public override IEnumerable<UnrealTargetPlatform> GetPlatforms()
		{
			yield return UnrealTargetPlatform.AppleWatch;
		}
	}
}
