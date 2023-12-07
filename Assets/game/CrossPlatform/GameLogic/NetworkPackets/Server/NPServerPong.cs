using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace HEXPLAY
{
	public class NPServerPong : NetworkPacket
	{
		public long time;

		public int ping;

		public NPServerPong()
		{
			id = (ushort)ID.NPServerPong;
		}

		public override void Write(MemoryBuffer mb)
		{
			mb.Write(id);
			mb.Write(time);
		}

		public override bool Read(MemoryBuffer mb)
		{
			if(!mb.Read(out id))
				return false;

			if(!mb.Read(out time))
				return false;

			ping = (int)((1000 * (Game.updateStopwatch.ElapsedTicks - time)) / Stopwatch.Frequency);

			return true;
		}

		public override void Process(GameSession session)
		{
			Console.WriteLine("Ping:{0}", ping);
		}
	}
}
