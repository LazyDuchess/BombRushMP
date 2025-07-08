using BombRushMP.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static UnityEngine.UIElements.TextField;

namespace BombRushMP.Plugin
{
    public static class ProfanityFilter
    {
        public const string CensoredMessage = "I said a naughty word!";
        public const string ProfanityError = "Your message contains banned words.";

        public const string CensoredStatus = "Saying Slurs";
        public const string StatusProfanityError = "Your status contains banned words.";

        public const string CensoredName = "Punished Gooner";

        public const string FilteredIndicator = "<sprite=37><color=red>[FILTERED]</color>";

        private static List<string> BannedContent = LoadBannedContent();
        private static List<string> SafeWords = LoadSafeWords();
        private static List<string> BadWords = LoadBadWords();

        private static List<string> LoadBadWords()
        {
            var result = new List<string>();

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BombRushMP.Plugin.res.badwords.txt");
            if (stream is null) throw new Exception("Could not load bad words for profanity filter");
            var text = new StreamReader(stream).ReadToEnd();

            var lines = text.Split('\n');
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("#")) continue;
                trimmed = RemoveSpecialCharacters(trimmed);
                trimmed = SpecialCharactersToLetters(trimmed);
                result.Add(trimmed);
            }

            return result;
        }

        private static List<string> LoadSafeWords()
        {
            var result = new List<string>();

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BombRushMP.Plugin.res.safewords.txt");
            if (stream is null) throw new Exception("Could not load safe words for profanity filter");
            var text = new StreamReader(stream).ReadToEnd();

            var lines = text.Split('\n');
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("#")) continue;
                trimmed = RemoveSpecialCharacters(trimmed);
                trimmed = SpecialCharactersToLetters(trimmed);
                result.Add(trimmed);
            }

            return result;
        }

        private static List<string> LoadBannedContent()
        {
            var result = new List<string>();

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BombRushMP.Plugin.res.profanity.txt");
            if (stream is null) throw new Exception("Could not load profanity filter");
            var text = new StreamReader(stream).ReadToEnd();

            var lines = text.Split('\n');
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.StartsWith("#")) continue;
                trimmed = RemoveSpecialCharacters(trimmed);
                trimmed = SpecialCharactersToLetters(trimmed);
                result.Add(trimmed);
            }

            return result;
        }

        public static bool ContainsBadWords(string text)
        {
            text = text.ToLower();
            foreach (var line in BadWords)
            {
                if (text == line) return true;
                if (text.StartsWith($"{line} ")) return true;
                if (text.EndsWith($" {line}")) return true;
                if (text.Contains($" {line} ")) return true;
            }
            return false;
        }

        public static string RemoveSafeWords(string text)
        {
            text = text.ToLower();
            foreach (var line in SafeWords)
            {
                if (text == line) return "";

                text = text.Replace($" {line} ", "");

                if (text.StartsWith($"{line} "))
                    text = text.Substring(line.Length + 1);

                if (text.EndsWith($" {line}"))
                    text = text.Substring(0, text.Length - line.Length - 1);
            }
            return text;
        }

        public static bool ContainsProfanity(string text)
        {
            foreach (var line in BannedContent)
            {
                if (text.Contains(line.ToLower())) return true;
            }

            return false;
        }

        public static bool TMPContainsProfanity(string tmpText)
        {
            var addedSpaces = AddSpaces(TMPFilter.RemoveAllTags(tmpText));
            var addedSpacesNoRepeats = RemoveRepeatedLetters(addedSpaces);
            if (ContainsBadWords(addedSpaces) || ContainsBadWords(addedSpacesNoRepeats))
                return true;

            tmpText = RemoveSafeWords(tmpText);

            var removedTags = TMPFilter.RemoveAllTags(tmpText);
            removedTags = SpecialCharactersToLetters(removedTags);
            removedTags = RemoveSpecialCharacters(removedTags);

            var withTags = tmpText;
            withTags = SpecialCharactersToLetters(withTags);
            withTags = RemoveSpecialCharacters(withTags);

            var withTagsNoRepeats = RemoveRepeatedLetters(withTags);
            var removedTagsNoRepeats = RemoveRepeatedLetters(removedTags);

            if (ContainsBadWords(withTags) || ContainsBadWords(removedTags) || ContainsBadWords(withTagsNoRepeats) || ContainsBadWords(removedTagsNoRepeats))
                return true;

            withTags = RemoveSpaces(withTags);
            removedTags = RemoveSpaces(removedTags);
            withTagsNoRepeats = RemoveSpaces(withTagsNoRepeats);
            removedTagsNoRepeats = RemoveSpaces(removedTagsNoRepeats);

            if (ContainsProfanity(withTags) || ContainsProfanity(removedTags) || ContainsProfanity(withTagsNoRepeats) || ContainsProfanity(removedTagsNoRepeats))
                return true;
            return false;
        }

        public static string RemoveRepeatedLetters(string text)
        {
            var final = "";
            var lastChar = char.MinValue;
            foreach (var c in text)
            {
                if (c != lastChar)
                {
                    lastChar = c;
                    final += c;
                }
            }
            return final;
        }

        public static string RemoveSpecialCharacters(string text)
        {
            var reg = new Regex(@"[^A-Za-z0-9 ]");
            text = reg.Replace(text, string.Empty);
            return text;
        }

        public static string AddSpaces(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            var specialSeparated = Regex.Replace(text, @"[^a-zA-Z0-9]", m => $" {m.Value} ");
            var capsSeparated = Regex.Replace(specialSeparated, @"(?<=[a-z0-9])(?=[A-Z])", " ");
            var cleaned = Regex.Replace(capsSeparated, @"\s+", " ").Trim();
            return cleaned;
        }

        public static string RemoveSpaces(string text)
        {
            return text.Replace(" ", string.Empty);
        }

        public static string SpecialCharactersToLetters(string text)
        {
            text = text.Replace("0", "o");
            text = text.Replace("1", "i");
            text = text.Replace("3", "e");
            text = text.Replace("4", "a");
            text = text.Replace("5", "s");
            text = text.Replace("6", "g");
            text = text.Replace("7", "t");
            text = text.Replace("0", "o");

            text = text.Replace("@", "a");
            return text;
        }
    }
}
