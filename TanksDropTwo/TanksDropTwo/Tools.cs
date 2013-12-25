using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

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
	}
}
