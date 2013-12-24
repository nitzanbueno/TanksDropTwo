using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace TanksDropTwo
{
	/// <summary>
	/// The basic and only tank usable in game.
	/// It can move and shoot bullets, and in the near future, place fences, take pickups and use power-ups.
	/// </summary>
	class Tank : GameEntity
	{
		public string Name;
		private KeySet keys;
		private Colors color;
		private Rectangle[] TankSourceRects;
		public float Speed;

		public Tank( string name, Vector2 startPosition, float startAngle, KeySet keys, Colors color, float speed )
		{
			Name = name;
			Speed = speed;
			Position = startPosition;
			Angle = startAngle;
			this.keys = keys;
			this.color = color;
			Scale = 2;
		}

		// The timeSpan in which the frame was updated.
		// If now - this > 175 milliseconds, update the frame.
		TimeSpan timeSinceLastFrameUpdate;

		// Tanks have 8 frames each, so this is the current frame.
		int frame = 0;

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, KeyboardState keyState )
		{
			// Update the tank's frame.
			if ( ( gameTime - timeSinceLastFrameUpdate ).TotalMilliseconds > 175 )
			{
				frame++;
				frame %= 8;
				timeSinceLastFrameUpdate = gameTime;
			}

			if ( keyState.IsKeyDown( keys.KeyForward ) )
			{
				Move( Speed );
			}

			if ( keyState.IsKeyDown( keys.KeyBackward ) )
			{
				Move( -Speed );
			}

			if ( keyState.IsKeyDown( keys.KeyLeft ) )
			{
				Angle -= 5;
			}

			if ( keyState.IsKeyDown( keys.KeyRight ) )
			{
				Angle += 5;
			}
		}

		public override void Draw( TimeSpan gameTime, SpriteBatch spriteBatch )
		{
			spriteBatch.Draw( Texture, Position, TankSourceRects[ frame ], Color.White, AngleInRadians, new Vector2( 16, 16 ), Scale, SpriteEffects.None, 0.5F );
		}

		public new void Move( float speed )
		{
			Position = Forward( speed );
			Position.X = Tools.Mod( Position.X + 50, ScreenWidth + 100 ) - 50;
			Position.Y = Tools.Mod( Position.Y + 50, ScreenHeight + 100 ) - 50;
		}

		public override void LoadContent( ContentManager Content, int ScreenWidth, int ScreenHeight )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\TankMap" );
			TankSourceRects = new Rectangle[ 8 ];
			int tankw = Texture.Width / 8;
			int tankh = Texture.Height / 8;
			for ( int x = 0; x < TankSourceRects.Length; x++ )
			{
				TankSourceRects[ x ] = new Rectangle( x * tankw, ( int )color * tankh, tankw, tankh );
			}
			base.LoadContent( Content, ScreenWidth, ScreenHeight );
		}
	}
}
