using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Tcp_Server_Dump
{
	class Program
	{
		static void Main(string[] args)
		{
			// try to parse port
			if (args.Length < 1 || !int.TryParse(args[0], out int port))
			{
				Console.WriteLine("No port or invalid port specified! The port should be the only argument.\nExample: Tcp-Server-Dump.exe 80");
				return;
			}

			var server = new TcpListener(IPAddress.Any, port);
			server.Start();

			Console.WriteLine($"Accepting clients on port {port}. Break execution to stop.");
			Console.WriteLine($"All data will be logged/appended to \"dump.raw\"");

			while (true)
			{
				int totalRead = 0;
				var client = server.AcceptTcpClient();
				Console.WriteLine($"Accepting client: {client.Client.LocalEndPoint.ToString()}");
				var readBuffer = new byte[1024];

				while (true)
				{
					int bytesRead = client.GetStream().Read(readBuffer, 0, 1024);
					totalRead += bytesRead;
					if (bytesRead == 0)
					{
						Console.WriteLine($"Disconnected. Read {totalRead} bytes in total.");
						break;
					}

					using (FileStream file = new FileStream("dump.raw", FileMode.Append))
					{
						file.Write(readBuffer, 0, bytesRead);
						file.Flush();
					}
				}
			}
		}
	}
}
