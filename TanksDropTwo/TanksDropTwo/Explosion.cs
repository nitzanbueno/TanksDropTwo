using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo
{
	/// <summary>
	/// An explosion effect that kills whomever enters it.
	/// </summary>
	public class Explosion : GameEntity
	{
		bool LoadScale;
		bool Hurt;

		public Explosion( TimeSpan gameTime )
		{
			spawnTime = gameTime;
			lifeTime = 2000;
			LoadScale = true;
			Hurt = true;
		}

		public Explosion( TimeSpan gameTime, int lifeTime, float radius, bool hurt )
		{
			spawnTime = gameTime;
			this.lifeTime = lifeTime;
			this.Scale = radius;
			this.Hurt = hurt;
			LoadScale = false;
		}

		public override void LoadContent( Microsoft.Xna.Framework.Content.ContentManager Content, int screenWidth, int screenHeight )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Boom" );
			Origin = new Vector2( 32, 32 );
			if ( LoadScale ) Scale = ( float )Game.Settings[ "BlastRadius" ].Item2;
			base.LoadContent( Content, screenWidth, screenHeight );
		}

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			foreach ( GameEntity entity in Entities )
			{
				if ( entity.CollidesWith( this ) && entity != this && Hurt )
				{
					entity.Destroy( gameTime );
				}
			}
			base.Update( gameTime, Entities, keyState );
		}
	}
}
