using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class GameSession
	{
		public ushort clientGameId;
		public uint clientGameVersion;

		public ushort serverGameId;
		public uint serverGameVersion;

		public GameSessionState state;

		public bool isActive;
		public bool isClosed;

		List<NetworkPacket> inPackets;
		List<NetworkPacket> outPackets;

		public int GetInPacketsCount() { lock(inPackets) return inPackets.Count; }
		public int GetOutPacketsCount() { lock(outPackets) return outPackets.Count; }

		public GameSession()
		{
#if SERVER
			serverGameId = GameServer.gameId;
			serverGameVersion = GameServer.gameVersion;
#else
			clientGameId = Game.gameId;
			clientGameVersion = Game.gameVersion;
#endif
			inPackets = new List<NetworkPacket>();
			outPackets = new List<NetworkPacket>();

			isActive = false;
			isClosed = false;
		}

		public NetworkPacket Pop()
		{
			NetworkPacket packet = null;

			lock(inPackets)
			{
				if(inPackets.Count > 0)
				{
					packet = inPackets[0];
					inPackets.RemoveAt(0);
				}
			}

			return packet;
		}

		public void Push(NetworkPacket packet)
		{
			lock(outPackets)
			{
				outPackets.Add(packet);
			}
		}

		public byte[] PopSendPacket(MemoryBuffer mb)
		{
			byte[] packet = null;

			lock(outPackets)
			{
				if(outPackets.Count > 0)
				{
					mb.Rest();

					outPackets[0].Write(mb);
					outPackets.RemoveAt(0);

					packet = mb.buffer;
				}
			}

			return packet;
		}

		public void ExtractReceivedPacket(MemoryBuffer mb, byte[] packet)
		{
			mb.Set(packet);

			NetworkPacket networkPacket;
			NetworkPacket.Read(mb, out networkPacket);

			if(networkPacket != null)
			{
				lock(inPackets)
				{
					inPackets.Add(networkPacket);
				}
			}
		}

		public void UpdateState()
		{
			if(state != null)
			{
				int packetCount = 0;

				while(packetCount < 10)
				{
					NetworkPacket packet = Pop();

					if(packet != null)
						state.ProcessInPacket(this, packet);
					else
						break;

					packetCount++;
				}
			}
		}

		public void Log()
		{
			lock(inPackets)
				lock(outPackets)
				{
					Console.WriteLine("{0}/{1}", inPackets.Count, outPackets.Count);
				}
		}
	}
}