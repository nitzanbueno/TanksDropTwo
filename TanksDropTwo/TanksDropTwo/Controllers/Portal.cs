using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDropTwo.Controllers
{
	public class Portal : ControllerEntity
	{
		bool isDestructed;

		/// <summary>
		/// The portal you leave through when entering this portal.
		/// </summary>
		public Portal otherPortal;

		public Portal( int lifeTime )
		{
			this.lifeTime = lifeTime;
		}

		public override void Initialize( TanksDrop game )
		{
			isDestructed = false;
			base.Initialize( game );
		}

		private int frameTime = 500;

		public int frame;

		TimeSpan timeSinceLastFrameUpdate;

		/// <summary>
		/// True if red portal, false if blue.
		/// </summary>
		public bool isRed;

		public override void OnCollision( GameEntity otherEntity )
		{
			float ang = otherEntity.Angle;
			if ( otherEntity is Tank )
			{
				ang = ( ( Tank )otherEntity ).RelativeAngle;
			}
			if ( otherEntity is Portal ) return;
			otherEntity.Position = otherPortal.Forward( Distance( GameWidth / 2, GameHeight / 2 ) + Distance( otherEntity.GameWidth / 2, otherEntity.GameHeight / 2 ), ang );
		}

		private float Distance( float x, float y )
		{
			return ( float )Math.Sqrt( x * x + y * y );
		}

		public override void Spawn( TimeSpan gameTime, TanksDrop game )
		{
			Random r = new Random();

			Portal redPortal = new Portal( lifeTime );
			Portal bluePortal = new Portal( lifeTime );

			redPortal.isRed = true;

			redPortal.Initialize( game );
			bluePortal.Initialize( game );
			redPortal.Position = new Vector2( r.Next( game.ScreenWidth ), r.Next( game.ScreenHeight ) );
			bluePortal.Position = new Vector2( r.Next( game.ScreenWidth ), r.Next( game.ScreenHeight ) );
			redPortal.LoadContent( game.Content, game.ScreenWidth, game.ScreenHeight );
			bluePortal.LoadContent( game.Content, game.ScreenWidth, game.ScreenHeight );
			redPortal.otherPortal = bluePortal;
			bluePortal.otherPortal = redPortal;
			redPortal.spawnTime = bluePortal.spawnTime = gameTime;

			game.QueueEntity( redPortal, bluePortal );
		}

		public override void LoadContent( Microsoft.Xna.Framework.Content.ContentManager Content, int screenWidth, int screenHeight )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\PortalMap" );
			isTextureAMap = true;
			SourceRectangle = new Rectangle( 0, isRed ? 32 : 0, 32, 32 );
			Scale = 4.0F;
			Origin = new Vector2( 16, 16 );
			base.LoadContent( Content, screenWidth, screenHeight );
		}

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			if ( ( gameTime - timeSinceLastFrameUpdate ).TotalMilliseconds > frameTime )
			{
				timeSinceLastFrameUpdate = gameTime;
				frame += 1;
				frame %= 4;
				SourceRectangle = new Rectangle( frame * 32, isRed ? 32 : 0, 32, 32 );
			}
			base.Update( gameTime, Entities, keyState );
		}

		public override void Destroy( TimeSpan gameTime )
		{
			if ( !isDestructed )
			{
				isDestructed = true;
				otherPortal.Destroy( gameTime );
				base.Destroy( gameTime );
			}
		}
	}
}
