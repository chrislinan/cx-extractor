using System;
using System.Text;
using System.IO;

namespace cx_extractor
{
    class Demo
    {
        static void Main(string[] args)
        {
            //demo
            StreamReader objReader = new StreamReader("E:\\Documents\\123.html", Encoding.Default);
            string sLine = objReader.ReadToEnd();
            objReader.Close();
            Console.Write(TextExtract.parse(sLine));
            Console.Read();
        }
    }
}
