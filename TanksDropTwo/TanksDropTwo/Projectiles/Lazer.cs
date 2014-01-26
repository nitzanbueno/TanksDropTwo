using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDropTwo
{
	public class Lazer : Projectile
	{
		List<LazerHelper> Helpers;
		LazerHelper lastHelper;
		bool hasCollided;
		IEnumerable<GameEntity> Tanks;

		public Lazer( Tank Owner )
			: base( Owner )
		{
		}

		public Lazer( Tank Owner, TimeSpan gameTime )
			: base( Owner, gameTime )
		{
		}

		public override void Initialize( TanksDrop game )
		{
			Helpers = new List<LazerHelper>();
			lastHelper = new LazerHelper( Position, Angle, game );
			hasCollided = false;
			Tanks = null;
			//lifeTime = 2000;
			base.Initialize( game );
		}

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			if ( Tanks == null )
			{
				Tanks = from e in Entities
						where e is Tank
						select e;
			}

			for ( int i = 0; i < 10; i++ )
			{
				if ( ( Helpers.Count >= 200 || hasCollided ) && Helpers.Count > 0 )
				{
					Helpers.RemoveAt( 0 );
				}
				if ( !hasCollided )
				{
					foreach ( GameEntity e in Tanks )
					{
						if ( e.CollidesWith( lastHelper ) && ( ( Tank )e ).IsAlive && ( ( Tank )e ).Hit( lastHelper ) )
						{
							hasCollided = true;
							break;
						}
					}
					LazerHelper nextHelper = ( LazerHelper )lastHelper.Clone();
					Helpers.Add( lastHelper );
					nextHelper.Move( Entities );
					lastHelper = nextHelper;
				}
			}
			base.Update( gameTime, Entities, keyState );
		}

		public override void Draw( TimeSpan gameTime, SpriteBatch spriteBatch )
		{
			foreach ( LazerHelper l in Helpers )
			{
				l.Draw( gameTime, spriteBatch );
			}
		}

		public override void LoadContent( Microsoft.Xna.Framework.Content.ContentManager Content, int screenWidth, int screenHeight )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Lazer" );
			Origin = new Vector2( 16, 16 );
			base.LoadContent( Content, screenWidth, screenHeight );
		}

		public override Projectile Clone()
		{
			Lazer l = new Lazer( owner );
			l.Angle = Angle;
			l.Position = Position;
			return l;
		}
	}

	public class LazerHelper : Projectile
	{
		public LazerHelper( Vector2 Position, float Angle, TanksDrop Game )
		{
			this.Position = Position;
			this.Angle = Angle;
			Initialize( Game );
			LoadContent( Game.Content, Game.ScreenWidth, Game.ScreenHeight );
		}

		private LazerHelper() { }

		public void Move( HashSet<GameEntity> Entities )
		{
			Move( 1 );
			CheckBounces();
			foreach ( GameEntity entity in Entities )
			{
				if ( entity != this && this.CollidesWith( entity ) && entity is Fence )
				{
					Fence HitFence = ( Fence )entity;
					float fangle = HitFence.Angle < 180 ? HitFence.Angle + 180 : HitFence.Angle;
					Angle = ( 2 * fangle ) - Angle;
					Move( 2 );
				}
			}
		}

		public override void LoadContent( Microsoft.Xna.Framework.Content.ContentManager Content, int screenWidth, int screenHeight )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\RedBullet" );
			Origin = new Vector2( 16, 16 );
			Scale = 0.25F;
			base.LoadContent( Content, screenWidth, screenHeight );
		}

		public override Projectile Clone()
		{
			return new LazerHelper( Position, Angle, Game );
		}
	}

}
