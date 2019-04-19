// MIT License - Copyright (c) 2016 Can Güney Aksakalli
// https://aksakalli.github.io/2014/02/24/simple-http-server-with-csparp.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace SmartGlass.Nano.Consumer
{
    public class SimpleHttpServer : IDisposable
    {
        private bool _disposed = false;

        private Thread _serverThread;
        private string _responseContent;
        private HttpListener _listener;
        private int _port;
    
        public int Port
        {
            get { return _port; }
            private set { }
        }
    
        /// <summary>
        /// Construct server with given port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        /// <param name="port">Port of the server.</param>
        public SimpleHttpServer(string path, int port)
        {
            this.Initialize(path, port);
        }
    
        /// <summary>
        /// Construct server with suitable port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        public SimpleHttpServer(string path)
        {
            //get an empty port
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            this.Initialize(path, port);
        }
    
        private void Listen()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://*:" + _port.ToString() + "/");
            _listener.Start();
            while (true)
            {
                HttpListenerContext context = _listener.GetContext();
                Process(context);
            }
        }
    
        private void Process(HttpListenerContext context)
        {
            var content = System.Text.UTF8Encoding.UTF8.GetBytes(_responseContent);
            string filename = context.Request.Url.AbsolutePath;
            filename = filename.Substring(1);

            var dateString = DateTime.Now.ToString("r");
            //Adding permanent http response headers
            context.Response.ContentType = "application/octet-stream";
            context.Response.ContentLength64 = _responseContent.Length;
            context.Response.AddHeader("Date", dateString);
            context.Response.AddHeader("Last-Modified", dateString);

            context.Response.OutputStream.Write(content, 0, content.Length);
            
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.OutputStream.Flush();
            context.Response.OutputStream.Close();
        }
    
        private void Initialize(string responseContent, int port)
        {
            this._responseContent = responseContent;
            this._port = port;
            _serverThread = new Thread(this.Listen);
            _serverThread.IsBackground = true;
            _serverThread.Start();
        }
    
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _serverThread.Abort();
                    _listener.Stop();
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