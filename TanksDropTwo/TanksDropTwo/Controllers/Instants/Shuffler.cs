using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	/// <summary>
	/// Shuffler is a UseableController that causes all entities on board to change location and angle randomly.
	/// </summary>
	public class Shuffler : UseableController
	{
		Random r;

		public Shuffler()
		{
			r = new Random();
		}

		public override void InstantControl( GameEntity control, TimeSpan gameTime )
		{
			// Change angle and position
			control.Position = control.RandomPosition( r );
			control.Angle = r.Next( 360 );
		}

		public override void InstantAction( TimeSpan gameTime )
		{
			Owner.Position = Owner.RandomPosition( r );
			Owner.Angle = r.Next( 360 );
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Shuffler" );
			Origin = new Vector2( 32, 32 );
			base.LoadTexture( Content );
		}

		public override GameController Clone()
		{
			return new Shuffler();
		}

		public override bool AddEntity( GameEntity entity )
		{
			return true;
		}
	}
}
