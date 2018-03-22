using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace FNATemplate
{
#if DEBUG
	static class AssetRebuild
	{
		public static bool Run()
		{
			try
			{
				// This file is generated at build time (see ContentRebuilder.targets)
				string infoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AssetRebuildInfo.txt");
				if(!File.Exists(infoPath))
				{
					Debug.WriteLine("AssetRebuild: Could not find file " + infoPath);
					return false;
				}

				string[] lines = File.ReadAllLines(infoPath);
				if(lines.Length < 2)
				{
					Debug.WriteLine("AssetRebuild: Missing data in " + infoPath);
					return false;
				}

				string msBuildDirectory = lines[0];
				if(!Directory.Exists(msBuildDirectory))
				{
					Debug.WriteLine("AssetRebuild: Could not directory " + msBuildDirectory);
					return false;
				}

				string projectPath = lines[1];
				if(!File.Exists(projectPath))
				{
					Debug.WriteLine("AssetRebuild: Could not find " + projectPath);
					return false;
				}
				
				string msBuildPath;
				if(Environment.OSVersion.Platform == PlatformID.Win32NT)
				{
					msBuildPath = Path.Combine(msBuildDirectory, "MSBuild");
				}
				else
				{
                    // The msbuild command isn't where we expect it on Xamarin, so just rely on PATH.
					msBuildPath = "msbuild";
				}

				Process process = new Process();
				process.StartInfo.FileName = msBuildPath;
				process.StartInfo.Arguments = @"/t:BuildContentOnly";
				process.StartInfo.WorkingDirectory = Path.GetDirectoryName(projectPath); // <- MSBuild will automatically find the csproj file.
				process.Start();
				process.WaitForExit();

				Debug.WriteLine("AssetRebuild: Completed");
				return true;
			}
			catch(Exception e)
			{
				Debug.WriteLine("AssetRebuild: Failed with: " + e.ToString());
				return false;
			}
		}
	}
#endif
}
