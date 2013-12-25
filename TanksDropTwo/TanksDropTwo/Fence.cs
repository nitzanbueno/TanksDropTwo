using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TanksDropTwo
{
	class Fence : GameEntity
	{
		private Tank owner;
		private int height;
		private int width;

		private TimeSpan spawnTime;

		public Fence( Vector2 Point1, Vector2 Point2, Tank Owner, float Width, TimeSpan gameTime )
		{
			owner = Owner;
			Angle = MathHelper.ToDegrees( ( float )Math.Atan2( Point2.Y - Point1.Y, Point2.X - Point1.X ) );
			height = ( int )Width;
			width = ( int )Vector2.Distance( Point1, Point2 );
			Origin = new Vector2( ( int )( width / 2 ), ( int )( height / 2 ) );
			Position = ( Point1 + Point2 ) / 2;
			Scale = 1;
			spawnTime = gameTime;
		}

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, KeyboardState keyState )
		{
			if ( ( gameTime - spawnTime ).TotalMilliseconds > 10000 )
			{
				Game.RemoveEntity( this );
			}
			base.Update( gameTime, Entities, keyState );
		}

		public override void LoadContent( ContentManager Content, int screenWidth, int screenHeight )
		{
			Texture = new Texture2D( Game.GraphicsDevice, width, height, false, SurfaceFormat.Color );
			Texture.SetData( Enumerable.Repeat<Color>( owner.Color, width * height ).ToArray<Color>() );
			base.LoadContent( Content, screenWidth, screenHeight );
		}


	}
}
