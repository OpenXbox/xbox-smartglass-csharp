using System;
using System.Collections.Generic;
using System.Text;

namespace SmartGlass.Channels.Broadcast.Messages {
	public class JsonBaseMessage {
		public string msgid { get; set; }
		public string request { get; set; }
		public object @params { get; set; }
	}
}
