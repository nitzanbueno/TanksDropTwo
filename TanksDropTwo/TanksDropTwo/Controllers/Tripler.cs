using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDropTwo.Controllers
{
	public class Tripler : TankController
	{
		public Tripler( Tank Owner, int lifeTime )
			: base( Owner, lifeTime )
		{
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager content )
		{
			Texture = content.Load<Texture2D>( "Sprites\\Tripler" );
			Scale = 2.0F;
		}

		public override bool ProjectileHit( Projectile hitter, TimeSpan gameTime )
		{
			return true;
		}

		public override bool Hit( TimeSpan gameTime )
		{
			return true;
		}

		public override bool OnPlaceFence()
		{
			return true;
		}

		public override void StopControl()
		{
		}

		public override GameController Clone()
		{
			Tripler t = new Tripler( Owner, lifeTime );
			t.Initialize( Game );
			t.LoadTexture( Game.Content );
			return t;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return false;
		}

		public override void Draw( Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch )
		{
			
		}

		public override bool Shoot( TimeSpan gameTime, Projectile shot )
		{
			Projectile rightShot = shot.Clone();
			Projectile leftShot = shot.Clone();
			rightShot.Angle += 45;
			leftShot.Angle -= 45;
			Game.QueueEntity( shot, rightShot, leftShot );
			return true;
		}
	}
}
