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
		protected float speed;
		private Tank owner;
		protected TimeSpan spawnTime;

		public Projectile( Tank Owner, TimeSpan gameTime )
		{
			owner = Owner;
			spawnTime = gameTime;
		}

		protected void UpdatePhysics( HashSet<GameEntity> Entities )
		{
			foreach ( GameEntity entity in Entities )
			{
				if ( entity != this && this.CollidesWith( entity ) )
				{
					// The projectile collided with an entity.
					if ( entity is Tank )
					{
						// The projectile hit a tank.
						Tank HitTank = ( Tank )entity;
						if ( HitTank.Hit( this ) )
						{
							Game.RemoveEntity( this );
						}
					}
					else if ( entity is Fence )
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

		protected void CheckDestruction( TimeSpan gameTime, int destructionMillisecs )
		{
			if ( ( gameTime - spawnTime ).TotalMilliseconds >= destructionMillisecs )
			{
				Destroy();
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

		protected void Destroy()
		{
			owner.NumberOfBullets--;
			Game.RemoveEntity( this );
		}
	}
}
