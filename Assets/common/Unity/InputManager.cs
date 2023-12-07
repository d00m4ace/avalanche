using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

namespace HEXPLAY
{
	public class InputManager
	{
		public static void Update(InputController input)
		{
			if(input.PlayersNumber == 1)
				UpdateAsPrimaryJoystick(input.GetPlayerJoystick(InputController.Player.Player1));
		}

		static KeyCode PS4ButtonA = KeyCode.Joystick1Button0;
		static KeyCode PS4ButtonB = KeyCode.Joystick1Button3;
		static KeyCode PS4ButtonC = KeyCode.Joystick1Button2;
		static KeyCode PS4ButtonD = KeyCode.Joystick1Button1;

		static bool IsDown(KeyCode[] keys)
		{
			int ic = keys.Length;
			for(int i = 0; i < ic; i++)
				if(Input.GetKey(keys[i]))
					return true;

			return false;
		}

		public static void UpdateAsPrimaryJoystick(InputController.Joystick joystick)
		{
			if(!Game.isReplay)
			{
				KeyCode[] ButtonA = { PS4ButtonA, KeyCode.Alpha1, KeyCode.Space };
				KeyCode[] ButtonB = { PS4ButtonB, KeyCode.Alpha2, KeyCode.Return };
				KeyCode[] ButtonC = { PS4ButtonC, KeyCode.Alpha3, KeyCode.LeftAlt, KeyCode.RightAlt };
				KeyCode[] ButtonD = { PS4ButtonD, KeyCode.Alpha4, KeyCode.LeftControl, KeyCode.RightControl };

				joystick.SetButtonDown(InputController.Joystick.Button.A, IsDown(ButtonA));
				joystick.SetButtonDown(InputController.Joystick.Button.B, IsDown(ButtonB));
				joystick.SetButtonDown(InputController.Joystick.Button.C, IsDown(ButtonC));
				joystick.SetButtonDown(InputController.Joystick.Button.D, IsDown(ButtonD));

				float xAxis = 0;
				float yAxis = 0;

				xAxis = Input.GetAxis("Horizontal"); //xAxis = -Input.gyro.rotationRateUnbiased.z;
				yAxis = Input.GetAxis("Vertical"); //yAxis = -Input.gyro.rotationRateUnbiased.x;

				if(xAxis == 0f) xAxis = Input.GetAxis("Mouse X");
				if(yAxis == 0f) yAxis = Input.GetAxis("Mouse Y");

				if(xAxis < -1f) xAxis = -1f; else if(xAxis > 1f) xAxis = 1f;
				if(yAxis < -1f) yAxis = -1f; else if(yAxis > 1f) yAxis = 1f;

				short sXAxis = (short)(xAxis * 4095);
				short sYAxis = (short)(yAxis * 4095);

				joystick.SetAxis(InputController.Joystick.Axis.X, ((Fixed)sXAxis) / 4095);
				joystick.SetAxis(InputController.Joystick.Axis.Y, ((Fixed)sYAxis) / 4095);

				if(Game.saveJoystickActions)
				{
					byte buttons = 0;
					buttons |= (byte)((joystick.IsButtonDown(InputController.Joystick.Button.A) ? 1 : 0) << 0);
					buttons |= (byte)((joystick.IsButtonDown(InputController.Joystick.Button.B) ? 1 : 0) << 1);
					buttons |= (byte)((joystick.IsButtonDown(InputController.Joystick.Button.C) ? 1 : 0) << 2);
					buttons |= (byte)((joystick.IsButtonDown(InputController.Joystick.Button.D) ? 1 : 0) << 3);
					buttons |= (byte)((joystick.IsButtonPressed(InputController.Joystick.Button.A) ? 1 : 0) << 4);
					buttons |= (byte)((joystick.IsButtonPressed(InputController.Joystick.Button.B) ? 1 : 0) << 5);
					buttons |= (byte)((joystick.IsButtonPressed(InputController.Joystick.Button.C) ? 1 : 0) << 6);
					buttons |= (byte)((joystick.IsButtonPressed(InputController.Joystick.Button.D) ? 1 : 0) << 7);

					Game.savedGameActions.Write(buttons);
					Game.savedGameActions.Write(sXAxis);
					Game.savedGameActions.Write(sYAxis);
				}
			}
			else
			{
				if(Game.savedGameActions.GetPos() < Game.savedGameActions.GetSize())
				{
					short sXAxis, sYAxis;
					byte buttons;

					Game.savedGameActions.Read(out buttons);
					Game.savedGameActions.Read(out sXAxis);
					Game.savedGameActions.Read(out sYAxis);

					joystick.buttonDown[0] = (buttons & (1 << 0)) > 0;
					joystick.buttonDown[1] = (buttons & (1 << 1)) > 0;
					joystick.buttonDown[2] = (buttons & (1 << 2)) > 0;
					joystick.buttonDown[3] = (buttons & (1 << 3)) > 0;
					joystick.buttonPressed[0] = (buttons & (1 << 4)) > 0;
					joystick.buttonPressed[1] = (buttons & (1 << 5)) > 0;
					joystick.buttonPressed[2] = (buttons & (1 << 6)) > 0;
					joystick.buttonPressed[3] = (buttons & (1 << 7)) > 0;

					joystick.SetAxis(InputController.Joystick.Axis.X, ((Fixed)sXAxis) / 4095);
					joystick.SetAxis(InputController.Joystick.Axis.Y, ((Fixed)sYAxis) / 4095);
				}
				else
				{
					joystick.buttonDown[0] = joystick.buttonDown[1] = joystick.buttonDown[2] = joystick.buttonDown[3] = joystick.buttonPressed[0] = joystick.buttonPressed[1] = joystick.buttonPressed[2] = joystick.buttonPressed[3] = false;
					joystick.axis[0] = joystick.axis[1] = 0;
				}
			}
		}
	}
}
