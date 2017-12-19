using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        /// 
        //done
        public bool ParseRequest()
        {
          //  throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter   
            contentLines = requestString.Split(new[] { "\r\n" }, StringSplitOptions.None);
          
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (contentLines.Length < 3)
                return false;

            // Parse Request line         
            if (!ParseRequestLine())
                return false;
            
            // Validate blank line exists
            if (!ValidateBlankLine())
                return false;
            
            // Load header lines into HeaderLines dictionary
            if (!LoadHeaderLines())
                return false;
            
            return true;
        }

        //done
        private bool ParseRequestLine()
        {
            //throw new NotImplementedException();
            requestLines = contentLines[0].Split(' ');
            if (requestLines.Length != 3)
            {
                return false;
            }
            //Method
            if (requestLines[0] == RequestMethod.GET.ToString())
            {
                method = RequestMethod.GET;
            }
            else if (requestLines[0] == RequestMethod.HEAD.ToString())
            {
                method = RequestMethod.HEAD;
                Console.WriteLine(method.ToString());
            }
            else if (requestLines[0] == RequestMethod.POST.ToString())
            {
                method = RequestMethod.POST;
            }
            else
                return false;
            //HTTP version
            //if (requestLines[2] != HTTPVersion.HTTP09.ToString() && requestLines[2] != HTTPVersion.HTTP10.ToString() && requestLines[2] != HTTPVersion.HTTP11.ToString())
            //    return false;
            if (requestLines[2] == "HTTP/0.9")
            {
                httpVersion = HTTPVersion.HTTP09;
            }
            else if (requestLines[2] == "HTTP/1.0")
            {
                httpVersion = HTTPVersion.HTTP10;
            }
            else if (requestLines[2] == "HTTP/1.1")
            {
                httpVersion = HTTPVersion.HTTP11;
            }
            else
            {
                return false;
            }
            //URI
            if (ValidateIsURI(requestLines[1]))
                relativeURI = requestLines[1];
            else {
                return false;
            }

            return true;
        }
        
        //done
        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }
        
        //done
        private bool LoadHeaderLines()
        {
            //throw new NotImplementedException();
            headerLines = new Dictionary<string, string>();
            int count = 0;
            for (int i = 1; i < contentLines.Length; i++)
            {
                string[] tmp = contentLines[i].Split(new[] { ": " }, StringSplitOptions.None);
                if (tmp.Length == 2)
                {
                    count++;
                    headerLines.Add(tmp[0], tmp[1]);
                }
                else
                {
                    break;
                }
            }
            if (count < 1 && httpVersion == HTTPVersion.HTTP11 )  /// ver 1.1 requires a host: header
                return false;
            return true;
        }
        
        //done
        private bool ValidateBlankLine()
        {
            //throw new NotImplementedException();
            int i;
            for (i=1; i < contentLines.Length; i++)
            {
                if (!contentLines[i].Contains(":"))
                {
                    break;
                }
            }
            if (string.IsNullOrWhiteSpace(contentLines[i]) && i == contentLines.Length-2)
                return true;

            return false;
        }

    }
}
