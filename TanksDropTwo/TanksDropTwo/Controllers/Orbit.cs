using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDropTwo.Controllers
{
	/// <summary>
	/// Orbit is a Sudden Death that causes a supermassive black hole to be formed in the middle of the screen that causes all entities to orbit it.
	/// </summary>
	public class Orbit : GameController
	{
		/// <summary>
		/// The black hole's position.
		/// </summary>
		Vector2 Position;
		/// <summary>
		/// The origin of te black hole.
		/// </summary>
		Vector2 Origin;
		/// <summary>
		/// The scale of the black hole.
		/// </summary>
		float Scale;
		/// <summary>
		/// The pull speed that the black hole starts with.
		/// </summary>
		float baseSpeed;
		/// <summary>
		/// The maximum pull speed that the black hole can have.
		/// </summary>
		float maxSpeed;
		/// <summary>
		/// The minimum pull speed that the black hole can have.
		/// </summary>
		float minSpeed;
		/// <summary>
		/// The number that you add to get the turn speed.
		/// </summary>
		float spiralFactor;
		/// <summary>
		/// The number added each frame to the pull speed.
		/// </summary>
		float acceleration;
		/// <summary>
		/// The texture of the black hole.
		/// </summary>
		Texture2D Texture;

		/// <summary>
		/// Initializes a new Orbit class.
		/// </summary>
		/// <param name="baseSpeed">The pull speed that the black hole starts with.</param>
		/// <param name="maxSpeed">The maximum pull speed that the black hole can have.</param>
		/// <param name="minSpeed">The minimum pull speed that the black hole can have.</param>
		/// <param name="acceleration">The number added each frame to the pull speed.</param>
		/// <param name="spiralFactor">The number that you add to the pull speed to get the turn speed.</param>
		public Orbit( float baseSpeed, float maxSpeed, float minSpeed, float acceleration, float spiralFactor )
		{
			// Initialize all values
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

			float vacSpeed = ( float )control.Variables[ "VacuumSpeed" ]; // The current pull speed
			float d = Vector2.Distance( Position, control.Position ); // The current distance
			if ( d <= 16 * Scale ) // if the distance is smaller than that value, the entity is inside the black hole and therefore should be destroyed.
			{
				// Destroy
				control.Variables.Remove( "VacuumSpeed" ); 
				control.ForceDestroy();
			}
			else // Else the control is outside so do the orbit
			{
				control.Move( vacSpeed, Tools.Angle( control.Position, Position ) ); // The pull
				control.Move( vacSpeed + spiralFactor, Tools.Angle( control.Position, Position ) + 90 ); // The turn
			}
			// Accelerate the pull speed
			vacSpeed += acceleration;
			if ( vacSpeed > maxSpeed && maxSpeed >= 0 )
			{
				// Limit the pull speed from the top
				vacSpeed = maxSpeed;
			}
			if ( vacSpeed < minSpeed && minSpeed >= 0 )
			{
				// Limit the pull speed from the bottom
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
			Position = new Vector2( game.ScreenWidth / 2, game.ScreenHeight / 2 ); // Center
			LoadContent( game.Content, game.ScreenWidth, game.ScreenHeight );
		}

		public override bool AddEntity( GameEntity entity )
		{
			// Lazers glitch out so they are not included
			return !( entity is Lazer );
		}

		public override void Draw( SpriteBatch spriteBatch )
		{
			// Draw the black hole in the middle of the screen
			spriteBatch.Draw( Texture, Position, null, Color.White, 0, Origin, Scale, SpriteEffects.None, 1F );
		}
	}
}
