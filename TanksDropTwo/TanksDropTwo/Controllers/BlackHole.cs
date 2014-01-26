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

		public override void Spawn( TimeSpan gameTime, TanksDrop game )
		{
			Random r = new Random();
			BlackHole hole = new BlackHole();
			hole.Position = new Vector2( r.Next( game.ScreenWidth ), r.Next( game.ScreenHeight ) );
			Game = game;
			hole.Controller = new BlackHoleController( hole.Position, hole );
			hole.Initialize( game );
			hole.LoadContent( game.Content, game.ScreenWidth, game.ScreenHeight );
			hole.VacuumSpeed = 0;
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
			VacuumSpeed = Math.Min( VacuumSpeed + 0.1F, 30 );
			base.Update( gameTime, Entities, keyState );
		}

		public override void OnCollision( GameEntity otherEntity )
		{
			// Nothing
		}
	}
}
