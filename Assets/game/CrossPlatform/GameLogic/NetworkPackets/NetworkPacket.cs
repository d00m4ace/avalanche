using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class NetworkPacket
	{
		public ushort id;

		public virtual void Write(MemoryBuffer mb)
		{
			mb.Write(id);
		}

		public virtual bool Read(MemoryBuffer mb)
		{
			if(!mb.Read(out id))
				return false;

			return true;
		}

		public virtual void Process(GameSession session) { }

		public enum ID
		{
			None = 0,

			ServerPackets,
			NPServerVersion = ServerPackets,
			NPServerPong,
			NPServerUpdateIterations,

			ServerPacketsCount,

			ClientPackets = 0x7FFF,
			NPClientVersion = ClientPackets,
			NPClientPing,
			NPClientUpdateIterations,
			NPClientUpdateSpaces,

			ClientPacketsCount,
		}

		public static bool Read(MemoryBuffer mb, out NetworkPacket packet)
		{
			packet = null;

			long pos = mb.GetPos();

			ushort packetId;

			if(!mb.Read(out packetId))
				return false;

			mb.SetPos(pos);

			switch((ID)packetId)
			{
				case ID.NPServerVersion: return (packet = new NPServerVersion()).Read(mb);
				case ID.NPServerPong: return (packet = new NPServerPong()).Read(mb);

				case ID.NPClientVersion: return (packet = new NPClientVersion()).Read(mb);
				case ID.NPClientPing: return (packet = new NPClientPing()).Read(mb);
				case ID.NPClientUpdateSpaces: return (packet = new NPClientUpdateSpaces()).Read(mb);
			}

			return false;
		}
	}
}