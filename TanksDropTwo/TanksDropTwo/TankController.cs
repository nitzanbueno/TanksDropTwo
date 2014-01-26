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

		public TankController( Tank Owner, int LifeTime )
		{
			this.Owner = Owner;
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

		public abstract void LoadTexture( ContentManager content );

		public override bool Control( GameEntity control, TimeSpan gameTime )
		{
			if ( control == Owner && ( gameTime - spawnTime ).TotalMilliseconds > lifeTime && lifeTime > 0 )
			{
				Owner.RemoveTankController( this );
				StopControl();
			}
			return true;
		}

		/// <summary>
		/// Called whenever the tank gets hit by a bullet.
		/// </summary>
		/// <param name="hitter">The projectile that hit the tank.</param>
		/// <returns>true if the tank should die, otherwise false.</returns>
		public abstract bool ProjectileHit( Projectile hitter );

		/// <summary>
		/// Called when the tank wants to shoot a bullet.
		/// </summary>
		/// <param name="gameTime">The current game time.</param>
		public virtual void Shoot( TimeSpan gameTime )
		{
			Owner.Shoot( gameTime, true );
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
		public abstract bool OnPlaceFence();

		/// <summary>
		/// Called when the TankController is released from its owner.
		/// </summary>
		public abstract void StopControl();

		public abstract GameController Clone();
	}
}
