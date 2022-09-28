using System;
using System.Collections.Generic;
using System.Text;

namespace MarkdownTranslatorCore
{
    public interface ITranslator
    {
        string Translate(string source);
    }
}
