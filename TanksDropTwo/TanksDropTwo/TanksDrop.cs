using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TanksDropTwo
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class TanksDrop : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		HashSet<GameEntity> Entities;
		TimeSpan currentGameTime;
		int ScreenWidth;
		int ScreenHeight;
		int NumOfPlayers;

		int WaitMillisecs;
		int FreezeMillisecs;
		int SpawnMillisecs;

		Projectile[] AvailableProjectiles = new Projectile[]
		{
			new HomingBullet( Tank.blank, 10, 0.1F, TimeSpan.Zero, 0 ),
		};
		Random r = new Random();

		public TanksDrop()
		{
			graphics = new GraphicsDeviceManager( this );
			ScreenWidth = 1000;
			ScreenHeight = 1000;
			graphics.PreferredBackBufferWidth = ScreenWidth;
			graphics.PreferredBackBufferHeight = ScreenHeight;
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			NumOfPlayers = 4;
			WaitMillisecs = 3000;
			FreezeMillisecs = 1000;
			SpawnMillisecs = 1000;

			// Shows mouse
			IsMouseVisible = true;

			Entities = new HashSet<GameEntity>();

			// Player 1
			Entities.Add( new Tank( "Player 1", new Vector2( 50, 50 ), 45, new KeySet( Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.Z, Keys.X ), Colors.Green, 5 ) );

			// Player 2
			Entities.Add( new Tank( "Player 2", new Vector2( ScreenWidth - 50, ScreenHeight - 50 ), 225, new KeySet( Keys.W, Keys.S, Keys.A, Keys.D, Keys.Q, Keys.E ), Colors.Red, 5 ) );

			if ( NumOfPlayers >= 3 )
			{
				Entities.Add( new Tank( "Player 3", new Vector2( ScreenWidth - 50, 50 ), 135, new KeySet( Keys.T, Keys.G, Keys.F, Keys.H, Keys.V, Keys.B ), Colors.Blue, 5 ) );
			}

			if ( NumOfPlayers >= 4 )
			{
				Entities.Add( new Tank( "Player 4", new Vector2( 50, ScreenHeight - 50 ), 315, new KeySet( Keys.NumPad5, Keys.NumPad2, Keys.NumPad1, Keys.NumPad3, Keys.NumPad7, Keys.NumPad8 ), Colors.Yellow, 5 ) );
			}

			foreach ( GameEntity entity in Entities )
			{
				entity.Initialize( this );
			}

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch( GraphicsDevice );

			foreach ( GameEntity entity in Entities )
			{
				entity.LoadContent( Content, ScreenWidth, ScreenHeight );
			}

			foreach ( Projectile p in AvailableProjectiles )
			{
				p.LoadContent( Content, ScreenWidth, ScreenHeight );
			}
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		// This TimeSpan is not null only when the game is waiting to go to the next round, and it represents the time when it started waiting.
		TimeSpan? BeganWait;

		// This is the TimeSpan used when checking for pickup spawns.
		TimeSpan timeSinceLastPickup;

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update( GameTime gameTime )
		{
			if ( currentGameTime == null )
			{
				currentGameTime = gameTime.TotalGameTime;
			}
			else
			{
				currentGameTime += gameTime.ElapsedGameTime;
			}

			KeyboardState keyState = Keyboard.GetState();

			if ( keyState.IsKeyDown( Keys.R ) )
			{
				Initialize();
			}

			if ( keyState.IsKeyDown( Keys.Escape ) )
			{
				this.Exit();
			}

			HashSet<GameEntity> EntitiesCopy = new HashSet<GameEntity>( Entities );
			int NumberOfLivingTanks = 0;
			foreach ( GameEntity entity in EntitiesCopy )
			{
				entity.Update( currentGameTime, EntitiesCopy, keyState );
				if ( entity is Tank && ( ( Tank )entity ).IsAlive )
				{
					NumberOfLivingTanks++;
				}
			}

			if ( ( currentGameTime - timeSinceLastPickup ).TotalMilliseconds > SpawnMillisecs )
			{
				SpawnPickup();
				timeSinceLastPickup = currentGameTime;
			}

			if ( NumberOfLivingTanks <= 1 )
			{
				if ( !BeganWait.HasValue )
				{
					BeganWait = currentGameTime;
				}
				else if ( ( currentGameTime - BeganWait.Value ).TotalMilliseconds >= WaitMillisecs )
				{
					System.Threading.Thread.Sleep( FreezeMillisecs );
					NewRound();
					BeganWait = null;
				}
			}
			base.Update( gameTime );
		}

		private void NewRound()
		{
			HashSet<GameEntity> OldEntities = new HashSet<GameEntity>( Entities );
			Entities = new HashSet<GameEntity>();
			foreach ( GameEntity entity in OldEntities )
			{
				if ( entity is Tank )
				{
					Tank t = ( Tank )entity;
					if ( t.IsAlive )
					{
						t.Score++;
					}
					t.Reset();
					Entities.Add( entity );
				}
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw( GameTime gameTime )
		{
			GraphicsDevice.Clear( Color.CornflowerBlue );
			spriteBatch.Begin();

			foreach ( GameEntity entity in Entities )
			{
				entity.Draw( currentGameTime, spriteBatch );
			}
			spriteBatch.End();

			base.Draw( gameTime );
		}

		/// <summary>
		/// Spawns a new pickup on the screen.
		/// </summary>
		protected void SpawnPickup()
		{
			Pickup p = null;
			// Null for when I add power-ups.
			p = AvailableProjectiles[ r.Next( AvailableProjectiles.Length ) ].Pickup;
			// Currently only projectiles.

			p.Position = new Vector2( r.Next( ScreenWidth ), r.Next( ScreenHeight ) );
			p.Initialize( this );
			QueueEntity( p );
		}

		/// <summary>
		/// Adds the entity to the game.
		/// </summary>
		/// <param name="entity">The entity to add. LoadContent is called within the function.</param>
		/// <returns>true if the entity was added, false if it already existed.</returns>
		public bool QueueEntity( GameEntity entity )
		{
			entity.LoadContent( Content, ScreenWidth, ScreenHeight );
			if ( Entities.Contains( entity ) )
			{
				return false;
			}
			Entities.Add( entity );
			return true;
		}

		/// <summary>
		/// Removes an existing entity from the game.
		/// </summary>
		/// <param name="entity">The entity to remove.</param>
		/// <returns>True if the entity was removed, otherwise false.</returns>
		public bool RemoveEntity( GameEntity entity )
		{
			if ( Entities.Contains( entity ) )
			{
				Entities.Remove( entity );
				return true;
			}
			return false;
		}
	}
}
