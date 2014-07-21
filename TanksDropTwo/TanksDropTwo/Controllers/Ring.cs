using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	public class Ring : UseableController
	{
		float radius;

		public Ring( float radius )
		{
			this.radius = radius;
		}

		public override void InstantControl( GameEntity control, TimeSpan gameTime )
		{

		}

		public override void InstantAction( TimeSpan gameTime )
		{
			RingCon r = new RingCon( radius, Owner.Position );
			r.Initialize( Game, Owner );
			Owner.SetTankController( r );
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Ring" );
		}

		public override GameController Clone()
		{
			Ring r = new Ring( radius );
			r.Initialize( Game, Owner );
			return r;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return false;
		}
	}

	public class RingCon : TankController
	{
		float angle;
		float orig_angle;
		float rad;
		Vector2 Position;

		public RingCon( float radius, Vector2 position )
			: base( -1 )
		{
			this.rad = radius;
			this.Position = position;
		}

		public override void Initialize( TanksDrop game )
		{
			base.Initialize( game );
			angle = 0;
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{

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
			angle += 5F;
			if ( angle >= 370 + orig_angle )
			{
				Owner.RemoveTankController();
			}
			else
			{
				float dist = Vector2.Distance( Vector2.Zero, Owner.Origin );
				float sideDeg = 40F;
				Vector2 Pos = Position + ( new Vector2( ( float )Math.Cos( MathHelper.ToRadians( angle ) ), ( float )Math.Sin( MathHelper.ToRadians( angle ) ) ) * rad );
				Fence newFence = new Fence(
					Pos + ( new Vector2( ( float )Math.Cos( MathHelper.ToRadians( angle + sideDeg ) ), ( float )Math.Sin( MathHelper.ToRadians( angle + sideDeg ) ) ) * dist * Scale * 1.5F ),
					Pos + ( new Vector2( ( float )Math.Cos( MathHelper.ToRadians( angle - sideDeg ) ), ( float )Math.Sin( MathHelper.ToRadians( angle - sideDeg ) ) ) * dist * Scale * 1.5F ), Owner, 16, gameTime, Owner.FenceLifeTime );
				newFence.Initialize( Game );
				Game.QueueEntity( newFence );
			}
			return base.Control( control, gameTime, keyState );
		}

		public override void Initialize( TanksDrop game, Tank Owner )
		{
			base.Initialize( game, Owner );
			angle = 0;
			orig_angle = angle;
		}

		public override void StopControl()
		{
		}

		public override GameController Clone()
		{
			return new RingCon( rad, Position );
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
