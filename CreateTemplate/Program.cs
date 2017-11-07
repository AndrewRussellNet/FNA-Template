using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CreateTemplate
{
	class Program
	{
		static void Main(string[] args)
		{
			// Validate command line:
			if(args.Length == 0 || args.Length != (args[0] == "--template" ? 4 : 3))
			{
				Console.WriteLine("Usage:");
				Console.WriteLine("  CreateTemplate [--template] <ProjectName> <SourceDirectory> <DestinationDirectory>");
				Console.WriteLine();
				Console.WriteLine("The output will be placed in the folder at <DestinationDirectory>" + Path.DirectorySeparatorChar + "<ProjectName>");
				Console.WriteLine();
				Console.WriteLine("Options:");
				Console.WriteLine("  --template: Generate a Visual Studio compatible project template.");
				Console.WriteLine();
				return;
			}


			bool vsTemplate = args[0] == "--template";
			string projectName = args[vsTemplate ? 1 : 0];
			string sourceDirectory = Path.GetFullPath(args[vsTemplate ? 2 : 1]);
			string destinationDirectory = Path.GetFullPath(args[vsTemplate ? 3 : 2]);

			if(!Directory.Exists(sourceDirectory))
			{
				Console.WriteLine("ERROR: Source directory not found!");
				return;
			}

			if(!File.Exists(Path.Combine(sourceDirectory, "FNATemplate.csproj")))
			{
				Console.WriteLine("ERROR: Source directory is missing \"FNATemplate.csproj\"");
				return;
			}

			if(destinationDirectory.StartsWith(sourceDirectory)) // <- Probably not the most robust way to do this...
			{
				Console.WriteLine("ERROR: Destination directory is contained with the source directory!");
				return;
			}



			// And off we go...

			string projectDirectory = Path.Combine(destinationDirectory, projectName);
			Directory.CreateDirectory(projectDirectory);
			StreamWriter templateFile = null;

			if(vsTemplate)
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
							lines[i] = "    <ProjectGuid>" + (vsTemplate ? "$guid1$" : Guid.NewGuid().ToString()) + "</ProjectGuid>";
						else if(lines[i].Contains("FNATemplate"))
							lines[i] = lines[i].Replace("FNATemplate", vsTemplate ? "$safeprojectname$" : projectName);
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
							lines[i] = "[assembly: Guid(\"" + (vsTemplate ? "$guid2$" : Guid.NewGuid().ToString()) + "\")]";
						else if(lines[i].Contains("FNATemplate"))
							lines[i] = lines[i].Replace("FNATemplate", vsTemplate ? "$safeprojectname$" : projectName);
					}
					File.WriteAllLines(outputPath, lines);

					replaceParameters = true;
				}
				else // All other files
				{
					File.Copy(inputPath, outputPath, true);
					replaceParameters = false;
				}

				if(vsTemplate)
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


			if(vsTemplate)
			{
				templateFile.WriteLine("    </Project>");
				templateFile.WriteLine("  </TemplateContent>");
				templateFile.WriteLine("</VSTemplate>");
				templateFile.Close();
			}

		}
	}
}
