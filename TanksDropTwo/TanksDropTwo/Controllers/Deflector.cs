﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDropTwo.Controllers
{
	/// <summary>
	/// A UseableController that causes all bullets to turn 180 degrees (Switch direction) when used.
	/// </summary>
	class Deflector : UseableController
	{
		public Deflector( Tank Owner )
			: base( Owner )
		{
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager content )
		{
			Texture = content.Load<Texture2D>( "Sprites\\Deflector" );
			Scale = 2F;
		}

		public override void InstantControl( GameEntity control, TimeSpan gameTime )
		{
			if ( control is Projectile )
			{
				control.Angle += 180;
			}
		}

		public override bool AddEntity( GameEntity entity )
		{
			return entity is Projectile;
		}

		public override GameController Clone()
		{
			Deflector d = new Deflector( Owner );
			d.Initialize( Game );
			d.LoadTexture( Game.Content );
			return d;
		}
	}
}