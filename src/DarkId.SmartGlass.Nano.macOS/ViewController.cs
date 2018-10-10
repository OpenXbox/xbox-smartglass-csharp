using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AppKit;
using DarkId.SmartGlass.Nano.AVFoundation;
using Foundation;

namespace DarkId.SmartGlass.Nano.macOS
{
    public partial class ViewController : NSViewController
    {
        private static readonly string _hostname = "192.168.2.26";
        private SmartGlassClient _smartGlassClient;
        private NanoClient _nanoClient;
        private AVFoundationConsumer _avConsumer;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // TODO: Extension method that just returns void from tasks, because this is dumb:
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() => CreateClient());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        public override void ViewDidAppear()
        {
            base.ViewDidAppear();
        }

        public async Task CreateClient()
        {
            Debug.WriteLine($"Connecting to console...");

            _smartGlassClient = await SmartGlassClient.ConnectAsync(_hostname);

            var broadcastChannel = await _smartGlassClient.GetBroadcastChannelAsync();
            var result = await broadcastChannel.StartGamestreamAsync();

            Debug.WriteLine($"Connecting to Nano, TCP: {result.TcpPort}, UDP: {result.UdpPort}");

            _avConsumer = new AVFoundationConsumer();

            _nanoClient = new NanoClient(_hostname, result.TcpPort, result.UdpPort, new Guid(), _avConsumer);
            _nanoClient.Initialize();

            Debug.WriteLine($"Nano connected and running.");
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
