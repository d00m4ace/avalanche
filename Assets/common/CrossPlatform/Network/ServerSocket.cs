using System;
using System.Collections;
using System.Collections.Generic;

using Hazel;
using Hazel.Tcp;

namespace HEXPLAY
{
	public class ServerSocket
	{
#if UNITY_WEBGL && !UNITY_EDITOR
		global::WebSocket webSocket;

		public readonly string ip;
		public readonly int port;

		public ServerSocket(string ip, int port)
		{
			this.ip = ip;
			this.port = port;

			webSocket = new global::WebSocket(new Uri("ws://" + ip + ":" + port));
		}

		public void Connect()
		{
			webSocket.Connect();
		}

		public bool IsConnected()
		{
			return webSocket.SocketState() == 1;
		}

		public bool IsDisconnected()
		{
			return webSocket.SocketState() == 3;
		}

		public bool GetReceivedPackets(List<byte[]> packets)
		{
			bool one = false;

			while(true)
			{
				byte[] packet = webSocket.Recv();

				if(packet!=null)
				{
					packets.Add(packet);
					one = true;
					continue;
				}

				break;
			}

			return one;
		}

		public void SendPacket(byte[] packet)
		{
			webSocket.Send(packet);
		}
#else
		TcpConnection connection;

		public readonly string ip;
		public readonly int port;

		List<byte[]> receivedPackets;

		public ServerSocket(string ip, int port)
		{
			this.ip = ip;
			this.port = port;

			connection = new TcpConnection(new NetworkEndPoint(this.ip, this.port));
			connection.DataReceived += DataReceived;

			receivedPackets = new List<byte[]>();
		}

		public void Connect()
		{
			connection.ConnectNoWait();
		}

		public bool IsConnected()
		{
			if(connection.State == ConnectionState.Connecting)
			{
				connection.ConnectNoWaitCheckIsCompleted();
				return false;
			}

			return connection.State == ConnectionState.Connected;
		}

		public bool IsDisconnected()
		{
			return connection.State == ConnectionState.NotConnected;
		}

		private void DataReceived(object sender, DataReceivedEventArgs args)
		{
			lock(receivedPackets)
			{
				receivedPackets.Add(args.Bytes);
			}

			args.Recycle();
		}

		public bool GetReceivedPackets(List<byte[]> packets)
		{
			lock(receivedPackets)
			{
				if(receivedPackets.Count > 0)
				{
					packets.AddRange(receivedPackets);
					receivedPackets.Clear();
					return true;
				}
			}

			return false;
		}

		public void SendPacket(byte[] packet)
		{
			connection.SendBytes(packet);
		}
#endif
	}
}