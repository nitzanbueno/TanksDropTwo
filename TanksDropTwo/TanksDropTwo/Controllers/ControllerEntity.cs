using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TanksDropTwo.Controllers
{
	public abstract class ControllerEntity : GameEntity
	{
		/// <summary>
		/// Puts a new instance of the controller on the board.
		/// </summary>
		/// <param name="gameTime">The current game time.</param>
		/// <param name="game">The game instance.</param>
		public abstract void Spawn( TimeSpan gameTime, TanksDrop game );

		public override void Update( TimeSpan gameTime, HashSet<GameEntity> Entities, Microsoft.Xna.Framework.Input.KeyboardState keyState )
		{
			foreach ( GameEntity entity in Entities )
			{
				if ( entity.CollidesWith( this ) )
				{
					OnCollision( entity );
				}
			}
			base.Update( gameTime, Entities, keyState );
		}

		public abstract void OnCollision( GameEntity otherEntity );
	}
}
