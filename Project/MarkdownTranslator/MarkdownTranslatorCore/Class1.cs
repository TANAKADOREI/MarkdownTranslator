using GoogleTranslateFreeApi;
using Markdig;
using Markdig.Syntax;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarkdownTranslatorCore
{
    public static class MarkdownTranslator
    {
        public static string Run(string source, out string _dest_en,out string _dest_jp)
        {
            string dest_en = "";
            string dest_jp = "";

            var task = Task.Run(async () =>
            {
                var md = Markdown.Parse(source);
                var translator = new GoogleTranslator();

                //Language from = Language.Auto;
                Language from = Language.Korean;
                Language to_en = Language.English;
                Language to_jp = Language.Japanese;


                foreach (LeafBlock i in from i in md where i is LeafBlock select i)
                {
                    if (i.Inline == null) continue;
                    var text = i.Inline.FirstChild.ToString();

                    var result = await translator.TranslateLiteAsync(text, from, to_en);

                    if (result.Corrections.TextWasCorrected)
                        Console.WriteLine(string.Join(",", result.Corrections.CorrectedWords));

                    Console.WriteLine(text);
                    Console.WriteLine($"[{result.MergedTranslation}]");
                }

                return "d";
            });

            while(true) { }

            _dest_en = dest_en;
            _dest_jp = dest_jp;

            return task.Result;
        }
    }
}
