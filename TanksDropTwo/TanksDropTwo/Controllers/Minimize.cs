using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	class Minimize : TankController
	{
		float OriginalTankScale;

		float MiniTankScale;

		bool isDone;

		public Minimize( Tank Owner, int LifeTime )
			: base( Owner, LifeTime )
		{
			SetOwner( Owner );
		}

		private void SetOwner( Tank Owner )
		{
			OriginalTankScale = Owner.Scale;
			MiniTankScale = Owner.Scale / 3;
		}

		public override void Initialize( TanksDrop game, Tank Owner )
		{
			SetOwner( Owner );
			isDone = false;
			base.Initialize( game, Owner );
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager content )
		{
			Texture = content.Load<Texture2D>( "Sprites\\Minimize" );
			Origin = new Vector2( 16, 16 );
			Scale = 2F;
		}

		public override bool ProjectileHit( Projectile hitter )
		{
			return true;
		}

		public override bool OnPlaceFence()
		{
			return true;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return false;
		}

		public override void Draw( Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch ) {}

		public override bool Control( GameEntity control, TimeSpan gameTime )
		{
			if ( control == Owner && !isDone )
			{
				Owner.Scale = MiniTankScale;
			}
			base.Control( control, gameTime );
			return true;
		}

		public override void StopControl()
		{
			Owner.Scale = OriginalTankScale;
			isDone = true;
		}

		public override GameController Clone()
		{
			Minimize m = new Minimize( Owner, lifeTime );
			m.Initialize( Game );
			m.LoadTexture( Game.Content );
			return m;
		}
	}
}
