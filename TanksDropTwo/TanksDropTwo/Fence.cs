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
	public class Fence : GameEntity
	{
		private Tank owner;
		private int height;
		private int width;

		/// <summary>
		/// Initializes a new Fence entity.
		/// </summary>
		/// <param name="point1">The first endpoint of the fence.</param>
		/// <param name="point2">The second endpoint of the fence.</param>
		/// <param name="owner">The owning tank (currently for color only).</param>
		/// <param name="thickness">The thickness of the fence.</param>
		/// <param name="gameTime">The current game time.</param>
		/// <param name="lifetime">The amount of time the fence stays on the board in milliseconds. -1 is infinity.</param>
		public Fence( Vector2 point1, Vector2 point2, Tank owner, float thickness, TimeSpan gameTime, int lifetime )
		{
			this.owner = owner;
			Angle = MathHelper.ToDegrees( ( float )Math.Atan2( point2.Y - point1.Y, point2.X - point1.X ) );
			height = ( int )thickness;
			width = ( int )Vector2.Distance( point1, point2 );
			Origin = new Vector2( ( int )( width / 2 ), ( int )( height / 2 ) );
			Position = ( point1 + point2 ) / 2;
			Scale = 1;
			spawnTime = gameTime;
			lifeTime = lifetime;
		}

		public override void LoadContent( ContentManager Content, int screenWidth, int screenHeight )
		{
			Texture = new Texture2D( Game.GraphicsDevice, width, height, false, SurfaceFormat.Color );
			Texture.SetData( Enumerable.Repeat<Color>( owner.Color, width * height ).ToArray<Color>() );
			base.LoadContent( Content, screenWidth, screenHeight );
		}


	}
}
