using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Menus
{
	public class PauseMenu : Menu
	{
		public PauseMenu( TanksDrop Game ) : base( Game ) { }

		public override void Draw( SpriteBatch spriteBatch, TimeSpan gameTime )
		{
			spriteBatch.Draw( Game.Blank, Game.BoundingBox, null, new Color( 0, 0, 0, 100 ), 0, Vector2.Zero, SpriteEffects.None, 0.01F );
			spriteBatch.DrawString( Game.Score, "Paused\n\n\n\nSpacebar or X to resume", new Vector2( Game.ScreenWidth / 4, Game.ScreenHeight / 10 ), Color.White, 0, new Vector2( 16, 16 ), 2, SpriteEffects.None, 0 );
		}

		public override void Update( TimeSpan gameTime, KeyboardState keyState, MouseState mouseState, GamePadState[] padStates )
		{
			if ( keyState.IsKeyDown( Keys.Space ) || padStates.Any(padState => padState.IsButtonDown( Buttons.X )) || keyState.IsKeyDown( Keys.X ) )
			{
				ReferMenu( null );
			}
		}
	}
}
