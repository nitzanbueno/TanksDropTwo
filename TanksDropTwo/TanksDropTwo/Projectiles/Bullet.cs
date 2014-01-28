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
		public Bullet( float Speed, Tank Owner, TimeSpan gameTime, int lifeTime )
			: base( Owner, gameTime )
		{
			this.Speed = Speed;
			Scale = 0.25F;
			Origin = new Vector2( 16, 16 );
			this.lifeTime = lifeTime;
		}

		public Bullet( float Speed, TimeSpan gameTime, int lifeTime ) : this( Speed, Tank.blank, gameTime, lifeTime ) { }

		protected Bullet( float speed ) { this.Speed = speed; }

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, KeyboardState keyState )
		{
			Move( Speed );
			CheckBounces();
			CheckHits( gameTime, Entities );
			base.Update( gameTime, Entities, keyState );
		}

		public override void LoadContent( ContentManager Content, int screenWidth, int screenHeight )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Bullet" );
			base.LoadContent( Content, screenWidth, screenHeight );
		}

		public override Projectile Clone()
		{
			Bullet b = new Bullet( Speed, owner, spawnTime, lifeTime );
			b.Initialize( Game );
			b.Angle = Angle;
			b.Position = Position;
			b.LoadContent( Game.Content, ScreenWidth, ScreenHeight );
			return b;
		}
	}
}
