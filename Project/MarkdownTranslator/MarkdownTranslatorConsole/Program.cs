using MarkdownTranslatorCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using static MarkdownTranslatorCore.MarkdownTranslator;

namespace MarkdownTranslatorConsole
{
    internal class Program
    {
        const string JSON_TRANSLATOR_PAPAGO = "PAPAGO";

        public class TranslatorJson
        {
            public class Papago
            {
                public string ID;
                public string SECRET;
            }

            [JsonProperty(nameof(TranslatorJsonVersion))]
            public const string TranslatorJsonVersion = "1";

            public string SelectTranslator = "";

            [JsonProperty(nameof(Papago))]
            public Papago T_Papago = new Papago();
        }

        static void Main(string[] args)
        {
#if DEBUG
            args = new string[] { "TranslatorJson.json", "README.md", "README_EN.md", "ko", "en" };
#endif

            if (!File.Exists(args[0])) goto create_file;
            var check_json = JObject.Parse(File.ReadAllText(args[0]));

            if (check_json[nameof(TranslatorJson.TranslatorJsonVersion)].ToString() != TranslatorJson.TranslatorJsonVersion)
            {
                File.Move(args[0], $"OLD_{args[0]}");
            }

        create_file:

            if (!File.Exists(args[0]))
            {
                File.WriteAllText(args[0], JsonConvert.SerializeObject(new TranslatorJson()));
            }

            TranslatorJson json = JsonConvert.DeserializeObject<TranslatorJson>(File.ReadAllText(args[0]));
            ITranslator translator = null;

            switch (json.SelectTranslator)
            {
                case nameof(TranslatorJson.Papago):
                    translator = new PapagoTranslator(json.T_Papago.ID, json.T_Papago.SECRET,
                        Enum.GetName(typeof(PapagoTranslator.Language),
                        (PapagoTranslator.Language)Enum.Parse(typeof(PapagoTranslator.Language), args[3])).Replace("_", "-"),
                        Enum.GetName(typeof(PapagoTranslator.Language),
                        (PapagoTranslator.Language)Enum.Parse(typeof(PapagoTranslator.Language), args[4])).Replace("_", "-"));
                    break;
                default:
                    throw new Exception("not supported translator");
            }

            string error_msg = Run(translator, args[1], args[2]);
            Console.WriteLine($"[MSG]:{error_msg}");
        }
    }
}
