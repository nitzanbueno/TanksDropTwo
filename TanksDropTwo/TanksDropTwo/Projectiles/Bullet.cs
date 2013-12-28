using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace TanksDropTwo
{
	/// <summary>
	/// A projectile which is black and kills the tank it first hits, including its owner.
	/// </summary>
	class Bullet : Projectile
	{
		public Bullet( float Speed, Tank Owner, TimeSpan gameTime )
			: base( Owner, gameTime )
		{
			speed = Speed;
			Scale = 0.25F;
			Origin = new Vector2( 16, 16 );
		}

		protected Bullet( float speed ) { this.speed = speed; }

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, KeyboardState keyState )
		{
			Move( speed );
			CheckBounces();
			CheckHits( Entities );
			CheckDestruction( gameTime, 10000 );
			base.Update( gameTime, Entities, keyState );
		}

		public override void LoadContent( ContentManager Content, int screenWidth, int screenHeight )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Bullet" );
			base.LoadContent( Content, screenWidth, screenHeight );
		}

		public override Projectile Clone()
		{
			Bullet b = new Bullet( speed, owner, spawnTime );
			b.Initialize( Game );
			b.Position = Position;
			b.LoadContent( Game.Content, ScreenWidth, ScreenHeight );
			return b;
		}
	}
}
