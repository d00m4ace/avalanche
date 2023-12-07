using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class AgentJoystickInput : Agent
	{
		public InputController.Player player;

		public Fixed impulse = 5;
		public Fixed angle = Math.PI / 180;
		public Vector2 force = Vector2.V(10, 10);

		public override void OnUpdateWorld(Actor actor)
		{
			InputController.Joystick joystick = Game.inputController.GetPlayerJoystick(player);
			Entity2D entity = actor.entity;

			if(entity.physicBody != null)
			{
				if(joystick.IsButtonPressed(InputController.Joystick.Button.C))
					entity.physicBody.ApplyImpulse(entity.dir * impulse);

				if(joystick.IsButtonDown(InputController.Joystick.Button.A))
					entity.AddAngle(angle);

				if(joystick.IsButtonDown(InputController.Joystick.Button.B))
					entity.AddAngle(-angle);

				if(joystick.GetAxis(InputController.Joystick.Axis.X) != 0 || joystick.GetAxis(InputController.Joystick.Axis.Y) != 0)
					entity.physicBody.AddForce(joystick.GetAxis().Scale(force));
			}
			else
			{
				if(joystick.IsButtonPressed(InputController.Joystick.Button.C))
					entity.vel = entity.vel + entity.dir * impulse;

				if(joystick.IsButtonDown(InputController.Joystick.Button.A))
					entity.AddAngle(angle);

				if(joystick.IsButtonDown(InputController.Joystick.Button.B))
					entity.AddAngle(-angle);

				if(joystick.GetAxis(InputController.Joystick.Axis.X) != 0 || joystick.GetAxis(InputController.Joystick.Axis.Y) != 0)
					entity.pos = entity.pos + joystick.GetAxis().Scale(force) * World2D.dt;
			}
		}
	}
}