using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	public class Dodger : TankController
	{
		public Dodger( int lifeTime )
			: base( lifeTime )
		{
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Dodger" );
			Origin = new Vector2( 32, 32 );
		}

		public override bool ProjectileHit( Projectile hitter, TimeSpan gameTime )
		{
			Owner.Position = Owner.RandomPosition();
			return false;
		}

		public override bool Hit( TimeSpan gameTime )
		{
			return true;
		}

		public override bool OnPlaceFence( TimeSpan gameTime )
		{
			return true;
		}

		public override void StopControl()
		{
		}

		public override GameController Clone()
		{
			Dodger d = new Dodger( lifeTime );
			d.Initialize( Game, Owner, spawnTime );
			d.LoadTexture( Game.Content );
			return d;
		}

		public override bool PickupProjectile( ProjectilePickup proj )
		{
			return true;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return false;
		}

		public override void Draw( Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch )
		{
			spriteBatch.Draw( Texture, Owner.Position, null, Color.White, 0, Origin, Owner.Scale * 0.75F, SpriteEffects.None, 1 );
		}
	}
}
