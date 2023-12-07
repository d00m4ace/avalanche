using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class NPClientPing : NetworkPacket
	{
		public long time;

		public NPClientPing()
		{
			id = (ushort)ID.NPClientPing;
		}

		public override void Write(MemoryBuffer mb)
		{
			time = Game.updateStopwatch.ElapsedTicks;

			mb.Write(id);
			mb.Write(time);
		}

		public override bool Read(MemoryBuffer mb)
		{
			if(!mb.Read(out id))
				return false;

			if(!mb.Read(out time))
				return false;

			return true;
		}

		public override void Process(GameSession session)
		{
#if SERVER
			session.Push(new NPServerPong() { time = time });
#endif
		}
	}
}