﻿using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FNATemplate
{
	class FNATemplateGame : Game
	{
		GraphicsDeviceManager graphics;

		public FNATemplateGame()
		{
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 720;
			graphics.PreferMultiSampling = true;

			Window.AllowUserResizing = true;
			IsMouseVisible = true;
		}


		SpriteBatch spriteBatch;
		Effect exampleEffect;

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// Effects need to be loaded from files built by fxc.exe from the DirectX SDK (June 2010)
			// (Note how each .fx file has the Build Action "CompileShader", which produces a .fxb file.)
			exampleEffect = new Effect(GraphicsDevice, File.ReadAllBytes(@"Effects/ExampleEffect.fxb"));

			base.LoadContent();
		}

		protected override void UnloadContent()
		{
			spriteBatch.Dispose();
			exampleEffect.Dispose();

			base.UnloadContent();
		}



		protected override void Update(GameTime gameTime)
		{
			Input.Update(IsActive);

			//
			// Asset Rebuilding:
#if DEBUG
			if(Input.KeyWentDown(Keys.F5))
			{
				if(AssetRebuild.Run())
				{
					UnloadContent();
					LoadContent();
				}
			}
#endif

			//
			// Insert your game update logic here.
			//

			base.Update(gameTime);
		}


		protected override void Draw(GameTime gameTime)
		{
			//
			// Replace this with your own drawing code.
			//

			GraphicsDevice.Clear(Color.CornflowerBlue);

			base.Draw(gameTime);
		}

	}
}
