using System;
using System.Collections;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class WebSocket
	{
#if UNITY_WEBGL && !UNITY_EDITOR
		global::WebSocket webSocket;

		public readonly string ip;
		public readonly int port;

		public WebSocket(string ip, int port)
		{
			this.ip = ip;
			this.port = port;

			webSocket = new global::WebSocket(new Uri("wss://" + ip + ":" + port));
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

		public byte[] GetReceivedPacket()
		{
			byte[] packet = webSocket.Recv();
			return packet;
		}

		public void SendPacket(byte[] packet)
		{
			webSocket.Send(packet);
		}

		public int SocketBufferedAmount()
		{
			int socketBufferedAmount = webSocket.SocketBufferedAmount();
			//Console.WriteLine("JSWebSocket SocketBufferedAmount:{0}", socketBufferedAmount);
			return socketBufferedAmount;
		}
#else
		WebSocketSharp.WebSocket m_Socket;

		bool m_IsConnected = false;
		bool m_IsDisconnected = false;
		string m_Error = null;

		public readonly string ip;
		public readonly int port;

		List<byte[]> receivedPackets;

		public WebSocket(string ip, int port)
		{
			this.ip = ip;
			this.port = port;

			m_Socket = new WebSocketSharp.WebSocket("wss://" + ip + ":" + port);

			m_Socket.OnMessage += (sender, e) =>
			{
				lock(receivedPackets)
				{
					receivedPackets.Add(e.RawData);
				}
			};

			m_Socket.OnOpen += (sender, e) => m_IsConnected = true;
			m_Socket.OnClose += (sender, e) => { m_IsDisconnected = true; m_IsConnected = false; };
			m_Socket.OnError += (sender, e) => m_Error = e.Message;

			receivedPackets = new List<byte[]>();
		}

		public void Connect()
		{
			m_Socket.ConnectAsync();
		}

		public bool IsConnected()
		{
			return m_IsConnected;
		}

		public bool IsDisconnected()
		{
			return m_IsDisconnected;
		}

		public byte[] GetReceivedPacket()
		{
			byte[] packet = null;

			lock(receivedPackets)
			{
				if(receivedPackets.Count > 0)
				{
					packet = receivedPackets[0];
					receivedPackets.RemoveAt(0);
				}
			}

			return packet;
		}

		int socketBufferedAmount;

		public void SendPacket(byte[] packet)
		{
			socketBufferedAmount = packet.Length;
			m_Socket.SendAsync(packet, (b) => { /*Console.WriteLine("Socket.SendAsync:{0}", b); if(b)*/ socketBufferedAmount = 0; });
		}

		public int SocketBufferedAmount()
		{
			//Console.WriteLine("WebSocketSharp SocketBufferedAmount:{0}", socketBufferedAmount);
			return socketBufferedAmount;
		}
#endif
	}
}