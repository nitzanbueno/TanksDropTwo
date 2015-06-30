using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace TanksDropTwo.Controllers
{
	public class IronDome : TankController
	{
		private int IronLifeTime;
		private float IronSpeed;
		private int Radius;
		private Random rand;
		private int probability;
		private SoundEffect launch;

		public IronDome( int lifeTime, int IronLifeTime, float IronSpeed, int radius, int prob )
			: base( lifeTime )
		{
			this.IronLifeTime = IronLifeTime;
			this.Radius = radius;
			this.IronSpeed = IronSpeed;
			this.probability = prob;
			this.rand = new Random( 10 );
		}

		public override void Initialize( TanksDrop game, Tank Owner )
		{
			base.Initialize( game, Owner );
			Game.AppendController( this );
		}

		public override bool Control( GameEntity control, TimeSpan gameTime, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			if ( Owner.IsAlive && control is Projectile && !( control is IronRocket ) && Vector2.DistanceSquared( control.Position, Owner.Position ) <= Math.Pow( Radius, 2 ) && Tools.IsGoingTowardsMe( Owner.Position, control.Angle, control.Position ) )
			{
				Projectile cont = ( Projectile )control;
				if ( ( !cont.Variables.ContainsKey( "IronRockets" ) || !( ( HashSet<Tank> )cont.Variables[ "IronRockets" ] ).Contains( Owner ) ) )
				{
					if ( rand.Next( 100 ) < probability )
					{
						IronRocket rocket = new IronRocket( IronLifeTime, IronSpeed, cont, Owner.Position, Owner.Angle );
						rocket.Initialize( Game, gameTime, Owner );
						rocket.LoadContent( Game.Content, Game.ScreenWidth, Game.ScreenHeight );
						Game.QueueEntity( rocket );
						launch.Play();
					}
					if ( !cont.Variables.ContainsKey( "IronRockets" ) )
					{
						cont.Variables[ "IronRockets" ] = new HashSet<Tank>();
					}
					( ( HashSet<Tank> )cont.Variables[ "IronRockets" ] ).Add( Owner );
				}
			}
			return base.Control( control, gameTime, keyState );
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\IronDome" );
			Origin = new Vector2( 16, 16 );
			Scale = 2;
			launch = Content.Load<SoundEffect>( "launch" );
		}

		public override bool ProjectileHit( Projectile hitter, TimeSpan gameTime )
		{
			return true;
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
			Game.RemoveController( this, x => true );
		}

		public override GameController Clone()
		{
			IronDome clone = new IronDome( lifeTime, IronLifeTime, IronSpeed, Radius, probability );
			clone.Initialize( Game, Owner );
			clone.LoadTexture( Game.Content );
			return clone;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return entity is Projectile && !( entity is Lazer ) && !( entity is LazerHelper );
		}

		public override void Draw( Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch )
		{
			if ( Owner.IsAlive )
				spriteBatch.Draw( Texture, Owner.Position, null, Color.White, 0, Origin, Owner.Scale * 1.5F, SpriteEffects.None, 1 );
		}

		public override bool PickupProjectile( ProjectilePickup proj )
		{
			return true;
		}
	}

	public class IronRocket : Projectile
	{
		public Projectile Homer;

		public IronRocket( int lifeTime, float speed, Projectile homer, Vector2 Position, float angle )
			: base()
		{
			this.Position = Position;
			this.Angle = angle;
			this.Homer = homer;
			this.Speed = speed;
			this.lifeTime = lifeTime;
		}

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			Move( Speed );
			float ang = Tools.Mod( MathHelper.ToDegrees( ( float )Math.Atan2( Position.Y - Homer.Position.Y, Position.X - Homer.Position.X ) ) + 180, 360 );
			this.Angle = ang;
			if ( this.CollidesWith( Homer ) || Vector2.DistanceSquared( Position, Homer.Position ) < 100 )
			{
				Homer.Destroy( gameTime );
				this.Destroy( gameTime );
			}
			base.Update( gameTime, Entities, keyState );
		}

		public override void LoadContent( Microsoft.Xna.Framework.Content.ContentManager Content, int screenWidth, int screenHeight )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Missile" );
			Origin = new Vector2( 16, 16 );
			base.LoadContent( Content, screenWidth, screenHeight );
		}

		public override void Destroy( TimeSpan gameTime )
		{
			Explosion explod = new Explosion( gameTime, 100, 0.5F, false );
			explod.Position = Position;
			explod.Initialize( Game );
			explod.LoadContent( Game.Content, ScreenWidth, ScreenHeight );
			Game.QueueEntity( explod );
			base.Destroy( gameTime );
		}

		public override void Draw( TimeSpan gameTime, SpriteBatch spriteBatch )
		{
			spriteBatch.Draw( Texture, Position, SourceRectangle, Color.Silver, AngleInRadians + ( float )Math.PI / 2, Origin, Scale, SpriteEffects.None, 0.25F );
			foreach ( GameController c in Controllers )
			{
				c.Draw( spriteBatch );
			}
		}

		public override Projectile Clone()
		{
			IronRocket clone = new IronRocket( lifeTime, Speed, Homer, Position, Angle );
			return clone;
		}
	}
}
