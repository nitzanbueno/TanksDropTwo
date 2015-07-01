using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace TanksDropTwo.Controllers
{
	public class Roulette : UseableController
	{
		List<Tank> Tanks;
		Tank chosenTank;
		SoundEffect reverseExplod;

		public override void Initialize( TanksDrop game )
		{
			Tanks = new List<Tank>();
			reverseExplod = game.Content.Load<SoundEffect>( "dolpxe" );
			base.Initialize( game );
		}

		public override void InstantControl( GameEntity control, TimeSpan gameTime )
		{
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Roulette" );
			Origin = new Vector2( 16, 16 );
			Scale = 2.0F;
		}

		public override GameController Clone()
		{
			return new Roulette();
		}

		public override bool AddEntity( GameEntity entity )
		{
			if ( entity is Tank )
			{
				Tanks.Add( ( Tank )entity );
			}
			return false;
		}

		public override void InstantAction( TimeSpan gameTime )
		{
			Random r = new Random();
			chosenTank = Tanks[ r.Next( Tanks.Count ) ];
			if ( chosenTank.IsAlive )
			{
				Explosion explod = new Explosion( gameTime );
				explod.Initialize( Game );
				explod.LoadContent( Game.Content, Game.ScreenWidth, Game.ScreenHeight );
				explod.Position = chosenTank.Position;
				Game.QueueEntity( explod );
				Game.explod.Play();
				chosenTank.Destroy( gameTime );
			}
			else
			{
				Explosion explod = new Explosion( gameTime, 2000, ( float )Game.Settings[ "BlastRadius" ].Item2, false );
				explod.Initialize( Game );
				explod.LoadContent( Game.Content, Game.ScreenWidth, Game.ScreenHeight );
				explod.Position = chosenTank.Position;
				Game.QueueEntity( explod );
				reverseExplod.Play();
				chosenTank.IsAlive = true;
			}
		}
	}
}
