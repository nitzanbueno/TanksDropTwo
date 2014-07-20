using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDropTwo.Controllers
{
	/// <summary>
	/// A UseableController that causes all bullets to turn 180 degrees (Switch direction) when used.
	/// </summary>
	public class Deflector : UseableController
	{
		public Deflector()
			: base()
		{
		}

		public override void LoadTexture( Microsoft.Xna.Framework.Content.ContentManager content )
		{
			Texture = content.Load<Texture2D>( "Sprites\\Deflector" );
			Scale = 2F;
		}

		public override void InstantControl( GameEntity control, TimeSpan gameTime )
		{
			
		}

		public override bool AddEntity( GameEntity entity )
		{
			return entity is Projectile;
		}

		public override GameController Clone()
		{
			Deflector d = new Deflector();
			d.Initialize( Game, Owner );
			d.LoadTexture( Game.Content );
			return d;
		}

		public override void InstantAction( TimeSpan gameTime )
		{
			Game.PutController( new DeflectorController() );
		}
	}

	public class DeflectorController : GameController
	{
		float max_speed = 10;

		public override void Initialize( TanksDrop game )
		{
			base.Initialize( game );
		}

		public override bool Control( GameEntity control, TimeSpan gameTime, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			Projectile p = ( Projectile )control;
			if ( !p.Variables.ContainsKey( "OriginalSpeed" ) )
			{
				p.Variables[ "OriginalSpeed" ] = p.Speed;
				p.Variables[ "SpeedDenominator" ] = 1;
				p.Variables[ "TurnAround" ] = false;
			}

			int speed_denominator = ( int )p.Variables[ "SpeedDenominator" ];
			float orig_speed = ( float )p.Variables[ "OriginalSpeed" ];
			bool turnAround = ( bool )p.Variables[ "TurnAround" ];

			if ( speed_denominator < 1 )
			{
				p.Variables.Remove( "SpeedDenominator" );
				p.Variables.Remove( "OriginalSpeed" );
				p.Variables.Remove( "TurnAround" );
				p.Speed = orig_speed;
				p.RemoveController( this );
			}
			else if ( speed_denominator < max_speed)
			{
				p.Speed = (float)-Math.Log( (double)speed_denominator / max_speed, max_speed ) * orig_speed;
				if ( turnAround )
				{
					speed_denominator -= 1;
				}
				else
				{
					speed_denominator += 1;
				}
				p.Variables[ "SpeedDenominator" ] = speed_denominator;
			}
			else
			{
				p.Variables[ "TurnAround" ] = true;
				p.Angle = ( p.Angle + 180 ) % 360;
				p.Variables[ "SpeedDenominator" ] = speed_denominator - 1;
			}

			return true;
		}

		public override void Draw( SpriteBatch spriteBatch )
		{
		}

		public override bool AddEntity( GameEntity entity )
		{
			return entity is Projectile;
		}

	}
}
