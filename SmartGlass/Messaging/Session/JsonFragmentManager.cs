using SmartGlass.Messaging.Session;
using SmartGlass.Messaging.Session.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartGlass.Messaging.Session {
	public class JsonMessageFragment {
		public int datagram_size;
		public int datagram_id;
		public int fragment_offset;
		public int fragment_length;
		public string fragment_data;

	}
	class JsonFragmentManager {
		public ConcurrentDictionary<int, List<JsonMessageFragment>> datagram_id_to_message_fragments = new ConcurrentDictionary<int, List<JsonMessageFragment>>();
		public JsonMessage HandleMessage(JsonMessageFragment msg) {
			var list = datagram_id_to_message_fragments.GetOrAdd(msg.datagram_id, (_) => new List<JsonMessageFragment>());
			lock (list) {
				if (list.Any(a => a.fragment_offset == msg.fragment_offset))
					return null;
				list.Add(msg);
				if (list.Count == 1)//first cant be done
					return null;

				if (list.Sum(a => a.fragment_length) != msg.datagram_size)//not yet done
					return null;
				var data = String.Join("", list.OrderBy(a => a.fragment_offset).Select(a => a.fragment_data));
				data = Encoding.UTF8.GetString(Convert.FromBase64String(data));
				datagram_id_to_message_fragments.TryRemove(msg.datagram_id, out _);
				return new JsonMessage { Json = data};

			}
		}
	}
}
