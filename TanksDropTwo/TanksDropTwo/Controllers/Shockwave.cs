﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	public class Shockwave : TankController
	{
		Knockback controller;

		public Shockwave()
			: base( -1 )
		{
		}

		public override void LoadTexture( ContentManager Content )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Shockwave" );
			Origin = new Vector2( 32, 32 );
		}

		public override GameController Clone()
		{
			Shockwave s = new Shockwave();
			s.Initialize( Game, Owner );
			s.LoadTexture( Game.Content );
			return s;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return false;
		}

		public override bool ProjectileHit( Projectile hitter, TimeSpan gameTime )
		{
			return true;
		}

		public override bool Hit( TimeSpan gameTime )
		{
			return true;
		}

		public override bool OnPlaceFence( TimeSpan gameTime )
		{
			controller = new Knockback( Owner, ( float )Game.Settings[ "ShockwaveRadius" ].Item2 );
			Game.AppendController( controller );
			Game.ScheduleTask( gameTime, 500, Stop );
			Owner.RemoveTankController( this );
			return false;
		}

		public void Stop()
		{
			Game.StopController( controller );
		}

		public override void StopControl()
		{
		}

		public override void Draw( SpriteBatch spriteBatch )
		{
		}
	}

	public class Knockback : GameController
	{
		private Tank owner;
		private Vector2 position;
		private float maxdist;
		private float startspeed;
		private const string currentdistString = "KnockbackCurrentDistance";
		private const string speedString = "KnockbackCurrentSpeed";
		private const string angleString = "KnockbackAngle";
		private const float speedFactor = 0.87F;

		public Knockback( Tank Owner, float MaxDistance )
			: base()
		{
			this.owner = Owner;
			this.position = Owner.Position;
			this.maxdist = MaxDistance;
			this.startspeed = MaxDistance * ( 1 - speedFactor );
		}

		public override bool Control( GameEntity control, TimeSpan gameTime )
		{
			float speed = ( float )control.Variables[ speedString ];
			float currentdist = ( float )control.Variables[ currentdistString ];
			speed *= speedFactor;
			currentdist += speed;
			float dist = Vector2.Distance( position, control.Position );
			if ( dist <= currentdist )
			{
				control.Move( speed, ( float )control.Variables[ angleString ] );
				dist += speed;
			}

			control.Variables[ speedString ] = speed;
			control.Variables[ currentdistString ] = currentdist;

			if ( currentdist > maxdist || speed == 0 )
			{
				control.Variables.Remove( angleString );
				control.Variables.Remove( speedString );
				control.Variables.Remove( currentdistString );
				control.RemoveController( this );
			}
			return true;
		}

		public override bool AddEntity( GameEntity entity )
		{
			if ( entity != owner && Vector2.Distance( entity.Position, position ) < maxdist )
			{
				float ang = Tools.Mod( MathHelper.ToDegrees( ( float )Math.Atan2( position.Y - entity.Position.Y, position.X - entity.Position.X ) ) + 180, 360 );
				entity.Variables[ angleString ] = ang;
				entity.Variables[ currentdistString ] = 0F;
				entity.Variables[ speedString ] = startspeed;
				return true;
			}
			return false;
		}

		public override void Draw( SpriteBatch spriteBatch )
		{
		}
	}
}