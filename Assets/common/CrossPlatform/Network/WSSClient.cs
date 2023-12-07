using System;
using System.Collections;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class WSSClient
	{
		public MemoryBuffer mb;

		public GameSession session;

		public WebSocket wss;

		public DateTime lastPingTime;

		public WSSClient()
		{
			mb = new MemoryBuffer(0);
			session = new GameSession();
		}

		public void Connect(string host, int port)
		{
			wss = new WebSocket(host, port);
			wss.Connect();
			lastPingTime = DateTime.UtcNow;
		}

		public void Update()
		{
			if(IsConnected())
			{
				UpdateReceived();
				UpdateSend();

				int totalMillisecondsFromLastPingTime = (int)(DateTime.UtcNow - lastPingTime).TotalMilliseconds;

				if(totalMillisecondsFromLastPingTime > 100)
				{
					session.Push(new NPClientPing() { });
					lastPingTime = DateTime.UtcNow;
				}
			}

			session.UpdateState();
		}

		void UpdateReceived()
		{
			if(wss != null)
			{
				int packetCount = 0;

				while(packetCount < 10)
				{
					byte[] packet = wss.GetReceivedPacket();

					if(packet == null)
						break;

					session.ExtractReceivedPacket(mb, packet);

					packetCount++;
				}
			}
		}

		void UpdateSend()
		{
			if(wss != null)
			{
				int packetCount = 0;

				while(packetCount < 10)
				{
					int socketBufferedAmount = wss.SocketBufferedAmount();

					if(socketBufferedAmount == 0)
					{
						byte[] packet = session.PopSendPacket(mb);

						if(packet == null)
							break;

						wss.SendPacket(packet);

						packetCount++;
					}
					else
						break;
				}
			}
		}

		public bool IsConnected() { return wss != null && wss.IsConnected(); }
	}
}