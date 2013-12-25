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
		private Color[][] texDatas;
		private Rectangle[] TankSourceRects;
		public string Name;
		private KeySet keys;
		private Colors color;
		public float Speed;
		public bool IsAlive;
		public int NumberOfBullets;
		public int Score;
		private Vector2 originalPosition;
		private float originalAngle;

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

		public Tank( string name, Vector2 startPosition, float startAngle, KeySet keys, Colors color, float speed )
		{
			Name = name;
			Speed = speed;
			originalPosition = startPosition;
			originalAngle = startAngle;
			this.keys = keys;
			this.color = color;
			Scale = 2;
			Origin = new Vector2( 16, 16 );
			Reset();
		}

		// The timeSpan in which the frame was updated.
		// If now - this > 175 milliseconds, update the frame.
		TimeSpan timeSinceLastFrameUpdate;

		// Tanks have 8 frames each, so this is the current frame.
		int frame = 0;

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

			if ( isKeyPressed( keyState, keys.KeyShoot ) )
			{
				Shoot( gameTime );
			}

			if ( isKeyPressed( keyState, keys.KeyPlace ) ) //keyState.IsKeyDown( keys.KeyPlace ) )
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
				Position + ( new Vector2( ( float )Math.Cos( MathHelper.ToRadians( Angle - sideDeg ) ), ( float )Math.Sin( MathHelper.ToRadians( Angle - sideDeg ) ) ) * dist * 2.5F ), this, 16, gameTime );
			newFence.Initialize( Game );
			Game.QueueEntity( newFence );
		}

		private void Shoot( TimeSpan gameTime )
		{
			Bullet newBullet = new Bullet( 10, this, gameTime );
			newBullet.Angle = Angle;
			newBullet.Position = Forward( 20 * Scale );
			NumberOfBullets++;
			newBullet.Initialize( Game );
			Game.QueueEntity( newBullet );
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

		public void Reset()
		{
			Position = originalPosition;
			Angle = originalAngle;
			IsAlive = true;
			NumberOfBullets = 0;
		}
	}
}
