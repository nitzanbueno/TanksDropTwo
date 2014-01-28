using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDropTwo
{
	public abstract class Projectile : GameEntity
	{
		public float Speed;
		protected Tank owner;

		public Projectile( Tank Owner, TimeSpan gameTime )
		{
			owner = Owner;
			spawnTime = gameTime;
		}

		public Projectile( TimeSpan gameTime ) : this( Tank.blank, gameTime ) { }

		public Projectile( Tank Owner )
		{
			owner = Owner;
		}

		protected Projectile() { }

		public void Initialize( TanksDrop game, TimeSpan gameTime )
		{
			spawnTime = gameTime;
			Initialize( game );
		}

		public void Initialize( TanksDrop game, TimeSpan gameTime, Tank owner )
		{
			this.owner = owner;
			Initialize( game, gameTime );
		}

		/// <summary>
		/// If hit a tank, kills the tank.
		/// If hit a fence, mirrors off.
		/// </summary>
		/// <param name="Entities">The list of entities.</param>
		/// <param name="CheckFences">True if should mirror angle when hits a fence, otherwise false.</param>
		protected void CheckHits( TimeSpan gameTime, HashSet<GameEntity> Entities, bool CheckFences = true )
		{
			foreach ( GameEntity entity in Entities )
			{
				if ( ( entity is Tank ^ ( entity is Fence && CheckFences ) ) && this.CollidesWith( entity ) )
				{
					// The projectile collided with an entity.
					if ( entity is Tank )
					{
						// The projectile hit a tank.
						Tank HitTank = ( Tank )entity;
						if ( HitTank.IsAlive && HitTank.ProjectileHit( this, gameTime ) )
						{
							Destroy( gameTime );
							break;
						}
					}
					else
					{
						Fence HitFence = ( Fence )entity;
						// The angle will always face upwards.
						float fangle = HitFence.Angle < 180 ? HitFence.Angle + 180 : HitFence.Angle;
						// Reflects the projectile from the fence magically.
						Angle = ( 2 * fangle ) - Angle;
						// I actually thought of this line myself, but I have no idea how it works.
						// It just does and I don't care.
					}
				}
			}
		}

		protected void CheckBounces()
		{
			if ( Position.X > ScreenWidth || Position.X < 0 )
			{
				Angle = 540 - Angle;
			}
			if ( Position.Y > ScreenHeight || Position.Y < 0 )
			{
				Angle = 360 - Angle;
			}
		}

		public override void Destroy( TimeSpan gameTime )
		{
			owner.NumberOfProjectiles--;
			Game.RemoveEntity( this );
		}

		public abstract Projectile Clone();
	}
}
