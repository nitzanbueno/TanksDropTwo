using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TanksDropTwo
{
	public class TankControllerPickup : Pickup
	{
		/// <summary>
		/// The projectile this pickup has.
		/// </summary>
		// You can think of it as what's inside the pickup box.
		public TankController Carrier
		{
			get
			{
				return ( TankController )carrier;
			}
			set
			{
				carrier = value;
			}
		}

		public TankControllerPickup( TankController carrier, int lifeTime )
			: base( lifeTime )
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

		protected override void TankPickup( Tank tank, TimeSpan gameTime )
		{
			if ( tank.PickupController( this, gameTime ) )
			{
				Game.RemoveEntity( this );
			}
		}
	}
}
