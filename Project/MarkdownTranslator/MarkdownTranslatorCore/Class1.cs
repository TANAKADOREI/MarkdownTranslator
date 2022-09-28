using Markdig;
using Markdig.Extensions.Footnotes;
using Markdig.Extensions.Tables;
using Markdig.Extensions.TaskLists;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownTranslatorCore
{
    public static class MarkdownTranslator
    {
        private static void InlineProcces(Inline inline)
        {
            string data = GetString(inline.Span);
            if (data.Trim().Length == 0)
            {
                return;
            }
            else if (inline is LiteralInline)
            {

            }
            else if (inline is CodeInline)
            {
                return;
            }
            else if (inline is TaskList)
            {
                return;
            }
            else
            {
                throw null;
            }

            Console.WriteLine($"{inline.GetType().Name},{GetString(inline.Span)}");
            Replace(inline.Span, TRANSLATOR.Translate(GetString(inline.Span)));
            //Replace(inline.Span, $"PP{GetString(inline.Span)}PP");
        }

        private static void IntoInline(Inline inline)
        {
            //ignore list
            if (inline is LinkInline)
            {
                return;
            }

            if (inline is ContainerInline)
            {
                foreach (var child in inline as ContainerInline)
                {
                    IntoInline(child);
                }
            }
            else if (inline is LeafInline)
            {
                InlineProcces(inline);
            }
            else
            {
                throw null;
            }
        }

        private static void BlockProcess(Block block)
        {
            if (block is LeafBlock)
            {
                if (block is CodeBlock)
                {
                    if (block is FencedCodeBlock)
                    {
                        return;
                    }
                    else
                    {
                        throw null;
                    }
                }
                else if (block is HeadingBlock)
                {
                }
                else if (block is HtmlBlock)
                {
                    return;
                }
                else if (block is EmptyBlock)
                {
                    return;
                }
                else if (block is ParagraphBlock)
                {
                }
                else if (block is ThematicBreakBlock)
                {
                    return;
                }
                else if (block is LinkReferenceDefinition)
                {

                }
                else
                {
                    throw null;
                }

                foreach (var inline in ((LeafBlock)block).Inline)
                {
                    IntoInline(inline);
                }
            }
            else if (block is ContainerBlock)
            {
                if (block is ListBlock)
                {

                }
                else if (block is ListItemBlock)
                {

                }
                else if (block is QuoteBlock)
                {

                }
                else if (block is LinkReferenceDefinitionGroup)
                {
                    return;
                }
                else if (block is Table)
                {

                }
                else if (block is TableRow)
                {

                }
                else if (block is TableCell)
                {

                }
                else if (block is FootnoteGroup)
                {
                    return;
                }
                else
                {
                    throw null;
                }

                IntoContainerBlock(block as ContainerBlock);
            }
            else
            {
                throw null;
            }
        }

        private static void IntoContainerBlock(IEnumerable<Block> container_block)
        {
            foreach (var child in container_block)
            {
                BlockProcess(child);
            }
        }

        private static string SOURCE;
        private static StringBuilder BUILDER;
        private static ITranslator TRANSLATOR;
        private static MarkdownDocument DOC;

        public static string Run(ITranslator translator, string source_path, string dest_path)
        {
            SOURCE = File.ReadAllText(source_path);
            BUILDER = new StringBuilder(SOURCE);
            TRANSLATOR = translator;
            DOC = Markdown.Parse(SOURCE, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build(), null);

            foreach (var child in DOC)
            {
                BlockProcess(child);
            }

            File.WriteAllText(dest_path, BUILDER.ToString());

            return "complete";
        }

        private static string GetString(SourceSpan target)
        {
            return SOURCE.Substring(target.Start, target.Length);
        }

        static int OFFSET = 0;

        private static void Replace(SourceSpan target, string new_string)
        {
            BUILDER.Remove(target.Start + OFFSET, target.Length);
            BUILDER.Insert(target.Start + OFFSET, new_string);
            OFFSET += new_string.Length - target.Length;
        }
    }
}
