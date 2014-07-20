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
		/// <summary>
		/// The ID of the entity, used for dictionaries.
		/// </summary>
		public int ID;

		/// <summary>
		/// The width of the screen.
		/// </summary>
		protected int ScreenWidth;

		/// <summary>
		/// The height of the screen.
		/// </summary>
		protected int ScreenHeight;

		/// <summary>
		/// The time, in milliseconds, the entity stays on the board.
		/// -1 means unlimited time.
		/// </summary>
		protected int lifeTime;

		/// <summary>
		/// The TimeSpan in which the entity has appeared on the board.
		/// </summary>
		protected TimeSpan spawnTime;

		/// <summary>
		/// Every update, the controllers mentioned here change the entity according to their needs.
		/// If all controllers allow the update, the entity is updated using the Update function.
		/// </summary>
		protected HashSet<GameController> Controllers = new HashSet<GameController>();

		/// <summary>
		/// The current game this entity is in.
		/// </summary>
		protected TanksDrop Game;

		/// <summary>
		/// The rectangle the texture is cropped from. If size is 0, uses the texture rectangle.
		/// </summary>
		public Rectangle? SourceRectangle;

		/// <summary>
		/// Allows controllers to store individual variables inside the entity.
		/// </summary>
		public Dictionary<string, object> Variables;

		/// <summary>
		/// The texture, as a color array.
		/// </summary>
		public Color[] TextureData;

		/// <summary>
		/// The angle in degrees, used privately to always give an angle between 0 to 360.
		/// </summary>
		private float angle;

		/// <summary>
		/// The current width of the entity, on the screen, in pixels.
		/// </summary>
		public int GameWidth
		{
			get
			{
				if ( SourceRectangle.HasValue )
				{
					return ( int )( SourceRectangle.Value.Width * Scale );
				}
				return ( int )( Texture.Width * Scale );
			}
		}

		/// <summary>
		/// The current height of the entity, on the screen, in pixels.
		/// </summary>
		public int GameHeight
		{
			get
			{
				if ( SourceRectangle.HasValue )
				{
					return ( int )( SourceRectangle.Value.Height * Scale );
				}
				return ( int )( Texture.Height * Scale );
			}
		}

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

		/// <summary>
		/// The angle of the entity in radians.
		/// </summary>
		public float AngleInRadians
		{
			get
			{
				return MathHelper.ToRadians( angle );
			}
		}

		/// <summary>
		/// Initializes the entity.
		/// </summary>
		/// <param name="game">The current game the entity exists in.</param>
		public virtual void Initialize( TanksDrop game )
		{
			ScreenWidth = game.ScreenWidth;
			ScreenHeight = game.ScreenHeight;
			Game = game;
			ID = Game.CurrentID;
			Game.CurrentID++;
		}

		/// <summary>
		/// Gives the position of the entity when bounded to the dimensions of the screen.
		/// </summary>
		/// <param name="Pos">The current position.</param>
		/// <returns>The bounded position.</returns>
		public Vector2 Bound( Vector2 Pos )
		{
			Pos.X = Tools.Mod( Pos.X + 50, ScreenWidth + 100 ) - 50;
			Pos.Y = Tools.Mod( Pos.Y + 50, ScreenHeight + 100 ) - 50;
			return Pos;
		}

		/// <summary>
		/// The scale in relation to the texture.
		/// </summary>
		public float Scale = 1;

		/// <summary>
		/// The texture of the entity.
		/// </summary>
		public Texture2D Texture;

		/// <summary>
		/// The origin of rotation in relation to the unscaled texture.
		/// </summary>
		public Vector2 Origin;

		public GameEntity()
		{
			Variables = new Dictionary<string, object>();
		}

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
		/// Moves the entity forward in the given angle.
		/// </summary>
		/// <param name="speed">The speed of movement.</param>
		/// <param name="angle">The angle of movement, in degrees..</param>
		public void Move( float speed, float angle )
		{
			Position = Forward( speed, angle );
		}

		/// <summary>
		/// Returns the entity's position moved forward the given amount of pixels in its angle's direction.
		/// </summary>
		/// <param name="speed">The amount of pixels to move</param>
		/// <returns>The moved position.</returns>
		public Vector2 Forward( float speed, float angle )
		{
			return Position + ( new Vector2( ( float )Math.Cos( MathHelper.ToRadians( angle ) ), ( float )Math.Sin( MathHelper.ToRadians( angle ) ) ) * speed );
		}

		// The previous key state, used to check key presses.
		private KeyboardState prevKey;

		/// <summary>
		/// Updates the entity, but only if its controllers allow it.
		/// </summary>
		/// <param name="gameTime">The current game time.</param>
		/// <param name="Entities">The other entities present on board.</param>
		/// <param name="keyState">The current keyboard state.</param>
		public void ConUpdate( TimeSpan gameTime, HashSet<GameEntity> Entities, KeyboardState keyState )
		{
			bool ShouldUpdate = true;
			HashSet<GameController> ControllersCopy = new HashSet<GameController>( Controllers );
			foreach ( GameController controller in ControllersCopy )
			{
				ShouldUpdate = ShouldUpdate && controller.Control( this, gameTime, keyState );
			}
			if ( ShouldUpdate )
			{
				Update( gameTime, Entities, keyState );
			}
		}

		/// <summary>
		/// Updates the entity using its logic.
		/// </summary>
		/// <param name="gameTime">The current game time.</param>
		/// <param name="Entities">The other entities present on board.</param>
		/// <param name="keyState">The current keyboard state.</param>
		public virtual void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, KeyboardState keyState )
		{
			prevKey = keyState;
			if ( lifeTime > 0 && ( gameTime - spawnTime ).TotalMilliseconds > lifeTime )
			{
				Destroy( gameTime );
			}
		}

		/// <summary>
		/// Returns true if the given key was pressed (was pushed this frame) and false if not.
		/// </summary>
		/// <param name="state">The current keyboard state.</param>
		/// <param name="key">The key to check.</param>
		protected bool isKeyPressed( KeyboardState state, Keys key )
		{
			return state.IsKeyDown( key ) && prevKey.IsKeyUp( key );
		}

		/// <summary>
		/// Draws the entity.
		/// </summary>
		/// <param name="gameTime">The current game time.</param>
		/// <param name="spriteBatch">The SpriteBatch to draw on. Already begun.</param>
		public virtual void Draw( TimeSpan gameTime, SpriteBatch spriteBatch )
		{
			spriteBatch.Draw( Texture, Position, SourceRectangle, Color.White, AngleInRadians, Origin, Scale, SpriteEffects.None, 0.25F );
			foreach ( GameController c in Controllers )
			{
				c.Draw( spriteBatch );
			}
		}

		/// <summary>
		/// Loads the entity's content.
		/// </summary>
		/// <param name="Content">The ContentManager to load the content from.</param>
		public virtual void LoadContent( ContentManager Content, int screenWidth, int screenHeight )
		{
			ScreenWidth = screenWidth;
			ScreenHeight = screenHeight;

			if ( TextureData == null )
			{
				TextureData =
					new Color[ Texture.Width * Texture.Height ];
				Texture.GetData( TextureData );
			}
		}

		public bool CollidesWith( GameEntity otherEntity )
		{
			Tuple<int, int> key = Tuple.Create<int, int>( Math.Min( ID, otherEntity.ID ), Math.Max( ID, otherEntity.ID ) );
			if ( Game.Collisions.ContainsKey( key ) )
			{
				return Game.Collisions[ key ];
			}
			else
			{
				bool result = CollisionCheck( otherEntity );
				Game.Collisions[ key ] = result;
				return result;
			}
		}

		public bool CollisionCheck( GameEntity otherEntity )
		{
			return CollisionCheck( otherEntity, Texture, TextureData, SourceRectangle );
		}

		// Copied from the internet, and set to match criteria.
		/// <summary>
		/// Returns whether or not this entity and the given one collide, pixel-wise.
		/// </summary>
		/// <param name="otherEntity">The entity to check collision with.</param>
		/// <returns>Whether or not the two entities touch.</returns>
		public bool CollisionCheck( GameEntity otherEntity, Texture2D Tex, Color[] TexD, Rectangle? sr )
		{
			int thisWidth = Tex.Width;
			int thisHeight = Tex.Height;
			if ( sr.HasValue )
			{
				thisWidth = sr.Value.Width;
				thisHeight = sr.Value.Height;
			}

			int otherWidth = otherEntity.Texture.Width;
			int otherHeight = otherEntity.Texture.Height;

			if ( otherEntity.SourceRectangle.HasValue )
			{
				otherWidth = otherEntity.SourceRectangle.Value.Width;
				otherHeight = otherEntity.SourceRectangle.Value.Height;
			}

			Matrix thisTransform =
				Matrix.CreateTranslation( new Vector3( -Origin, 0.0f ) ) *
				Matrix.CreateScale( Scale ) *
				Matrix.CreateRotationZ( AngleInRadians ) *
				Matrix.CreateTranslation( new Vector3( Position, 0.0f ) );

			Rectangle thisRect = CalculateBoundingRectangle( new Rectangle( 0, 0, thisWidth, thisHeight ), thisTransform );

			// Build the other entity's transform
			Matrix otherTransform =
				Matrix.CreateTranslation( new Vector3( -otherEntity.Origin, 0.0f ) ) *
				Matrix.CreateScale( otherEntity.Scale ) *
				Matrix.CreateRotationZ( otherEntity.AngleInRadians ) *
				Matrix.CreateTranslation( new Vector3( otherEntity.Position, 0.0f ) );

			// Calculate the bounding rectangle of the other entity in world space
			Rectangle otherRect = CalculateBoundingRectangle( new Rectangle( 0, 0, otherWidth, otherHeight ), otherTransform );

			// The per-pixel check is expensive, so check the bounding rectangles
			// first to prevent testing pixels when collisions are impossible.
			if ( thisRect.Intersects( otherRect ) )
			{
				// Check collision with entity
				return IntersectPixels( thisTransform, thisWidth,
									thisHeight, TexD,
									otherTransform, otherWidth,
									otherHeight, otherEntity.TextureData );
			}
			return false;
		}

		/// <summary>
		/// Returns true if the entity collides with the other entity, AFTER the current entity has undergone the given transformation.
		/// </summary>
		/// <param name="otherEntity">The entity to check collision with.</param>
		/// <param name="Transformation">The transformation to apply.</param>
		/// <returns></returns>
		protected bool CollidesWith( GameEntity otherEntity, Matrix Transformation )
		{
			int thisWidth = Texture.Width;
			int thisHeight = Texture.Height;
			if ( SourceRectangle.HasValue )
			{
				thisWidth = SourceRectangle.Value.Width;
				thisHeight = SourceRectangle.Value.Height;
			}

			int otherWidth = otherEntity.Texture.Width;
			int otherHeight = otherEntity.Texture.Height;

			if ( otherEntity.SourceRectangle.HasValue )
			{
				otherWidth = otherEntity.SourceRectangle.Value.Width;
				otherHeight = otherEntity.SourceRectangle.Value.Height;
			}

			Matrix thisTransform = Transformation;

			Rectangle thisRect = CalculateBoundingRectangle( new Rectangle( 0, 0, thisWidth, thisHeight ), thisTransform );

			// Build the other entity's transform
			Matrix otherTransform =
				Matrix.CreateTranslation( new Vector3( -otherEntity.Origin, 0.0f ) ) *
				Matrix.CreateScale( otherEntity.Scale ) *
				Matrix.CreateRotationZ( otherEntity.AngleInRadians ) *
				Matrix.CreateTranslation( new Vector3( otherEntity.Position, 0.0f ) );

			// Calculate the bounding rectangle of the other entity in world space
			Rectangle otherRect = CalculateBoundingRectangle( new Rectangle( 0, 0, otherWidth, otherHeight ), otherTransform );

			// The per-pixel check is expensive, so check the bounding rectangles
			// first to prevent testing pixels when collisions are impossible.
			if ( thisRect.Intersects( otherRect ) )
			{
				// Check collision with entity
				return IntersectPixels( thisTransform, thisWidth,
									thisHeight, TextureData,
									otherTransform, otherWidth,
									otherHeight, otherEntity.TextureData );
			}
			return false;
		}

		// Copied from the internet.
		/// <summary>
		/// Calculates an axis aligned rectangle which fully contains an arbitrarily
		/// transformed axis aligned rectangle.
		/// </summary>
		/// <param name="rectangle">Original bounding rectangle.</param>
		/// <param name="transform">World transform of the rectangle.</param>
		/// <returns>A new rectangle which contains the trasnformed rectangle.</returns>
		public Rectangle CalculateBoundingRectangle( Rectangle rectangle, Matrix transform )
		{
			// Get all four corners in local space
			Vector2 leftTop = new Vector2( rectangle.Left, rectangle.Top );
			Vector2 rightTop = new Vector2( rectangle.Right, rectangle.Top );
			Vector2 leftBottom = new Vector2( rectangle.Left, rectangle.Bottom );
			Vector2 rightBottom = new Vector2( rectangle.Right, rectangle.Bottom );

			// Transform all four corners into work space
			Vector2.Transform( ref leftTop, ref transform, out leftTop );
			Vector2.Transform( ref rightTop, ref transform, out rightTop );
			Vector2.Transform( ref leftBottom, ref transform, out leftBottom );
			Vector2.Transform( ref rightBottom, ref transform, out rightBottom );

			// Find the minimum and maximum extents of the rectangle in world space
			Vector2 min = Vector2.Min( Vector2.Min( leftTop, rightTop ),
									  Vector2.Min( leftBottom, rightBottom ) );
			Vector2 max = Vector2.Max( Vector2.Max( leftTop, rightTop ),
									  Vector2.Max( leftBottom, rightBottom ) );

			// Return that as a rectangle
			return new Rectangle( ( int )min.X, ( int )min.Y,
								 ( int )( max.X - min.X ), ( int )( max.Y - min.Y ) );
		}

		// Copied from the internet.
		/// <summary>
		/// Determines if there is overlap of the non-transparent pixels
		/// between two sprites.
		/// </summary>
		/// <param name="rectangleA">Bounding rectangle of the first sprite</param>
		/// <param name="dataA">Pixel data of the first sprite</param>
		/// <param name="rectangleB">Bouding rectangle of the second sprite</param>
		/// <param name="dataB">Pixel data of the second sprite</param>
		/// <returns>True if non-transparent pixels overlap; false otherwise</returns>
		public bool IntersectPixels( Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB )
		{
			// Find the bounds of the rectangle intersection
			int top = Math.Max( rectangleA.Top, rectangleB.Top );
			int bottom = Math.Min( rectangleA.Bottom, rectangleB.Bottom );
			int left = Math.Max( rectangleA.Left, rectangleB.Left );
			int right = Math.Min( rectangleA.Right, rectangleB.Right );

			// Check every point within the intersection bounds
			for ( int y = top; y < bottom; y++ )
			{
				for ( int x = left; x < right; x++ )
				{
					// Get the color of both pixels at this point
					Color colorA = dataA[ ( x - rectangleA.Left ) +
										 ( y - rectangleA.Top ) * rectangleA.Width ];
					Color colorB = dataB[ ( x - rectangleB.Left ) +
										 ( y - rectangleB.Top ) * rectangleB.Width ];

					// If both pixels are not completely transparent,
					if ( colorA.A != 0 && colorB.A != 0 )
					{
						// then an intersection has been found
						return true;
					}
				}
			}

			// No intersection found
			return false;
		}

		// Copied from the internet.
		/// <summary>
		/// Determines if there is overlap of the non-transparent pixels between two
		/// sprites.
		/// </summary>
		/// <param name="transformA">World transform of the first sprite.</param>
		/// <param name="widthA">Width of the first sprite's texture.</param>
		/// <param name="heightA">Height of the first sprite's texture.</param>
		/// <param name="dataA">Pixel color data of the first sprite.</param>
		/// <param name="transformB">World transform of the second sprite.</param>
		/// <param name="widthB">Width of the second sprite's texture.</param>
		/// <param name="heightB">Height of the second sprite's texture.</param>
		/// <param name="dataB">Pixel color data of the second sprite.</param>
		/// <returns>True if non-transparent pixels overlap; false otherwise</returns>
		public bool IntersectPixels( Matrix transformA, int widthA, int heightA, Color[] dataA, Matrix transformB, int widthB, int heightB, Color[] dataB )
		{
			// Calculate a matrix which transforms from A's local space into
			// world space and then into B's local space
			Matrix transformAToB = transformA * Matrix.Invert( transformB );

			// When a point moves in A's local space, it moves in B's local space with a
			// fixed direction and distance proportional to the movement in A.
			// This algorithm steps through A one pixel at a time along A's X and Y axes
			// Calculate the analogous steps in B:
			Vector2 stepX = Vector2.TransformNormal( Vector2.UnitX, transformAToB );
			Vector2 stepY = Vector2.TransformNormal( Vector2.UnitY, transformAToB );

			// Calculate the top left corner of A in B's local space
			// This variable will be reused to keep track of the start of each row
			Vector2 yPosInB = Vector2.Transform( Vector2.Zero, transformAToB );

			// For each row of pixels in A
			for ( int yA = 0; yA < heightA; yA++ )
			{
				// Start at the beginning of the row
				Vector2 posInB = yPosInB;

				// For each pixel in this row
				for ( int xA = 0; xA < widthA; xA++ )
				{
					// Round to the nearest pixel
					int xB = ( int )Math.Round( posInB.X );
					int yB = ( int )Math.Round( posInB.Y );

					// If the pixel lies within the bounds of B
					if ( 0 <= xB && xB < widthB &&
						0 <= yB && yB < heightB )
					{
						// Get the colors of the overlapping pixels
						Color colorA = dataA[ xA + yA * widthA ];
						Color colorB = dataB[ xB + yB * widthB ];

						// If both pixels are not completely transparent,
						if ( colorA.A != 0 && colorB.A != 0 )
						{
							// then an intersection has been found
							return true;
						}
					}

					// Move to the next pixel in the row
					posInB += stepX;
				}

				// Move to the next row
				yPosInB += stepY;
			}

			// No intersection found
			return false;
		}

		/// <summary>
		/// Adds the controller to the entity's controllers, if not already present.
		/// </summary>
		/// <param name="controller">The controller to add.</param>
		/// <returns>True if the controller was added, false if it already existed.</returns>
		public bool AppendController( GameController controller )
		{
			if ( Controllers.Contains( controller ) )
			{
				return false;
			}
			Controllers.Add( controller );
			return true;
		}

		/// <summary>
		/// Removes the controller from the entity's controllers, if present.
		/// </summary>
		/// <param name="controller">The controller to remove.</param>
		/// <returns>True if the controller was removed, false if it doesn't exist.</returns>
		public bool RemoveController( GameController controller )
		{
			if ( Controllers.Contains( controller ) )
			{
				Controllers.Remove( controller );
				return true;
			}
			return false;
		}

		/// <summary>
		/// Returns whether or not the given controller controls this entity.
		/// </summary>
		/// <param name="controller">The controller to check.</param>
		/// <returns></returns>
		public bool HasController( GameController controller )
		{
			return Controllers.Contains( controller );
		}

		public Vector2 RandomPosition()
		{
			Random r = new Random();
			return new Vector2( r.Next( ScreenWidth ), r.Next( ScreenHeight ) );
		}

		public virtual void Destroy( TimeSpan gameTime )
		{
			Game.RemoveEntity( this );
		}
	}
}
