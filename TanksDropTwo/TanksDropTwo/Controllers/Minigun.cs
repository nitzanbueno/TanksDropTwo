using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	public class Minigun : TankController
	{
		TimeSpan lastShot;
		int speed;

		public Minigun( int lifeTime, int speed )
			: base( lifeTime )
		{
			this.speed = speed;
		}

		public override void Initialize( TanksDrop game )
		{
			lastShot = TimeSpan.Zero;
			base.Initialize( game );
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Minigun" );
			Origin = new Vector2( 16, 16 );
			Scale = 2;
		}

		public override bool ProjectileHit( Projectile hitter, TimeSpan gameTime )
		{
			return true;
		}

		public override bool Hit( TimeSpan gameTime )
		{
			return true;
		}

		public override bool OnPlaceFence( TimeSpan gameTime )
		{
			return true;
		}

		public override bool Control( GameEntity control, TimeSpan gameTime, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			if ( control == Owner && Owner.IsAlive && keyState.IsKeyDown( Owner.Keys.KeyShoot ) && ( lastShot == TimeSpan.Zero || ( gameTime - lastShot ).TotalMilliseconds > speed ) )
			{
				Owner.Shoot( gameTime );
				lastShot = gameTime;
			}
			return base.Control( control, gameTime, keyState );
		}

		public override void StopControl()
		{

		}

		public override GameController Clone()
		{
			Minigun clone = new Minigun( lifeTime, speed );
			clone.Initialize( Game, Owner );
			return clone;
		}

		public override bool PickupProjectile( ProjectilePickup proj )
		{
			return true;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return false;
		}

		public override void Draw( Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch )
		{
		}
	}
}
