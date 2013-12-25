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
	class Bullet : Projectile
	{
		public Bullet( float Speed, Tank Owner, TimeSpan gameTime )
			: base( Owner, gameTime )
		{
			speed = Speed;
			Scale = 0.25F;
			Origin = new Vector2( 16, 16 );
		}

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, KeyboardState keyState )
		{
			Move( speed );
			CheckBounces();
			UpdatePhysics( Entities );
			CheckDestruction( gameTime, 10000 );
			base.Update( gameTime, Entities, keyState );
		}

		public override void LoadContent( ContentManager Content, int screenWidth, int screenHeight )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Bullet" );
			base.LoadContent( Content, screenWidth, screenHeight );
		}
	}
}
