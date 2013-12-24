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
	public class TanksDrop : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		HashSet<GameEntity> Entities;
		TimeSpan currentGameTime;
		int ScreenWidth;
		int ScreenHeight;

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
			// TODO: Add your initialization logic here
			Entities = new HashSet<GameEntity>();
			Entities.Add( new Tank( "Player 1", new Vector2( 50, 50 ), 45, new KeySet( Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.Z, Keys.X ), Colors.Green, 5 ) );
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
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

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

			foreach ( GameEntity entity in Entities )
			{
				entity.Update( currentGameTime, Entities, keyState );
			}

			base.Update( gameTime );
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
	}
}
