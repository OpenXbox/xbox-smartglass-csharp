using System.Linq;
using System.Collections.Generic;
using SmartGlass.Connection;
using SmartGlass.Common;
using SmartGlass.Messaging.Session.Messages;
using System.Diagnostics;

namespace SmartGlass.Messaging.Session
{
    /// <summary>
    /// Fragment message manager.
    /// Assembles fragmented SessionMessages.
    /// </summary>
    internal class FragmentMessageManager
    {
        private Dictionary<int, byte[]> _fragmentQueue;
        public FragmentMessageManager()
        {
            _fragmentQueue = new Dictionary<int, byte[]>();
        }

        public SessionMessageBase AssembleFragment(SessionMessageBase message, uint sequenceNumber)
        {
            FragmentMessage fragment = message as FragmentMessage;

            SessionMessageType messageType = fragment.Header.SessionMessageType;
            int sequenceBegin = (int)fragment.SequenceBegin;
            int sequenceEnd = (int)fragment.SequenceEnd;

            _fragmentQueue[(int)sequenceNumber] = fragment.Data;

            IEnumerable<int> neededSequences = Enumerable.Range(sequenceBegin, sequenceEnd - sequenceBegin);
            foreach (var seq in neededSequences)
            {
                if (!_fragmentQueue.ContainsKey(seq))
                    return null;
            }

            EndianWriter writer = new EndianWriter();
            foreach (int seq in neededSequences)
            {
                byte[] data = _fragmentQueue[seq];
                writer.Write(data);
                _fragmentQueue.Remove(seq);
            }

            SessionMessageBase assembled = SessionMessageTransport.CreateFromMessageType(messageType);
            assembled.Deserialize(new EndianReader(writer.ToBytes()));
            return assembled;
        }
    }
}
