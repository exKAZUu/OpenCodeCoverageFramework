using System;
using System.Net;
using System.Net.Sockets;
using Occf.Core.CoverageInformation;

namespace Occf.Reader.TcpIp {
	public class TcpIpReporter {
		public void Start(int port, CoverageInfo info) {
			var address = IPAddress.Parse("127.0.0.1");
			var server = new TcpListener(address, port);
			server.Start();

			while (true) {
				var client = server.AcceptTcpClient();
				Action action = () => {
					using (var stream = client.GetStream()) {
						while (true) {
							var index = (stream.ReadByte() << 24) + (stream.ReadByte() << 16) +
							            (stream.ReadByte() << 8) + (stream.ReadByte() << 0);
							var value = stream.ReadByte();
							if (value == -1)
								break;
							info.Targets[index].UpdateState((CoverageState)value);
						}
					}
				};
				action.BeginInvoke(action.EndInvoke, null);
			}
		}
	}
}