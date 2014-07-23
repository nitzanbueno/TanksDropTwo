using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace TanksDropTwo
{
	/// <summary>
	/// This class gives its owner its control scheme, by telling it which key moves what part of it.
	/// </summary>
	public class KeySet
	{
		public Keys KeyForward;
		public Keys KeyBackward;
		public Keys KeyLeft;
		public Keys KeyRight;
		public Keys KeyPlace;
		public Keys KeyShoot;

		public KeySet( Keys forward, Keys backward, Keys left, Keys right, Keys place, Keys shoot )
		{
			KeyForward = forward;
			KeyBackward = backward;
			KeyLeft = left;
			KeyRight = right;
			KeyPlace = place;
			KeyShoot = shoot;
		}

		public static KeySet None
		{
			get
			{
				return new KeySet( Keys.None, Keys.None, Keys.None, Keys.None, Keys.None, Keys.None );
			}
		}

		public KeySet Clone()
		{
			return new KeySet( KeyForward, KeyBackward, KeyLeft, KeyRight, KeyPlace, KeyShoot );
		}
	}

	/// <summary>
	/// This Enumeration represents all possible tank colors.
	/// </summary>
	public enum Colors
	{
		Green = 0,
		Red = 1,
		Yellow = 2,
		Blue = 3,
		Purple = 4,
		Aqua = 5,
		Orange = 6,
		Pink = 7
	}

	/// <summary>
	/// This class has static functions useful for all classes.
	/// </summary>
	public class Tools
	{
		/// <summary>
		/// An improved modulo function which also works for negatives.
		/// </summary>
		/// <param name="modder">The number to the right of the %.</param>
		/// <param name="moddee">The number to the left of the %.</param>
		/// <returns>Modder % Moddee.</returns>
		public static float Mod( float modder, float moddee )
		{
			float result = modder;
			while ( result >= moddee )
			{
				result -= moddee;
			}
			while ( result < 0 )
			{
				result += moddee;
			}
			return result;
		}

		public static bool IsGoingTowardsMe( Vector2 TankPosition, float angle, Vector2 BulletPosition )
		{
			float window = 20;
			float ang = Tools.Mod( MathHelper.ToDegrees( ( float )Math.Atan2( TankPosition.Y - BulletPosition.Y, TankPosition.X - BulletPosition.X ) ), 360 );
			if ( ang < window )
			{
				return !( ( angle >= ang && angle < ang + window ) || angle <= ang || angle >= ang + 360 - window );
			}
			else if ( ang >= window && ang <= 360 - window )
			{
				return !( angle >= ang - window && angle <= ang + window );
			}
			else
			{
				return !( angle <= ang - 360 + window || angle >= ang - window );
			}
		}

		public static float Angle( Vector2 FromPosition, Vector2 ToPosition )
		{
			return Tools.Mod( MathHelper.ToDegrees( ( float )Math.Atan2( FromPosition.Y - ToPosition.Y, FromPosition.X - ToPosition.X ) ) + 180, 360 );
		}

		public static float Angle( GameEntity FromEntity, GameEntity ToEntity )
		{
			return Tools.Mod( MathHelper.ToDegrees( ( float )Math.Atan2( FromEntity.Position.Y - ToEntity.Position.Y, FromEntity.Position.X - ToEntity.Position.X ) ) + 180, 360 );
		}

		public static float HomeAngle( float TurnSpeed, float Angle, Vector2 Position, Vector2 HomingPosition )
		{
			float ang = Tools.Angle( Position, HomingPosition );

			if ( Angle == ang )
			{
				return 0;
				// Don't move if the bullet is in the right direction.
			}
			bool ToRight = Angle > 180 ? !( ang < Angle && ang > Angle - 180 ) : ( ang > Angle && ang < Angle + 180 ); // Determines, in one line, whether the bullet should turn right or left.
			if ( ToRight )
			{
				float difference = Math.Min( Math.Abs( ang - Angle ), Math.Abs( ang - Angle + 360 ) );
				return Math.Min( difference, TurnSpeed );
			}
			else
			{
				float difference = Math.Min( Math.Abs( ang - Angle ), Math.Abs( ang - Angle + 360 ) );
				return -Math.Min( difference, TurnSpeed );
			}
		}
	}
}
