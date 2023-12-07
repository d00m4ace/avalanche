using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameSessionState
	{
		public virtual void ProcessInPacket(GameSession session, NetworkPacket packet)
		{
			packet.Process(session);
		}
	}
}