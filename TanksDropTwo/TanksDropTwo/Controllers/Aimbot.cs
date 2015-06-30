using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	/// <summary>
	/// Aimbot is a UseableController that, on use, will shoot an accurate bullet at each of its opponents and aims the owner at the closest tank to it.
	/// </summary>
	public class Aimbot : UseableController
	{
		HashSet<Tank> Tanks;

		public override void Initialize( TanksDrop game )
		{
			Tanks = new HashSet<Tank>();
			base.Initialize( game );
		}

		public override void InstantControl( GameEntity control, TimeSpan gameTime )
		{
			float closestDistance = -1; // The distance to the closest tank
			Tank closestTank = null; // The closest tank
			// Find the closest living tank
			foreach ( Tank t in Tanks )
			{
				float dist = Vector2.DistanceSquared( Owner.Position, t.Position );
				if ( t.IsAlive )
				{
					if ( ( closestTank == null || dist < closestDistance ) )
					{
						closestDistance = dist;
						closestTank = t;
					}
				}
			}
			if ( closestTank != null ) // If no tank is alive don't do anything
			{
				// Set owner's angle
				float ang = Tools.Angle( control.Position, closestTank.Position );
				DeflectorController c = new DeflectorController( ang );
				control.AppendController( c );
			}
		}

		public override void InstantAction( TimeSpan gameTime )
		{
			AimbotCon c = new AimbotCon();
			c.Initialize( Game, Owner );
			Game.PutController( c );
			Owner.RemoveTankController();
			Owner.SetTankController( c );
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Aimbot" );
			Origin = new Vector2( 32, 32 );
		}

		public override GameController Clone()
		{
			Aimbot clone = new Aimbot();
			clone.Initialize( Game, Owner, spawnTime );
			return clone;
		}

		public override bool AddEntity( GameEntity entity )
		{
			// Add tank to tanks set
			if ( entity is Tank && entity != Owner )
			{
				Tanks.Add( ( Tank )entity );
				entity.Variables[ "Aimbot" ] = true;
			}
			return entity is Projectile;
		}
	}

	public class AimbotCon : TankController
	{
		/// <summary>
		/// The set of tanks.
		/// </summary>
		private HashSet<Tank> Tanks;

		public AimbotCon()
			: base( -1 )
		{
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Aimbot" );
			Origin = new Vector2( 32, 32 );
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
			if ( control == Owner )
			{
				float closestDistance = -1; // The distance to the closest tank
				Tank closestTank = null; // The closest tank
				// Find the closest living tank
				foreach ( Tank t in Tanks )
				{
					float dist = Vector2.DistanceSquared( Owner.Position, t.Position );
					if ( t.IsAlive )
					{
						if ( ( closestTank == null || dist < closestDistance ) )
						{
							closestDistance = dist;
							closestTank = t;
						}
						if ( ( bool )t.Variables[ "Aimbot" ] && t != Owner && t.IsAlive )
						{
							Projectile p = Owner.OriginalProjectile;
							p.Angle = Tools.Angle( Owner, t );
							p.Position = Owner.PositionShift( 20 * Owner.Scale, p.Angle );
							p.Initialize( Game, gameTime, Owner );
							Game.QueueEntity( p );
							t.Variables[ "Aimbot" ] = false;
						}
					}
				}
				if ( closestTank != null ) // If no tank is alive don't do anything
				{
					// Set owner's angle
					float ang = Tools.HomeAngle( Owner.TurnSpeed, Owner.Angle, Owner.Position, closestTank.Position );
					Owner.Angle += ang;
					if ( ang == 0 )
					{
						Owner.RemoveTankController();
					}
				}
			}

			return base.Control( control, gameTime, keyState );
		}

		public override void StopControl()
		{
		}

		public override void Initialize( TanksDrop game, Tank Owner )
		{
			base.Initialize( game, Owner );
			Tanks = new HashSet<Tank>();
			Game.PutController( this );
		}

		public override GameController Clone()
		{
			Aimbot clone = new Aimbot();
			clone.Initialize( Game, Owner, spawnTime );
			return clone;
		}

		public override bool PickupProjectile( ProjectilePickup proj )
		{
			return true;
		}

		public override bool AddEntity( GameEntity entity )
		{
			// Add tank to tanks set
			if ( entity is Tank && entity != Owner )
			{
				Tanks.Add( ( Tank )entity );
				entity.Variables[ "Aimbot" ] = true;
			}
			return entity == Owner;
		}

		public override void Draw( Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch )
		{
		}
	}
}
