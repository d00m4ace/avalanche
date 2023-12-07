using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;

#if UNITY_WEBGL && !UNITY_EDITOR
public class WebSocket
{
	private Uri mUrl;

	public WebSocket(Uri url)
	{
		mUrl = url;

		string protocol = mUrl.Scheme;
		if(!protocol.Equals("ws") && !protocol.Equals("wss"))
			throw new ArgumentException("Unsupported protocol: " + protocol);
	}

	public void SendString(string str)
	{
		Send(Encoding.UTF8.GetBytes(str));
	}

	public string RecvString()
	{
		byte[] retval = Recv();
		if(retval == null)
			return null;
		return Encoding.UTF8.GetString(retval);
	}

	[DllImport("__Internal")]
	private static extern int SocketCreate(string url);

	[DllImport("__Internal")]
	private static extern int SocketState(int socketInstance);

	[DllImport("__Internal")]
	private static extern void SocketSend(int socketInstance, byte[] ptr, int length);

	[DllImport("__Internal")]
	private static extern int SocketBufferedAmount(int socketInstance);

	[DllImport("__Internal")]
	private static extern void SocketRecv(int socketInstance, byte[] ptr, int length);

	[DllImport("__Internal")]
	private static extern int SocketRecvLength(int socketInstance);

	[DllImport("__Internal")]
	private static extern void SocketClose(int socketInstance);

	[DllImport("__Internal")]
	private static extern int SocketError(int socketInstance, byte[] ptr, int length);

	int m_NativeRef = 0;

	public void Send(byte[] buffer)
	{
		SocketSend(m_NativeRef, buffer, buffer.Length);
	}

	public int SocketBufferedAmount()
	{
		return SocketBufferedAmount(m_NativeRef);
	}

	public byte[] Recv()
	{
		int length = SocketRecvLength(m_NativeRef);
		if(length == 0)
			return null;
		byte[] buffer = new byte[length];
		SocketRecv(m_NativeRef, buffer, length);
		return buffer;
	}

	public void Connect()
	{
		m_NativeRef = SocketCreate(mUrl.ToString());
	}

	public int SocketState()
	{
		//Ready state constants
		//These constants are used by the readyState attribute to describe the state of the WebSocket connection.
		//Constant Value   Description
		//CONNECTING  0   The connection is not yet open.
		//OPEN    1   The connection is open and ready to communicate.
		//CLOSING 2   The connection is in the process of closing.
		//CLOSED  3   The connection is closed or couldn't be opened.

		return SocketState(m_NativeRef);
	}

	public void Close()
	{
		SocketClose(m_NativeRef);
	}

	public string error
	{
		get
		{
			const int bufsize = 1024;
			byte[] buffer = new byte[bufsize];
			int result = SocketError(m_NativeRef, buffer, bufsize);

			if(result == 0)
				return null;

			return Encoding.UTF8.GetString(buffer);
		}
	}
}
#endif