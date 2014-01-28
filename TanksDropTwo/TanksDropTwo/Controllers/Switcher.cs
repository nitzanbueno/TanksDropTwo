﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	class Switcher : UseableController
	{
		bool isTankApplied;
		Tank switchedTank;

		public Switcher()
			: base()
		{
		}

		public override void Initialize( TanksDrop game )
		{
			isTankApplied = false;
			base.Initialize( game );
		}

		public override void InstantControl( GameEntity control, TimeSpan gameTime )
		{
			if ( !isTankApplied ) return;
			Vector2 OwnerPos = Owner.Position;
			float OwnerScale = Owner.Scale;
			float OwnerAngle = Owner.Angle;
			bool OwnerAlive = Owner.IsAlive;
			Owner.Position = switchedTank.Position;
			Owner.Scale = switchedTank.Scale;
			Owner.Angle = switchedTank.Angle;
			Owner.IsAlive = switchedTank.IsAlive;
			switchedTank.Position = OwnerPos;
			switchedTank.Angle = OwnerAngle;
			switchedTank.Scale = OwnerScale;
			switchedTank.IsAlive = OwnerAlive;
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
				Tank t = ( Tank )entity;
				if ( t.IsAlive && t != Owner && !isTankApplied )
				{
					isTankApplied = true;
					switchedTank = t;
					return true;
				}
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
	}
}
