using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace TanksDropTwo
{
	public abstract class TankController : GameController
	{
		public Tank Owner;
		public float Scale = 1;
		public Texture2D Texture;
		public Vector2 Origin;
		public TimeSpan spawnTime;
		protected int lifeTime;

		public TankController( int LifeTime )
		{
			this.Owner = Tank.blank;
			lifeTime = LifeTime;
		}

		public virtual void Initialize( TanksDrop game, Tank Owner, TimeSpan spawnTime )
		{
			this.spawnTime = spawnTime;
			Initialize( game, Owner );
		}

		public virtual void Initialize( TanksDrop game, Tank Owner )
		{
			this.Owner = Owner;
			Initialize( game );
		}

		public abstract void LoadTexture( ContentManager Content );

		public override bool Control( GameEntity control, TimeSpan gameTime )
		{
			if ( control == Owner && ( gameTime - spawnTime ).TotalMilliseconds > lifeTime && lifeTime > 0 )
			{
				Owner.RemoveTankController();
				StopControl();
			}
			return true;
		}

		/// <summary>
		/// Called whenever the tank gets hit by a bullet.
		/// </summary>
		/// <param name="hitter">The projectile that hit the tank.</param>
		/// <returns>true if the tank should die, otherwise false.</returns>
		public abstract bool ProjectileHit( Projectile hitter, TimeSpan gameTime );

		/// <summary>
		/// Called when the tank is hit by a mystical force.
		/// </summary>
		/// <param name="gameTime">The current game time.</param>
		/// <returns>True if the tank should die - otherwise false.</returns>
		public abstract bool Hit( TimeSpan gameTime );

		/// <summary>
		/// Called when the tank wants to shoot a bullet.
		/// </summary>
		/// <param name="gameTime">The current game time.</param>
		public virtual bool Shoot( TimeSpan gameTime, Projectile shot )
		{
			return false;
		}

		/// <summary>
		/// Called when the tank hits a fence.
		/// Returns false if the tank should stop and true if it should keep going.
		/// </summary>
		/// <param name="hitter">The hit fence.</param>
		/// <returns>False if the tank should stop - otherwise true.</returns>
		public virtual bool HitFence( Fence hitter )
		{
			return false;
		}

		/// <summary>
		/// Called when the tank wants to place a fence.
		/// Returns true if it should, false if it shouldn't.
		/// </summary>
		/// <returns>true if the tank should place the fence, false if it shouldn't.</returns>
		public abstract bool OnPlaceFence( TimeSpan gameTime );

		/// <summary>
		/// Called when the TankController is released from its owner.
		/// </summary>
		public abstract void StopControl();

		public abstract GameController Clone();
	}
}
