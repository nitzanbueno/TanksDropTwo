using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TanksDropTwo.SuddenDeaths
{
	public class ShrinkyDeath : GameController
	{
		public override bool Control( GameEntity control, TimeSpan gameTime, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			Tank tank = ( Tank )control;
			tank.RemoveTankController();
			tank.Scale -= 0.1F;
			if ( tank.Scale <= 0 )
			{
				tank.IsAlive = false;
				tank.RemoveController( this );
			}
			return true;
		}

		public override bool AddEntity( GameEntity entity )
		{
			return entity is Tank;
		}

		public override void Draw( Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch )
		{
		}
	}
}
