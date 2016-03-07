using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Opendag
{
    class Program
    {
        private static PortWriter pw;

        [STAThread]
        static void Main(string[] args)
        {
            pw = new PortWriter();
            
            if (pw.TryOpenPort(args[0]))
                Console.WriteLine("Succesfully opened " + args[0]);
            else
                Console.WriteLine("Failed to open " + args[0]);

            Console.WriteLine();

            var server = new WebServer(SendResponse, "http://localhost:8080/test/");
            server.Run();

            Console.WriteLine("A simple webserver for the Open Day of University of Utrecht.");
            Console.ReadKey();
            server.Stop();
        }

        /// <summary>
        /// Simple function to send the HTTP response when someone opens/pings the server
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string SendResponse(HttpListenerRequest request)
        {
            // Ping my student server containing the int whether the spray should or shouldn't
            WebRequest r = WebRequest.Create("https://www.students.science.uu.nl/~4173309/cigniter/spray/checkspray.php");
            WebResponse response = r.GetResponse();
            Stream data = response.GetResponseStream();

            string html = String.Empty;

            using (StreamReader sr = new StreamReader(data))
                html = sr.ReadToEnd();

            if (html == "0")
                return string.Format(htmlPage(html + "\n" + DateTime.Now));

            pw.Spray();
            return htmlPage("SPRAYING");
        }

        /// <summary>
        /// An ugly function to create the desired HTML page
        /// </summary>
        /// <param name="body"> The desired value for the body of the HTML response </param>
        /// <returns></returns>
        private static string htmlPage(string body)
        {
            StringBuilder page = new StringBuilder();
            page.Append("<html> \n <meta http-equiv=\"refresh\" content=\"5;\" /> \n <head></head> \n <body> \n  ");
            page.Append(body);
            page.Append("</body>\n</html>");
            return page.ToString();
        }
    }

    internal class PortWriter
    {
        private static SerialPort writePort;

        public PortWriter()
        {

        }

        public bool TryOpenPort(string portName)
        {
            try
            {
                writePort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
                writePort.Open();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void Spray()
        {
            byte[] buffer = new byte[1];
            buffer[0] = byte.MaxValue;
            writePort.Write(buffer, 0, buffer.Length);
        }
    }

    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;

        public WebServer(string[] prefixes, Func<HttpListenerRequest, string> method)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");

            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // A responder method is required
            if (method == null)
                throw new ArgumentException("method");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _responderMethod = method;
            _listener.Start();
        }

        public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method) { }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                string rstr = _responderMethod(ctx.Request);
                                byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}
