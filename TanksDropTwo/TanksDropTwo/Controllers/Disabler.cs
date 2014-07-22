﻿using System;
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
			VacuumController d = new VacuumController( Owner, maxSpeed, x => !( x is Tank ), true, 1, true );
			d.Initialize( Game );
			Game.PutController( d );

		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Disabler" );
			Scale = 2F;
			Origin = new Vector2( 16, 16 );
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

	/// <summary>
	/// Has an owner, and pulls all entities it controls to the owner like a black hole.
	/// Can spiral them and not.
	/// </summary>
	public class VacuumController : GameController
	{
		private GameEntity Owner;
		private float maxSpeed;
		private float acceleration;
		private Predicate<GameEntity> match;
		private VacuumTankController t;
		private float baseSpeed;
		private bool spiral;
		/// <summary>
		/// As soon as the game doesn't have this as controller (all entities are vacuumed), this increments to 1000 then the VacuumTankController is removed.
		/// </summary>
		private int destroyTime;

		public VacuumController( GameEntity Owner, float maxSpeed, Predicate<GameEntity> match, bool giveTankController, float baseSpeed, bool spiral )
		{
			this.Owner = Owner;
			this.maxSpeed = maxSpeed;
			if ( spiral )
			{
				this.acceleration = 0.5F;
			}
			else
			{
				this.acceleration = 1;
			}
			this.match = match;
			this.spiral = spiral;
			this.baseSpeed = baseSpeed;
			this.destroyTime = 0;
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
			destroyTime = 0;
		}

		public override bool Control( GameEntity control, TimeSpan gameTime, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			if ( !control.Variables.ContainsKey( "VacuumSpeed" ) )
			{
				control.Variables[ "VacuumSpeed" ] = baseSpeed;
			}

			float vacSpeed = ( float )control.Variables[ "VacuumSpeed" ];
			float d = Vector2.Distance( Owner.Position, control.Position );
			if ( d * 2 <= Owner.GameWidth + Owner.GameHeight )
			{
				control.Variables.Remove( "VacuumSpeed" );
				control.ForceDestroy();
			}
			else
			{
				control.Move( vacSpeed, Tools.Angle( control, Owner ) );
				if ( spiral )
				{
					control.Move( vacSpeed + 10, Tools.Angle( control, Owner ) + 90 );
				}
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

			if ( t != null )
			{
				Tank ta = ( Tank )Owner;
				if ( !Game.HasController( this ) )
				{
					destroyTime += 1;
					if ( destroyTime > 1000 )
					{
						ta.RemoveTankController();
					}
				}
			}

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

		public VacuumTankController( VacuumController owner )
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
			return false;
		}

		public override bool OnPlaceFence( TimeSpan gameTime )
		{
			return false;
		}

		public override void StopControl()
		{
		}

		public override GameController Clone()
		{
			return new VacuumTankController( owner );
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
