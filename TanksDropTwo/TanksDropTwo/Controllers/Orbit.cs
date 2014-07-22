using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDropTwo.Controllers
{
	public class Orbit : GameController
	{
		Vector2 Position;
		Vector2 Origin;
		float Scale;
		float baseSpeed;
		float maxSpeed;
		float minSpeed;
		float spiralFactor;
		float acceleration;
		Texture2D Texture;

		public Orbit( float baseSpeed, float maxSpeed, float minSpeed, float acceleration, float spiralFactor )
		{
			this.baseSpeed = baseSpeed;
			this.maxSpeed = maxSpeed;
			this.minSpeed = minSpeed;
			this.acceleration = acceleration;
			this.spiralFactor = spiralFactor;
		}

		public override bool Control( GameEntity control, TimeSpan gameTime, KeyboardState keyState )
		{
			if ( !control.Variables.ContainsKey( "VacuumSpeed" ) )
			{
				control.Variables[ "VacuumSpeed" ] = baseSpeed;
			}

			float vacSpeed = ( float )control.Variables[ "VacuumSpeed" ];
			float d = Vector2.Distance( Position, control.Position );
			if ( d <= 16 * Scale )
			{
				control.Variables.Remove( "VacuumSpeed" );
				control.ForceDestroy();
			}
			else
			{
				control.Move( vacSpeed, Tools.Angle( control.Position, Position ) );
				control.Move( vacSpeed + spiralFactor, Tools.Angle( control.Position, Position ) + 90 );
			}
			vacSpeed += acceleration;
			if ( vacSpeed > maxSpeed && maxSpeed >= 0 )
			{
				vacSpeed = maxSpeed;
			}
			if ( vacSpeed < minSpeed && minSpeed >= 0 )
			{
				vacSpeed = minSpeed;
			}
			control.Variables[ "VacuumSpeed" ] = vacSpeed;
			return true;
		}

		public void LoadContent( Microsoft.Xna.Framework.Content.ContentManager Content, int screenWidth, int screenHeight )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\BlackHole" );
			Origin = new Vector2( 16, 16 );
			Scale = 5;
		}

		public override void Initialize( TanksDrop game )
		{
			Position = new Vector2( game.ScreenWidth / 2, game.ScreenHeight / 2 );
			LoadContent( game.Content, game.ScreenWidth, game.ScreenHeight );
		}

		public override bool AddEntity( GameEntity entity )
		{
			return !( entity is Lazer );
		}

		float ang;

		public override void Draw( SpriteBatch spriteBatch )
		{
			spriteBatch.Draw( Texture, Position, null, Color.White, ang, Origin, Scale, SpriteEffects.None, 1F );
		}
	}
}
