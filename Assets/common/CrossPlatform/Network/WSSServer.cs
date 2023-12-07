using System;
using System.IO;
using System.Collections.Generic;

#if SERVER
using System.Net;
using System.Threading;

using System.ServiceModel.Channels;
using System.Threading.Tasks;

using System.Security.Cryptography.X509Certificates;

using vtortola.WebSockets;
using vtortola.WebSockets.Rfc6455;
#endif

namespace HEXPLAY
{
	public class WSSServer
	{
		public List<GameSession> sessions;

#if SERVER
		WebSocketListener wssServer;
		IPEndPoint endpoint;
		CancellationTokenSource cancellation;
		Task acceptingTask;

		public WSSServer(X509Certificate2 certificate, int port = 443, int maxClients = 1000)
		{
			sessions = new List<GameSession>();

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			cancellation = new CancellationTokenSource();

			// local endpoint
			endpoint = new IPEndPoint(IPAddress.Any, port);

			// starting the server
			wssServer = new WebSocketListener(endpoint, new WebSocketListenerOptions()
			{
				PingMode = PingModes.BandwidthSaving,

				UseNagleAlgorithm = false,

				PingTimeout = TimeSpan.FromSeconds(5),
				NegotiationTimeout = TimeSpan.FromSeconds(5),

				ParallelNegotiations = 16,
				NegotiationQueueCapacity = 1000,

				TcpBacklog = 1000,

				BufferManager = BufferManager.CreateBufferManager((8192 + 1024) * maxClients, 8192 + 1024)
			});

			var rfc6455 = new WebSocketFactoryRfc6455(wssServer);

			wssServer.Standards.RegisterStandard(rfc6455);

			wssServer.ConnectionExtensions.RegisterExtension(new WebSocketSecureConnectionExtension(certificate));
		}

		public void Start()
		{
			wssServer.Start();
			Log("Server started at " + endpoint.ToString());
			acceptingTask = Task.Run(() => AcceptWebSocketClients(this, cancellation.Token));
		}

		public void Stop()
		{
			Log("Server stoping");
			wssServer.Stop();
			cancellation.Cancel();
			acceptingTask.Wait();
		}

		public GameSession GetNewSession()
		{
			GameSession session = new GameSession();
			session.state = new GSStateWelcome(session);
			lock(sessions) sessions.Add(session);
			return session;
		}

		public void Update()
		{
			int i = 0;

			while(true)
			{
				GameSession session = null;

				lock(sessions)
				{
					if(i >= sessions.Count)
						break;

					session = sessions[i];
				}

				if(session != null)
				{
					if(session.isActive)
					{
						//session.Log();
						session.UpdateState();
					}

					i++;
					continue;
				}

				break;
			}
		}

		static async Task AcceptWebSocketClients(WSSServer server, CancellationToken token)
		{
			while(!token.IsCancellationRequested)
			{
				try
				{
					var ws = await server.wssServer.AcceptWebSocketAsync(token).ConfigureAwait(false);

					if(ws == null)
						continue;

					if(false)
					{
						Task.Run(() => HandleConnectionAsync(server, ws, token));
					}

					if(true)
					{
						GameSession session = server.GetNewSession();
						session.isActive = true;

						Task.Run(() => HandleSessionReadAsync(server, ws, session, token));
						Task.Run(() => HandleSessionWriteAsync(server, ws, session, token));
					}
				}
				catch(Exception aex)
				{
					var ex = aex.GetBaseException();
					Log("Error Accepting client: " + ex.GetType().Name + ": " + ex.Message);
				}
			}

			Log("Server Stop accepting clients");
		}


		static async Task HandleSessionReadAsync(WSSServer server, vtortola.WebSockets.WebSocket ws, GameSession session, CancellationToken cancellation)
		{
			try
			{
				using(MemoryBuffer mb = new MemoryBuffer(0))
					while(ws.IsConnected && !cancellation.IsCancellationRequested)
					{
						byte[] packet = null;

						{
							using(var msg = await ws.ReadMessageAsync(cancellation).ConfigureAwait(false))
							{
								if(msg == null)
									continue;

								using(var ms = new MemoryStream())
								{
									await msg.CopyToAsync(ms);
									await msg.FlushAsync();
									packet = ms.ToArray();
								}
							}
						}

						if(packet != null)
						{
							session.ExtractReceivedPacket(mb, packet);
						}
					}
			}
			catch(TaskCanceledException)
			{
			}
			catch(Exception aex)
			{
				Log("Error Handling connection: " + aex.GetBaseException().Message);
				try { ws.Close(); }
				catch { }
			}
			finally
			{
				if(session != null)
				{
					session.isActive = false;
					session.isClosed = true;
				}

				ws.Dispose();

				Log("connection closed");
			}
		}

		static async Task HandleSessionWriteAsync(WSSServer server, vtortola.WebSockets.WebSocket ws, GameSession session, CancellationToken cancellation)
		{
			try
			{
				using(MemoryBuffer mb = new MemoryBuffer(0))
					while(ws.IsConnected && !cancellation.IsCancellationRequested)
					{
						byte[] packet = session.PopSendPacket(mb);

						if(packet == null)
						{
							await Task.Delay(10).ConfigureAwait(false);
							//await Task.Delay(250).ConfigureAwait(false); // Create lag 250ms
						}
						else
						{
							while(packet != null && ws.IsConnected && !cancellation.IsCancellationRequested)
							{
								if(packet != null)
								{
									using(var msg = ws.CreateMessageWriter(WebSocketMessageType.Binary))
									{
										await msg.WriteAsync(packet, 0, packet.Length, cancellation).ConfigureAwait(false);
										await msg.FlushAsync().ConfigureAwait(false);
									}
								}

								packet = session.PopSendPacket(mb);
							}
						}
					}
			}
			catch(TaskCanceledException)
			{
			}
			catch(Exception aex)
			{
				Log("Error Handling connection: " + aex.GetBaseException().Message);
				try { ws.Close(); }
				catch { }
			}
			finally
			{
			}
		}

		static async Task HandleConnectionAsync(WSSServer server, vtortola.WebSockets.WebSocket ws, CancellationToken cancellation)
		{
			GameSession session = null;

			try
			{
				using(MemoryBuffer mb = new MemoryBuffer(0))
					while(ws.IsConnected && !cancellation.IsCancellationRequested)
					{
						byte[] packet = null;

						{
							using(var msg = await ws.ReadMessageAsync(cancellation).ConfigureAwait(false))
							{
								if(msg == null)
									continue;

								using(var ms = new MemoryStream())
								{
									await msg.CopyToAsync(ms);
									await msg.FlushAsync();
									packet = ms.ToArray();
								}
							}
						}

						if(session == null)
						{
							session = server.GetNewSession();
							session.isActive = true;
						}

						if(packet != null)
						{
							session.ExtractReceivedPacket(mb, packet);
						}

						packet = session.PopSendPacket(mb);

						while(packet != null && ws.IsConnected && !cancellation.IsCancellationRequested)
						{
							if(packet != null)
							{
								using(var msg = ws.CreateMessageWriter(WebSocketMessageType.Binary))
								{
									await msg.WriteAsync(packet, 0, packet.Length, cancellation).ConfigureAwait(false);
									await msg.FlushAsync().ConfigureAwait(false);
								}
							}

							packet = session.PopSendPacket(mb);
						}
					}
			}
			catch(TaskCanceledException)
			{
			}
			catch(Exception aex)
			{
				Log("Error Handling connection: " + aex.GetBaseException().Message);
				try { ws.Close(); }
				catch { }
			}
			finally
			{
				if(session != null)
				{
					session.isActive = false;
					session.isClosed = true;
				}

				ws.Dispose();

				Log("connection closed");
			}
		}

#endif

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine("Unhandled Exception: ", e.ExceptionObject as Exception);
		}

		static void Log(string line)
		{
			Console.WriteLine(DateTime.Now.ToString("dd/MM/yyy hh:mm:ss.fff ") + line);
		}
	}
}