using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace TanksDropTwo
{
	/// <summary>
	/// The parent class for anything that is seen on the screen, such as tanks, bullets and pickups.
	/// </summary>
	public abstract class GameEntity
	{
		protected int ScreenWidth;
		protected int ScreenHeight;

		/// <summary>
		/// The angle in degrees, used privately to always give an angle between 0 to 360.
		/// </summary>
		private float angle;

		/// <summary>
		/// Position on the game board.
		/// </summary>
		public Vector2 Position;

		/// <summary>
		/// The angle of rotation in DEGREES.
		/// Always between 0 to 360.
		/// </summary>
		public float Angle
		{
			get
			{
				return angle;
			}

			set
			{
				float correctAngle = value % 360;
				while ( correctAngle < 0 )
				{
					correctAngle += 360;
				}
				angle = correctAngle;
			}
		}

		public float AngleInRadians
		{
			get
			{
				return MathHelper.ToRadians( angle );
			}
		}

		/// <summary>
		/// The scale in relation to the texture.
		/// </summary>
		public float Scale;

		/// <summary>
		/// The texture of the entity.
		/// </summary>
		public Texture2D Texture;

		/// <summary>
		/// Moves the entity forward the specified number of pixels.
		/// </summary>
		/// <param name="speed">The amount to move in pixels.
		/// If negative, moves backwards.</param>
		public void Move( float speed )
		{
			Position = Forward( speed );
		}

		/// <summary>
		/// Returns the entity's position moved forward the given amount of pixels in its angle's direction.
		/// </summary>
		/// <param name="speed">The amount of pixels to move</param>
		/// <returns>The moved position.</returns>
		protected Vector2 Forward( float speed )
		{
			return Position + ( new Vector2( ( float )Math.Cos( MathHelper.ToRadians( Angle ) ), ( float )Math.Sin( MathHelper.ToRadians( Angle ) ) ) * speed );
		}

		/// <summary>
		/// Updates the entity using its logic.
		/// </summary>
		/// <param name="gameTime">The current game time.</param>
		/// <param name="Entities">The other entities present on board.</param>
		/// <param name="keyState">The current keyboard state.</param>
		public abstract void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, KeyboardState keyState );

		/// <summary>
		/// Draws the entity.
		/// </summary>
		/// <param name="gameTime">The current game time.</param>
		/// <param name="spriteBatch">The SpriteBatch to draw on. Already begun.</param>
		public abstract void Draw( TimeSpan gameTime, SpriteBatch spriteBatch );

		/// <summary>
		/// Loads the entity's content.
		/// </summary>
		/// <param name="Content">The ContentManager to load the content from.</param>
		public virtual void LoadContent( ContentManager Content, int screenWidth, int screenHeight )
		{
			ScreenWidth = screenWidth;
			ScreenHeight = screenHeight;
		}
	}
}
