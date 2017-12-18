using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectionPath)
        {
            // throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            if (!string.IsNullOrWhiteSpace(content))
            {
                headerLines.Add("Content-Type: " + contentType);
                headerLines.Add("Content-Length: " + content.Length.ToString());
            }
            headerLines.Add("Date: "+ DateTime.Now.ToString());
            if(!String.IsNullOrEmpty( redirectionPath))
            headerLines.Add("location: "+redirectionPath);


            // TODO: Create the request string
            //Request request = new Request(redirectionPath);
            responseString = GetStatusLine(code);
            foreach (var item in headerLines)
            {
                responseString += item + "\r\n";
            }
            responseString += "\r\n" +content ;
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = "HTTP/1.0 "; /// is it only http/1.0 !!
            switch (code)
            {
                case StatusCode.OK:
                    statusLine += "200 " +StatusCode.OK + "\n";
                    break;
                case StatusCode.InternalServerError:
                    statusLine += "500 "+ StatusCode.InternalServerError + "\n";
                    break;
                case StatusCode.NotFound:
                    statusLine += "404 "+ StatusCode.NotFound + "\n";

                    break;
                case StatusCode.BadRequest:
                    statusLine += "400 "+StatusCode.BadRequest + "\n";

                    break;
                case StatusCode.Redirect:
                    statusLine += "301 " + StatusCode.Redirect + "\n";
                    break;
                default:
                    break;
            }
            return statusLine;
        }
    }
}
