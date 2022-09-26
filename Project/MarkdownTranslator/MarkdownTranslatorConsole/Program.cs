using MarkdownTranslatorCore;
using System;
using System.IO;

namespace MarkdownTranslatorConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            args = new string[] { "README.md", "README_EN.md" , "README_JP.md" };
            string result_en;
            string result_jp;
            string error_msg = MarkdownTranslator.Run(File.ReadAllText(args[0]), out result_en,out result_jp);
            if (error_msg != null)
                Console.WriteLine(error_msg);
            else
            {
                File.WriteAllText(args[1], result_en);
                File.WriteAllText(args[2], result_jp);
                Console.WriteLine("Complete");
            }
        }
    }
}
