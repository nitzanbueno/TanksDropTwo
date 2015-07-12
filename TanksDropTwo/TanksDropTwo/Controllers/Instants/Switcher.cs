using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	/// <summary>
	/// A UseableController that switches the user with a random living tank.
	/// </summary>
	class Switcher : UseableController
	{
		List<Tank> switchableTanks;

		public Switcher()
			: base()
		{
		}

		public override void Initialize( TanksDrop game )
		{
			switchableTanks = new List<Tank>();
			base.Initialize( game );
		}

		public override void InstantControl( GameEntity control, TimeSpan gameTime )
		{
			
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager content )
		{
			Texture = content.Load<Texture2D>( "Sprites\\Switcher" );
			Origin = new Vector2( 16, 16 );
			Scale = 2.0F;
		}

		public override bool AddEntity( GameEntity entity )
		{
			if ( entity is Tank )
			{
				if ( entity != Owner && ( ( Tank )entity ).IsAlive )
					switchableTanks.Add( ( Tank )entity );
				return true;
			}
			return false;
		}

		public override GameController Clone()
		{
			Switcher s = new Switcher();
			s.Initialize( Game, Owner );
			s.LoadTexture( Game.Content );
			return s;
		}

		public override void InstantAction( TimeSpan gameTime )
		{
			Random r = new Random();
			if ( switchableTanks.Count <= 0 )
				return;
			Tank switchedTank = switchableTanks[ r.Next( switchableTanks.Count ) ];
			if ( !switchedTank.IsAlive )
			{
				switchableTanks.Remove( switchedTank );
				InstantAction( gameTime );
				return;
			}
			Vector2 OwnerPos = Owner.Position;
			float OwnerAngle = Owner.Angle;
			bool OwnerAlive = Owner.IsAlive;
			Owner.Position = switchedTank.Position;
			Owner.Angle = switchedTank.Angle;
			Owner.IsAlive = switchedTank.IsAlive;
			switchedTank.Position = OwnerPos;
			switchedTank.Angle = OwnerAngle;
			switchedTank.IsAlive = OwnerAlive;
			switchableTanks = new List<Tank>();
		}
	}
}
