using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace TanksDropTwo.Controllers
{
	public abstract class UseableController : TankController
	{
		private bool isDestructed;

		public UseableController()
			: base( -1 )
		{
		}

		public override void Initialize( TanksDrop game )
		{
			base.Initialize( game );
			game.AppendController( this );
			isDestructed = false;
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
			return true;
		}
		

		/// <summary>
		/// Sets off the UseableController's action.
		/// </summary>
		/// <param name="gameTime">The current game time.</param>
		public void Activate( TimeSpan gameTime )
		{
			useSound.Play();
			isDestructed = true;
			Owner.RemoveTankController();
			InstantAction( gameTime );
			Game.StopController( this );
		}

		SoundEffect useSound;

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{
			useSound = Content.Load<SoundEffect>( "instant" );
		}

		public override bool Control( GameEntity control, TimeSpan gameTime, KeyboardState keyState )
		{
			if ( isDestructed )
			{
				InstantControl( control, gameTime );
				control.RemoveController( this );
			}
			return true;
		}

		/// <summary>
		/// Called once to make the change for the entity instantly.
		/// </summary>
		/// <param name="control">The entity to change.</param>
		/// <param name="gameTime">The current game time.</param>
		public abstract void InstantControl( GameEntity control, TimeSpan gameTime );

		/// <summary>
		/// Called when the owner places a fence.
		/// </summary>
		/// <param name="gameTime">The current game time.</param>
		public abstract void InstantAction( TimeSpan gameTime );

		public override void Draw( SpriteBatch spriteBatch )
		{
		}

		public override void StopControl()
		{
		}

		public override bool PickupProjectile( ProjectilePickup proj )
		{
			return true;
		}
	}
}
