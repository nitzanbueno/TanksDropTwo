using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

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
		private Color[][] texDatas;
		/// <summary>
		/// Since this is an animated entity, the source rectangles for each frame.
		/// </summary>
		private Rectangle[] TankSourceRects;
		/// <summary>
		/// The name of the tank.
		/// Currently purely aesthetic and useless.
		/// </summary>
		public string Name;
		/// <summary>
		/// The keys of the tank.
		/// </summary>
		private KeySet keys;
		/// <summary>
		/// The color of the tank.
		/// </summary>
		private Colors color;
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
		/// The number of bullets owned by this tank currently on the screen.
		/// </summary>
		public int NumberOfBullets;
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

		public static Tank blank = new Tank();

		public Color Color
		{
			get
			{
				switch ( color )
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

		private Tank() { }

		public Tank( string name, Vector2 startPosition, float startAngle, KeySet keys, Colors color, float speed, Projectile originalProjectile )
		{
			Construct( name, startPosition, startAngle, keys, color, speed, originalProjectile );
		}

		private void Construct( string name, Vector2 startPosition, float startAngle, KeySet keys, Colors color, float speed, Projectile originalProjectile )
		{
			Name = name;
			Speed = speed;
			originalPosition = startPosition;
			originalAngle = startAngle;
			this.keys = keys;
			this.color = color;
			Scale = 2;
			Origin = new Vector2( 16, 16 );
			this.originalProjectile = originalProjectile;
			Reset(false);
		}

		public Tank( string name, Vector2 startPosition, float startAngle, KeySet keys, Colors color, float speed )
		{
			Construct( name, startPosition, startAngle, keys, color, speed, new Bullet( 10, this, TimeSpan.Zero ) );
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
				SourceRectangle = TankSourceRects[ frame ];
				TextureData = texDatas[ frame ];
			}

			Vector2 newPosition = Position;
			float newAngle = Angle;

			if ( keyState.IsKeyDown( keys.KeyForward ) )
			{
				// Move forward
				newPosition = BoundTank( Forward( Speed ) );
			}

			if ( keyState.IsKeyDown( keys.KeyBackward ) )
			{
				// Move backward
				newPosition = Forward( -Speed );
				newPosition = BoundTank( newPosition );
			}

			if ( keyState.IsKeyDown( keys.KeyLeft ) )
			{
				// Turn left
				newAngle -= 5;
			}

			if ( keyState.IsKeyDown( keys.KeyRight ) )
			{
				// Turn right
				newAngle += 5;
			}

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
					// If when I move, I will hit a fence,
					toMove = false;
					// I won't move!
					break;
				}
			}

			// If I don't hit a fence I will move.
			if ( toMove )
			{
				Position = newPosition;
				Angle = newAngle;
			}

			if ( isKeyPressed( keyState, keys.KeyShoot ) && IsAlive )
			{
				Shoot( gameTime );
			}

			if ( isKeyPressed( keyState, keys.KeyPlace ) && IsAlive ) //keyState.IsKeyDown( keys.KeyPlace ) )
			{
				PlaceFence( gameTime );
			}

			base.Update( gameTime, Entities, keyState );
		}

		private void PlaceFence( TimeSpan gameTime )
		{
			float dist = Vector2.Distance( Vector2.Zero, Origin );
			float sideDeg = 40F;
			Fence newFence = new Fence(
				Position + ( new Vector2( ( float )Math.Cos( MathHelper.ToRadians( Angle + sideDeg ) ), ( float )Math.Sin( MathHelper.ToRadians( Angle + sideDeg ) ) ) * dist * 2.5F ),
				Position + ( new Vector2( ( float )Math.Cos( MathHelper.ToRadians( Angle - sideDeg ) ), ( float )Math.Sin( MathHelper.ToRadians( Angle - sideDeg ) ) ) * dist * 2.5F ), this, 16, gameTime, -1 );
			newFence.Initialize( Game );
			Game.QueueEntity( newFence );
		}

		private void Shoot( TimeSpan gameTime )
		{
			nextProjectile.Angle = Angle;
			nextProjectile.Position = Forward( 20 * Scale );
			NumberOfBullets++;
			nextProjectile.Initialize( Game, gameTime );
			Game.QueueEntity( nextProjectile );
			nextProjectile = OriginalProjectile;
		}

		public override void Draw( TimeSpan gameTime, SpriteBatch spriteBatch )
		{
			if ( IsAlive )
			{
				base.Draw( gameTime, spriteBatch );
			}
		}

		public Vector2 BoundTank( Vector2 Pos )
		{
			Pos.X = Tools.Mod( Pos.X + 50, ScreenWidth + 100 ) - 50;
			Pos.Y = Tools.Mod( Pos.Y + 50, ScreenHeight + 100 ) - 50;
			return Pos;
		}

		public override void LoadContent( ContentManager Content, int ScreenWidth, int ScreenHeight )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\TankMap" );
			TankSourceRects = new Rectangle[ 8 ];
			texDatas = new Color[ 8 ][];
			int tankw = Texture.Width / 8;
			int tankh = Texture.Height / 8;
			Color[] tankMapData = new Color[ Texture.Width * Texture.Height ];
			Texture.GetData( tankMapData );
			for ( int x = 0; x < TankSourceRects.Length; x++ )
			{
				TankSourceRects[ x ] = new Rectangle( x * tankw, ( int )color * tankh, tankw, tankh );
				texDatas[ x ] = GetImageData( tankMapData, Texture.Width, TankSourceRects[ x ] );
			}
			SourceRectangle = TankSourceRects[ frame ];
			TextureData = texDatas[ frame ];
			base.LoadContent( Content, ScreenWidth, ScreenHeight );
		}

		/// <summary>
		/// Called when a projectile hits the tank.
		/// </summary>
		/// <param name="Hitter">The projectile that hit the tank.</param>
		/// <returns>True if the tank was killed, otherwise false.</returns>
		public bool Hit( Projectile Hitter )
		{
			IsAlive = false;
			return true;
		}

		Color[] GetImageData( Color[] colorData, int width, Rectangle rectangle )
		{
			Color[] color = new Color[ rectangle.Width * rectangle.Height ];
			for ( int x = 0; x < rectangle.Width; x++ )
				for ( int y = 0; y < rectangle.Height; y++ )
					color[ x + y * rectangle.Width ] = colorData[ x + rectangle.X + ( y + rectangle.Y ) * width ];
			return color;
		}

		public bool PickupProjectile( ProjectilePickup proj )
		{
			if ( nextProjectile.GetType() == OriginalProjectile.GetType() )
			{
				nextProjectile = proj.Carrier;
				return true;
			}
			return false;
		}

		public void Reset(bool proj = true)
		{
			Position = originalPosition;
			Angle = originalAngle;
			if ( proj )
			{
				nextProjectile = OriginalProjectile;
			}
			IsAlive = true;
			NumberOfBullets = 0;
		}
	}
}
