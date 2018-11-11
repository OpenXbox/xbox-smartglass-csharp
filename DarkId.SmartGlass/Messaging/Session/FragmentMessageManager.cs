using System.Linq;
using System.Collections.Generic;
using DarkId.SmartGlass.Connection;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Messaging.Session.Messages;
using System.Diagnostics;

namespace DarkId.SmartGlass.Messaging.Session
{
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

            BEWriter writer = new BEWriter();
            foreach (int seq in neededSequences)
            {
                try
                {
                    byte[] data = _fragmentQueue[seq];
                    writer.Write(data);
                }
                catch (KeyNotFoundException)
                {
                    return null;
                }
            }

            // Pop obsolete fragment data
            foreach (int seq in neededSequences)
            {
                _fragmentQueue.Remove(seq);
            }

            SessionMessageBase assembled = SessionMessageTransport.CreateFromMessageType(messageType);
            assembled.Deserialize(new BEReader(writer.ToArray()));
            return assembled;
        }
    }
}