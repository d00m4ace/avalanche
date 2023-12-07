using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class NPClientVersion : NetworkPacket
	{
		public ushort gameId;
		public uint gameVersion;

		public NPClientVersion()
		{
			id = (ushort)ID.NPClientVersion;

			gameId = Game.gameId;
			gameVersion = Game.gameVersion;
		}

		public override void Write(MemoryBuffer mb)
		{
			mb.Write(id);
			mb.Write(gameId);
			mb.Write(gameVersion);
		}

		public override bool Read(MemoryBuffer mb)
		{
			if(!mb.Read(out id))
				return false;

			if(!mb.Read(out gameId))
				return false;

			if(!mb.Read(out gameVersion))
				return false;

			return true;
		}

		public override void Process(GameSession session)
		{
#if SERVER
			session.clientGameId = gameId;
			session.clientGameVersion = gameVersion;

			session.Push(new NPServerVersion());
#endif
		}
	}
}