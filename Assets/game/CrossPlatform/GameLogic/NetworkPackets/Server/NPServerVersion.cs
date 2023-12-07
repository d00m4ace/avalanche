using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class NPServerVersion : NetworkPacket
	{
		public ushort gameId;
		public uint gameVersion;

		public NPServerVersion()
		{
			id = (ushort)ID.NPServerVersion;
#if SERVER
			gameId = GameServer.gameId;
			gameVersion = GameServer.gameVersion;
#endif
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
			session.serverGameId = gameId;
			session.serverGameVersion = gameVersion;

			Console.WriteLine("Server gameId:{0} gameVersion:{1}", session.serverGameId, session.serverGameVersion);
		}
	}
}