using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDropTwo.Controllers
{
	public class BlackHole : ControllerEntity
	{
		BlackHoleController Controller;
		public float VacuumSpeed;
		private const float MaxVacuum = 100;

		public override void Spawn( TimeSpan gameTime, TanksDrop game )
		{
			Random r = new Random();
			BlackHole hole = new BlackHole();
			Game = game;
			hole.Initialize( game );
			hole.LoadContent( game.Content, game.ScreenWidth, game.ScreenHeight );
			hole.Position = hole.RandomPosition();
			hole.Controller = new BlackHoleController( hole.Position, hole );
			hole.Controller.Initialize( game );
			hole.VacuumSpeed = 0;
			hole.spawnTime = gameTime;
			hole.lifeTime = 10000;
			Game.AppendController( hole.Controller );
			Game.QueueEntity( hole );
		}

		public override void LoadContent( Microsoft.Xna.Framework.Content.ContentManager Content, int screenWidth, int screenHeight )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\BlackHole" );
			Origin = new Vector2( 16, 16 );
			base.LoadContent( Content, screenWidth, screenHeight );
		}

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			VacuumSpeed = Math.Min( VacuumSpeed + 0.5F, MaxVacuum );
			base.Update( gameTime, Entities, keyState );
		}

		public override void OnCollision( GameEntity otherEntity )
		{
			// Nothing
		}

		public void Vanish()
		{
			Game.RemoveEntity( this );
		}

		public override void Destroy( TimeSpan gameTime )
		{
			Game.RemoveController( Controller, x => true );
			base.Destroy( gameTime );
		}
	}
}
