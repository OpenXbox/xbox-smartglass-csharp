using System.Collections.Generic;
using SmartGlass;
using SmartGlass.Common;
using SmartGlass.Messaging;
using SmartGlass.Messaging.Session;
using Xunit;

using SmartGlass.Tests.Resources;
using Msgn = SmartGlass.Messaging;
using Msgs = SmartGlass.Messaging.Session;
using Conn = SmartGlass.Connection;

namespace SmartGlass.Tests
{
    public class TestPacketAssembly
    {
        private Conn.CryptoContext _crypto;

        public TestPacketAssembly()
        {
            byte[] sharedSecretBlob = new byte[]
            {
                0x82, 0xbb, 0xa5, 0x14, 0xe6, 0xd1, 0x95, 0x21, 0x11, 0x49,
                0x40, 0xbd, 0x65, 0x12, 0x1a, 0xf2, 0x34, 0xc5, 0x36, 0x54,
                0xa8, 0xe6, 0x7a, 0xdd, 0x77, 0x10, 0xb3, 0x72, 0x5d, 0xb4,
                0x4f, 0x77, 0x30, 0xed, 0x8e, 0x3d, 0xa7, 0x01, 0x5a, 0x09,
                0xfe, 0x0f, 0x08, 0xe9, 0xbe, 0xf3, 0x85, 0x3c, 0x05, 0x06,
                0x32, 0x7e, 0xb7, 0x7c, 0x99, 0x51, 0x76, 0x9d, 0x92, 0x3d,
                0x86, 0x3a, 0x2f, 0x5e
            };

            byte[] dummyPublicKey = new byte[64];
            for (int i = 0; i < dummyPublicKey.Length; i++)
            {
                dummyPublicKey[i] = 0xFF;
            }

            _crypto = new Conn.CryptoContext(
                sharedSecretBlob,
                dummyPublicKey
            );
        }

        private byte[] AssembleSessionMessage(SessionMessageBase message,
                                             uint sequenceNumber,
                                             uint sourceParticipantId,
                                             uint targetParticipantId = 0)
        {
            var fragment = new SessionFragmentMessage();

            fragment.Header.IsFragment = message.Header.IsFragment;
            fragment.Header.ChannelId = message.Header.ChannelId;
            fragment.Header.RequestAcknowledge = message.Header.RequestAcknowledge;
            fragment.Header.SessionMessageType = message.Header.SessionMessageType;
            fragment.Header.Version = message.Header.Version;

            fragment.Header.SequenceNumber = sequenceNumber;
            fragment.Header.SourceParticipantId = sourceParticipantId;
            fragment.Header.TargetParticipantId = targetParticipantId;

            var writer = new EndianWriter();
            message.Serialize(writer);
            fragment.Fragment = writer.ToBytes();

            ((ICryptoMessage)fragment).Crypto = _crypto;

            var finalWriter = new EndianWriter();
            fragment.Serialize(finalWriter);
            return finalWriter.ToBytes();
        }

        private byte[] AssembleMessage(IMessage message)
        {
            var writer = new EndianWriter();
            message.Serialize(writer);
            return writer.ToBytes();
        }


        [Fact]
        public void TestPresenceRequest()
        {
            var message = new Msgn.Discovery.PresenceRequestMessage()
            {
                DeviceType = DeviceType.WindowsStore
            };
            var packet = AssembleMessage(message);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("presence_request.bin", ResourceType.SmartGlass), packet);
        }

        [Fact]
        public void TestConnectRequest()
        {
            var message = new Msgn.Connection.ConnectRequestMessage()
            {
                Authorization = "dummy_token",
                UserHash = "deadbeefdeadbeefde",
                DeviceId = System.Guid.Parse("545d30de-b475-1b43-adb2-eb6b9e546014"),
                Crypto = _crypto,
                InitVector = new byte[]
                {
                    0x29, 0x79, 0xd2, 0x5e, 0xa0, 0x3d, 0x97, 0xf5,
                    0x8f, 0x46, 0x93, 0x0a, 0x28, 0x8b, 0xf5, 0xd2
                },
                SequenceNumber = 0,
                SequenceBegin = 0,
                SequenceEnd = 2
            };

            var packet = AssembleMessage(message);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("connect_request.bin", ResourceType.SmartGlass), packet);
        }

        [Fact]
        public void TestPowerOn()
        {
            var message = new Msgn.Power.PowerOnMessage()
            {
                LiveId = "FD00112233FFEE66"
            };

            var packet = AssembleMessage(message);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("poweron.bin", ResourceType.SmartGlass), packet);
        }

        [Fact(Skip = "No real-world test data available")]
        public void TestAccelerometerMessage()
        {
            var message = new Msgs.Messages.AccelerometerMessage()
            {
                Timestamp = 0,
                AccelerationX = 1.0f,
                AccelerationY = 1.0f,
                AccelerationZ = 1.0f
            };

            var packet = AssembleSessionMessage(message, 0, 0);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("accelerometer.bin", ResourceType.SmartGlass), packet);
        }

        [Fact]
        public void TestAckMessage()
        {
            var message = new Msgs.Messages.AckMessage
            {
                LowWatermark = 0,
                ProcessedList = new HashSet<uint>(new uint[] { 1 }),
                RejectedList = new HashSet<uint>(new uint[] { }),
            };

            message.Header.RequestAcknowledge = false;
            message.Header.ChannelId = 0x1000000000000000;

            var packet = AssembleSessionMessage(message, 1, 0, targetParticipantId: 31);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("acknowledge.bin", ResourceType.SmartGlass), packet);
        }

        [Fact(Skip = "No real-world test data available")]
        public void TestCompassMessage()
        {
            var message = new Msgs.Messages.CompassMessage()
            {
                Timestamp = 0,
                MagneticNorth = 0,
                TrueNorth = 0
            };

            var packet = AssembleSessionMessage(message, 0, 0);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("compass.bin", ResourceType.SmartGlass), packet);
        }

        [Fact]
        public void TestDisconnectMessage()
        {
            var message = new Msgs.Messages.DisconnectMessage()
            {
                ErrorCode = 0,
                Reason = DisconnectReason.Unspecified
            };

            var packet = AssembleSessionMessage(message, 57, 31);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("disconnect.bin", ResourceType.SmartGlass), packet);
        }

        [Fact]
        public void TestFragmentMessage()
        {
            byte[] fragment = new byte[]
            {
                0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30,
                0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30,
                0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30,
                0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30,
                0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30,
                0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30,
                0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x31, 0x30, 0x30, 0x46,
                0x46, 0x37, 0x46, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30,
                0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30,
                0x30, 0x30, 0x30, 0x33, 0x37, 0x33, 0x30, 0x33, 0x35, 0x36, 0x36, 0x36, 0x32, 0x33, 0x36, 0x33,
                0x36, 0x33, 0x34, 0x36, 0x34, 0x33, 0x30, 0x33, 0x34, 0x33, 0x31, 0x36, 0x32, 0x33, 0x34, 0x33,
                0x34, 0x36, 0x35, 0x33, 0x34, 0x33, 0x37, 0x33, 0x32, 0x33, 0x36, 0x36, 0x33, 0x33, 0x35, 0x33,
                0x37, 0x33, 0x35, 0x33, 0x39, 0x36, 0x35, 0x33, 0x35, 0x33, 0x36, 0x33, 0x31, 0x36, 0x33, 0x33,
                0x34, 0x36, 0x33, 0x00, 0x00, 0x02, 0x00, 0x02, 0x00, 0x00, 0x83, 0xda, 0x00, 0x04, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3f, 0xa1, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x01, 0xf5, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x01, 0xf5, 0x60, 0x00, 0x02, 0x00, 0x05, 0x74, 0x69,
                0x74, 0x6c, 0x65, 0x00, 0x00, 0x14, 0x42, 0x6c, 0x75, 0x2d, 0x72, 0x61, 0x79, 0x20, 0x26, 0x20,
                0x44, 0x56, 0x44, 0x20, 0x50, 0x6c, 0x61, 0x79, 0x65, 0x72, 0x00, 0x00, 0x08, 0x73, 0x75, 0x62,
                0x74, 0x69, 0x74, 0x6c, 0x65, 0x00, 0x00, 0x00, 0x00
            };

            var message = new Msgs.Messages.FragmentMessage()
            {
                SequenceBegin = 22,
                SequenceEnd = 25,
                Data = fragment
            };
            message.Header.ChannelId = 148;
            message.Header.RequestAcknowledge = true;
            message.Header.IsFragment = true;
            message.Header.SessionMessageType = SessionMessageType.MediaState;
            var packet = AssembleSessionMessage(message, 24, 0, targetParticipantId: 31);

            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("fragment_media_state_2.bin", ResourceType.SmartGlass), packet);
        }

        [Fact]
        public void TestGameDvrRecordMessage()
        {
            var message = new Msgs.Messages.GameDvrRecordMessage()
            {
                StartTimeDelta = -60,
                EndTimeDelta = 0
            };
            message.Header.ChannelId = 0;
            message.Header.RequestAcknowledge = true;

            var packet = AssembleSessionMessage(message, 70, 1);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("gamedvr_record.bin", ResourceType.SmartGlass), packet);
        }

        [Fact]
        public void TestGamepadMessage()
        {
            var message = new Msgs.Messages.GamepadMessage()
            {
                State = new GamepadState()
                {
                    Timestamp = 0,
                    Buttons = GamepadButtons.B,
                    LeftTrigger = 0.0f,
                    RightTrigger = 0.0f,
                    LeftThumbstickX = 0.0f,
                    RightThumbstickX = 0.0f,
                    LeftThumbstickY = 0.0f,
                    RightThumbstickY = 0.0f
                }
            };

            message.Header.ChannelId = 180;
            message.Header.RequestAcknowledge = false;

            var packet = AssembleSessionMessage(message, 79, 41);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("gamepad.bin", ResourceType.SmartGlass), packet);
        }

        [Fact(Skip = "No real-world test data available")]
        public void TestGyrometerMessage()
        {
            var message = new Msgs.Messages.GyrometerMessage()
            {
                Timestamp = 0,
                AngularVelocityX = 0,
                AngularVelocityY = 0,
                AngularVelocityZ = 0
            };

            message.Header.ChannelId = 0;
            message.Header.RequestAcknowledge = false;

            var packet = AssembleSessionMessage(message, 0, 0);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("gyrometer.bin", ResourceType.SmartGlass), packet);
        }

        [Fact(Skip = "No real-world test data available")]
        public void TestInclinometerMessage()
        {
            var message = new Msgs.Messages.InclinometerMessage()
            {
                Timestamp = 0,
                Roll = 0,
                Pitch = 0,
                Yaw = 0
            };

            message.Header.ChannelId = 0;
            message.Header.RequestAcknowledge = false;

            var packet = AssembleSessionMessage(message, 0, 0);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("inclinometer.bin", ResourceType.SmartGlass), packet);
        }

        [Fact]
        public void TestJsonMessage()
        {
            var message = new Msgs.Messages.JsonMessage()
            {
                Json = "{\"msgid\":\"2ed6c0fd.2\",\"request\":\"GetConfiguration\"}"
            };

            message.Header.ChannelId = 151;
            message.Header.RequestAcknowledge = true;

            var packet = AssembleSessionMessage(message, 11, 31);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("json.bin", ResourceType.SmartGlass), packet);
        }

        [Fact]
        public void TestLocalJoinMessage()
        {
            var message = new Msgs.Messages.LocalJoinMessage()
            {
                DeviceType = DeviceType.Android,
                NativeWidth = 600,
                NativeHeight = 1024,
                DpiX = 160,
                DpiY = 160,
                DeviceCapabilities = DeviceCapabilities.SupportsAll,
                ClientVersion = 133713371,
                OsMajorVersion = 42,
                OsMinorVersion = 0,
                DisplayName = "package.name.here"
            };

            //FIXME: Version == 0 ?
            message.Header.ChannelId = 0;
            message.Header.RequestAcknowledge = true;
            message.Header.Version = 0;

            var packet = AssembleSessionMessage(message, 1, 31);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("local_join.bin", ResourceType.SmartGlass), packet);
        }

        [Fact]
        public void TestMediaCommandMessage()
        {
            var message = new Msgs.Messages.MediaCommandMessage()
            {
                State = new MediaCommandState()
                {
                    Command = MediaControlCommands.FastForward,
                    TitleId = 274278798,
                    SeekPosition = 0
                }
            };
            message.Header.ChannelId = 153;
            message.Header.RequestAcknowledge = true;

            var packet = AssembleSessionMessage(message, 597, 32);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("media_command.bin", ResourceType.SmartGlass), packet);
        }

        [Fact(Skip = "No real-world test data available")]
        public void TestOrientationMessage()
        {
            var message = new Msgs.Messages.OrientationMessage()
            {
                Timestamp = 0,
                RotationMatrixValue = 0,
                RotationW = 0,
                RotationX = 0,
                RotationY = 0,
                RotationZ = 0
            };

            message.Header.ChannelId = 0;
            message.Header.RequestAcknowledge = false;

            var packet = AssembleSessionMessage(message, 0, 0);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("orientation.bin", ResourceType.SmartGlass), packet);
        }

        [Fact]
        public void TestPowerOffMessage()
        {
            var message = new Msgs.Messages.PowerOffMessage()
            {
                LiveId = "FD00112233FFEE66"
            };

            message.Header.ChannelId = 0;
            message.Header.RequestAcknowledge = true;

            var packet = AssembleSessionMessage(message, 1882, 2);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("power_off.bin", ResourceType.SmartGlass), packet);
        }

        [Fact]
        public void TestStartChannelRequestMessage()
        {
            var message = new Msgs.Messages.StartChannelRequestMessage()
            {
                ActivityId = 0,
                ChannelRequestId = 1,
                ServiceType = ServiceType.SystemInput,
                TitleId = 0
            };

            message.Header.ChannelId = 0;
            message.Header.RequestAcknowledge = true;

            var packet = AssembleSessionMessage(message, 2, 31);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("start_channel_request.bin", ResourceType.SmartGlass), packet);
        }

        [Fact(Skip = "No real-world test data available")]
        public void TestStopChannelMessage()
        {
            var message = new Msgs.Messages.StopChannelMessage()
            {
                ChannelIdToStop = 2
            };

            message.Header.ChannelId = 0;
            message.Header.RequestAcknowledge = false;

            var packet = AssembleSessionMessage(message, 0, 0);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("stop_channel.bin", ResourceType.SmartGlass), packet);
        }

        [Fact]
        public void TestSystemTextAcknowledgeMessage()
        {
            var message = new Msgs.Messages.SystemTextAcknowledgeMessage()
            {
                TextSessionId = 8,
                TextVersionAck = 2
            };

            message.Header.ChannelId = 0x9A;
            message.Header.RequestAcknowledge = true;

            var packet = AssembleSessionMessage(message, 0x2e, 0, targetParticipantId: 32);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("system_text_acknowledge.bin", ResourceType.SmartGlass), packet);
        }

        [Fact]
        public void TestSystemTextDoneMessage()
        {
            var message = new Msgs.Messages.SystemTextDoneMessage()
            {
                Flags = 0,
                Result = TextResult.Cancel,
                TextSessionId = 0,
                TextVersion = 0
            };

            message.Header.ChannelId = 0x9A;
            message.Header.RequestAcknowledge = true;

            var packet = AssembleSessionMessage(message, 0x5a, 0, targetParticipantId: 32);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("system_text_done.bin", ResourceType.SmartGlass), packet);
        }

        [Fact]
        public void TestSystemTextInputMessage()
        {
            var message = new Msgs.Messages.SystemTextInputMessage()
            {
                BaseVersion = 1,
                Delta = new TextDelta[0],
                Flags = 0,
                SelectionStart = -1,
                SelectionLength = -1,
                SubmittedVersion = 2,
                TextChunk = "h",
                TextChunkByteStart = 0,
                TextSessionId = 8,
                TotalTextBytelength = 1
            };

            message.Header.ChannelId = 0x9A;
            message.Header.RequestAcknowledge = true;

            var packet = AssembleSessionMessage(message, 0x97, 32);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("system_text_input.bin", ResourceType.SmartGlass), packet);
        }

        [Fact]
        public void TestSystemTouchMessage()
        {
            var message = new Msgs.Messages.SystemTouchMessage()
            {
                Timestamp = 182459592,
                Touchpoints = new TouchPoint[]
                {
                    new TouchPoint()
                    {
                        Action = TouchAction.Down,
                        Id = 1,
                        PointX = 244,
                        PointY = 255
                    }
                }
            };

            message.Header.ChannelId = 0x98;
            message.Header.RequestAcknowledge = true;

            var packet = AssembleSessionMessage(message, 26, 32);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("system_touch.bin", ResourceType.SmartGlass), packet);
        }

        [Fact]
        public void TestTitleLaunchMessage()
        {
            var message = new Msgs.Messages.TitleLaunchMessage()
            {
                Location = ActiveTitleLocation.Fill,
                Uri = "ms-xbl-0D174C79://default/"
            };

            message.Header.ChannelId = 0;
            message.Header.RequestAcknowledge = true;

            var packet = AssembleSessionMessage(message, 685, 32);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("title_launch.bin", ResourceType.SmartGlass), packet);
        }

        [Fact(Skip = "No real-world test data available")]
        public void TestTitleTouchMessage()
        {
            var message = new Msgs.Messages.TitleTouchMessage()
            {

            };

            message.Header.ChannelId = 0;
            message.Header.RequestAcknowledge = false;

            var packet = AssembleSessionMessage(message, 0, 0);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("title_touch.bin", ResourceType.SmartGlass), packet);
        }

        [Fact(Skip = "Un/snap functionality is deprecated")]
        public void TestUnsnapMessage()
        {
            var message = new Msgs.Messages.UnsnapMessage()
            {
                Unknown = 1
            };

            message.Header.ChannelId = 0;
            message.Header.RequestAcknowledge = false;

            var packet = AssembleSessionMessage(message, 0, 0);
            Assert.Equal<byte[]>(ResourcesProvider.GetBytes("unsnap.bin", ResourceType.SmartGlass), packet);
        }
    }
}