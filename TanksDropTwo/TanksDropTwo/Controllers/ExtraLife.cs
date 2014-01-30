using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	public class ExtraLife : TankController
	{
		float Rotation;

		public ExtraLife()
			: base( -1 )
		{
			Rotation = 0;
		}

		private Texture2D Halo;

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\ExtraLife" );
			Halo = Content.Load<Texture2D>( "Sprites\\ExtraAlive" );
			Scale = 2.0F;
		}

		public override bool ProjectileHit( Projectile hitter, TimeSpan gameTime )
		{
			hitter.Destroy( gameTime );
			return Hit( gameTime );
		}

		public override bool Hit( TimeSpan gameTime )
		{
			Owner.RemoveTankController();
			return false;
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
			ExtraLife e = new ExtraLife();
			e.Initialize( Game, Owner );
			e.LoadTexture( Game.Content );
			return e;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return false;
		}

		public override void Draw( SpriteBatch spriteBatch )
		{
			spriteBatch.Draw( Halo, Owner.Position, null, Color.White, Rotation, new Vector2( 16, 16 ), Owner.Scale * 1.5F, SpriteEffects.None, 0.24F );
			Rotation += MathHelper.ToRadians( 10 );
		}
	}
}
