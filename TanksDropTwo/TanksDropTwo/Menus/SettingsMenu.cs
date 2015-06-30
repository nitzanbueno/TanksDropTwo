using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDropTwo.Menus
{
	public class SettingsMenu : Menu
	{
		bool isSettingSetting;
		string[] AllNames;
		int CurrentSettingIndex;
		Tuple<Type, object> CurrentSetting;

		public SettingsMenu( TanksDrop Game )
			: base( Game )
		{
			AllNames = ( from keyv in Game.Settings
						 select keyv.Key ).ToArray();
		}

		KeyboardState prevKeyState;

		public override void Update( TimeSpan gameTime, KeyboardState keyState, MouseState mouseState, GamePadState padState )
		{
			if ( !isSettingSetting )
			{
				if ( isKeyPressed( Keys.Left, keyState ) )
				{
					CurrentSettingIndex = ( int )Tools.Mod( AllNames.Length, CurrentSettingIndex - 1 );
				}
				else if ( isKeyPressed( Keys.Right, keyState ) )
				{
					CurrentSettingIndex = ( int )Tools.Mod( AllNames.Length, CurrentSettingIndex + 1 );
				}
				else if ( isKeyPressed( Keys.Enter, keyState ) )
				{
					isSettingSetting = true;
				}
			}
			else
			{

			}

			prevKeyState = keyState;
		}

		bool isKeyPressed( Keys key, KeyboardState keyState )
		{
			return prevKeyState.IsKeyUp( key ) && keyState.IsKeyDown( key );
		}

		public override void Draw( SpriteBatch spriteBatch, TimeSpan gameTime )
		{

		}
	}
}
