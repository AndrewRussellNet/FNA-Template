using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace FNATemplate
{
	public static class Input
	{
		public static MouseState mouseState;
		public static MouseState lastMouseState;

		public static KeyboardState keyboardState;
		public static KeyboardState lastKeyboardState;

		public static bool IsActive { get; private set; }

		public static void Update(bool isActive)
		{
			lastMouseState = mouseState;
			mouseState = Mouse.GetState();
			UpdateScrollWheel();

			lastKeyboardState = keyboardState;
			keyboardState = Keyboard.GetState();

			Input.IsActive = isActive;
		}


		#region Key Presses

		public static bool KeyWentDown(Keys key)
		{
			return lastKeyboardState.IsKeyUp(key) && keyboardState.IsKeyDown(key);
		}

		public static bool KeyWentUp(Keys key)
		{
			return lastKeyboardState.IsKeyDown(key) && keyboardState.IsKeyUp(key);
		}

		#endregion


		#region Keyboard State

		public static bool IsKeyDown(Keys key)
		{
			return keyboardState.IsKeyDown(key);
		}

		public static bool IsKeyUp(Keys key)
		{
			return keyboardState.IsKeyUp(key);
		}

		public static bool Shift
		{
			get { return keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift); }
		}

		public static bool Control
		{
			get { return keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl); }
		}

		public static bool Alt
		{
			get { return keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt); }
		}

		#endregion


		#region Mouse State

		public static bool LeftMouseWentDown
		{
			get { return IsActive && lastMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed; }
		}

		public static bool LeftMouseWentUp
		{
			get { return IsActive && lastMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released; }
		}

		public static bool LeftMouseIsDown
		{
			get { return IsActive && mouseState.LeftButton == ButtonState.Pressed; }
		}

		public static bool LeftMouseIsUp
		{
			get { return !IsActive || mouseState.LeftButton == ButtonState.Released; }
		}


		public static bool RightMouseWentDown
		{
			get { return IsActive && lastMouseState.RightButton == ButtonState.Released && mouseState.RightButton == ButtonState.Pressed; }
		}

		public static bool RightMouseWentUp
		{
			get { return IsActive && lastMouseState.RightButton == ButtonState.Pressed && mouseState.RightButton == ButtonState.Released; }
		}

		public static bool RightMouseIsDown
		{
			get { return IsActive && mouseState.RightButton == ButtonState.Pressed; }
		}

		public static bool RightMouseIsUp
		{
			get { return !IsActive || mouseState.RightButton == ButtonState.Released; }
		}


		public static bool MiddleMouseWentDown
		{
			get { return IsActive && lastMouseState.MiddleButton == ButtonState.Released && mouseState.MiddleButton == ButtonState.Pressed; }
		}

		public static bool MiddleMouseWentUp
		{
			get { return IsActive && lastMouseState.MiddleButton == ButtonState.Pressed && mouseState.MiddleButton == ButtonState.Released; }
		}

		public static bool MiddleMouseIsDown
		{
			get { return IsActive && mouseState.MiddleButton == ButtonState.Pressed; }
		}

		public static bool MiddleMouseIsUp
		{
			get { return !IsActive || mouseState.MiddleButton == ButtonState.Released; }
		}


		public static Point MousePosition
		{
			get { return new Point(mouseState.X, mouseState.Y); }
		}

		public static Point LastMousePosition
		{
			get { return new Point(lastMouseState.X, lastMouseState.Y); }
		}


		public static Point LeftMouseDrag
		{
			get
			{
				if(IsActive && mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Pressed)
					return new Point(mouseState.X - lastMouseState.X, mouseState.Y - lastMouseState.Y);
				else
					return new Point();
			}
		}

		public static Point MiddleMouseDrag
		{
			get
			{
				if(IsActive && mouseState.MiddleButton == ButtonState.Pressed && lastMouseState.MiddleButton == ButtonState.Pressed)
					return new Point(mouseState.X - lastMouseState.X, mouseState.Y - lastMouseState.Y);
				else
					return new Point();
			}
		}

		public static Point RightMouseDrag
		{
			get
			{
				if(IsActive && mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Pressed)
					return new Point(mouseState.X - lastMouseState.X, mouseState.Y - lastMouseState.Y);
				else
					return new Point();
			}
		}

		#endregion


		#region Mouse Scroll

		public static int scrollWheelAccumulate;
		public static bool ScrollWheelClickUp { get; private set; }
		public static bool ScrollWheelClickDown { get; private set; }
		public static int ScrollWheelClick { get; private set; }

		private static void UpdateScrollWheel()
		{
			ScrollWheelClickUp = ScrollWheelClickDown = false;
			ScrollWheelClick = 0;

			scrollWheelAccumulate += (mouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue);

			if(scrollWheelAccumulate >= 120)
			{
				ScrollWheelClickUp = true;
				ScrollWheelClick = 1;
				scrollWheelAccumulate -= 120;
			}
			if(scrollWheelAccumulate <= -120)
			{
				ScrollWheelClickDown = true;
				ScrollWheelClick = -1;
				scrollWheelAccumulate += 120;
			}
		}

		#endregion


		#region Keyboard Movement

		public static Point SimpleKeyboardMovement
		{
			get
			{
				Point move = Point.Zero;
				if(keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
					move.X -= 1;
				if(keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
					move.X += 1;
				if(keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
					move.Y -= 1;
				if(keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
					move.Y += 1;
				return move;
			}
		}

		#endregion

	}
}
