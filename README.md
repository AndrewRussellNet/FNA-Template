FNA Template
============

FNA Template is a quick, easy and robust way to start new projects using FNA (http://fna-xna.github.io/), Ethan Lee's excellent reimplementation of Microsoft XNA Game Studio.

It has been tested with Visual Studio 2010 (on Windows) and Visual Studio Community 2017 (on OSX). But it should run on other versions of Visual Studio, or directly with MSBuild.

It uses MonoGame's content pipeline for building assets (except shaders), but does not use MonoGame at runtime. It does NOT require XNA or XNA Game Studio.

FNA Template is released under the Microsoft Public License. (Note that this is the same license as FNA).


Getting Started
===============

To use FNA Template you will need to install the following:

- MonoGame (tested with MonoGame 3.6) from http://www.monogame.net/ for building content
- DirectX SDK (June 2010) from https://www.microsoft.com/en-us/download/details.aspx?id=6812 for building shaders

Building shaders on OSX
-----------------------

On OSX, the DirectX SDK is still required (and we need wine to help). Here is how to install it:

- Install Homebrew from https://brew.sh/
- Install wine with `brew install wine`
- Install winetricks with `brew install winetricks`
- (If you already have these installed, update with: `brew update`, `brew upgrade wine`, `brew upgrade winetricks`)
- Setup wine with `winecfg`
- Install the DirectX SDK with `winetricks dxsdk_jun2010`

NOTE: At time of writing there is a bug in winetricks that will cause the DirectX SDK to not install correctly. See https://github.com/Winetricks/winetricks/issues/841

You can either modify your winetricks to fix that bug (`which winetricks`, and then find and replace the broken hash value), or use `winetricks d3dcompiler_43` instead and put a copy of `fxc.exe` from the DirectX SDK in the `builds/tools` directory (see `BuildShaders.targets` for details).

Required FNA components
-----------------------

You need to add the following directories at the same level as the solution file (note case sensitivity):

- "FNA" containing the FNA project from https://github.com/FNA-XNA/FNA
- "FNALibs" containing the FNA libraries from http://fna.flibitijibibo.com/archive/fnalibs.tar.bz2

At this point you should be able to open the solution file, and build and run the FNATemplate project.

Debugging on OSX
----------------

In order to run in the debugger on Visual Studio on OSX, you will need to add an environment variable:

- Right click the FNATemplate project
- Options
- Run -> Configurations -> Default
- Environment Variables -> Add
- Variable = `DYLD_LIBRARY_PATH`, Value = `./osx/`

You will need to repeat these steps for any new projects you create from the template (because they are per-user debugging settings, not part of the project file).

If the template crashes inside FNA with a DllNotFoundException as the inner exception of a TypeInitializationException, you forgot this step!

Using the CreateTemplate tool
-----------------------------

Use the CreateTemplate tool to create new versions of the FNATemplate (NOTE: Including any local modifications). This is easier than copying the files and fixing up names and GUIDs by hand.

The command line is:

`CreateTemplate [--template] <ProjectName> <SourceDirectory> <DestinationDirectory>`

You can use it to directly create new projects:

`CreateTemplate NewProject "X:\pathToThisSolution\FNATemplate" "X:\yourSolution\NewProject"`

Or you can create a Visual Studio template file, which itself can be used to create projects.

`CreateTemplate --template FNATemplate "X:\pathToThisSolution\FNATemplate" .`

To install the generated template in Visual Studio:

- Go to the output folder (containing the resulting .vstemplate file)
- Select all files in that folder (Ctrl+A)
- Add them to a zip (Right click -> Send To -> Compressed (zipped) folder)
- Move that zip file to the project templates directory for your platform (eg: "C:\Users\ USERNAME\Documents\Visual Studio 2010\Templates\ProjectTemplates\Visual C#")

(NOTE: Ensure that the .vstemplate file is at the root of the resulting .zip file. Otherwise the template will not work.)

Note that the template requires the same "FNA" and "FNALibs" directories as specified above, as well as the "build" directory, exist in the directory above your project file. You will also need to add the FNA project to your solution.


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

