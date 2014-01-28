using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	/// <summary>
	/// A TankController that causes the owner to be able to pass through bullets and fences.
	/// </summary>
	public class Ghost : TankController
	{
		public Ghost( int LifeTime )
			: base( LifeTime )
		{
		}

		public override void Initialize( TanksDrop game )
		{
			base.Initialize( game );
		}

		public override bool HitFence( Fence hitter )
		{
			return true;
		}

		public override bool ProjectileHit( Projectile hitter, TimeSpan gameTime )
		{
			return false;
		}

		public override bool Hit( TimeSpan gameTime )
		{
			return false;
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager content )
		{
			Texture = content.Load<Texture2D>( "Sprites\\Ghost" );
			Origin = new Vector2( 16, 16 );
			Scale = 2;
		}

		public override void Draw( SpriteBatch spriteBatch )
		{
		}

		public override bool OnPlaceFence( TimeSpan gameTime )
		{
			return true;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return false;
		}

		public override void StopControl()
		{
		}

		public override GameController Clone()
		{
			Ghost g = new Ghost( lifeTime );
			g.Initialize( Game, Owner );
			g.LoadTexture( Game.Content );
			return g;
		}
	}
}
