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
		bool die;

		public Rider()
			: base()
		{
			didDestroyTank = false;
			Controller = new RiderCon();
			otherRiders = new List<Rider>();
			otherRiders.Add( this );
			isOtherRider = false;
		}

		public Rider( float Speed, int LifeTime, bool die )
			: this()
		{
			this.Speed = Speed;
			this.lifeTime = LifeTime;
			this.die = die;
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
			owner.Angle += ( float )( Game.Settings[ "RiderTwist" ].Item2 );
			Move( Speed );
			owner.Position = Position;
			CheckBounces();
			foreach ( GameEntity entity in Entities )
			{
				if ( entity != owner && !( entity is Rider ) && CollisionCheck( entity, owner.Texture, owner.TextureData, owner.SourceRectangle ) )
				{
					if ( entity is Tank )
					{
						didDestroyTank = true;
						( ( Tank )entity ).ProjectileHit( this, gameTime );
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
			if ( ControllerIsTripler )
			{
				foreach ( Rider r in otherRiders )
				{
					spriteBatch.Draw( owner.Texture, r.Position, owner.SourceRectangle, Color.White, owner.AngleInRadians, owner.Origin, owner.Scale, SpriteEffects.None, 0.25F );
				}
			}
		}

		public override void Initialize( TanksDrop game, TimeSpan gameTime, Tank owner )
		{
			owner.AppendController( Controller );
			Position = owner.Position;
			ControllerIsTripler = owner.Controller is Tripler;
			base.Initialize( game, gameTime, owner );
		}

		public override void Destroy( TimeSpan gameTime )
		{
			owner.RemoveController( Controller );
			if ( !didDestroyTank && otherRiders.All( x => !x.didDestroyTank ) && !isOtherRider && die )
			{
				owner.Destroy( gameTime );
			}
			base.Destroy( gameTime );
		}

		public override Projectile Clone()
		{
			Rider r = new Rider( Speed, lifeTime, die );
			r.owner = owner;
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
		public override bool Control( GameEntity control, TimeSpan gameTime )
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
