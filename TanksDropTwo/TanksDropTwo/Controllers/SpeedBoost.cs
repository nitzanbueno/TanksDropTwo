using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDropTwo.Controllers
{
	public class SpeedBoost : TankController
	{
		float OriginalSpeed;
		float factor;

		public SpeedBoost( int lifeTime, float factor )
			: base( lifeTime )
		{
			this.factor = factor;
		}

		public override void Initialize( TanksDrop game, Tank Owner )
		{
			OriginalSpeed = Owner.Speed;
			base.Initialize( game, Owner );
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager content )
		{
			Texture = content.Load<Texture2D>( "Sprites\\SpeedBoost" );
			Scale = 2;
		}

		public override bool ProjectileHit( Projectile hitter, TimeSpan gameTime )
		{
			return true;
		}

		public override bool Hit( TimeSpan gameTime )
		{
			return true;
		}

		public override bool Shoot( TimeSpan gameTime, Projectile shot )
		{
			Projectile nextProjectile = shot.Clone();
			nextProjectile.Speed *= factor;
			Game.QueueEntity( nextProjectile );
			return true;
		}

		public override bool Control( GameEntity control, TimeSpan gameTime, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			Owner.Speed = OriginalSpeed * factor;
			return base.Control( control, gameTime, keyState );
		}

		public override bool OnPlaceFence( TimeSpan gameTime )
		{
			return true;
		}

		public override void StopControl()
		{
			Owner.Speed = OriginalSpeed;
		}

		public override GameController Clone()
		{
			SpeedBoost s = new SpeedBoost( lifeTime, factor );
			s.Initialize( Game, Owner );
			return s;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return false;
		}

		public override void Draw( Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch )
		{
		}
	}
}
