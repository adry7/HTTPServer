using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //FileStream file;
            //TextWriter tw;
            //if (File.Exists("log.txt"))
            //{
            //    file = File.Create("log.txt");
            //    tw = new StreamWriter(file);
            //    tw.Write(ex.Message);

            //}
            //else
            //{
                File.AppendAllText("log.txt",ex.Message + DateTime.Now+"\r\n");
               
           
            // for each exception write its details associated with datetime 
        }
    }
}
