using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace MarkdownTranslatorCore
{
    public class PapagoTranslator : ITranslator
    {
        public enum Language
        {
            ko,
            ja,
            zh_CN,
            zh_TW,
            hi,
            en,
            es,
            fr,
            de,
            pt,
            vi,
            id,
            fa,
            ar,
            mm,
            th,
            ru,
            it,
            unk,
        }

        const string URL = "https://openapi.naver.com/v1/papago/n2mt";
        readonly string LangFrom, LangTo;
        readonly string ID;
        readonly string SECRET;

        public PapagoTranslator(string api_id,string api_secret, string lang_from, string lang_to)
        {
            LangFrom = lang_from;
            LangTo = lang_to;
            ID = api_id;
            SECRET = api_secret;
        }

        public string Translate(string source)
        {
            int count = 0;

        retry:
            try
            {
                string sParam = string.Format("source={0}&target={1}&text={2}", LangFrom, LangTo, source);
                byte[] bytearry = Encoding.UTF8.GetBytes(sParam);

                WebRequest webRequest = WebRequest.Create(URL);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Headers.Add("X-Naver-Client-Id", ID);
                webRequest.Headers.Add("X-Naver-Client-Secret", SECRET);
                webRequest.ContentLength = bytearry.Length;

                Stream stream = webRequest.GetRequestStream();
                stream.Write(bytearry, 0, bytearry.Length);
                stream.Close();

                WebResponse webResponse = webRequest.GetResponse();
                stream = webResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream);
                string sReturn = streamReader.ReadToEnd();

                streamReader.Close();
                stream.Close();
                webResponse.Close();

                JObject jObject = JObject.Parse(sReturn);
                return jObject["message"]["result"]["translatedText"].ToString();
            }
            catch(Exception e)
            {
                count++;

                if (count >= 10)
                {
                    throw e;
                }

                Thread.Sleep(5000);
                goto retry;
            }
        }
    }
}
