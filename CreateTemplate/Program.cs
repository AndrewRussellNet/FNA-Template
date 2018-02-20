using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CreateTemplate
{
	class Program
	{
		static int Main(string[] args)
		{
			if(args.Length == 0)
			{
				Console.WriteLine("Creates a copy of FNA Template suitable for starting new FNA projects.");
				Console.WriteLine();
				Console.WriteLine("Usage:");
				Console.WriteLine("  CreateTemplate [--source <SourceDir>] [--solution] [--template]");
				Console.WriteLine("                 <ProjectName> <DestinationDir>");
				Console.WriteLine();
				Console.WriteLine("Options:");
				Console.WriteLine("  --source (-s)    : Sets the source directory containing FNATemplate.csproj.");
				Console.WriteLine("                     If not specified, it is searched for up the directory");
				Console.WriteLine("                     hierarchy with the name FNATemplate.");
				Console.WriteLine("  --solution       : Generates a full solution in the output directory.");
				Console.WriteLine("  --template (-t)  : Generate a Visual Studio compatible project template.");
				Console.WriteLine();
				Console.WriteLine("The output project is placed at <DestinationDir>" + Path.DirectorySeparatorChar + "<ProjectName>");
				Console.WriteLine();
				return 1;
			}

			bool createTemplate = false;
			bool createSolution = false;
			string sourceDirectory = null;
			string projectName = null;
			string destinationDirectory = null;
			int parameterNumber = 0;

			Queue<string> arguments = new Queue<string>(args);
			while(arguments.Count > 0)
			{
				string a = arguments.Dequeue();
				switch(a)
				{
					case "--source":
						if(arguments.Count > 0)
							sourceDirectory = Path.GetFullPath(arguments.Dequeue());
						else
						{
							Console.WriteLine("ERROR: Must specify a directory for --source.");
							return 1;
						}
						break;

					case "--solution":
						createSolution = true;
						break;

					case "--template":
					case "-t":
						createTemplate = true;
						break;

					default:
						switch(parameterNumber)
						{
							case 0:
								projectName = a;
								break;
							case 1:
								destinationDirectory = Path.GetFullPath(a);
								break;
							default:
								Console.WriteLine("ERROR: Too many parameters.");
								return 1;
						}
						parameterNumber++;
						break;
				}
			}

			if(parameterNumber < 2)
			{
				Console.WriteLine("ERROR: Not enough parameters.");
				return 1;
			}

			if(createSolution && createTemplate)
			{
				Console.WriteLine("WARNING: --template and --solution options together are kind of silly.");
			}


			// Search for the source directory if it is not specified
			if(sourceDirectory == null)
			{
				DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
				while(true)
				{
					string possibleSourceDir = Path.Combine(di.FullName, "FNATemplate");
					if(Directory.Exists(possibleSourceDir) && File.Exists(Path.Combine(possibleSourceDir, "FNATemplate.csproj")))
					{
						sourceDirectory = possibleSourceDir;
						Console.WriteLine("Found source directory at:");
						Console.WriteLine(sourceDirectory);
						break;
					}
					else
					{
						if(di.Parent != null)
							di = di.Parent;
						else
						{
							Console.WriteLine("ERROR: Could not find SourceDir. Specify with --source or ensure CreateTemplate is in the correct location.");
							return 1;
						}
					}
				}
			}

			if(!Directory.Exists(sourceDirectory))
			{
				Console.WriteLine("ERROR: Source directory not found!");
				return 1;
			}

			if(!File.Exists(Path.Combine(sourceDirectory, "FNATemplate.csproj")))
			{
				Console.WriteLine("ERROR: Source directory is missing \"FNATemplate.csproj\"");
				return 1;
			}

			if(destinationDirectory.StartsWith(sourceDirectory)) // <- Probably not the most robust way to do this...
			{
				Console.WriteLine("ERROR: Destination directory is contained within the source directory!");
				return 1;
			}


			// If we're creating a solution, we need these paths:
			string fnaProjectDirectory = null;
			string fnaProjectFile = null;
			string fnalibsDirectory = null;
			string buildDirectory = null;
			if(createSolution)
			{
				DirectoryInfo di = new DirectoryInfo(sourceDirectory).Parent;

				fnaProjectDirectory = Path.Combine(di.FullName, "FNA");
				fnaProjectFile = Path.Combine(fnaProjectDirectory, "FNA.csproj");
				fnalibsDirectory = Path.Combine(di.FullName, "fnalibs");
				buildDirectory = Path.Combine(di.FullName, "build");

				if(!Directory.Exists(fnaProjectDirectory))
				{
					Console.WriteLine("ERROR: Could not find directory \"" + fnaProjectDirectory + "\".");
					return 1;
				}

				if(!File.Exists(fnaProjectFile))
				{
					Console.WriteLine("ERROR: Could not find \"" + fnaProjectFile + "\".");
					return 1;
				}

				if(!Directory.Exists(fnalibsDirectory))
				{
					Console.WriteLine("ERROR: Could not find directory \"" + fnalibsDirectory + "\".");
					return 1;
				}

				if(!Directory.Exists(buildDirectory))
				{
					Console.WriteLine("ERROR: Could not find directory \"" + buildDirectory + "\".");
					return 1;
				}
			}



			//
			// And off we go...
			//

			string projectDirectory = Path.Combine(destinationDirectory, projectName);
			Directory.CreateDirectory(projectDirectory);

			StreamWriter templateFile = null;
			StreamWriter solutionFile = null;

			string fnaProjectGuid = null;
			string projectGuid = Guid.NewGuid().ToString("B").ToUpperInvariant();

			if(createSolution)
			{
				string fnaProjectText = File.ReadAllText(fnaProjectFile);
				string projectGuidString = "<ProjectGuid>";
				int guidStart = fnaProjectText.IndexOf(projectGuidString);
				int guidEnd = fnaProjectText.IndexOf("</ProjectGuid>");
				if(guidStart >= 0 && guidEnd >= guidStart + projectGuidString.Length)
				{
					fnaProjectGuid = fnaProjectText.Substring(guidStart + projectGuidString.Length, guidEnd - (guidStart + projectGuidString.Length));
				}
				else
				{
					Console.WriteLine("ERROR: Could not find ProjectGuid in FNA.csproj.");
					return 1;
				}

				solutionFile = new StreamWriter(Path.Combine(destinationDirectory, projectName + ".sln"), false,
						Encoding.UTF8); // <- NOTE: Encoding required so we get a BOM so the VS version selector can deal with us
				solutionFile.WriteLine("");
				solutionFile.WriteLine("Microsoft Visual Studio Solution File, Format Version 11.00");
				solutionFile.WriteLine("# Visual Studio 2010");
				solutionFile.WriteLine("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"" + projectName + "\", \"" + projectName + "\\" + projectName + ".csproj\", \"" + projectGuid + "\"");
				solutionFile.WriteLine("EndProject");
				solutionFile.WriteLine("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"FNA\", \"FNA\\FNA.csproj\", \"" + fnaProjectGuid + "\"");
				solutionFile.WriteLine("EndProject");
				solutionFile.WriteLine("Global");
				solutionFile.WriteLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
				solutionFile.WriteLine("\t\tDebug|Any CPU = Debug|Any CPU");
				solutionFile.WriteLine("\t\tDebug|x86 = Debug|x86");
				solutionFile.WriteLine("\t\tRelease|Any CPU = Release|Any CPU");
				solutionFile.WriteLine("\t\tRelease|x86 = Release|x86");
				solutionFile.WriteLine("\tEndGlobalSection");
				solutionFile.WriteLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
				solutionFile.WriteLine("\t\t" + projectGuid + ".Debug|Any CPU.ActiveCfg = Debug|Any CPU");
				solutionFile.WriteLine("\t\t" + projectGuid + ".Debug|Any CPU.Build.0 = Debug|Any CPU");
				solutionFile.WriteLine("\t\t" + projectGuid + ".Debug|x86.ActiveCfg = Debug|x86");
				solutionFile.WriteLine("\t\t" + projectGuid + ".Debug|x86.Build.0 = Debug|x86");
				solutionFile.WriteLine("\t\t" + projectGuid + ".Release|Any CPU.ActiveCfg = Release|Any CPU");
				solutionFile.WriteLine("\t\t" + projectGuid + ".Release|Any CPU.Build.0 = Release|Any CPU");
				solutionFile.WriteLine("\t\t" + projectGuid + ".Release|x86.ActiveCfg = Release|x86");
				solutionFile.WriteLine("\t\t" + projectGuid + ".Release|x86.Build.0 = Release|x86");
				solutionFile.WriteLine("\t\t" + fnaProjectGuid + ".Debug|Any CPU.ActiveCfg = Debug|Any CPU");
				solutionFile.WriteLine("\t\t" + fnaProjectGuid + ".Debug|Any CPU.Build.0 = Debug|Any CPU");
				solutionFile.WriteLine("\t\t" + fnaProjectGuid + ".Debug|x86.ActiveCfg = Debug|Any CPU");
				solutionFile.WriteLine("\t\t" + fnaProjectGuid + ".Debug|x86.Build.0 = Debug|Any CPU");
				solutionFile.WriteLine("\t\t" + fnaProjectGuid + ".Release|Any CPU.ActiveCfg = Release|Any CPU");
				solutionFile.WriteLine("\t\t" + fnaProjectGuid + ".Release|Any CPU.Build.0 = Release|Any CPU");
				solutionFile.WriteLine("\t\t" + fnaProjectGuid + ".Release|x86.ActiveCfg = Release|Any CPU");
				solutionFile.WriteLine("\t\t" + fnaProjectGuid + ".Release|x86.Build.0 = Release|Any CPU");
				solutionFile.WriteLine("\tEndGlobalSection");
				solutionFile.WriteLine("\tGlobalSection(SolutionProperties) = preSolution");
				solutionFile.WriteLine("\t\tHideSolutionNode = FALSE");
				solutionFile.WriteLine("\tEndGlobalSection");
				solutionFile.WriteLine("EndGlobal");
				solutionFile.Close();

				string[] ignore = { "bin", "obj" };
				CopyDirectoryRecursive(fnaProjectDirectory, Path.Combine(destinationDirectory, "FNA"), ignore);
				CopyDirectoryRecursive(fnalibsDirectory, Path.Combine(destinationDirectory, "fnalibs"), ignore);
				CopyDirectoryRecursive(buildDirectory, Path.Combine(destinationDirectory, "build"), ignore);
			}

			if(createTemplate)
			{
				templateFile = new StreamWriter(Path.Combine(projectDirectory, projectName + ".vstemplate"));
				templateFile.WriteLine("<VSTemplate Version=\"3.0.0\" xmlns=\"http://schemas.microsoft.com/developer/vstemplate/2005\" Type=\"Project\">");
				templateFile.WriteLine("  <TemplateData>");
				templateFile.WriteLine("    <Name>FNA Game Project</Name>");
				templateFile.WriteLine("    <Description>A cross-platform game project using FNA.</Description>");
				templateFile.WriteLine("    <ProjectType>CSharp</ProjectType>");
				templateFile.WriteLine("    <NumberOfParentCategoriesToRollUp>1</NumberOfParentCategoriesToRollUp>");
				templateFile.WriteLine("    <ProjectSubType>");
				templateFile.WriteLine("    </ProjectSubType>");
				templateFile.WriteLine("    <SortOrder>43900</SortOrder>");
				templateFile.WriteLine("    <CreateNewFolder>true</CreateNewFolder>");
				templateFile.WriteLine("    <CreateInPlace>true</CreateInPlace>"); // <- Required because we have relative import paths in the project file!
				templateFile.WriteLine("    <DefaultName>MyFNAGameProject</DefaultName>");
				templateFile.WriteLine("    <ProvideDefaultName>true</ProvideDefaultName>");
				templateFile.WriteLine("    <LocationField>Enabled</LocationField>");
				templateFile.WriteLine("    <EnableLocationBrowseButton>true</EnableLocationBrowseButton>");
				//templateFile.WriteLine("    <Icon>__TemplateIcon.png</Icon>");
				//templateFile.WriteLine("    <PreviewImage>__PreviewImage.png</PreviewImage>");
				templateFile.WriteLine("  </TemplateData>");
				templateFile.WriteLine("  <TemplateContent>");
				templateFile.WriteLine("    <Project TargetFileName=\"" + projectName + ".csproj\" File=\"" + projectName + ".csproj\" ReplaceParameters=\"true\">");
			}

			string binDir = Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar;
			string objDir = Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar;
			string binDirFirst = "bin" + Path.DirectorySeparatorChar;
			string objDirFirst = "obj" + Path.DirectorySeparatorChar;

			foreach(var inputPath in Directory.EnumerateFiles(sourceDirectory, "*.*", SearchOption.AllDirectories))
			{
				string localPath = inputPath.Substring(sourceDirectory.EndsWith(Path.DirectorySeparatorChar.ToString()) ? sourceDirectory.Length : sourceDirectory.Length + 1);

				if(localPath.Contains(binDir) || localPath.Contains(objDir) || localPath.StartsWith(binDirFirst) || localPath.StartsWith(objDirFirst))
				{
					continue;
				}

				string outputLocalPath = localPath.Replace("FNATemplate", projectName);

				// NOTE: "GetFileName" here is because VS bizarrely combines the path from the ProjectItem AND the TargetFileName
				string templateTargetFileName = Path.GetFileName(localPath).Replace("FNATemplate", "$safeprojectname$");

				string outputPath = Path.Combine(projectDirectory, outputLocalPath);
				Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

				// Quick'n'dirty text replacement:
				bool replaceParameters;
				if(outputLocalPath.EndsWith(".csproj"))
				{
					var lines = File.ReadAllLines(inputPath);
					for(int i = 0; i < lines.Length; i++)
					{
						if(lines[i].Contains("<ProjectGuid>"))
							lines[i] = "    <ProjectGuid>" + (createTemplate ? "$guid1$" : projectGuid) + "</ProjectGuid>"; // <- TODO: Handle more than one project?
						else if(lines[i].Contains("FNATemplate"))
							lines[i] = lines[i].Replace("FNATemplate", createTemplate ? "$safeprojectname$" : projectName);
					}
					File.WriteAllLines(outputPath, lines);

					replaceParameters = true;
				}
				else if(localPath.EndsWith(".cs"))
				{
					var lines = File.ReadAllLines(inputPath);
					for(int i = 0; i < lines.Length; i++)
					{
						if(lines[i].Contains("[assembly: Guid("))
							lines[i] = "[assembly: Guid(\"" + (createTemplate ? "$guid2$" : Guid.NewGuid().ToString()) + "\")]";
						else if(lines[i].Contains("FNATemplate"))
							lines[i] = lines[i].Replace("FNATemplate", createTemplate ? "$safeprojectname$" : projectName);
					}
					File.WriteAllLines(outputPath, lines);

					replaceParameters = true;
				}
				else // All other files
				{
					File.Copy(inputPath, outputPath, true);
					replaceParameters = false;
				}

				if(createTemplate)
				{
					templateFile.Write("      <ProjectItem TargetFileName=\"");
					templateFile.Write(templateTargetFileName);
					templateFile.Write("\"");
					if(replaceParameters)
						templateFile.Write(" ReplaceParameters=\"true\"");
					templateFile.Write(">");
					templateFile.Write(outputLocalPath);
					templateFile.WriteLine("</ProjectItem>");
				}
			}

			if(createTemplate)
			{
				templateFile.WriteLine("    </Project>");
				templateFile.WriteLine("  </TemplateContent>");
				templateFile.WriteLine("</VSTemplate>");
				templateFile.Close();
			}

			return 0;
		}



		private static void CopyDirectoryRecursive(string sourceDirName, string destDirName, IEnumerable<string> ignore = null)
		{
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);

			if(!dir.Exists)
			{
				throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
			}

			DirectoryInfo[] dirs = dir.GetDirectories();
			if(!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			FileInfo[] files = dir.GetFiles();
			foreach(FileInfo file in files)
			{
				if(ignore != null && ignore.Contains(file.Name))
					continue;

				string temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, true);
			}
			
			foreach(DirectoryInfo subdir in dirs)
			{
				if(ignore != null && ignore.Contains(subdir.Name))
					continue;

				string temppath = Path.Combine(destDirName, subdir.Name);
				CopyDirectoryRecursive(subdir.FullName, temppath, ignore);
			}
		}
	}
}
