using System;
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
			controller.Initialize( Game );
			Game.AppendController( controller );
			Game.ScheduleTask( gameTime, 500, Stop );
			Game.ScheduleTask( gameTime, 2000, Remove );
			Owner.RemoveTankController();
			return false;
		}

		public void Stop()
		{
			Game.StopController( controller );
		}

		public void Remove()
		{
			Game.RemoveController( controller, x => true );
		}

		public override void StopControl()
		{
		}

		public override void Draw( SpriteBatch spriteBatch )
		{
		}

		public override bool PickupProjectile( ProjectilePickup proj )
		{
			return true;
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
		private float speedFactor = 0.87F;
		private const float epsilon = 0.1F;

		public Knockback( Tank Owner, float MaxDistance )
			: this( Owner, MaxDistance, 0.87F )
		{

		}

		public Knockback( Tank Owner, float MaxDistance, float SpeedFactor )
			: base()
		{
			this.owner = Owner;
			this.position = Owner.Position;
			this.maxdist = MaxDistance;
			this.startspeed = MaxDistance * ( 1 - speedFactor );
			this.speedFactor = SpeedFactor;
		}

		public override bool Control( GameEntity control, TimeSpan gameTime, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			if ( control is Projectile )
			{
				control.Angle = ( float )control.Variables[ angleString ];
			}

			float speed = ( float )control.Variables[ speedString ];
			float currentdist = ( float )control.Variables[ currentdistString ];
			float ang = ( float )control.Variables[ angleString ];
			speed *= speedFactor;
			currentdist += speed;
			float dist = Vector2.Distance( position, control.Position );
			if ( dist <= currentdist )
			{
				control.Move( speed, ang );
				control.Position = control.Bound( control.Position );
				dist += speed;
			}

			if ( control is Tank && ( ( Tank )control ).Controller != null )
			{
				TankController t = ( TankController )( ( Tank )control ).Controller.Clone();
				( ( Tank )control ).RemoveTankController();
				TankControllerPickup p = new TankControllerPickup( t, 3000 );
				float d = maxdist;
				p.Position = control.Bound( control.Forward( d, ang ) );
				p.Initialize( Game, gameTime );
				p.Variables[ speedString ] = speed;
				p.Variables[ currentdistString ] = currentdist + d;
				p.Variables[ angleString ] = ang;
				p.AppendController( this );
				Game.QueueEntity( p );
			}

			control.Variables[ speedString ] = speed;
			control.Variables[ currentdistString ] = currentdist;

			if ( currentdist >= maxdist - epsilon || speed <= epsilon )
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
