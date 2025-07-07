using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BombRushMP.Common
{
    public static class TMPFilter
    {
        public class Criteria
        {
            public enum Kinds
            {
                Whitelist,
                Blacklist
            }
            public Kinds ListKind = Kinds.Whitelist;
            public string[] List;

            public Criteria(string[] list, Kinds listKind)
            {
                List = list;
                ListKind = listKind;
            }

            internal bool CheckTag(string tag)
            {
                if (List.Contains(tag.ToLowerInvariant()))
                    return ListKind == Kinds.Whitelist;
                else
                    return ListKind == Kinds.Blacklist;
            }
        }


        public static string[] EnclosingTags =
        {
            "align",
            "allcaps",
            "b",
            "color",
            "cspace",
            "font",
            "font-weight",
            "gradient",
            "i",
            "indent",
            "line-height",
            "link",
            "lowercase",
            "margin",
            "mark",
            "mspace",
            "nobr",
            "noparse",
            "pos",
            "rotate",
            "s",
            "size",
            "smallcaps",
            "style",
            "sub",
            "sup",
            "u",
            "uppercase",
            "voffset",
            "width"
        };

        public static bool IsValidChatMessage(string message)
        {
            message = Sanitize(message);
            message = RemoveAllTags(message);
            if (string.IsNullOrWhiteSpace(message))
                return false;
            return true;
        }

        public static string RemoveAllTags(string text)
        {
            var rich = new Regex(@"<[^>]*>");
            text = rich.Replace(text, string.Empty);
            return text;
        }

        public static string EscapeAllTags(string text)
        {
            return FilterTags(text, new Criteria([], Criteria.Kinds.Whitelist));
        }

        public static string FilterTags(string text, Criteria criteria)
        {
            var inTag = false;
            var tagStartIndex = 0;
            var tag = "";
            var strBuilder = new StringBuilder(text);
            var offset = 0;

            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                if (!inTag)
                {
                    if (c == '<')
                    {
                        inTag = true;
                        tag = "";
                        tagStartIndex = i;
                        continue;
                    }
                }
                else
                {
                    if (c == '<')
                    {
                        inTag = true;
                        tag = "";
                        tagStartIndex = i;
                        continue;
                    }
                    if (c == '>')
                    {
                        inTag = false;

                        if (tag[0] == '/')
                            tag = tag.Substring(1);

                        if (tag[0] == '#')
                            tag = "color";

                        tag = tag.ToLowerInvariant();

                        if (tag.Contains("="))
                        {
                            tag = tag.Split('=')[0];
                        }

                        if (!criteria.CheckTag(tag))
                        {
                            strBuilder.Insert(tagStartIndex + 1 + offset, "​");
                            offset += 1;

                            // Replace with full width versions of < and >
                            //strBuilder[tagStartIndex] = '\uFF1C';
                            //strBuilder[i] = '\uFF1E';
                        }
                        continue;
                    }
                    tag += c;
                }
            }
            return strBuilder.ToString();
        }

        public static string Sanitize(string text)
        {
            text = text.Replace("\n", "");
            text = text.Replace("\t", "");
            text = text.Replace("\r", "");
            return text;
        }

        public static string CloseAllTags(string text)
        {
            var inTag = false;
            var tag = "";
            var tags = new List<string>();

            foreach (var c in text)
            {
                if (!inTag)
                {
                    if (c == '<')
                    {
                        inTag = true;
                        tag = "";
                        continue;
                    }
                }
                else
                {
                    if (c == '<')
                    {
                        inTag = true;
                        tag = "";
                        continue;
                    }
                    if (c == '>')
                    {
                        inTag = false;
                        if (tag[0] == '/')
                        {
                            if (tags.Count <= 0)
                                continue;
                            tag = tag.Substring(1);
                            if (tag == tags[0])
                            {
                                tags.RemoveAt(0);
                            }
                            continue;
                        }
                        if (tag[0] == '#')
                            tag = "color";
                        tag = tag.ToLowerInvariant();
                        if (tag.Contains("="))
                        {
                            tag = tag.Split('=')[0];
                        }
                        if (EnclosingTags.Contains(tag))
                        {
                            tags.Insert(0, tag);
                        }
                        continue;
                    }
                    tag += c;
                }
            }

            var closeText = "";
            foreach (var tg in tags)
            {
                closeText += $"</{tg}>";
            }

            text += closeText;

            return text;
        }
    }
}