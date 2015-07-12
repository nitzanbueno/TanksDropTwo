using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	/// <summary>
	/// Disabler is a UseableController that causes all non-tank entities on board to get sucked into the owning tank and be destroyed on use.
	/// </summary>
	public class Disabler : UseableController
	{
		private float maxSpeed;

		public Disabler( float maxSpeed )
		{
			this.maxSpeed = maxSpeed;
		}

		public override void InstantControl( GameEntity control, TimeSpan gameTime )
		{
			( ( Tank )control ).RemoveTankController();
		}

		public override void InstantAction( TimeSpan gameTime )
		{
			// Suck all non-tank entities
			VacuumController d = new VacuumController( Owner, maxSpeed, x => !( x is Tank ), true, 1, true );
			d.Initialize( Game );
			Game.PutController( d );
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Disabler" );
			Scale = 2F;
			Origin = new Vector2( 16, 16 );
		}

		public override GameController Clone()
		{
			Disabler d = new Disabler( maxSpeed );
			d.Initialize( Game, Owner );
			d.LoadTexture( Game.Content );
			return d;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return entity is Tank;
		}

		public override bool PickupProjectile( ProjectilePickup proj )
		{
			return true;
		}
	}
}
