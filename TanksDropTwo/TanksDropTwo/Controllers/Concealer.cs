using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	public class Concealer : ControllerEntity
	{
		public Concealer( int lifeTime )
		{
			this.lifeTime = lifeTime;
		}

		private Dictionary<GameEntity, Tuple<Texture2D, bool, Color[]>> appliedEntities;

		public override void Initialize( TanksDrop game )
		{
			appliedEntities = new Dictionary<GameEntity, Tuple<Texture2D, bool, Color[]>>();
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
				if ( !( control is Lazer || control is LazerHelper || control.Texture == this.Texture ) )
				{
					if ( !appliedEntities.ContainsKey( control ) )
					{
						appliedEntities[ control ] = Tuple.Create( control.Texture, control.isTextureAMap, control.TextureData );
						control.Texture = this.Texture;
						control.TextureData = this.TextureData;
						control.isTextureAMap = false;
					}
				}
			}
			base.Update( gameTime, Entities, keyState );
		}

		public override void Destroy( TimeSpan gameTime )
		{
			foreach ( GameEntity ent in appliedEntities.Keys )
			{
				var e = appliedEntities[ ent ];
				ent.Texture = e.Item1;
				ent.isTextureAMap = e.Item2;
				ent.TextureData = e.Item3;
			}
			appliedEntities = new Dictionary<GameEntity, Tuple<Texture2D, bool, Color[]>>();
			base.Destroy( gameTime );
		}

		public override void OnCollision( GameEntity otherEntity )
		{
		}
	}

}
