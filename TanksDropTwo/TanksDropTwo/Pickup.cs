using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace TanksDropTwo
{
	/// <summary>
	/// Pickups are like boxes - They are the connector between a specific pickupable and the tank itself,
	/// for raw pickupables cannot appear on the screen.
	/// When a tank collides with the pickup, it recieves what's inside.
	/// </summary>
	public abstract class Pickup : GameEntity
	{
		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, KeyboardState keyState )
		{
			CheckPickup( Entities );
			base.Update( gameTime, Entities, keyState );
		}

		public override void LoadContent( ContentManager Content, int screenWidth, int screenHeight )
		{
			if ( Texture == null )
			{
				Texture = new Texture2D( Game.GraphicsDevice, 1, 1 );
				Texture.SetData( new[] { Color.White } );
			}
			base.LoadContent( Content, screenWidth, screenHeight );
		}

		public void CheckPickup( HashSet<GameEntity> Entities )
		{
			foreach ( GameEntity entity in Entities )
			{
				if ( entity is Tank && ( ( Tank )entity ).IsAlive && entity.CollidesWith( this ) )
				{
					TankPickup( ( Tank )entity );
				}
			}
		}

		/// <summary>
		/// Happens when a tank collides with the pickup.
		/// </summary>
		/// <param name="tank">The tank that collided.</param>
		/// <remarks>Used to have the tank get the pickup.</remarks>
		protected abstract void TankPickup( Tank tank );
	}
}
