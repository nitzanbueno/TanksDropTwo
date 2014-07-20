using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo
{
	public abstract class Projectile : GameEntity
	{
		/// <summary>
		/// Bounce axis.
		/// </summary>
		protected int bAxis;

		/// <summary>
		/// Determines whether or not to remove a tank's bullet number when destroyed.
		/// </summary>
		public bool doesCountAsTankProjectile;

		public float Speed;
		public Tank owner;

		public Projectile( Tank Owner, TimeSpan gameTime )
			: this( Owner )
		{
			spawnTime = gameTime;
		}

		public Projectile( TimeSpan gameTime ) : this( Tank.blank, gameTime ) { }

		public Projectile( Tank Owner )
		{
			owner = Owner;
			doesCountAsTankProjectile = true;
		}

		protected Projectile() { }

		public virtual void Initialize( TanksDrop game, TimeSpan gameTime )
		{
			spawnTime = gameTime;
			Initialize( game );
		}

		public virtual void Initialize( TanksDrop game, TimeSpan gameTime, Tank owner )
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
						// Reflects the projectile from the fence.
						Angle = ( 2 * fangle ) - Angle;
						// This line works on the following concept:
						// I need a symmetrical angle to my current one.
						// So, I will solve the following equation:
						// fangle - Angle = NewAngle - fangle
						// That's also why the angle has to face upwards.
						// The result is: NewAngle = 2 * fangle - Angle.
						Move( Speed );
						// Makes sure that the projectile doesn't get stuck inside.
					}
				}
			}
		}

		protected void CheckBounces()
		{
			int olda = bAxis;
			bAxis = 0;
			if ( olda > 0 )
			{
				return;
			}
			if ( Position.X > ScreenWidth || Position.X < 0 )
			{
				Angle = 540 - Angle;
				bAxis += 1;
			}
			if ( Position.Y > ScreenHeight || Position.Y < 0 )
			{
				Angle = 360 - Angle;
				bAxis += 2;
			}
		}

		public override void Destroy( TimeSpan gameTime )
		{
			if ( doesCountAsTankProjectile )
			{
				owner.NumberOfProjectiles--;
			}
			Game.RemoveEntity( this );
		}

		public abstract Projectile Clone();

		public virtual Projectile TriplerClone( TimeSpan gameTime )
		{
			Projectile p = Clone();
			p.doesCountAsTankProjectile = false;
			return p;
		}
	}
}
