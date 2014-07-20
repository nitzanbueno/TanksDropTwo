using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	public class Disabler : UseableController
	{
		private float maxSpeed;

		public Disabler( float maxSpeed )
		{
			this.maxSpeed = maxSpeed;
		}

		public override void InstantControl( GameEntity control, TimeSpan gameTime )
		{
			( ( Tank )control ).RemoveTankController();
		}

		public override void InstantAction( TimeSpan gameTime )
		{
			VacuumController d = new VacuumController( Owner, maxSpeed, x => !( x is Tank ), true, 1 );
			d.Initialize( Game );
			Game.PutController( d );

		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Disabler" );
			Scale = 2F;
		}

		public override GameController Clone()
		{
			Disabler d = new Disabler( maxSpeed );
			d.Initialize( Game, Owner );
			d.LoadTexture( Game.Content );
			return d;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return entity is Tank;
		}

		public override bool PickupProjectile( ProjectilePickup proj )
		{
			return true;
		}
	}

	public class VacuumController : GameController
	{
		private GameEntity Owner;
		private float maxSpeed;
		private float acceleration;
		private Predicate<GameEntity> match;
		private VacuumTankController t;
		private float baseSpeed;

		public VacuumController( GameEntity Owner, float maxSpeed, Predicate<GameEntity> match, bool giveTankController, float baseSpeed )
		{
			this.Owner = Owner;
			this.maxSpeed = maxSpeed;
			this.acceleration = 1;
			this.match = match;
			this.baseSpeed = baseSpeed;
			if ( Owner is Tank && giveTankController )
			{
				t = new VacuumTankController( this );
			}
		}

		public override void Initialize( TanksDrop game )
		{
			base.Initialize( game );
			if ( t != null )
			{
				t.Initialize( Game, ( Tank )Owner );
				( ( Tank )Owner ).AppendController( t );
				( ( Tank )Owner ).Controller = t;
			}
		}

		public override bool Control( GameEntity control, TimeSpan gameTime, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			if ( !control.Variables.ContainsKey( "VacuumSpeed" ) )
			{
				control.Variables[ "VacuumSpeed" ] = baseSpeed;
			}

			float vacSpeed = ( float )control.Variables[ "VacuumSpeed" ];
			float d = Vector2.Distance( Owner.Position, control.Position );
			if ( d <= vacSpeed )
			{
				control.Variables.Remove( "VacuumSpeed" );
				Game.RemoveEntity( control );
				if ( t != null )
				{
					Tank ta = ( Tank )Owner;
					if ( !Game.HasController( this ) )
					{
						ta.RemoveTankController();
					}
				}
			}
			else
			{
				control.Move( vacSpeed, Tools.Mod( MathHelper.ToDegrees( ( float )Math.Atan2( Owner.Position.Y - control.Position.Y, Owner.Position.X - control.Position.X ) ), 360 ) );
			}
			if ( vacSpeed < maxSpeed )
			{
				vacSpeed += acceleration;
			}
			else
			{
				vacSpeed = maxSpeed;
			}
			control.Variables[ "VacuumSpeed" ] = vacSpeed;

			return true;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return match( entity );
		}

		public override void Draw( SpriteBatch spriteBatch )
		{
			return;
		}
	}

	public class VacuumTankController : TankController
	{
		VacuumController owner;

		public VacuumTankController(VacuumController owner)
			: base( -1 )
		{
			this.owner = owner;
		}

		public override bool Control( GameEntity control, TimeSpan gameTime, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			if ( !Game.HasController( owner ) )
			{
				Owner.RemoveTankController();
			}
			return base.Control( control, gameTime, keyState );
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{
		}

		public override bool ProjectileHit( Projectile hitter, TimeSpan gameTime )
		{
			return false;
		}

		public override bool Hit( TimeSpan gameTime )
		{
			return true;
		}

		public override bool OnPlaceFence( TimeSpan gameTime )
		{
			return true;
		}

		public override void StopControl()
		{
			return;
		}

		public override GameController Clone()
		{
			return new VacuumTankController(owner);
		}

		public override bool PickupProjectile( ProjectilePickup proj )
		{
			return false;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return entity == Owner;
		}

		public override void Draw( SpriteBatch spriteBatch )
		{
		}
	}
}
