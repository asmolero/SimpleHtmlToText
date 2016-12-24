using System;
using CommandLine;
using HtmlAgilityPack;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Xml;
using System.Text.RegularExpressions;

namespace SimpleHtmlToText
{
	class MainClass
	{
		static Options _options;

		enum ElementType
		{
			Inline,
			Block,
			Ignore
		}

		static Dictionary<string, ElementType> _elementMap;
		static Regex _multipleSpaces;

		public static void Main(string[] args)
		{
			_options = new Options();
			if (!CommandLine.Parser.Default.ParseArguments(args, _options))
			{
				Console.WriteLine("Wrong arguments");
				Environment.Exit(1);
			}

			if (!File.Exists(_options.InputFile)) {
				Console.WriteLine("File doesn't exist");
				Environment.Exit(1);
			}
			HtmlDocument document = new HtmlDocument();
			document.Load(_options.InputFile);

			_multipleSpaces = new Regex(@"\s+", RegexOptions.Compiled);

			_elementMap = new Dictionary<string, ElementType>();

		    // Default block
			_elementMap.Add("html", ElementType.Block);
			_elementMap.Add("body", ElementType.Block);
			_elementMap.Add("address", ElementType.Block);
		    _elementMap.Add("article", ElementType.Block);
		    _elementMap.Add("aside", ElementType.Block);
		    _elementMap.Add("blockquote", ElementType.Block);
		    _elementMap.Add("canvas", ElementType.Ignore);
		    _elementMap.Add("dd", ElementType.Block);
		    _elementMap.Add("div", ElementType.Block);
		    _elementMap.Add("dl", ElementType.Block);
		    _elementMap.Add("fieldset", ElementType.Ignore);
		    _elementMap.Add("figcaption", ElementType.Block);
		    _elementMap.Add("figure", ElementType.Block);
		    _elementMap.Add("footer", ElementType.Ignore);
		    _elementMap.Add("form", ElementType.Ignore);
		    _elementMap.Add("h1", ElementType.Block);
		    _elementMap.Add("h2", ElementType.Block);
		    _elementMap.Add("h3", ElementType.Block);
		    _elementMap.Add("h4", ElementType.Block);
		    _elementMap.Add("h5", ElementType.Block);
		    _elementMap.Add("h6", ElementType.Block);
		    _elementMap.Add("header", ElementType.Ignore);
		    _elementMap.Add("hgroup", ElementType.Block);
		    _elementMap.Add("hr", ElementType.Block);
		    _elementMap.Add("li", ElementType.Block);
		    _elementMap.Add("main", ElementType.Block);
		    _elementMap.Add("nav", ElementType.Ignore);
		    _elementMap.Add("noscript", ElementType.Block);
		    _elementMap.Add("ol", ElementType.Block);
		    _elementMap.Add("output", ElementType.Ignore);
		    _elementMap.Add("p", ElementType.Block);
		    _elementMap.Add("pre", ElementType.Ignore);
		    _elementMap.Add("section", ElementType.Block);
		    _elementMap.Add("table", ElementType.Block);
		    _elementMap.Add("tfoot", ElementType.Block);
		    _elementMap.Add("ul", ElementType.Block);
		    _elementMap.Add("video", ElementType.Ignore);

		    // Default inline
		    _elementMap.Add("b", ElementType.Inline);
		    _elementMap.Add("big", ElementType.Inline);
		    _elementMap.Add("i", ElementType.Inline);
		    _elementMap.Add("small", ElementType.Inline);
		    _elementMap.Add("tt", ElementType.Inline);
		    _elementMap.Add("abbr", ElementType.Inline);
		    _elementMap.Add("acronym", ElementType.Inline);
		    _elementMap.Add("cite", ElementType.Inline);
		    _elementMap.Add("code", ElementType.Ignore);
		    _elementMap.Add("dfn", ElementType.Inline);
		    _elementMap.Add("em", ElementType.Inline);
		    _elementMap.Add("kbd", ElementType.Inline);
		    _elementMap.Add("strong", ElementType.Inline);
		    _elementMap.Add("samp", ElementType.Inline);
		    _elementMap.Add("time", ElementType.Inline);
		    _elementMap.Add("var", ElementType.Inline);
		    _elementMap.Add("a", ElementType.Inline);
		    _elementMap.Add("bdo", ElementType.Inline);
		    _elementMap.Add("br", ElementType.Block);
		    _elementMap.Add("img", ElementType.Ignore);
		    _elementMap.Add("map", ElementType.Ignore);
		    _elementMap.Add("object", ElementType.Ignore);
		    _elementMap.Add("q", ElementType.Inline);
		    _elementMap.Add("script", ElementType.Ignore);
		    _elementMap.Add("span", ElementType.Inline);
		    _elementMap.Add("sub", ElementType.Ignore);
		    _elementMap.Add("sup", ElementType.Ignore);
		    _elementMap.Add("button", ElementType.Ignore);
		    _elementMap.Add("input", ElementType.Ignore);
		    _elementMap.Add("label", ElementType.Ignore);
		    _elementMap.Add("select", ElementType.Ignore);
		    _elementMap.Add("textarea", ElementType.Ignore);


			_elementMap.Add("style", ElementType.Ignore);
			_elementMap.Add("title", ElementType.Ignore);

			string output = ConvertNode(document.DocumentNode);
			File.WriteAllText(_options.OutputFile, output);
		}

		static string ConvertNode(HtmlNode node)
		{
			string output = "";

			switch (node.NodeType)
			{
				case HtmlNodeType.Comment:
					// Ignore comment
					break;
				case HtmlNodeType.Text:
					string nodeText = node.InnerText;
					string sanitized = nodeText.Replace("\r\n", " ").Replace("\n", " ");
					//string deentitized = HtmlEntity.DeEntitize(sanitized); HTML Agility Pack's method is very broken
					output += _multipleSpaces.Replace(sanitized, " ");
					break;
				case HtmlNodeType.Element:
				case HtmlNodeType.Document:
					ElementType elementType = ElementType.Ignore;
					_elementMap.TryGetValue(node.Name, out elementType);
					if (elementType == ElementType.Ignore) break;
					if (elementType == ElementType.Block)
					{
						output += "\n";
					}
					if (node.HasChildNodes)
					{
						foreach (HtmlNode child in node.ChildNodes)
						{
							output += ConvertNode(child);
						}
					}
					break;
				}
									
			return output;
		}
	}
}
