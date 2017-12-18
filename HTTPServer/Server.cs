using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;
        //done
        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);

            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket.Bind(ipEnd);
        }
        //done
        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(100);

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();

                Console.WriteLine("New client accepted: {0}", clientSocket.RemoteEndPoint);
                //RemoteEndPoint Gets the IP address and Port number of the client

                //Create a thread that works on HandleConnection function
                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                //Start the thread
                newthread.Start(clientSocket);

            }
        }
        //done
        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientSocket = (Socket)obj;

            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSocket.ReceiveTimeout = 0;

            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] requestReceived = new byte[1024 * 1024];
                    int receivedLength = clientSocket.Receive(requestReceived);
                    string req = Encoding.ASCII.GetString(requestReceived, 0, receivedLength);

                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", clientSocket.RemoteEndPoint);
                        break;
                    }

                    // TODO: Create a Request object using received request string
                    Request requestObj = new Request(req);

                    // TODO: Call HandleRequest Method that returns the response
                    Response responseObj = HandleRequest(requestObj);

                    // TODO: Send Response back to client
                    byte[] messageByteArray = Encoding.ASCII.GetBytes(responseObj.ResponseString);
                    clientSocket.Send(messageByteArray);

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSocket.Close();
        }

        Response HandleRequest(Request request)

        {
            // throw new NotImplementedException();
            string content = "", content_type = "text / html";
            string redirect, physical_path;

            try
            {
                

                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    content = LoadDefaultPage( Configuration.BadRequestDefaultPageName);
                    return new Response(StatusCode.BadRequest, content_type, content, null);
                }
                if (request.relativeURI == "/")
                    return new Response(StatusCode.OK, content_type, LoadDefaultPage("main.html"), null);

                //TODO: map the relativeURI in request to get the physical path of the resource.
                physical_path = Configuration.RootPath + request.relativeURI;

                //TODO: check for redirect
                redirect = GetRedirectionPagePathIFExist(request.relativeURI);
                if (!string.IsNullOrEmpty(redirect))
                {
                    physical_path = Configuration.RootPath + redirect;
                    content = File.ReadAllText(physical_path);
                    return new Response(StatusCode.Redirect, content_type, content, redirect);
                }
                //TODO: check file exists
                //TODO: read the physical file
                if (File.Exists(physical_path))
                    content = File.ReadAllText(physical_path);
                else
                {

                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    return new Response(StatusCode.NotFound, content_type, content, redirect);
                }

                // Create OK response
                return new Response(StatusCode.OK, content_type, content, null);
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error. 
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                return new Response(StatusCode.InternalServerError, content_type, content, null);
            }
        }
        //done
        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            try
            {
                relativePath = relativePath.Remove(relativePath.IndexOf('/'), 1);
                return Configuration.redirectionRules[relativePath];
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        //done
        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string           
            try
            {
                var content = File.ReadAllText(filePath);
                return content;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return string.Empty;
                // throw;
            }
            // else read file and return its content

        }


        //done
        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                Configuration.redirectionRules = new Dictionary<string, string>();
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    // TODO: using the filepath paramter read the redirection rules from file 
                    // then fill Configuration.RedirectionRules dictionary 
                    var str = line.Split(',');
                    Configuration.redirectionRules.Add(str[0], str[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // Environment.Exit(1);
            }
        }
    }
}
