using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartGlass.Channels.Broadcast.Messages;
using SmartGlass.Common;

namespace SmartGlass.Channels {
	/// <summary>
	/// Input TV Remote channel.
	/// Handles TV streaming from USB tuner.
	/// (Seems broken on console side right now, 2019-04-02)
	/// </summary>
	public class InputTVRemoteChannel : IDisposable {
		private bool _disposed = false;
		private readonly ChannelMessageTransport _baseTransport;
		private readonly JsonMessageTransport<JsonBaseMessage> _transport;
		private static readonly ILogger logger = Logging.Factory.CreateLogger<InputTVRemoteChannel>();

		/// <summary>
		/// Initializes a new instance of the <see cref="T:SmartGlass.Channels.InputTVRemoteChannel"/> class.
		/// </summary>
		/// <param name="transport">Base transport.</param>
		internal InputTVRemoteChannel(ChannelMessageTransport transport) {
			if (msg_id_prefix == null)
			msg_id_prefix = Guid.NewGuid().ToString().Replace("-", "").Substring(0,8);
			_baseTransport = transport;
			_transport = new JsonMessageTransport<JsonBaseMessage>(_baseTransport, ChannelJsonSerializerOptions.GetBroadcastOptions());
			_transport.MessageReceived += OnMessageReceived;
		}

		private void OnMessageReceived(object sender, MessageReceivedEventArgs<JsonBaseMessage> e) {

			logger.LogTrace("Received JsonMsg:\r\n{0}\r\n{1}",
				e.Message.ToString(),
				JsonSerializer.Serialize(e.Message, new JsonSerializerOptions() { WriteIndented = true }));
		}

		/// <summary>
		/// Sends a ir command.
		/// </summary>
		/// <returns>Send ir command task.</returns>
		/// <param name="state">State ir to send.</param>
		public async Task SendIRCommandStateAsync(IRCommandState state) {
			var msg = new JsonBaseMessage { msgid = next_msg_id, request = "SendKey", @params = new { button_id = $"{state.cmd}", device_id = state.stump_device } };

			await _transport.SendAsync(msg);
		}

		/// <summary>
		/// get a ir commands
		/// </summary>
		/// <returns>Array of ir commands can be issued task.</returns>
		public async Task<IEnumerable<IRDevice>> GetIRCommandsAsync() {
			var message = new JsonBaseMessage { msgid = next_msg_id, request = "GetConfiguration" };

			var receive_task = MessageExtensions.WaitForMessageAsync<JsonBaseMessage, JsonBaseMessage>(
				_transport,
				TimeSpan.FromSeconds(10),
				()=>_transport.SendAsync(message),
				(to_filter) => to_filter.msgid == message.msgid
				);

			var res = await receive_task;
			if (res.@params is JsonElement je)
            {
				return JsonSerializer.Deserialize<IRDevice[]>(je.GetRawText());
            }
			return null;
		}
		private static string next_msg_id => $"{msg_id_prefix}.{msg_cnt++}";
		private static string msg_id_prefix;
		private static volatile int msg_cnt;

		protected virtual void Dispose(bool disposing) {
			if (!_disposed) {
				if (disposing) {
				}
				_disposed = true;
			}
		}

		public void Dispose() {
			Dispose(true);
		}
	}
}
