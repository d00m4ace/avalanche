using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class NPClientUpdateSpaces : NetworkPacket
	{
		public AABB2D aabb;

		public NPClientUpdateSpaces()
		{
			id = (ushort)ID.NPClientUpdateSpaces;
		}

		public override void Write(MemoryBuffer mb)
		{
			mb.Write(id);
			mb.Write(aabb);
		}

		public override bool Read(MemoryBuffer mb)
		{
			if(!mb.Read(out id))
				return false;

			if(!mb.Read(out aabb))
				return false;

			return true;
		}

		public override void Process(GameSession session)
		{
#if SERVER
#endif
		}
	}
}
