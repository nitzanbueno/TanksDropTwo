using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDropTwo
{
	/// <summary>
	/// Game Controllers use the Control(GameEntity) method to make changes in what happens to the entity.
	/// </summary>
	public abstract class GameController
	{
		protected TanksDrop game;

		/// <summary>
		/// Makes changes to the entity according to what is required.
		/// </summary>
		/// <param name="control">The entity to change.</param>
		/// <param name="gameTime">The current game time.</param>
		/// <returns>True the entity should keep updating, otherwise false.</returns>
		public abstract bool Control( GameEntity control, TimeSpan gameTime );

		public virtual void Initialize( TanksDrop game )
		{
			this.game = game;
		}

		/// <summary>
		/// Determines whether or not the controller should stick to the given entity.
		/// </summary>
		/// <param name="entity">The entity to check.</param>
		/// <returns>True if the controller needs to stick to the entity, false otherwise.</returns>
		public abstract bool AddEntity( GameEntity entity );

		public abstract void Draw( SpriteBatch spriteBatch );
	}
}
