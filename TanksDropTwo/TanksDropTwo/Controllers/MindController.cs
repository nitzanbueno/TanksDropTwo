using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	public class MindController : TankController
	{
		private KeySet controlKeys;
		private int TankNumber;
		private Random r;
		private List<Tank> Tanks;
		private Tank selectedTank;

		public MindController( int LifeTime )
			: base( LifeTime )
		{
			controlKeys = null;
			r = new Random();
			Tanks = new List<Tank>();
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager Content )
		{
			Texture = Content.Load<Texture2D>( "Sprites\\MindController" );
			Origin = new Vector2( 32, 32 );
			Scale = 1F;
		}

		public override void Initialize( TanksDrop game )
		{
			base.Initialize( game );
		}

		public override void Initialize( TanksDrop game, Tank owner )
		{
			game.PutController( this );
			base.Initialize( game, owner );
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
			if ( selectedTank != null )
			{
				Owner.Keys = selectedTank.Keys;
			}
			Owner.RemoveTankController();
			return false;
		}

		public override void StopControl()
		{
			try
			{
				Owner.Keys = selectedTank.Keys;
				selectedTank.Keys = selectedTank.OriginalKeys;
				selectedTank.TankColor = selectedTank.OriginalColor;
			}
			catch ( Exception ) { }
		}

		public override GameController Clone()
		{
			MindController m = new MindController( lifeTime );
			m.Initialize( Game, Owner, spawnTime );
			m.controlKeys = controlKeys;
			m.selectedTank = selectedTank;
			m.TankNumber = TankNumber;
			m.Tanks = Tanks;
			return m;
		}

		public override bool AddEntity( GameEntity entity )
		{
			if ( entity is Tank && entity != Owner )
			{
				Tank t = ( Tank )entity;
				if ( t.IsAlive && !( t.Controller is MindController ) )
				{
					Tanks.Add( t );
				}
			}
			return false;
		}

		public override void Draw( Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch )
		{
			try
			{
				selectedTank.TankColor = ( Colors )( ( int )( selectedTank.TankColor + 1 ) % 8 );
			}
			catch ( Exception ) { }
		}

		public override bool Control( GameEntity control, TimeSpan gameTime, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			if ( selectedTank == null )
			{
				List<Tank> TanksCopy = new List<Tank>( Tanks );
				if ( TanksCopy.Count == 0 )
					return true;
				int i = r.Next( TanksCopy.Count );
				while ( TanksCopy[ i ].Controller is MindController )
				{
					TanksCopy.RemoveAt( i );
					i = r.Next( TanksCopy.Count );
					if ( TanksCopy.Count == 0 )
						return true;
				}
				selectedTank = TanksCopy[ i ];
			}
			if ( controlKeys == null )
			{
				controlKeys = selectedTank.Keys;
				selectedTank.Keys = Owner.Keys;
				Owner.Keys = new KeySet( Keys.None, Keys.None, Keys.None, Keys.None, Owner.Keys.KeyPlace, Keys.None );
			}
			base.Control( control, gameTime, keyState );
			return true;
		}

		public override bool PickupProjectile( ProjectilePickup proj )
		{
			return true;
		}
	}
}
