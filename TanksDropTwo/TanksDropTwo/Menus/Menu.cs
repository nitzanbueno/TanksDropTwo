using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TanksDropTwo.Menus
{
	public abstract class Menu
	{
		protected TanksDrop Game;

		public Menu( TanksDrop Game )
		{
			this.Game = Game;
		}

		public abstract void Update( TimeSpan gameTime, KeyboardState keyState, MouseState mouseState, GamePadState[] padStates );

		public abstract void Draw( SpriteBatch spriteBatch, TimeSpan gameTime );

		public void ReferMenu( Menu menu )
		{
			Game.CurrentMenu = menu;
		}
	}
}
