using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TanksDropTwo.Controllers
{
	public class BlackHoleController : GameController
	{
		/// <summary>
		/// The position to avert the bullets to.
		/// </summary>
		Vector2 position;
		BlackHole master;
		Random r;
		bool shouldExplode;

		bool isLastOneInside;
		GameEntity LastOneInside;

		public BlackHoleController( Vector2 Position, BlackHole master )
		{
			position = Position;
			this.master = master;
			shouldExplode = false;
			isLastOneInside = false;
			r = new Random();
		}

		public override bool Control( GameEntity control, TimeSpan gameTime )
		{
			if ( !shouldExplode )
			{
				if ( Vector2.Distance( control.Position, master.Position ) > master.VacuumSpeed )
				{
					// This occurs when the projectile is further than the amount it's going to pass this frame.
					// So I move it towards me.
					control.Move( master.VacuumSpeed, Tools.Mod( MathHelper.ToDegrees( ( float )Math.Atan2( position.Y - control.Position.Y, position.X - control.Position.X ) ), 360 ) );
					isLastOneInside = false;
				}
				else
				{
					// Otherwise, I cannot move it towards me, as it may go to the other side, then I will try to pull it and it will return to where it started, etc.
					control.Position = master.Position;
					// So I center it in me.
					// From now on, it is considered "Inside" me.
					// A little explanation on the next few lines:
					// I Assume the HashSet goes through a cycle, and will not break the cycle.
					// For instance, if we have ["A","B","C"], I assume it will go either ABCABCABC.. or CBACBA.. but never ABACABC...
					// So that means the following:
					// Assume there is a projectile inside me.
					// If I do a cycle and all projectiles are inside me, then I reach the SAME projectile, that means ALL projectiles are inside me, as the control has completed a full cycle.
					// Then I can explode.
					if ( !isLastOneInside )
					{ 
						// So here, I set the projectile to begin the cycle with, since the previous projectile was outside.
						LastOneInside = control;
						isLastOneInside = true;
					}
					else // If the previous projectile was already inside, I am in the middle of a cycle.
						if ( control == LastOneInside ) // And if the projectile I began with is now the control, I have completed the cycle.
					{
						// Thus I can explode.
						shouldExplode = true;
						master.Vanish();
					}
				}
				return false;
			}
			else
			{
				control.Angle = r.Next( 360 );
				control.RemoveController( this );
				return true;
			}
		}

		public override bool AddEntity( GameEntity entity )
		{
			if ( entity is Projectile )
			{
				return true;
			}
			return false;
		}

		public override void Draw( Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch )
		{
		}
	}
}
