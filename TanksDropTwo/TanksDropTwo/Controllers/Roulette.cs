﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	public class Roulette : UseableController
	{
		List<Tank> Tanks;
		Tank chosenTank;
		bool isObliterated;

		public override void Initialize( TanksDrop game )
		{
			Tanks = new List<Tank>();
			base.Initialize( game );
			isObliterated = false;
		}

		public override void InstantControl( GameEntity control, TimeSpan gameTime )
		{
			if ( !isObliterated )
			{
				if ( chosenTank.IsAlive )
				{
					chosenTank.Destroy( gameTime );
				}
				else
				{
					chosenTank.IsAlive = true;
				}
				isObliterated = true;
			}
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Roulette" );
			Scale = 2.0F;
		}

		public override GameController Clone()
		{
			return new Roulette();
		}

		public override bool AddEntity( GameEntity entity )
		{
			if ( entity is Tank )
			{
				Tanks.Add( ( Tank )entity );
				if ( Tanks.Count >= ( int )Game.Settings[ "Players" ].Item2 )
				{
					Random r = new Random();
					chosenTank = Tanks[ r.Next( Tanks.Count ) ];
				}
				return true;
			}
			return false;
		}
	}
}
