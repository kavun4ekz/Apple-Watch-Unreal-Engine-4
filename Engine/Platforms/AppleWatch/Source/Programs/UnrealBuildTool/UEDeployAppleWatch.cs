// Copyright Epic Games, Inc. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Tools.DotNETCommon;

namespace UnrealBuildTool
{
	class UEDeployAppleWatch : UEDeployIOS
	{
		private const string AppleWatchSettingsSection = "/Script/AppleWatchRuntimeSettings.AppleWatchRuntimeSettings";

		protected override string GetTargetPlatformName()
		{
			return "AppleWatch";
		}

		public static bool GenerateAppleWatchPList(FileReference ProjectFile, UnrealTargetConfiguration Config, string ProjectDirectory, bool bIsUE4Game, string GameName, bool bIsClient, string ProjectName, string InEngineDir, string AppDirectory, UnrealPluginLanguage UPL, string BundleID)
		{
			string BuildDirectory = ProjectDirectory + "/Build/AppleWatch";
			string IntermediateDirectory = (bIsUE4Game ? InEngineDir : ProjectDirectory) + "/Intermediate/AppleWatch";
			string PListFile = IntermediateDirectory + "/" + GameName + "-Info.plist";
			VersionUtilities.BuildDirectory = BuildDirectory;
			VersionUtilities.GameName = GameName;

			string OldPListData = File.Exists(PListFile) ? File.ReadAllText(PListFile) : "";
			DirectoryReference DirRef = bIsUE4Game ? (!string.IsNullOrEmpty(UnrealBuildTool.GetRemoteIniPath()) ? new DirectoryReference(UnrealBuildTool.GetRemoteIniPath()) : null) : new DirectoryReference(ProjectDirectory);
			ConfigHierarchy Ini = ConfigCache.ReadHierarchy(ConfigHierarchyType.Engine, DirRef, UnrealTargetPlatform.AppleWatch);

			string BundleDisplayName;
			Ini.GetString(AppleWatchSettingsSection, "BundleDisplayName", out BundleDisplayName);

			string BundleIdentifier;
			Ini.GetString(AppleWatchSettingsSection, "BundleIdentifier", out BundleIdentifier);
			if (!string.IsNullOrEmpty(BundleID))
			{
				BundleIdentifier = BundleID;
			}

			string BundleName;
			Ini.GetString(AppleWatchSettingsSection, "BundleName", out BundleName);

			string BundleShortVersion;
			Ini.GetString(AppleWatchSettingsSection, "VersionInfo", out BundleShortVersion);

			string MinVersion = "13.0";
			Ini.GetString(AppleWatchSettingsSection, "MinimumWatchOSVersion", out MinVersion);
			if (MinVersion.StartsWith("WatchOS_"))
			{
				MinVersion = MinVersion.Substring("WatchOS_".Length) + ".0";
			}

			string ExtraData = "";
			Ini.GetString(AppleWatchSettingsSection, "AdditionalPlistData", out ExtraData);

			string BundleExecutable = bIsUE4Game ? (bIsClient ? "UE4Client" : "UE4Game") : (bIsClient ? GameName + "Client" : GameName);

			StringBuilder Text = new StringBuilder();
			Text.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			Text.AppendLine("<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");
			Text.AppendLine("<plist version=\"1.0\">");
			Text.AppendLine("<dict>");
			Text.AppendLine("\t<key>CFBundleDevelopmentRegion</key>");
			Text.AppendLine("\t<string>en</string>");
			Text.AppendLine("\t<key>CFBundleDisplayName</key>");
			Text.AppendLine(string.Format("\t<string>{0}</string>", EncodeBundleName(BundleDisplayName, ProjectName)));
			Text.AppendLine("\t<key>CFBundleExecutable</key>");
			Text.AppendLine(string.Format("\t<string>{0}</string>", BundleExecutable));
			Text.AppendLine("\t<key>CFBundleIdentifier</key>");
			Text.AppendLine(string.Format("\t<string>{0}</string>", BundleIdentifier.Replace("[PROJECT_NAME]", ProjectName).Replace("_", "")));
			Text.AppendLine("\t<key>CFBundleInfoDictionaryVersion</key>");
			Text.AppendLine("\t<string>6.0</string>");
			Text.AppendLine("\t<key>CFBundleName</key>");
			Text.AppendLine(string.Format("\t<string>{0}</string>", EncodeBundleName(BundleName, ProjectName)));
			Text.AppendLine("\t<key>CFBundlePackageType</key>");
			Text.AppendLine("\t<string>APPL</string>");
			Text.AppendLine("\t<key>CFBundleSignature</key>");
			Text.AppendLine("\t<string>????</string>");
			Text.AppendLine("\t<key>CFBundleVersion</key>");
			Text.AppendLine(string.Format("\t<string>{0}</string>", VersionUtilities.UpdateBundleVersion(OldPListData, InEngineDir)));
			Text.AppendLine("\t<key>CFBundleShortVersionString</key>");
			Text.AppendLine(string.Format("\t<string>{0}</string>", BundleShortVersion));
			Text.AppendLine("\t<key>LSRequiresIPhoneOS</key>");
			Text.AppendLine("\t<true/>");
			Text.AppendLine("\t<key>WKApplication</key>");
			Text.AppendLine("\t<true/>");
			Text.AppendLine("\t<key>WKRunsIndependentlyOfCompanionApp</key>");
			Text.AppendLine("\t<true/>");
			Text.AppendLine("\t<key>MinimumOSVersion</key>");
			Text.AppendLine(string.Format("\t<string>{0}</string>", MinVersion));
			Text.AppendLine("\t<key>UIRequiredDeviceCapabilities</key>");
			Text.AppendLine("\t<array>");
			Text.AppendLine("\t\t<string>arm64</string>");
			Text.AppendLine("\t\t<string>watch-companion</string>");
			Text.AppendLine("\t</array>");
			Text.Append(ExtraData);
			Text.AppendLine("</dict>");
			Text.AppendLine("</plist>");

			if (!Directory.Exists(IntermediateDirectory))
			{
				Directory.CreateDirectory(IntermediateDirectory);
			}

			if (UPL != null)
			{
				XDocument XDoc;
				try
				{
					XDoc = XDocument.Parse(Text.ToString());
				}
				catch (Exception Ex)
				{
					throw new BuildException("plist is invalid {0}\n{1}", Ex, Text.ToString());
				}

				XDoc.DocumentType.InternalSubset = "";
				UPL.ProcessPluginNode("None", "iosPListUpdates", "", ref XDoc);
				string Result = XDoc.Declaration.ToString() + "\n" + XDoc.ToString().Replace("<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\"[]>", "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">");
				File.WriteAllText(PListFile, Result);
			}
			else
			{
				File.WriteAllText(PListFile, Text.ToString());
			}

			if (BuildHostPlatform.Current.Platform == UnrealTargetPlatform.Mac)
			{
				if (!Directory.Exists(AppDirectory))
				{
					Directory.CreateDirectory(AppDirectory);
				}
				File.WriteAllText(AppDirectory + "/Info.plist", Text.ToString());
			}

			return true;
		}

		public override bool GeneratePList(FileReference ProjectFile, UnrealTargetConfiguration Config, string ProjectDirectory, bool bIsUE4Game, string GameName, bool bIsClient, string ProjectName, string InEngineDir, string AppDirectory, List<string> UPLScripts, VersionNumber SdkVersion, string BundleID, bool bBuildAsFramework, out bool bSupportsPortrait, out bool bSupportsLandscape, out bool bSkipIcons)
		{
			bSupportsPortrait = false;
			bSupportsLandscape = false;
			bSkipIcons = false;
			UnrealPluginLanguage UPL = new UnrealPluginLanguage(ProjectFile, UPLScripts, new List<string>() { "arm64" }, "", "", UnrealTargetPlatform.AppleWatch);
			return GenerateAppleWatchPList(ProjectFile, Config, ProjectDirectory, bIsUE4Game, GameName, bIsClient, ProjectName, InEngineDir, AppDirectory, UPL, BundleID);
		}

		protected override void CopyIconResources(string InEngineDir, string AppDirectory, string BuildDirectory)
		{
			CopyFiles(InEngineDir + "/Platforms/AppleWatch/Build/AppleWatch/Resources/Graphics", AppDirectory, "Icon*.png", true);
			CopyFiles(BuildDirectory + "/Resources/Graphics", AppDirectory, "Icon*.png", true);
		}
	}
}
