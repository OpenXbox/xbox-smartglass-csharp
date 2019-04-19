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
using System.Threading.Tasks;

namespace SmartGlass.Nano.Consumer
{
    public class SimpleHttpServer : IDisposable
    {
        private bool _disposed = false;

        private CancellationTokenSource _cancellationTokenSource;
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
        /// <param name="responseContent">File content to serve.</param>
        /// <param name="port">Port of the server.</param>
        public SimpleHttpServer(string responseContent, int port)
        {
            _responseContent = responseContent;
            _port = port;

            _cancellationTokenSource = new CancellationTokenSource();
            _listener = new HttpListener();

            Initialize(responseContent, port);
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
            Task.Run(() => 
            {
                _listener.Prefixes.Add("http://*:" + _port.ToString() + "/");
                _listener.Start();

                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    HttpListenerContext context = _listener.GetContext();
                    Process(context);
                }
            }, _cancellationTokenSource.Token);
        }
    
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _cancellationTokenSource.Cancel();
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