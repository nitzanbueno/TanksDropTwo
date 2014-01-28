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
		protected object carrier;

		public Pickup( int lifeTime )
		{
			this.lifeTime = lifeTime;
		}

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, KeyboardState keyState )
		{
			CheckPickup( Entities, gameTime );
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

		public void CheckPickup( HashSet<GameEntity> Entities, TimeSpan gameTime )
		{
			foreach ( GameEntity entity in Entities )
			{
				if ( entity is Tank && ( ( Tank )entity ).IsAlive && entity.CollidesWith( this ) )
				{
					TankPickup( ( Tank )entity, gameTime );
				}
			}
		}

		public void Initialize( TanksDrop game, TimeSpan spawnTime )
		{
			this.spawnTime = spawnTime;
			this.Game = game;
			InitializeCarrier( game );
			Initialize( game );
		}

		/// <summary>
		/// Happens when a tank collides with the pickup.
		/// </summary>
		/// <param name="tank">The tank that collided.</param>
		/// <remarks>Used to have the tank get the pickup.</remarks>
		protected abstract void TankPickup( Tank tank, TimeSpan gameTime );

		public void InitializeCarrier( TanksDrop game )
		{
			if ( carrier is TankController )
			{
				( ( TankController )carrier ).Initialize( game );
				( ( TankController )carrier ).LoadTexture( game.Content );
			}
			else if ( carrier is Projectile )
			{
				( ( Projectile )carrier ).Initialize( game );
			}
		}
	}
}
