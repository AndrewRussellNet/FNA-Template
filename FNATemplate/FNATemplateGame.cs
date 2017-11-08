﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
			Content.RootDirectory = "Content";

			Window.AllowUserResizing = true;
			IsMouseVisible = true;
		}


		SpriteBatch spriteBatch;
		SpriteFont font;
		Texture2D smile;
		Effect exampleEffect;

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// Most content can be loaded from MonoGame Content Builder projects.
			// (Note how "Content.mgcb" has the Build Action "MonoGameContentReference".)
			font = Content.Load<SpriteFont>("Font");
			smile = Content.Load<Texture2D>("Smile");

			// Effects need to be loaded from files built by fxc.exe from the DirectX SDK (June 2010)
			// (Note how each .fx file has the Build Action "CompileShader", which produces a .fxb file.)
			exampleEffect = new Effect(GraphicsDevice, File.ReadAllBytes(@"Effects/ExampleEffect.fxb"));

			base.LoadContent();
		}


		protected override void Update(GameTime gameTime)
		{
			// Insert your game update logic here.

			base.Update(gameTime);
		}


		protected override void Draw(GameTime gameTime)
		{
			// Replace this with your own drawing code.

			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();
			spriteBatch.DrawString(font, "Insert awesome game here!", new Vector2(20, 20), Color.White);
			spriteBatch.End();

			spriteBatch.Begin(0, null, null, null, null, exampleEffect);
			spriteBatch.Draw(smile, new Vector2(20, 60), Color.White);
			spriteBatch.End();

			base.Draw(gameTime);
		}

	}
}
