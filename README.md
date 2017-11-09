FNA Template
============

FNA Template is a simple, cross-platform way to start new projects using FNA (http://fna-xna.github.io/), Ethan Lee's excellent reimplementation of Microsoft XNA Game Studio.

It has been tested with Visual Studio 2010 (on Windows) and Visual Studio Community 2017 (on macOS), and should work with other versions of Visual Studio, MonoDevelop, or directly with MSBuild.

It uses MonoGame's content pipeline for building assets (except shaders), but does not use MonoGame at runtime. It does NOT require XNA or XNA Game Studio.

FNA Template is released under the Microsoft Public License. (Note that this is the same license as FNA.)


Getting Started
===============

To use FNA Template you will need to install the following:

- **MonoGame** (tested with MonoGame 3.6) from http://www.monogame.net/ for building content
- **DirectX SDK (June 2010)** for building shaders
  - *On Windows:* Download from https://www.microsoft.com/en-us/download/details.aspx?id=6812
  - *On Linux/macOS:* Install using Wine and winetricks (instructions below)

You will also need to add FNA itself. Place the following directories at the same level as the solution file (note case sensitivity):

- **"FNA"** containing the FNA project from https://github.com/FNA-XNA/FNA
- **"fnalibs"** containing the FNA libraries from http://fna.flibitijibibo.com/archive/fnalibs.tar.bz2

At this point you should be able to open and build the solution. On Windows you can now run and debug the FNATemplate project. On Linux/macOS there is an additional step to run it in the debugger (instructions below).

(Linux/macOS) Installing the DirectX SDK on Wine
------------------------------------------------

On Linux and macOS, the DirectX SDK is still required to compile shaders. On these platforms we use Wine to run the DirectX SDK tools.

To install Wine and winetricks on Linux, refer to your distribution's package database. Typically the package names will simply be `wine` and `winetricks`.

To install Wine and winetricks on macOS:

- Install Homebrew from https://brew.sh/
- Install wine with `brew install wine`
- Install winetricks with `brew install winetricks`
- (If you already have these installed, update with: `brew update`, `brew upgrade wine`, `brew upgrade winetricks`)

Once Wine and winetricks are installed:

- Setup wine with `winecfg`
- Install the DirectX SDK with `winetricks dxsdk_jun2010`

NOTE: At time of writing there is a bug in winetricks that will cause the DirectX SDK to not install correctly. See https://github.com/Winetricks/winetricks/issues/841. You can either fix that bug (`which winetricks`, then find and replace the broken hash value), or use the alternative method:

**Alternative:** Instead of installing the DirectX SDK, you can place a copy of `fxc.exe` from the DirectX SDK in the `build/tools` directory. Then use `winetricks d3dcompiler_43` to install the required DLL from the DirectX redistributable (this is a smaller download than the SDK). See `BuildShaders.targets` for details. The same fallback also works on Windows.

(Linux/macOS) Setting the library path for debugging
----------------------------------------------------

In order to run in the debugger on Visual Studio for Mac or MonoDevelop, you will need to add an environment variable:

- Right click the FNATemplate project
- Options
- Run -> Configurations -> Default
- Environment Variables -> Add

On macOS, set the following environment variable:

- Variable = `DYLD_LIBRARY_PATH`, Value = `./osx/`

On Linux, set one of:

- 64-bit: Variable = `LD_LIBRARY_PATH`, Value = `./lib64`
- 32-bit: Variable = `LD_LIBRARY_PATH`, Value = `./lib`

You will need to repeat these steps for any new projects you create from the template (because they are per-user debugging settings, not part of the project file).

If the template crashes inside FNA with a DllNotFoundException, possibly as the inner exception of a TypeInitializationException, you forgot this step!

Using the CreateTemplate tool
=============================

Use the CreateTemplate tool to create new versions of the FNATemplate project (NOTE: Including any local modifications). This is easier than copying the files and fixing up names and GUIDs by hand.

The command line is:

`CreateTemplate [--template] <ProjectName> <SourceDirectory> <DestinationDirectory>`

You can use it to directly create new projects:

`CreateTemplate NewProject "X:\pathToThisSolution\FNATemplate" "X:\yourSolution"`

Or you can create a Visual Studio template file, which itself can be used to create projects.

`CreateTemplate --template FNATemplate "X:\pathToThisSolution\FNATemplate" .`

To install the generated template in Visual Studio:

- Go to the output folder (containing the resulting .vstemplate file)
- Select all files in that folder (Ctrl+A)
- Add them to a zip (Right click -> Send To -> Compressed (zipped) folder)
- Move that zip file to the project templates directory for your platform (eg: "C:\Users\USERNAME\Documents\Visual Studio 2010\Templates\ProjectTemplates\Visual C#")

(NOTE: Ensure that the .vstemplate file is at the root of the resulting .zip file. Otherwise the template will not work.)

Creating your own solution
--------------------------

To use FNA Template to create projects in a fresh solution, you will need to:

- Copy the "build" directory into the same directory as your new solution
- Add the "FNA" and "FNALibs" directories as specified above
- Add the FNA project to your solution (Right click solution -> Add Existing Project)

From this point, you can use the CreateTemplate tool (or a Visual Studio template created by the same) to add new FNA game projects to your solution.


Building and loading shaders
============================

FNA Template uses fxc.exe (from the DirectX SDK) to build shaders, rather than using a content pipleine. This produces raw .fxb files rather than the usual .xnb content files.

To add .fx (shader source) files to your project: Add them and then set their Build Action to "Compile Shader" in the Properties window (typically the F4 key). (Set "Copy to Output Directory" to "Do not copy".)

To load a shader, read it directly from the .fxb file. For example:

`myEffect = new Effect(GraphicsDevice, File.ReadAllBytes(@"Effects/MyEffect.fxb"));`

An example shader is included in FNATemplate.

Note that if you are not creating your own shaders, you can safely delete the shader file and associated code from the template, and skip installing the DirectX SDK.


Building and loading other content
==================================

Other content is built using the MonoGame Content Pipeline. The .mgcb file in the template can be opened with the MonoGame Content Pipeline Tool.

Unlike MonoGame, simply create the content for the Windows platform.

Load content the same as you would with XNA or MonoGame. For example:

`font = Content.Load<SpriteFont>("Font");`


Distributing your game
======================

Please refer to https://github.com/FNA-XNA/FNA/wiki/3:-Distributing-FNA-Games

