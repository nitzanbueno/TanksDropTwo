using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo
{
	class Missile : Projectile
	{
		bool hasExploded;

		public Missile( Tank Owner, float speed, int lifeTime )
			: base( Owner )
		{
			this.speed = speed;
			this.lifeTime = lifeTime;
		}

		private Missile()
		{
		}

		public override void Initialize( TanksDrop game )
		{
			hasExploded = false;
			base.Initialize( game );
		}

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			Move( speed );
			CheckBounces();
			CheckHits( gameTime, Entities, true );
			base.Update( gameTime, Entities, keyState );
		}

		public override void Destroy( TimeSpan gameTime )
		{
			if ( !hasExploded )
			{
				Explosion explod = new Explosion( gameTime );
				explod.Position = Position;
				explod.Initialize( Game );
				explod.LoadContent( Game.Content, ScreenWidth, ScreenHeight );
				Game.QueueEntity( explod );
				hasExploded = true;
			}
			base.Destroy( gameTime );
		}

		public override void Draw( TimeSpan gameTime, SpriteBatch spriteBatch )
		{
			spriteBatch.Draw( Texture, Position, SourceRectangle, Color.White, AngleInRadians + (float)Math.PI / 2, Origin, Scale, SpriteEffects.None, 0 );
			foreach ( GameController c in Controllers )
			{
				c.Draw( spriteBatch );
			}
		}

		public override void LoadContent( Microsoft.Xna.Framework.Content.ContentManager Content, int screenWidth, int screenHeight )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Missile" );
			Origin = new Vector2( 16, 16 );
			base.LoadContent( Content, screenWidth, screenHeight );
		}

		public override Projectile Clone()
		{
			Missile m = new Missile();
			m.owner = owner;
			m.Position = Position;
			m.Angle = Angle;
			m.Initialize( Game );
			m.speed = speed;
			m.LoadContent( Game.Content, Game.ScreenWidth, Game.ScreenHeight );
			return m;
		}
	}
}
