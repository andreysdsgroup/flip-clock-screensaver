using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Fliqlo
{
    public static class LocalServer
    {
        private static HttpListener _listener;
        private static int _port = 0;
        private static readonly object _lock = new object();

        public static int Port
        {
            get
            {
                EnsureStarted();
                return _port;
            }
        }

        public static string GetUrl(string query)
        {
            EnsureStarted();
            string q = string.IsNullOrEmpty(query) ? "" : (query.StartsWith("?") ? query : "?" + query);
            return "http://127.0.0.1:" + _port + "/" + q;
        }

        public static void EnsureStarted()
        {
            lock (_lock)
            {
                if (_listener != null && _listener.IsListening)
                    return;

                int[] candidatePorts = new int[] { 8011, 0 };
                foreach (int cand in candidatePorts)
                {
                    try
                    {
                        int p = cand == 0 ? GetFreePort() : cand;
                        HttpListener listener = new HttpListener();
                        listener.Prefixes.Add("http://127.0.0.1:" + p + "/");
                        listener.Start();
                        _listener = listener;
                        _port = p;
                        ThreadPool.QueueUserWorkItem(ListenLoop);
                        return;
                    }
                    catch
                    {
                        // Port might be in use, try next
                    }
                }
            }
        }

        private static int GetFreePort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        private static void ListenLoop(object state)
        {
            while (_listener != null && _listener.IsListening)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    ThreadPool.QueueUserWorkItem(ProcessRequest, context);
                }
                catch
                {
                    break;
                }
            }
        }

        private static void ProcessRequest(object state)
        {
            HttpListenerContext context = (HttpListenerContext)state;
            try
            {
                string rawPath = context.Request.Url.AbsolutePath;
                string path = rawPath.TrimStart('/');
                if (string.IsNullOrEmpty(path))
                    path = "index.html";

                byte[] data = WebAssets.Get(path);
                if (data == null && path.Contains("/"))
                {
                    string filenameOnly = Path.GetFileName(path);
                    data = WebAssets.Get(filenameOnly);
                }

                if (data != null)
                {
                    context.Response.ContentType = GetContentType(path);
                    context.Response.ContentLength64 = data.Length;
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    context.Response.Headers.Add("Cache-Control", "public, max-age=86400");
                    context.Response.OutputStream.Write(data, 0, data.Length);
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
                context.Response.Close();
            }
            catch
            {
                try
                {
                    context.Response.StatusCode = 500;
                    context.Response.Close();
                }
                catch { }
            }
        }

        private static string GetContentType(string path)
        {
            string ext = Path.GetExtension(path).ToLowerInvariant();
            switch (ext)
            {
                case ".html": return "text/html; charset=utf-8";
                case ".css": return "text/css; charset=utf-8";
                case ".js": return "application/javascript; charset=utf-8";
                case ".woff": return "font/woff";
                case ".ttf": return "font/ttf";
                case ".eot": return "application/vnd.ms-fontobject";
                case ".png": return "image/png";
                default: return "application/octet-stream";
            }
        }

        public static void Stop()
        {
            lock (_lock)
            {
                if (_listener != null)
                {
                    try { _listener.Stop(); } catch { }
                    _listener = null;
                }
            }
        }
    }
}
