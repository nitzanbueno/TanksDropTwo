using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using TanksDropTwo.Controllers;
using Microsoft.Xna.Framework.Audio;

namespace TanksDropTwo
{
	/// <summary>
	/// The basic and only tank usable in game.
	/// It can move and shoot bullets, and in the near future, place fences, take pickups and use power-ups.
	/// </summary>
	public class Tank : GameEntity
	{
		/// <summary>
		/// Since this is an animated entity, the texture datas for each frame.
		/// </summary>
		private Color[ , ][] texDatas;

		/// <summary>
		/// Since this is an animated entity, the source rectangles for each frame.
		/// </summary>
		private Rectangle[ , ] TankSourceRects;

		/// <summary>
		/// The name of the tank.
		/// Currently purely aesthetic and useless.
		/// </summary>
		public string Name;

		/// <summary>
		/// The keys of the tank.
		/// </summary>
		public KeySet Keys;

		/// <summary>
		/// The color of the tank.
		/// </summary>
		public Colors TankColor;

		/// <summary>
		/// The speed of the tank.
		/// </summary>
		public float Speed;

		/// <summary>
		/// true if the tank is alive, otherwise false.
		/// If this is false, the tank is invisiblem cannot shoot or place fences, and does not interact with the projectiles.
		/// </summary>
		public bool IsAlive;

		/// <summary>
		/// The number of projectiles owned by this tank currently on the screen.
		/// </summary>
		public int NumberOfProjectiles;

		/// <summary>
		/// The number of fences owned by this tank currently on the screen.
		/// </summary>
		public int NumberOfFences;

		/// <summary>
		/// The number of times this tank has won the round.
		/// </summary>
		public int Score;

		/// <summary>
		/// The original position of the tank.
		/// </summary>
		private Vector2 originalPosition;

		/// <summary>
		/// The original angle of the tank.
		/// </summary>
		private float originalAngle;

		/// <summary>
		/// The original tank scale.
		/// </summary>
		private float originalScale;

		/// <summary>
		/// The original tank speed.
		/// </summary>
		private float originalSpeed;

		/// <summary>
		/// The projectile loaded in the tank currently.
		/// Used for projectile pickups.
		/// </summary>
		private Projectile nextProjectile;

		/// <summary>
		/// The original projectile of this tank.
		/// Normally a normal bullet, except when testing or badassing.
		/// </summary>
		private Projectile originalProjectile;

		/// <summary>
		/// A useable clone of the original projectile.
		/// Used because if using the original, will set all bullets' position.
		/// </summary>
		public Projectile OriginalProjectile
		{
			get
			{
				return originalProjectile.Clone();
			}
		}

		/// <summary>
		/// The original tank color.
		/// </summary>
		public Colors OriginalColor;

		private KeySet origKeys;

		/// <summary>
		/// The original keys.
		/// </summary>
		public KeySet OriginalKeys
		{
			get
			{
				return origKeys.Clone();
			}

			set
			{
				origKeys = value.Clone();
			}
		}

		/// <summary>
		/// This tank's controller, if exists.
		/// </summary>
		public TankController Controller;

		public int FenceLifeTime;

		public static Tank blank = new Tank();

		public Color Color
		{
			get
			{
				switch ( TankColor )
				{
					case Colors.Aqua:
						return Color.Aqua;
					case Colors.Blue:
						return Color.Blue;
					case Colors.Green:
						return Color.Green;
					case Colors.Orange:
						return Color.Orange;
					case Colors.Pink:
						return Color.Pink;
					case Colors.Purple:
						return Color.Purple;
					case Colors.Red:
						return Color.Red;
					case Colors.Yellow:
						return Color.Yellow;
					default:
						return Color.White;
				}
			}
		}

		private Random r;

		public bool IsGoingBackwards;

		public float RelativeAngle
		{
			get
			{
				return IsGoingBackwards ? ( Angle + 180 ) % 360 : Angle;
			}
		}

		private Tank() { }

		public int ProjectileLimit;

		public int FenceLimit;

		public float TurnSpeed;

		private Texture2D originalTexture;

		public bool AI;

		private SoundEffect powerUpSound;
		private SoundEffect hitSound;
		private SoundEffect shootSound;
		private SoundEffect placeSound;
		private SoundEffect instantSound;

		public Tank( string name, Vector2 startPosition, float startAngle, KeySet keys, Colors color, float speed, Projectile originalProjectile, int BulletLimit, int FenceLimit, int FenceTime, float Scale, bool AI )
		{
			this.Name = name;
			this.Speed = speed;
			this.originalSpeed = speed;
			this.originalPosition = startPosition;
			this.originalAngle = startAngle;
			this.Keys = keys;
			this.OriginalKeys = keys;
			this.TankColor = color;
			this.OriginalColor = color;
			this.Origin = new Vector2( 16, 16 );
			this.originalProjectile = originalProjectile;
			this.ProjectileLimit = BulletLimit;
			this.FenceLimit = FenceLimit;
			this.originalScale = Scale;
			this.Scale = Scale;
			this.FenceLifeTime = FenceTime;
			this.TurnSpeed = 5;
			this.AI = AI;
			this.r = new Random( 10 );
			Reset( false );
		}

		// The timeSpan in which the frame was updated.
		// If now - this > 175 milliseconds, update the frame.
		TimeSpan timeSinceLastFrameUpdate;

		// Tanks have 8 frames each, so this is the current frame.
		int frame = 0;

		public override void Initialize( TanksDrop game )
		{
			originalProjectile.Initialize( game );
			nextProjectile = OriginalProjectile;
			base.Initialize( game );
		}

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, KeyboardState keyState )
		{
			// Update the tank's frame.
			if ( ( gameTime - timeSinceLastFrameUpdate ).TotalMilliseconds > 175 )
			{
				frame++;
				frame %= 8;
				timeSinceLastFrameUpdate = gameTime;
				SourceRectangle = TankSourceRects[ frame, ( int )TankColor ];
				TextureData = texDatas[ frame, ( int )TankColor ];
			}

			if ( AI && IsAlive )
				DoAI( Entities, gameTime );

			Vector2 newPosition = Position;
			float newAngle = Angle;

			if ( keyState.IsKeyDown( Keys.KeyForward ) )
			{
				// Move forward
				newPosition = Bound( Forward( Speed ) );
			}

			if ( keyState.IsKeyDown( Keys.KeyBackward ) )
			{
				// Move backward
				newPosition = Forward( -Speed );
				newPosition = Bound( newPosition );
				IsGoingBackwards = true;
			}
			else
			{
				IsGoingBackwards = false;
			}

			if ( keyState.IsKeyDown( Keys.KeyLeft ) )
			{
				// Turn left
				newAngle -= TurnSpeed;
			}

			if ( keyState.IsKeyDown( Keys.KeyRight ) )
			{
				// Turn right
				newAngle += TurnSpeed;
			}

			// The transformation matrix used to pre-check collision
			Matrix Transformation =
				Matrix.CreateTranslation( new Vector3( -Origin, 0.0f ) ) *
				Matrix.CreateScale( Scale ) *
				Matrix.CreateRotationZ( MathHelper.ToRadians( newAngle ) ) *
				Matrix.CreateTranslation( new Vector3( newPosition, 0.0f ) );

			bool toMove = true;

			foreach ( GameEntity entity in Entities )
			{
				if ( entity is Fence && this.CollidesWith( entity, Transformation ) )
				{
					// If the tank hits a fence if it moves
					toMove = false;
					if ( Controller != null ) // And if the tank has a controller
						toMove = Controller.HitFence( ( Fence )entity ); // that doesn't allow that to happen,
					// The tank will not move.
					// However, due to the ghost controller and other tanks, the tank may be stuck inside a fence.
					// In that case, it should be allowed to escape it.
					toMove = toMove || CollidesWith( entity );
					if ( !toMove )
						break;
				}
			}

			// If I don't hit a fence I will move.
			if ( toMove )
			{
				Position = newPosition;
				Angle = newAngle;
			}

			if ( isKeyPressed( keyState, Keys.KeyShoot ) && IsAlive )
			{
				Shoot( gameTime );
			}

			if ( isKeyPressed( keyState, Keys.KeyPlace ) )
			{
				CheckPlaceFence( gameTime );
			}

			base.Update( gameTime, Entities, keyState );
		}

		//int AIFencePart = 0;

		TimeSpan lastShot = TimeSpan.Zero;

		public void DoAI( HashSet<GameEntity> Entities, TimeSpan gameTime )
		{
			GameEntity ClosestEntity = null;
			double ClosestDistance = ScreenHeight + ScreenWidth;
			foreach ( GameEntity entity in Entities )
			{
				double dist = Vector2.Distance( Position, entity.Position );
				if ( ( ClosestEntity == null || dist < ClosestDistance ) 
					// Specify entities not to count when finding closest entity
					&& entity != this // the tank itself
					&& (entity is Tank) && ((Tank)entity).IsAlive)
					/*&& !( entity is Fence ) // Fences
					&& ( !( entity is Projectile ) || ( Tools.IsGoingTowardsMe( Position, entity.Angle, entity.Position ) && dist < 300 ) ) // Projectiles heading away from the tank
					&& ( !( entity is ProjectilePickup ) || nextProjectile.GetType() == originalProjectile.GetType() ) // Projectile pickups, if the tank is already loaded
					&& ( !( entity is TankControllerPickup ) || Controller == null ) // Power-up pickups, if the tank has one already
					&& ( !( entity is Tank ) || ( ( Tank )entity ).IsAlive ) ) // Dead tanks*/
				{
					ClosestDistance = dist;
					ClosestEntity = entity;
				}
			}

			if ( ClosestEntity == null )
				return;

			if ( Controller is UseableController && r.Next( 100 ) <= 5 )
			{
				CheckPlaceFence( gameTime );
			}

			if ( ClosestEntity is Tank )
			{
				float ang = Home( ClosestEntity.Position, Entities, true );
				if ( ang == 0 || Math.Abs( Tools.Angle( this, ClosestEntity ) - Angle ) <= 10 )
				{
					if ( lastShot == TimeSpan.Zero || ( gameTime - lastShot ).TotalMilliseconds > 1000 )
					{
						//Shoot( gameTime );
						lastShot = gameTime;
					}
				}
				Angle += ang;
				if ( ClosestDistance > 100 )
				{
					// Get closer to the tank in order to be able to react better
					Move( Speed );
				}
			}
			else if ( ClosestEntity is Pickup || ClosestEntity is ControllerEntity )
			{
				float ang = Home( ClosestEntity.Position, Entities, true );
				Angle += ang;
				Move( Speed );
			}
			else if ( ClosestEntity is Projectile )
			{
				float ang = Home( ClosestEntity.Position, Entities, false );
				if ( lastShot == TimeSpan.Zero || ( gameTime - lastShot ).TotalMilliseconds > 500 )
				{
					PlaceFence( gameTime );
					lastShot = gameTime;
				}
				Angle += ang;
			}
		}

		/// <summary>
		/// Returns the angle the tank AI needs to turn to point towards HomingPosition or dodge close fences if required.
		/// </summary>
		/// <param name="HomingPosition">The position to home onto.</param>
		/// <param name="Entities">The entities on the board (used to check fences).</param>
		/// <param name="CheckFences">Whether or not to turn away from fences so the tank won't hit them.</param>
		/// <returns></returns>
		private float Home( Vector2 HomingPosition, HashSet<GameEntity> Entities, bool CheckFences = true )
		{
			// Check for fences

			// Get closest fence within a 500-unit radius that is at most 20 degrees apart from the tank's angle
			Fence ClosestFence = null;
			double ClosestDistance = ScreenHeight + ScreenWidth;
			foreach ( GameEntity entity in Entities )
			{
				double dist = Vector2.Distance( Position, entity.Position );
				bool todo = false;
				if ( ( ClosestFence == null || dist < ClosestDistance ) && entity is Fence )
				{
					if ( Math.Abs( Tools.Angle( this, entity ) - Angle ) < 150 )
					{
						todo = dist < 500;
					}
					else
					{
						todo = false;
					}
				}
				if ( todo )
				{
					ClosestDistance = dist;
					ClosestFence = ( Fence )entity;
				}
			}

			if ( ClosestFence != null )
			{
				// If there is such a fence, return the angle of going away from it, which is the negative of the angle of going towards it
				float ang = Tools.HomeAngle( TurnSpeed, Angle, Position, ClosestFence.Position );
				ang = ang == 0 ? 1 : ang;
				Angle -= ang * (200 / (float)ClosestDistance);
			}

			return Tools.HomeAngle( TurnSpeed, Angle, Position, HomingPosition );
		}

		/// <summary>
		/// Places fence if the controller doesn't do anything.
		/// </summary>
		/// <param name="gameTime">The current game time.</param>
		private void CheckPlaceFence( TimeSpan gameTime )
		{
			if ( ( Controller == null || Controller.OnPlaceFence( gameTime ) ) && IsAlive && ( NumberOfFences < FenceLimit || FenceLimit <= 0 ) )
			{
				PlaceFence( gameTime );
			}
			else if ( IsAlive )
			{
				instantSound.Play();
			}
		}

		/// <summary>
		/// Places a new fence on the board.
		/// </summary>
		/// <param name="gameTime">The current game time.</param>
		private void PlaceFence( TimeSpan gameTime )
		{
			float dist = Vector2.Distance( Vector2.Zero, Origin );
			float sideDeg = 40F;
			Fence newFence = new Fence(
				Position + ( new Vector2( ( float )Math.Cos( MathHelper.ToRadians( Angle + sideDeg ) ), ( float )Math.Sin( MathHelper.ToRadians( Angle + sideDeg ) ) ) * dist * Scale * 1.5F ),
				Position + ( new Vector2( ( float )Math.Cos( MathHelper.ToRadians( Angle - sideDeg ) ), ( float )Math.Sin( MathHelper.ToRadians( Angle - sideDeg ) ) ) * dist * Scale * 1.5F ), this, 16, gameTime, FenceLifeTime );
			newFence.Initialize( Game );
			NumberOfFences++;
			Game.QueueEntity( newFence );
			placeSound.Play();
		}

		/// <summary>
		/// Shoots the pending bullet if either the controller allows, or force is true.
		/// </summary>
		/// <param name="gameTime">The current game time.</param>
		/// <param name="force">True if the tank must shoot, otherwise false.</param>
		public void Shoot( TimeSpan gameTime, bool force = false )
		{
			if ( NumberOfProjectiles >= ProjectileLimit && ProjectileLimit > 0 )
				return;
			nextProjectile.Angle = Angle;
			nextProjectile.Position = Forward( 20 * Scale );
			NumberOfProjectiles++;
			nextProjectile.Initialize( Game, gameTime, this );
			if ( force || Controller == null || !Controller.Shoot( gameTime, nextProjectile ) )
			{
				Game.QueueEntity( nextProjectile );
			}
			nextProjectile = OriginalProjectile;
			shootSound.Play();
		}

		/// <summary>
		/// Draws the tank.
		/// </summary>
		/// <param name="gameTime">The current game time.</param>
		/// <param name="spriteBatch">The SpriteBatch to draw on.</param>
		public override void Draw( TimeSpan gameTime, SpriteBatch spriteBatch )
		{
			if ( IsAlive )
			{
				base.Draw( gameTime, spriteBatch );
			}
			spriteBatch.DrawString( Game.Score, Score.ToString(), originalPosition, Color.White, 0, new Vector2( 16, 16 ), 1, 0, 1 );
		}

		public override void LoadContent( ContentManager Content, int ScreenWidth, int ScreenHeight )
		{
			powerUpSound = Content.Load<SoundEffect>( "powerup" );
			hitSound = Content.Load<SoundEffect>( "hit" );
			shootSound = Content.Load<SoundEffect>( "shoot" );
			placeSound = Content.Load<SoundEffect>( "place" );
			instantSound = Content.Load<SoundEffect>( "instant" );
			Texture = Content.Load<Texture2D>( "Sprites\\TankMap" );
			originalTexture = Content.Load<Texture2D>( "Sprites\\TankMap" );
			isTextureAMap = true;
			TankSourceRects = new Rectangle[ 8, 8 ];
			texDatas = new Color[ 8, 8 ][];
			int tankw = Texture.Width / 8;
			int tankh = Texture.Height / 8;
			Color[] tankMapData = new Color[ Texture.Width * Texture.Height ];
			Texture.GetData( tankMapData );
			for ( int x = 0; x < TankSourceRects.GetLength( 0 ); x++ )
			{
				for ( int y = 0; y < 8; y++ )
				{
					TankSourceRects[ x, y ] = new Rectangle( x * tankw, y * tankh, tankw, tankh );
					texDatas[ x, y ] = GetImageData( tankMapData, Texture.Width, TankSourceRects[ x, y ] );
				}
			}
			SourceRectangle = TankSourceRects[ frame, ( int )TankColor ];
			TextureData = texDatas[ frame, ( int )TankColor ];
			base.LoadContent( Content, ScreenWidth, ScreenHeight );
		}

		/// <summary>
		/// Called when a projectile hits the tank.
		/// </summary>
		/// <param name="Hitter">The projectile that hit the tank.</param>
		/// <returns>True if the tank was killed, otherwise false.</returns>
		public bool ProjectileHit( Projectile Hitter, TimeSpan gameTime )
		{
			if ( Controller == null || Controller.ProjectileHit( Hitter, gameTime ) )
			{
				IsAlive = false;
				hitSound.Play();
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Returns a cropped texture data array from the full one.
		/// </summary>
		/// <param name="colorData">The texture data to crop.</param>
		/// <param name="width">The width of the texture.</param>
		/// <param name="rectangle">The rectangle to crop in texels.</param>
		/// <returns>The cropped data.</returns>
		Color[] GetImageData( Color[] colorData, int width, Rectangle rectangle )
		{
			Color[] color = new Color[ rectangle.Width * rectangle.Height ];
			for ( int x = 0; x < rectangle.Width; x++ )
				for ( int y = 0; y < rectangle.Height; y++ )
					color[ x + y * rectangle.Width ] = colorData[ x + rectangle.X + ( y + rectangle.Y ) * width ];
			return color;
		}

		/// <summary>
		/// Called when the tank should pick up a ProjectilePickup.
		/// </summary>
		/// <param name="proj">The projectile pickup.</param>
		/// <returns>True if the projectile was picked up - otherwise false.</returns>
		public bool PickupProjectile( ProjectilePickup proj )
		{
			if ( nextProjectile.GetType() == OriginalProjectile.GetType() && ( Controller == null || Controller.PickupProjectile( proj ) ) )
			{
				nextProjectile = proj.Carrier.Clone();
				powerUpSound.Play();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Resets the tank to the original form.
		/// </summary>
		/// <param name="proj">Whether or not to reset the projectile.</param>
		public void Reset( bool proj = true )
		{
			Position = originalPosition;
			Angle = originalAngle;
			Scale = originalScale;
			Texture = originalTexture;
			Speed = originalSpeed;
			isTextureAMap = true;
			if ( proj )
			{
				nextProjectile = OriginalProjectile.Clone();
			}
			IsAlive = true;
			NumberOfProjectiles = 0;
			NumberOfFences = 0;
			RemoveTankController();
			Keys = OriginalKeys.Clone();
			Controllers = new HashSet<GameController>();
		}

		/// <summary>
		/// Called when the tank collides with a TankControllerPickup.
		/// </summary>
		/// <param name="tankControllerPickup">The pickup the tank collided with.</param>
		/// <param name="gameTime">The current game time.</param>
		/// <returns>Whether ot not the tank picked up the controller.</returns>
		public bool PickupController( TankControllerPickup tankControllerPickup, TimeSpan gameTime )
		{
			if ( Controller == null )
			{
				TankController controller = ( TankController )tankControllerPickup.Carrier.Clone();
				controller.Initialize( Game, this, gameTime );
				Controllers.Add( controller );
				Controller = controller;
				powerUpSound.Play();
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Removes the current tank controller.
		/// </summary>
		public void RemoveTankController()
		{
			if ( Controller != null )
				Controller.StopControl();
			Controllers.Remove( Controller );
			Controller = null;
		}

		public void SetTankController( TankController t )
		{
			Controllers.Add( t );
			Controller = t;
		}

		public override void Destroy( TimeSpan gameTime )
		{
			if ( Controller == null || Controller.Hit( gameTime ) )
			{
				IsAlive = false;
			}
		}
	}
}
