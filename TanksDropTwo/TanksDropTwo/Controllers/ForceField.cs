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
	public class ForceField : TankController
	{
		public ForceField( int LifeTime )
			: base( LifeTime )
		{
		}

		public override void Initialize( TanksDrop game )
		{
			base.Initialize( game );
		}

		public override bool HitFence( Fence hitter )
		{
			hitter.Move( Owner.Speed, Owner.RelativeAngle );
			return false;
		}

		public override bool ProjectileHit( Projectile hitter, TimeSpan gameTime )
		{
			hitter.Angle += 180;
			hitter.Move( 2 );
			return false;
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager content )
		{
			Texture = content.Load<Texture2D>( "Sprites\\ForceField" );
			Origin = new Vector2( 16, 16 );
			Scale = 2;
		}

		public override void Draw( SpriteBatch spriteBatch )
		{
			spriteBatch.Draw( Texture, Owner.Position, null, Color.White, 0, Origin, Owner.Scale * 1.5F, SpriteEffects.None, 1 );
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
			ForceField f = new ForceField( lifeTime );
			f.Initialize( Game, Owner );
			f.LoadTexture( Game.Content );
			return f;
		}

		public override bool Hit( TimeSpan gameTime )
		{
			return false;
		}
	}
}
