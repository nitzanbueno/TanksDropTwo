using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDropTwo.Controllers
{
	public class Concealer : ControllerEntity
	{
		public Concealer( int lifeTime )
		{
			this.lifeTime = lifeTime;
		}

		private Dictionary<GameEntity, Texture2D> appliedEntities;

		public override void Initialize( TanksDrop game )
		{
			appliedEntities = new Dictionary<GameEntity, Texture2D>();
			base.Initialize( game );
		}

		public override void LoadContent( Microsoft.Xna.Framework.Content.ContentManager Content, int screenWidth, int screenHeight )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Concealer" );
			base.LoadContent( Content, screenWidth, screenHeight );
		}

		public override void Spawn( TimeSpan gameTime, TanksDrop game )
		{
			Concealer c = new Concealer( lifeTime );
			c.Initialize( game );
			c.spawnTime = gameTime;
			game.QueueEntity( c );
		}

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			if ( Texture == null )
			{
				this.Texture = Game.Content.Load<Texture2D>( "Sprites\\Concealer" );
			}
			foreach ( GameEntity control in Entities )
			{
				if ( !( control is Tank || control.Texture == this.Texture ) )
				{
					if ( !appliedEntities.ContainsKey( control ) )
					{
						appliedEntities[ control ] = control.Texture;
						control.Texture = this.Texture;
					}
				}
			}
			base.Update( gameTime, Entities, keyState );
		}

		public override void Destroy( TimeSpan gameTime )
		{
			foreach ( GameEntity ent in appliedEntities.Keys )
			{
				ent.Texture = appliedEntities[ ent ];
			}
			appliedEntities = new Dictionary<GameEntity, Texture2D>();
			base.Destroy( gameTime );
		}

		public override void OnCollision( GameEntity otherEntity )
		{
		}
	}

}
