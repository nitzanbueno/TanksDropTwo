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
using TanksDropTwo.Controllers;
using System.IO;
using TanksDropTwo.Menus;

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
		HashSet<GameController> MasterControllers;
		TimeSpan currentGameTime;
		public int ScreenWidth;
		public int ScreenHeight;
		int NumOfPlayers;

		int PickupLifetime;

		int WaitMillisecs;
		int FreezeMillisecs;
		int SpawnMillisecs;

		public int CurrentID;

		Projectile[] AvailableProjectiles;
		TankController[] AvailableControllers;
		ControllerEntity[] AvailableConEnts;
		Random r = new Random();

		public Menu CurrentMenu;

		public SpriteFont Score;

		Projectile defaultBullet;

		public Texture2D Blank;

		public Dictionary<string, Tuple<Type, object>> Settings;

		public Dictionary<Tuple<int, int>, bool> Collisions;

		public Rectangle BoundingBox
		{
			get
			{
				return new Rectangle( 0, 0, ScreenWidth, ScreenHeight );
			}
		}

		StreamReader reader;
		List<string> Lines;

		public TanksDrop()
		{
			Settings = new Dictionary<string, Tuple<Type, object>>();
			graphics = new GraphicsDeviceManager( this );
			Read();
			ScreenWidth = LoadSetting( "ScreenWidth", 1000 );
			ScreenHeight = LoadSetting( "ScreenHeight", 1000 );
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
			NumOfPlayers = LoadSetting( "Players", 2 );
			CurrentID = -NumOfPlayers;
			WaitMillisecs = LoadSetting( "EndingDelay", 3000 );
			FreezeMillisecs = LoadSetting( "FreezeTime", 1000 );
			SpawnMillisecs = LoadSetting( "PickupTime", 5000 );
			PickupLifetime = LoadSetting( "PickupLifeTime", 10000 );

			LoadSetting( "BlastRadius", 10.0F );

			KeySet p1keys = LoadSetting( "Player1Keys", new KeySet( Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.Z, Keys.X ) );
			KeySet p2keys = LoadSetting( "Player2Keys", new KeySet( Keys.W, Keys.S, Keys.A, Keys.D, Keys.Q, Keys.E ) );
			KeySet p3keys = LoadSetting( "Player3Keys", new KeySet( Keys.T, Keys.G, Keys.F, Keys.H, Keys.V, Keys.B ) );
			KeySet p4keys = LoadSetting( "Player4Keys", new KeySet( Keys.NumPad5, Keys.NumPad2, Keys.NumPad1, Keys.NumPad3, Keys.NumPad7, Keys.NumPad8 ) );

			Colors p1Color = LoadSetting( "Player1Color", Colors.Green );
			Colors p2Color = LoadSetting( "Player2Color", Colors.Purple );
			Colors p3Color = LoadSetting( "Player3Color", Colors.Blue );
			Colors p4Color = LoadSetting( "Player4Color", Colors.Orange );

			int ProjectileTime = LoadSetting( "ProjectileTime", 10000 );
			int ProjectileSpeed = LoadSetting( "ProjectileSpeed", 10 );
			int ControllerTime = LoadSetting( "ControllerTime", 10000 );
			int FenceTime = LoadSetting( "FenceTime", 10000 );
			int TankSpeed = LoadSetting( "TankSpeed", 5 );
			int BulletLifeTime = LoadPositiveSetting( "BulletLifeTime", ProjectileTime );

			float TankScale = LoadSetting( "TankScale", 2F );

			defaultBullet = new Bullet( LoadPositiveSetting( "BulletSpeed", ProjectileSpeed ), Tank.blank, TimeSpan.Zero, BulletLifeTime );

			int FenceLimit = LoadSetting( "FenceLimit", 10 );
			int ProjectileLimit = LoadSetting( "ProjectileLimit", 3 );

			// Shows mouse
			IsMouseVisible = true;

			MasterControllers = new HashSet<GameController>();

			Entities = new HashSet<GameEntity>();

			// Player 1
			Entities.Add( new Tank( "Player 1", new Vector2( 50, 50 ), 45, p1keys, p1Color, TankSpeed, defaultBullet, ProjectileLimit, FenceLimit, FenceTime, TankScale ) );

			// Player 2
			Entities.Add( new Tank( "Player 2", new Vector2( ScreenWidth - 50, ScreenHeight - 50 ), 225, p2keys, p2Color, TankSpeed, defaultBullet, ProjectileLimit, FenceLimit, FenceTime, TankScale ) );

			if ( NumOfPlayers >= 3 )
			{
				Entities.Add( new Tank( "Player 3", new Vector2( ScreenWidth - 50, 50 ), 135, p3keys, p3Color, TankSpeed, defaultBullet, ProjectileLimit, FenceLimit, FenceTime, TankScale ) );
			}

			if ( NumOfPlayers >= 4 )
			{
				Entities.Add( new Tank( "Player 4", new Vector2( 50, ScreenHeight - 50 ), 315, p4keys, p4Color, TankSpeed, defaultBullet, ProjectileLimit, FenceLimit, FenceTime, TankScale ) );
			}

			foreach ( GameEntity entity in Entities )
			{
				entity.Initialize( this );
			}

			AvailableProjectiles = new Projectile[]
			{
				new HomingBullet( Tank.blank, LoadPositiveSetting( "HomingBulletSpeed", ProjectileSpeed ), LoadPositiveSetting( "HomingBulletTurnSpeed", 5 ), TimeSpan.Zero, LoadPositiveSetting( "HomingBulletNoticeTime", 1000 ), LoadPositiveSetting( "HomingBulletTime", ProjectileTime ) ),
				new Missile( Tank.blank, LoadPositiveSetting( "MissileSpeed", ProjectileSpeed ), LoadPositiveSetting( "MissileTime", ProjectileTime ) ),
				new Lazer( Tank.blank ),
			};

			AvailableControllers = new TankController[]
			{
				new Ghost( Tank.blank, LoadPositiveSetting( "GhostTime", ControllerTime ) ),
				new Deflector( Tank.blank ),
				new SpeedBoost( Tank.blank, LoadPositiveSetting( "SpeedBoostTime", ControllerTime ), LoadSetting( "SpeedBoostFactor", 2F ) ),
				new Minimize( Tank.blank, LoadPositiveSetting( "MinimizeTime", ControllerTime ) ),
				new Switcher( Tank.blank ),
				new ForceField( Tank.blank, LoadPositiveSetting( "ForceFieldTime", ControllerTime ) ),
				new Tripler( Tank.blank, LoadPositiveSetting( "TriplerTime", ControllerTime ) ),
				new ExtraLife( Tank.blank ),
			};

			AvailableConEnts = new ControllerEntity[]
			{
				new Portal( LoadPositiveSetting( "PortalTime", ControllerTime ) ),
				new BlackHole()
			};
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

			Score = Content.Load<SpriteFont>( "Score" );

			Blank = new Texture2D( GraphicsDevice, 1, 1 );
			Blank.SetData( new Color[] { Color.White } );

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

		#region Setting Functions
		private void Read()
		{
			Lines = new List<string>();
			try
			{
				reader = new StreamReader( File.OpenRead( "settings.ini" ) );
			}
			catch ( Exception )
			{
				return;
			}
			while ( !reader.EndOfStream )
			{
				try
				{
					Lines.Add( reader.ReadLine() );
				}
				catch ( Exception )
				{
					break;
				}
			}
		}

		private Colors LoadSetting( string setting, Colors defaultSetting )
		{
			string color = LoadSetting( setting );
			Colors set;
			try
			{
				set = ( Colors )Enum.Parse( typeof( Colors ), color );
			}
			catch ( Exception ) { set = defaultSetting; }
			Settings.Add( setting, Tuple.Create<Type, object>( set.GetType(), set ) );
			return set;
		}

		private KeySet LoadSetting( string setting, KeySet defaultSetting )
		{
			KeySet set;
			string keystr = LoadSetting( setting );
			if ( keystr == "" ) return defaultSetting;
			string[] keys = keystr.Replace( " ", string.Empty ).Split( ',' );
			set = new KeySet(
			LoadKey( keys[ 0 ], defaultSetting.KeyForward ),
			LoadKey( keys[ 2 ], defaultSetting.KeyBackward ),
			LoadKey( keys[ 1 ], defaultSetting.KeyLeft ),
			LoadKey( keys[ 3 ], defaultSetting.KeyRight ),
			LoadKey( keys[ 4 ], defaultSetting.KeyPlace ),
			LoadKey( keys[ 5 ], defaultSetting.KeyShoot ) );
			Settings.Add( setting, Tuple.Create<Type, object>( set.GetType(), set ) );
			return set;
		}

		/// <summary>
		/// Turns the key given as a string into a Keys object.
		/// </summary>
		/// <param name="key">The key as a string.</param>
		/// <param name="defaultKey">The key to return if the conversion fails.</param>
		/// <returns>The key is a Keys.</returns>
		private Keys LoadKey( string key, Keys defaultKey )
		{
			try
			{
				return ( Keys )Enum.Parse( typeof( Keys ), key );
			}
			catch ( Exception )
			{
				return defaultKey;
			}
		}

		private string LoadSetting( string setting )
		{
			foreach ( string l in Lines )
			{
				if ( l != "" && l[ 0 ] != '#' && l[ 0 ] != '[' )
				{
					string[] Line = l.Split( '=' );
					if ( Line[ 0 ].ToLower() == setting.ToLower() )
					{
						try
						{
							return Line[ 1 ];
						}
						catch ( Exception ) { }
					}
				}
			}
			return "";
		}

		private string LoadSetting( string setting, string defaultSetting )
		{
			string set = LoadSetting( setting );
			set = set == "" ? defaultSetting : set;
			Settings.Add( setting, Tuple.Create<Type, object>( set.GetType(), set ) );
			return set;
		}

		private int LoadSetting( string setting, int defaultSetting )
		{
			int set;
			try
			{
				set = int.Parse( LoadSetting( setting ) );
			}
			catch ( Exception )
			{
				set = defaultSetting;
			}
			Settings.Add( setting, Tuple.Create<Type, object>( set.GetType(), set ) );
			return set;
		}

		private double LoadSetting( string setting, double defaultSetting )
		{
			double set;
			try
			{
				set = Double.Parse( LoadSetting( setting ) );
			}
			catch ( Exception )
			{
				set = defaultSetting;
			}
			Settings.Add( setting, Tuple.Create<Type, object>( set.GetType(), set ) );
			return set;
		}

		private float LoadSetting( string setting, float defaultSetting )
		{
			float set;
			try
			{
				set = float.Parse( LoadSetting( setting ) );
			}
			catch ( Exception )
			{
				set = defaultSetting;
			}
			Settings.Add( setting, Tuple.Create<Type, object>( set.GetType(), set ) );
			return set;
		}

		private int LoadPositiveSetting( string setting, int defaultSetting )
		{
			int set;
			try
			{
				set = Int32.Parse( LoadSetting( setting ) );
				if ( set == 0 )
				{
					set = defaultSetting;
				}
			}
			catch ( Exception )
			{
				set = defaultSetting;
			}
			Settings.Add( setting, Tuple.Create<Type, object>( set.GetType(), set ) );
			return set;
		}

		#endregion

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
			KeyboardState keyState = Keyboard.GetState();
			MouseState mouseState = Mouse.GetState();

			Collisions = new Dictionary<Tuple<int, int>, bool>();

			if ( keyState.IsKeyDown( Keys.Escape ) )
			{
				this.Exit();
			}

			if ( CurrentMenu != null )
			{
				CurrentMenu.Update( currentGameTime, keyState, mouseState );
				return;
			}

			if ( currentGameTime == null )
			{
				currentGameTime = gameTime.TotalGameTime;
			}
			else
			{
				currentGameTime += gameTime.ElapsedGameTime;
			}
			if ( keyState.IsKeyDown( Keys.R ) )
			{
				NewRound( false );
			}
			if ( keyState.IsKeyDown( Keys.P ) )
			{
				CurrentMenu = new PauseMenu( this );
			}

			HashSet<GameEntity> EntitiesCopy = new HashSet<GameEntity>( Entities );
			int NumberOfLivingTanks = 0;
			foreach ( GameEntity entity in EntitiesCopy )
			{
				entity.ConUpdate( currentGameTime, EntitiesCopy, keyState );
				if ( entity is Tank && ( ( Tank )entity ).IsAlive )
				{
					NumberOfLivingTanks++;
				}
			}

			if ( ( currentGameTime - timeSinceLastPickup ).TotalMilliseconds > SpawnMillisecs )
			{
				SpawnPickup( currentGameTime );
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
			int x = Entities.Count( r => r is Projectile );
			base.Update( gameTime );
		}

		private void NewRound( bool Score = true )
		{
			CurrentID = 1;
			HashSet<GameEntity> OldEntities = new HashSet<GameEntity>( Entities );
			Entities = new HashSet<GameEntity>();
			MasterControllers = new HashSet<GameController>();
			foreach ( GameEntity entity in OldEntities )
			{
				if ( entity is Tank )
				{
					Tank t = ( Tank )entity;
					if ( t.IsAlive && Score )
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
			spriteBatch.Begin( SpriteSortMode.BackToFront, BlendState.AlphaBlend );

			foreach ( GameEntity entity in Entities )
			{
				entity.Draw( currentGameTime, spriteBatch );
			}

			if ( CurrentMenu != null )
			{
				CurrentMenu.Draw( spriteBatch, currentGameTime );
			}

			spriteBatch.End();

			base.Draw( gameTime );
		}

		/// <summary>
		/// Spawns a new pickup on the screen.
		/// </summary>
		protected void SpawnPickup( TimeSpan gameTime, bool blackHole = false )
		{
			int ProjLen = AvailableProjectiles.Length;
			int ConLen = AvailableControllers.Length;
			int ConEntLen = AvailableConEnts.Length;
			int Category = r.Next( ProjLen + ConLen + ConEntLen );

			if ( Category < ProjLen + ConLen )
			{
				Pickup p = null;

				p = Category < AvailableProjectiles.Length ? new ProjectilePickup( AvailableProjectiles[ Category ], PickupLifetime ) : ( Pickup )new TankControllerPickup( AvailableControllers[ Category - ProjLen ], PickupLifetime );

				p.Position = new Vector2( r.Next( ScreenWidth ), r.Next( ScreenHeight ) );
				p.Initialize( this, gameTime );
				QueueEntity( p );
			}
			else
			{
				ControllerEntity e = AvailableConEnts[ Category - ProjLen - ConLen ];

				// Make black holes rarer by doing the function one more time if a black hole was chosen.
				// Black holes are now 1/(ProjLen + ConLen + ConEntLen)^2 rare.
				// Note this only calls the function ONCE if a black hole was chosen.
				// If the black hole gets chosen twice, it gets spawned.
				if ( !( e is BlackHole ) || blackHole || r.Next( 5 ) == 1 )
				{
					e.Spawn( gameTime, this );
				}
				else
				{
					SpawnPickup( gameTime, true );
				}
			}
		}

		/// <summary>
		/// Adds the entity to the game.
		/// </summary>
		/// <param name="entity">The entity to add. LoadContent is called within the function.</param>
		/// <returns>true if the entity was added, false if it already existed.</returns>
		public bool QueueEntity( params GameEntity[] entities )
		{
			bool b = true;
			foreach ( GameEntity entity in entities )
			{
				entity.LoadContent( Content, ScreenWidth, ScreenHeight );
				if ( Entities.Contains( entity ) )
				{
					b = false;
				}
				foreach ( GameController controller in MasterControllers )
				{
					if ( controller.AddEntity( entity ) )
					{
						entity.AppendController( controller );
					}
				}
				Entities.Add( entity );
			}
			return b;
		}

		/// <summary>
		/// Removes an existing entity from the game.
		/// </summary>
		/// <param name="entity">The entity to remove.</param>
		/// <returns>True if the entity was removed, otherwise false.</returns>
		public bool RemoveEntity( GameEntity entity )
		{
			return Entities.Remove( entity );
		}

		/// <summary>
		/// Adds the controller to all entities on the board that satisfy the controller's AddEntity condition, as well as future entities that are spawned.
		/// </summary>
		/// <param name="controller">The controller to append.</param>
		public void AppendController( GameController controller )
		{
			foreach ( GameEntity entity in Entities )
			{
				if ( controller.AddEntity( entity ) )
				{
					entity.AppendController( controller );
				}
			}
			MasterControllers.Add( controller );
		}

		/// <summary>
		/// Stops sticking the given controller to future entities.
		/// </summary>
		/// <param name="controller">The controller to stop.</param>
		public void StopController( GameController controller )
		{
			MasterControllers.Remove( controller );
		}

		/// <summary>
		/// Stops the controller from sticking to future entities, and removes it from all entities that have it and satisfy the match condition.
		/// </summary>
		/// <param name="controller"></param>
		public void RemoveController( GameController controller, Predicate<GameEntity> match )
		{
			foreach ( GameEntity entity in Entities )
			{
				if ( match( entity ) )
				{
					entity.RemoveController( controller );
				}
			}
			MasterControllers.Remove( controller );
		}
	}
}
