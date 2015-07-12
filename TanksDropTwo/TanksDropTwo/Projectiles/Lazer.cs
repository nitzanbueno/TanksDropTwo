using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDropTwo
{
	/// <summary>
	/// A red laser that bounces off walls and fences quickly, and kills on contact with the tip.
	/// </summary>
	public class Lazer : Projectile
	{
		List<LazerHelper> Helpers;
		LazerHelper lastHelper;
		bool hasCollided;
		IEnumerable<GameEntity> Tanks;
		int Trail;

		public Lazer( Tank Owner )
			: base( Owner )
		{
			this.Speed = 100;
			this.Trail = 200;
		}

		public Lazer( Tank Owner, TimeSpan gameTime )
			: base( Owner, gameTime )
		{
		}

		public Lazer() : this( Tank.blank ) { }

		public Lazer( int lifeTime, int speed, int trail ) : this()
		{
			this.lifeTime = lifeTime;
			this.Speed = speed;
			this.Trail = trail;
		}

		public override void Initialize( TanksDrop game )
		{
			Helpers = new List<LazerHelper>();
			lastHelper = new LazerHelper( Position, Angle, game, owner );
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

			for ( int i = 0; i < Speed; i++ )
			{
				if ( ( Helpers.Count >= Trail || hasCollided ) && Helpers.Count > 0 )
				{
					try
					{
						Helpers.RemoveAt( 0 );
					}
					catch ( Exception ) { }
				}
				if ( Helpers.Count <= 2 && hasCollided )
				{
					Helpers = new List<LazerHelper>();
					lastHelper.Destroy( gameTime );
				}
				if ( !hasCollided )
				{
					foreach ( GameEntity e in Tanks )
					{
						if ( e.CollidesWith( lastHelper ) && ( ( Tank )e ).IsAlive )
						{
							if ( ( ( Tank )e ).ProjectileHit( lastHelper, gameTime ) )
							{
								hasCollided = true;
								break;
							}
						}
					}
					if ( lastHelper.Destroyed )
					{
						hasCollided = true;
						break;
					}
					LazerHelper nextHelper = ( LazerHelper )lastHelper.Clone();
					Helpers.Add( lastHelper );
					nextHelper.Move( Entities );
					Game.RemoveEntity( lastHelper );
					lastHelper = nextHelper;
					Game.QueueEntity( lastHelper );
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
			l.Trail = Trail;
			l.Speed = Speed;
			l.Game = Game;
			l.Angle = Angle;
			l.Position = Position;
			l.lifeTime = lifeTime;
			l.Helpers = Helpers;
			l.lastHelper = lastHelper;
			l.Tanks = Tanks;
			return l;
		}

		public override void Destroy( TimeSpan gameTime )
		{
			lastHelper.Destroy( gameTime );
			base.Destroy( gameTime );
		}
	}

	/// <summary>
	/// A LazerHelper marks a single red dot of the lazer's trail.
	/// </summary>
	public class LazerHelper : Projectile
	{
		public bool Destroyed;

		public LazerHelper( Vector2 Position, float Angle, TanksDrop Game, Tank Owner )
			: base( Owner )
		{
			this.Position = Position;
			this.Angle = Angle;
			Initialize( Game );
			LoadContent( Game.Content, Game.ScreenWidth, Game.ScreenHeight );
			this.Destroyed = false;
			this.lifeTime = lifeTime;
		}

		public void Move( HashSet<GameEntity> Entities )
		{
			Move( 10 );
			CheckBounces();
			foreach ( GameEntity entity in Entities )
			{
				if ( entity is Fence && this.CollidesWith( entity ) )
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
			return new LazerHelper( Position, Angle, Game, owner );
		}

		public override void Destroy( TimeSpan gameTime )
		{
			Destroyed = true;
			base.Destroy( gameTime );
		}
	}

}
