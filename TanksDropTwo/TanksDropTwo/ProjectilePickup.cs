using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TanksDropTwo
{
	/// <summary>
	/// A pickup that has a projectile in it.
	/// </summary>
	public class ProjectilePickup : Pickup
	{
		public ProjectilePickup( Projectile carrier )
		{
			Carrier = carrier;
		}

		public override void LoadContent( Microsoft.Xna.Framework.Content.ContentManager Content, int screenWidth, int screenHeight )
		{
			Texture = Carrier.Texture;
			Origin = Carrier.Origin;
			Scale = Carrier.Scale;
			base.LoadContent( Content, screenWidth, screenHeight );
		}

		/// <summary>
		/// The projectile this pickup has.
		/// </summary>
		// You can think of it as what's inside the pickup box.
		public Projectile Carrier;

		protected override void TankPickup( Tank tank )
		{
			if ( tank.PickupProjectile( this ) )
			{
				Game.RemoveEntity( this );
			}
		}
	}
}
