using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartGlass.Common;
using SmartGlass.Messaging.Session;
using SmartGlass.Messaging.Session.Messages;

namespace SmartGlass.Channels
{
    /// <summary>
    /// Text channel.
    /// Handles entering text on the console, instead of using the
    /// on-screen-keyboard.
    /// </summary>
    public class TextChannel : IDisposable
    {
        private bool _disposed = false;
        private readonly ChannelMessageTransport _transport;
        private static readonly ILogger logger = Logging.Factory.CreateLogger<TextChannel>();
        private uint current_text_version = 0;
        private SystemTextConfigurationMessage session_config;
        private SystemTextInputMessage current_session_input;

        private bool is_first_session = true;
        public EventHandler SystemTextConfigurationMessageReceived;
        public EventHandler SystemTextDoneMessageReceived;
        public EventHandler SystemTextInputMessageReceived;
        public string CurrentSessionInputChunk => current_session_input?.TextChunk;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SmartGlass.Channels.TextChannel"/> class.
        /// </summary>
        /// <param name="transport">Base transport.</param>
        internal TextChannel(ChannelMessageTransport transport)
        {
            _transport = transport;
            _transport.MessageReceived += OnMessageReceived;
        }

        /// <summary>
        /// Appends the text to the current on screen text input
        /// </summary>
        /// <param name="text">String to append</param>
        public Task AppendText(String text)
        {
            return OverrideText(CurrentSessionInputChunk + text);
        }
        /// <summary>
        /// Overrides the text on screen with this input
        /// </summary>
        /// <param name="text">String to override with</param>
        public async Task OverrideText(String text)
        {
            var new_version = current_text_version + 1;
            var msg = new SystemTextInputMessage()
            {
                TextSessionId = (uint)session_config.TextSessionId,
                BaseVersion = current_text_version,
                SubmittedVersion = new_version,
                TotalTextBytelength = (uint)text.Length,
                SelectionStart = -1,
                SelectionLength = -1,
                TextChunk = text,

            };
            await _transport.SendAsync(msg);
            current_session_input = msg;
        }
        /// <summary>
        /// Accepts the current input dialog
        /// </summary>
        public async Task AcceptTextInput()
        {
            var done = new SystemTextDoneMessage
            {
                TextSessionId = (uint)session_config.TextSessionId,
                TextVersion = current_session_input.SubmittedVersion,
                Result = TextResult.Accept
            };
            await _transport.SendAsync(done);

        }
        private void ResetSession()
        {
            session_config = null;
            current_session_input = null;
            current_text_version = 0;

        }


        private async void OnMessageReceived(object sender, MessageReceivedEventArgs<SessionMessageBase> e)
        {
            if (e.Message is SystemTextConfigurationMessage msg_config)
            {
                ResetSession();
                session_config = msg_config;
                logger.LogTrace($"SystemTextConfigurationMessage received prompt: {session_config.Prompt}");
                SystemTextConfigurationMessageReceived?.Invoke(this, new EventArgs { });
            }
            else if (e.Message is SystemTextDoneMessage msg_done)
            {
                if (session_config?.TextSessionId == msg_done.TextSessionId)
                    ResetSession();
                else if (session_config == null && is_first_session) //we joined an in progress session, a done message will fire often on first letter hit so we can hijack it to be able to send text
                {
                    session_config = new SystemTextConfigurationMessage { TextSessionId = msg_done.TextSessionId };
                    current_text_version = msg_done.TextVersion;

                }
                is_first_session = false;
                SystemTextDoneMessageReceived?.Invoke(this, new EventArgs { });
            }
            else if (e.Message is SystemTextInputMessage msg_text)
            {
                current_session_input = msg_text;
                current_text_version = msg_text.SubmittedVersion;
                var ack_msg = new SystemTextAcknowledgeMessage { TextSessionId = (uint)session_config.TextSessionId, TextVersionAck = current_text_version };
                logger.LogTrace($"SystemTextInputMessage text: {current_session_input.TextChunk} Sending textack of: TextSessionId {ack_msg.TextSessionId} TextVersionAck: {ack_msg.TextVersionAck}");
                await _transport.SendAsync(ack_msg);
                SystemTextInputMessageReceived?.Invoke(this, new EventArgs { });
            }
            else if (e.Message is SystemTextAcknowledgeMessage msg_ack)
            {
                current_text_version = msg_ack.TextVersionAck;
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transport.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
