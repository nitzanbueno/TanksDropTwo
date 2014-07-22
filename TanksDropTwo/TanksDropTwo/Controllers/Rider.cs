using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using TanksDropTwo.Controllers;

namespace TanksDropTwo
{
	/// <summary>
	/// A projectile that turns the user INTO the projectile!
	/// </summary>
	public class Rider : Projectile
	{
		/// <summary>
		/// Did the tank kill another tank.
		/// Used to know whether to kill the tank in the end or not.
		/// </summary>
		public bool didDestroyTank;

		/// <summary>
		/// The rider controller.
		/// Essentially useless but makes the tank lose control during flight.
		/// </summary>
		RiderCon Controller;

		/// <summary>
		/// True if the tank's controller is a tripler.
		/// Used to triple the rider.
		/// </summary>
		bool ControllerIsTripler;
		List<Rider> otherRiders;
		public bool isOtherRider;
		/// <summary>
		/// This allows the normal rider to know when the other two died so it can self-destruct.
		/// </summary>
		int deadRiders;
		bool die;
		bool isDead;
		float twist;

		public Rider()
			: base()
		{
			didDestroyTank = false;
			Controller = new RiderCon();
			otherRiders = new List<Rider>();
			otherRiders.Add( this );
			isOtherRider = false;
		}

		public Rider( float Speed, int LifeTime, bool die, float twist )
			: this()
		{
			this.Speed = Speed;
			this.lifeTime = LifeTime;
			this.die = die;
			this.twist = twist;
		}

		public Rider( Tank Owner )
			: this()
		{
			this.owner = Owner;
			ControllerIsTripler = owner.Controller is Tripler;
		}

		public override void LoadContent( ContentManager Content, int screenWidth, int screenHeight )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\Rider" );
			Origin = new Vector2( 16, 16 );
			Scale = 2;
			base.LoadContent( Content, screenWidth, screenHeight );
		}

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			if ( !isDead )
				Move( Speed );
			if ( !isOtherRider )
			{
				owner.Angle += twist;
				owner.Position = Position;
			}
			CheckBounces();
			if ( !owner.IsAlive )
			{
				Destroy( gameTime );
			}
			if ( ( Position.Y < -1 || Position.X < -1 ) && bAxis == 0 )
			{
				Position += new Vector2( 10, 10 );
			}
			else if ( ( Position.Y > ScreenHeight + 1 || Position.X > ScreenWidth + 1 ) && bAxis == 0 )
			{
				Position -= new Vector2( 10, 10 );
			}
			foreach ( GameEntity entity in Entities )
			{
				if ( entity != owner && !( entity is Rider ) && CollisionCheck( entity, owner.Texture, owner.TextureData, owner.SourceRectangle ) )
				{
					if ( entity is Tank )
					{
						didDestroyTank = true;
						( ( Tank )entity ).ProjectileHit( this, gameTime );
					}
					else if ( entity is Fence )
					{
						Fence HitFence = ( Fence )entity;
						float fangle = HitFence.Angle < 180 ? HitFence.Angle + 180 : HitFence.Angle;
						// Reflects the projectile from the fence magically.
						Angle = ( 2 * fangle ) - Angle;
						Move( Speed );
					}
					else
					{
						entity.Destroy( gameTime );
					}
				}
			}
			base.Update( gameTime, Entities, keyState );
		}

		public override void Draw( TimeSpan gameTime, SpriteBatch spriteBatch )
		{
			/*Color c = Color.Blue;
			if ( isOtherRider )
			{
				c = Color.Green;
			}*/
			spriteBatch.Draw( owner.Texture, this.Position, owner.SourceRectangle, Color.White, owner.AngleInRadians, owner.Origin, owner.Scale, SpriteEffects.None, 0.25F );
		}

		public override void Initialize( TanksDrop game, TimeSpan gameTime, Tank owner )
		{
			owner.AppendController( Controller );
			Position = owner.Position;
			ControllerIsTripler = owner.Controller is Tripler;
			isDead = false;
			base.Initialize( game, gameTime, owner );
		}

		public override void Destroy( TimeSpan gameTime )
		{
			if ( !ControllerIsTripler || !isOtherRider && otherRiders.All( x => x.isDead ) )
			{
				base.Destroy( gameTime );
			}
			if ( isDead )
				return;
			isDead = true;
			owner.RemoveController( Controller );
			if ( !isOtherRider )
			{
				VacuumController v = new VacuumController( owner, Speed + 1, x => x is Rider && ( ( Rider )x ).owner == this.owner && ( ( Rider )x ).isOtherRider, false, Speed, true );
				v.Initialize( Game );
				Game.PutController( v );
			}
			if ( !didDestroyTank && otherRiders.All( x => !x.didDestroyTank ) && !isOtherRider && die )
			{
				owner.Destroy( gameTime );
			}
		}

		public override void ForceDestroy()
		{
			isDead = true;
			if ( !isOtherRider )
			{
				owner.RemoveController( Controller );
			}
			Game.RemoveEntity( this );
		}

		public override Projectile Clone()
		{
			Rider r = new Rider( Speed, lifeTime, die, twist );
			r.owner = owner;
			r.Angle = Angle;
			r.Position = Position;
			r.spawnTime = spawnTime;
			r.LoadContent( Game.Content, Game.ScreenWidth, Game.ScreenHeight );
			r.Initialize( Game );
			return r;
		}

		public override Projectile TriplerClone( TimeSpan gameTime )
		{
			Rider other = ( Rider )Clone();
			otherRiders.Add( other );
			other.Initialize( Game, gameTime, owner );
			other.isOtherRider = true;
			return other;
		}
	}

	public class RiderCon : GameController
	{
		public override bool Control( GameEntity control, TimeSpan gameTime, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			return false;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return false;
		}

		public override void Draw( SpriteBatch spriteBatch )
		{
		}
	}
}
