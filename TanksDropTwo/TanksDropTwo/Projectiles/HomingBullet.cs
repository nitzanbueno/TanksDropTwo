using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace TanksDropTwo
{
	/// <summary>
	/// A projectile which is red and sets its angle to always aim at the closest tank to it, including its owner.
	/// </summary>
	class HomingBullet : Projectile
	{
		private float turnSpeed;
		private int noticeTime;

		public HomingBullet( Tank Owner, float Speed, float TurnSpeed, TimeSpan gameTime, int NoticeTime, int lifeTime )
			: base( Owner, gameTime )
		{
			speed = Speed;
			turnSpeed = TurnSpeed;
			noticeTime = NoticeTime;
			this.lifeTime = lifeTime;
		}

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, KeyboardState keyState )
		{
			if ( ( gameTime - spawnTime ).TotalMilliseconds > noticeTime )
			{
				Home( Entities, turnSpeed );
			}
			Move( speed );
			CheckHits( gameTime, Entities );
			CheckBounces();
			base.Update( gameTime, Entities, keyState );
		}

		/// <summary>
		/// Homes onto the closest tank.
		/// </summary>
		/// <param name="Entities">The list of entities to find the tank with.</param>
		/// <param name="speed">The greatest turn value the bullet can have in one tick.</param>
		private void Home( HashSet<GameEntity> Entities, float speed )
		{
			Tank HomingTank = null;
			float ClosestDistance = -1;
			foreach ( GameEntity entity in Entities )
			{
				if ( entity is Tank )
				{
					Tank tank = ( Tank )entity;
					if ( tank.IsAlive )
					{
						float newDistance = Vector2.Distance( tank.Position, Position );
						if ( newDistance < ClosestDistance || ClosestDistance < 0 )
						{
							ClosestDistance = newDistance;
							HomingTank = tank;
						}
					}
				}
			}

			if ( HomingTank == null ) return; // Make sure there is a tank I home onto.

			float ang = Tools.Mod( MathHelper.ToDegrees( ( float )Math.Atan2( Position.Y - HomingTank.Position.Y, Position.X - HomingTank.Position.X ) ) + 180, 360 );

			if ( Angle == ang )
			{
				return;
				// Don't move if the bullet is in the right direction.
			}
			bool ToRight = Angle > 180 ? !( ang < Angle && ang > Angle - 180 ) : ( ang > Angle && ang < Angle + 180 ); // Determines, in one line, whether the bullet should turn right or left.
			if ( ToRight )
			{
				float difference = Math.Min( Math.Abs( ang - Angle ), Math.Abs( ang - Angle + 360 ) );
				Angle += Math.Min( difference, speed );
			}
			else
			{
				float difference = Math.Min( Math.Abs( ang - Angle ), Math.Abs( ang - Angle + 360 ) );
				Angle -= Math.Min( difference, speed );
			}
		}

		public override void LoadContent( ContentManager Content, int screenWidth, int screenHeight )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\RedBullet" );
			Scale = 0.25F;
			base.LoadContent( Content, screenWidth, screenHeight );
		}

		public override Projectile Clone()
		{
			HomingBullet h = new HomingBullet( owner, speed, turnSpeed, spawnTime, noticeTime, lifeTime );
			h.Initialize( Game );
			h.Position = Position;
			h.LoadContent( Game.Content, ScreenWidth, ScreenHeight );
			return h;
		}
	}
}
