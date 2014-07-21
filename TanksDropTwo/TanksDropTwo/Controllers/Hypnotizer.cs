using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	public class Hypnotizer : TankController
	{
		float rad;

		public Hypnotizer( int lifeTime, float radius )
			: base( lifeTime )
		{
			rad = radius;
		}

		public override void Initialize( TanksDrop game )
		{
			base.Initialize( game );
		}

		public override void Initialize( TanksDrop game, Tank Owner )
		{
			game.AppendController( this );
			base.Initialize( game, Owner );
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Hypnotizer" );
			Origin = new Vector2( 32, 32 );
		}

		public override bool ProjectileHit( Projectile hitter, TimeSpan gameTime )
		{
			return false;
		}

		public override bool Hit( TimeSpan gameTime )
		{
			return true;
		}

		public override bool OnPlaceFence( TimeSpan gameTime )
		{
			return true;
		}

		public override bool Shoot( TimeSpan gameTime, Projectile shot )
		{
			shot.Variables[ "Hypnotize" ] = new HashSet<Tank>( new[] { Owner } );
			return base.Shoot( gameTime, shot );
		}

		public override void StopControl()
		{
			Game.RemoveController( this, x => true );
		}

		public override bool Control( GameEntity control, TimeSpan gameTime, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			if ( control is Projectile )
			{
				HashSet<Tank> tanks = new HashSet<Tank>();
				if ( control.Variables.ContainsKey( "Hypnotize" ) )
				{
					tanks = ( ( HashSet<Tank> )control.Variables[ "Hypnotize" ] );
				}
				if ( Vector2.Distance( control.Position, Owner.Position ) < rad )// && !Tools.IsGoingTowardsMe( Owner.Position, control.Angle, control.Position ) )
				{
					if ( !control.Variables.ContainsKey( "Hypnotize" ) || !tanks.Contains( Owner ) )
					{
						DeflectorController d = new DeflectorController( Tools.Angle( Owner.Position, control.Position ) );
						control.AppendController( d );
						tanks.Add( Owner );
					}
				}
				else
				{
					tanks.Remove( Owner );
				}
				control.Variables[ "Hypnotize" ] = tanks;
			}
			return base.Control( control, gameTime, keyState );
		}

		public override GameController Clone()
		{
			Hypnotizer clone = new Hypnotizer( lifeTime, rad );
			clone.Initialize( Game, Owner, spawnTime );
			clone.LoadTexture( Game.Content );
			return clone;
		}

		public override bool PickupProjectile( ProjectilePickup proj )
		{
			return true;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return entity is Projectile;
		}

		public override void Draw( Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch )
		{
			spriteBatch.Draw( Texture, Owner.Position, null, Color.White, 0, Origin, Owner.Scale * 0.75F, SpriteEffects.None, 1 );
		}
	}
}
