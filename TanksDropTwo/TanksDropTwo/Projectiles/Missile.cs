using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TanksDropTwo
{
	/// <summary>
	/// A missile that blows up with an explosion after the tank presses the shoot button or destroyed.
	/// </summary>
	public class Missile : Projectile
	{
		bool hasExploded;

		MissileController con;

		Keys keyShoot;

		public Missile( Tank Owner, float speed )
			: base( Owner )
		{
			this.Speed = speed;
			this.lifeTime = -1;
		}

		public Missile( float speed ) : this( Tank.blank, speed ) { }

		public override void Initialize( TanksDrop game, TimeSpan gameTime, Tank owner )
		{
			keyShoot = owner.Keys.KeyShoot;
			con = new MissileController( owner, this, keyShoot );
			owner.Keys.KeyShoot = Keys.None;
			owner.AppendController( con );
			base.Initialize( game, gameTime, owner );
		}

		public override void Initialize( TanksDrop game )
		{
			hasExploded = false;
			base.Initialize( game );
		}

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			Move( Speed );
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
				con.conCount = 1;
			}
			base.Destroy( gameTime );
		}

		public override void Draw( TimeSpan gameTime, SpriteBatch spriteBatch )
		{
			spriteBatch.Draw( Texture, Position, SourceRectangle, Color.White, AngleInRadians + ( float )Math.PI / 2, Origin, Scale, SpriteEffects.None, 0 );
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
			Missile m = new Missile( owner, Speed );
			m.Position = Position;
			m.con = con;
			m.Angle = Angle;
			m.Initialize( Game );
			m.LoadContent( Game.Content, Game.ScreenWidth, Game.ScreenHeight );
			return m;
		}
	}

	/// <summary>
	/// The missile helper controller.
	/// </summary>
	public class MissileController : GameController
	{
		Missile missile;
		Tank owner;
		KeyboardState prevKeyState;
		Keys shoot;
		public int conCount;

		public MissileController( Tank owner, Missile missile, Keys keyShoot )
		{
			this.owner = owner;
			this.missile = missile;
			this.shoot = keyShoot;
			this.prevKeyState = new KeyboardState( keyShoot );
			this.conCount = 0;
		}

		public override bool Control( GameEntity control, TimeSpan gameTime, KeyboardState keyState )
		{
			if ( keyState.IsKeyDown( shoot ) && prevKeyState.IsKeyUp( shoot ) )
			{
				missile.Destroy( gameTime );
			}
			prevKeyState = keyState;
			if ( conCount != 0 )
			{
				conCount++;
			}
			if ( conCount > 5 )
			{
				control.RemoveController( this );
				owner.Keys.KeyShoot = shoot;
			}
			return true;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return entity == owner;
		}

		public override void Draw( SpriteBatch spriteBatch )
		{
		}
	}
}
