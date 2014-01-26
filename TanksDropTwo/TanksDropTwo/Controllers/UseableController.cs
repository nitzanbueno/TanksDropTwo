using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDropTwo.Controllers
{
	public abstract class UseableController : TankController
	{
		private bool isDestructed;

		public UseableController( Tank Owner )
			: base( Owner, -1 )
		{
		}

		public override void Initialize( TanksDrop game )
		{
			game.AppendController( this );
			isDestructed = false;
			base.Initialize( game );
		}

		public override bool ProjectileHit( Projectile hitter )
		{
			return true;
		}

		public override bool OnPlaceFence()
		{
			isDestructed = true;
			Owner.RemoveTankController( this );
			Game.StopController( this );
			return false;
		}

		public override bool Control( GameEntity control, TimeSpan gameTime )
		{
			if ( isDestructed )
			{
				InstantControl( control, gameTime );
				control.RemoveController( this );
			}
			return true;
		}

		/// <summary>
		/// Called once to make the change for the entity instantly.
		/// </summary>
		/// <param name="control">The entity to change.</param>
		/// <param name="gameTime">The current game time.</param>
		public abstract void InstantControl( GameEntity control, TimeSpan gameTime );

		public override void Draw( SpriteBatch spriteBatch )
		{
		}

		public override void StopControl()
		{
		}
	}
}
