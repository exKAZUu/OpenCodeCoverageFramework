#region License

// Copyright (C) 2009-2012 Kazunori Sakamoto
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

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
							if (value == -1) {
								break;
							}
							info.Targets[index].UpdateState((CoverageState)value);
						}
					}
				};
				action.BeginInvoke(action.EndInvoke, null);
			}
		}
	}
}