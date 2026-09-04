// Copyright Epic Games, Inc. All Rights Reserved.

using System.Collections.Generic;
using System.Reflection;
using Tools.DotNETCommon;

namespace UnrealBuildTool
{
	class AppleWatchProjectSettings : IOSProjectSettings
	{
		private const string AppleWatchSettingsSection = "/Script/AppleWatchRuntimeSettings.AppleWatchRuntimeSettings";
		private string AppleWatchRuntimeVersion = "13.0";

		public override string RuntimeVersion
		{
			get { return AppleWatchRuntimeVersion; }
		}

		public override string RuntimeDevices
		{
			get { return "1"; }
		}

		public AppleWatchProjectSettings(FileReference ProjectFile, string Bundle)
			: base(ProjectFile, UnrealTargetPlatform.AppleWatch, Bundle)
		{
			ReadAppleWatchSettings(ProjectFile, Bundle);
		}

		private void ReadAppleWatchSettings(FileReference ProjectFile, string Bundle)
		{
			DirectoryReference ProjectDirectory = DirectoryReference.FromFile(ProjectFile);
			ConfigHierarchy Ini = ConfigCache.ReadHierarchy(ConfigHierarchyType.Engine, ProjectDirectory, UnrealTargetPlatform.AppleWatch);

			bool BoolValue;
			string StringValue;

			if (Ini.GetBool(AppleWatchSettingsSection, "bBuildAsFramework", out BoolValue)) SetReadOnlyField("bBuildAsFramework", BoolValue);
			if (Ini.GetBool(AppleWatchSettingsSection, "bGeneratedSYMFile", out BoolValue)) SetReadOnlyField("bGeneratedSYMFile", BoolValue);
			if (Ini.GetBool(AppleWatchSettingsSection, "bGeneratedSYMBundle", out BoolValue)) SetReadOnlyField("bGeneratedSYMBundle", BoolValue);
			if (Ini.GetBool(AppleWatchSettingsSection, "bGenerateCrashReportSymbols", out BoolValue)) SetReadOnlyField("bGenerateCrashReportSymbols", BoolValue);
			if (Ini.GetBool(AppleWatchSettingsSection, "bShipForBitcode", out BoolValue)) SetReadOnlyField("bShipForBitcode", BoolValue);
			if (Ini.GetBool(AppleWatchSettingsSection, "bEnableRemoteNotificationsSupport", out BoolValue)) SetReadOnlyField("bNotificationsEnabled", BoolValue);
			if (Ini.GetBool(AppleWatchSettingsSection, "bEnableBackgroundFetch", out BoolValue)) SetReadOnlyField("bBackgroundFetchEnabled", BoolValue);
			if (Ini.GetBool(AppleWatchSettingsSection, "bSupportsFileSharing", out BoolValue)) SetReadOnlyField("bFileSharingEnabled", BoolValue);
			if (Ini.GetBool(AppleWatchSettingsSection, "bAutomaticSigning", out BoolValue)) SetReadOnlyField("bAutomaticSigning", BoolValue);
			if (Ini.GetBool(AppleWatchSettingsSection, "bDisableForceInline", out BoolValue)) SetReadOnlyField("bDisableForceInline", BoolValue);

			if (Ini.GetString(AppleWatchSettingsSection, "AdditionalShippingLinkerFlags", out StringValue)) SetReadOnlyField("AdditionalShippingLinkerFlags", StringValue);
			if (Ini.GetString(AppleWatchSettingsSection, "AdditionalLinkerFlags", out StringValue)) SetReadOnlyField("AdditionalLinkerFlags", StringValue);
			if (Ini.GetString(AppleWatchSettingsSection, "MobileProvision", out StringValue)) SetReadOnlyField("MobileProvision", StringValue);
			if (Ini.GetString(AppleWatchSettingsSection, "SigningCertificate", out StringValue)) SetReadOnlyField("SigningCertificate", StringValue);
			if (Ini.GetString(AppleWatchSettingsSection, "TeamID", out StringValue)) SetReadOnlyField("TeamID", StringValue);
			if (Ini.GetString(AppleWatchSettingsSection, "BundleIdentifier", out StringValue) && !string.IsNullOrEmpty(StringValue))
			{
				SetReadOnlyField("BundleIdentifier", StringValue.Replace("[PROJECT_NAME]", ((ProjectFile != null) ? ProjectFile.GetFileNameWithoutAnyExtensions() : "UE4Game")).Replace("_", ""));
			}
			if ((ProjectFile == null || string.IsNullOrEmpty(ProjectFile.FullName)) && !string.IsNullOrEmpty(Bundle))
			{
				SetReadOnlyField("BundleIdentifier", Bundle);
			}

			if (Ini.GetString(AppleWatchSettingsSection, "MinimumWatchOSVersion", out StringValue))
			{
				AppleWatchRuntimeVersion = ParseWatchOSVersion(StringValue);
			}
		}

		private void SetReadOnlyField(string FieldName, object Value)
		{
			FieldInfo Field = typeof(IOSProjectSettings).GetField(FieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (Field != null)
			{
				Field.SetValue(this, Value);
			}
		}

		private static string ParseWatchOSVersion(string Value)
		{
			if (Value.StartsWith("WatchOS_"))
			{
				return Value.Substring("WatchOS_".Length) + ".0";
			}
			return string.IsNullOrEmpty(Value) ? "13.0" : Value;
		}
	}

	class AppleWatchProvisioningData : IOSProvisioningData
	{
		public AppleWatchProvisioningData(AppleWatchProjectSettings ProjectSettings, bool bForDistribution)
			: base(ProjectSettings, true, bForDistribution)
		{
		}
	}

	class AppleWatchPlatform : IOSPlatform
	{
		public AppleWatchPlatform(IOSPlatformSDK InSDK)
			: base(InSDK, UnrealTargetPlatform.AppleWatch)
		{
		}

		public static string AppleWatchArchitecture = "";

		public override string GetDefaultArchitecture(FileReference ProjectFile)
		{
			return AppleWatchArchitecture;
		}

		public override void ValidateTarget(TargetRules Target)
		{
			base.ValidateTarget(Target);

			Target.bCompileAgainstApplicationCore = false;
			Target.bCompileAgainstCoreUObject = true;
			Target.bCompileAgainstEngine = Target.bCompileAgainstEngine && Target.Type != TargetType.Program;

			if (Target.GlobalDefinitions.Contains("HAS_METAL=0"))
			{
				Target.GlobalDefinitions.Remove("HAS_METAL=0");
			}
			if (!Target.GlobalDefinitions.Contains("HAS_METAL=1"))
			{
				Target.GlobalDefinitions.Add("HAS_METAL=1");
			}
			if (!Target.ExtraModuleNames.Contains("MetalRHI"))
			{
				Target.ExtraModuleNames.Add("MetalRHI");
			}
		}

		public new AppleWatchProjectSettings ReadProjectSettings(FileReference ProjectFile, string Bundle = "")
		{
			return (AppleWatchProjectSettings)base.ReadProjectSettings(ProjectFile, Bundle);
		}

		protected override IOSProjectSettings CreateProjectSettings(FileReference ProjectFile, string Bundle)
		{
			return new AppleWatchProjectSettings(ProjectFile, Bundle);
		}

		public AppleWatchProvisioningData ReadProvisioningData(AppleWatchProjectSettings ProjectSettings, bool bForDistribution = false)
		{
			return (AppleWatchProvisioningData)base.ReadProvisioningData(ProjectSettings, bForDistribution);
		}

		protected override IOSProvisioningData CreateProvisioningData(IOSProjectSettings ProjectSettings, bool bForDistribution)
		{
			return new AppleWatchProvisioningData((AppleWatchProjectSettings)ProjectSettings, bForDistribution);
		}

		public override void ModifyModuleRulesForOtherPlatform(string ModuleName, ModuleRules Rules, ReadOnlyTargetRules Target)
		{
			base.ModifyModuleRulesForOtherPlatform(ModuleName, Rules, Target);

			if (!UEBuildPlatform.IsPlatformAvailable(Platform))
			{
				return;
			}

			if ((Target.Platform == UnrealTargetPlatform.Win32) || (Target.Platform == UnrealTargetPlatform.Win64) || (Target.Platform == UnrealTargetPlatform.Mac))
			{
				if (Target.bForceBuildTargetPlatforms)
				{
					Rules.DynamicallyLoadedModuleNames.Add("IOSTargetPlatform");
				}
				if (Target.Type == TargetType.Editor && ModuleName == "UnrealEd")
				{
					Rules.DynamicallyLoadedModuleNames.Add("AppleWatchPlatformEditor");
				}
			}
		}

		public override void SetUpEnvironment(ReadOnlyTargetRules Target, CppCompileEnvironment CompileEnvironment, LinkEnvironment LinkEnvironment)
		{
			base.SetUpEnvironment(Target, CompileEnvironment, LinkEnvironment);

			CompileEnvironment.Definitions.Add("PLATFORM_APPLEWATCH=1");
			CompileEnvironment.Definitions.Add("PLATFORM_WATCHOS=1");
			CompileEnvironment.Definitions.Add("OVERRIDE_PLATFORM_HEADER_NAME=IOS");
			CompileEnvironment.Definitions.Add("MINIMUM_UE4_COMPILED_WATCHOS_VERSION=130000");
		}

		public override UEToolChain CreateToolChain(ReadOnlyTargetRules Target)
		{
			AppleWatchProjectSettings ProjectSettings = ((AppleWatchPlatform)UEBuildPlatform.GetBuildPlatform(UnrealTargetPlatform.AppleWatch)).ReadProjectSettings(Target.ProjectFile);
			return new AppleWatchToolChain(Target, ProjectSettings);
		}

		public override void Deploy(TargetReceipt Receipt)
		{
			new UEDeployAppleWatch().PrepTargetForDeployment(Receipt);
		}
	}

	class AppleWatchPlatformFactory : UEBuildPlatformFactory
	{
		public override UnrealTargetPlatform TargetPlatform
		{
			get { return UnrealTargetPlatform.AppleWatch; }
		}

		public override void RegisterBuildPlatforms()
		{
			IOSPlatformSDK SDK = new IOSPlatformSDK();
			SDK.ManageAndValidateSDK();

			UEBuildPlatform.RegisterBuildPlatform(new AppleWatchPlatform(SDK));
			UEBuildPlatform.RegisterPlatformWithGroup(UnrealTargetPlatform.AppleWatch, UnrealPlatformGroup.Apple);
			UEBuildPlatform.RegisterPlatformWithGroup(UnrealTargetPlatform.AppleWatch, UnrealPlatformGroup.IOS);
		}
	}
}
