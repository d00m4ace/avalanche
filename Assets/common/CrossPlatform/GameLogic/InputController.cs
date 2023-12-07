using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class InputController
	{
		public class Joystick
		{
			public enum Button { A, B, C, D }
			public enum Axis { X, Y }

			public bool[] buttonPressed = { false, false, false, false };
			public bool[] buttonDown = { false, false, false, false };
			public Fixed[] axis = { 0, 0 };

			public bool IsButtonPressed(Button b) { return buttonPressed[(int)b]; }
			public bool IsButtonDown(Button b) { return buttonDown[(int)b]; }
			public Fixed GetAxis(Axis a) { return axis[(int)a]; }
			public Vector2 GetAxis() { return Vector2.V(axis[0], axis[1]); }

			public void SetButtonDown(Button b, bool down) { buttonPressed[(int)b] = !buttonDown[(int)b] && down; buttonDown[(int)b] = down; }
			public void SetAxis(Axis a, Fixed f) { axis[(int)a] = f; }
		}

		int playersNumber;
		Joystick[] joysticks;

		public int PlayersNumber { get { return playersNumber; } }

		public InputController(int playersNumber = 1)
		{
			SetPlayersNumber(playersNumber);
		}

		public void SetPlayersNumber(int playersNumber)
		{
			this.playersNumber = playersNumber;
			joysticks = new Joystick[playersNumber];

			for(int i = 0; i < playersNumber; i++)
				joysticks[i] = new Joystick();
		}

		public enum Player { Player1, Player2, Player3, Player4 }

		public Joystick GetPlayerJoystick(Player player)
		{
			return joysticks[(int)player % playersNumber];
		}
	}
}