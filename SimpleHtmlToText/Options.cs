using System;
using CommandLine;
namespace SimpleHtmlToText
{
	class Options
	{
		[Option('i', "input", Required = true,
		  HelpText = "Input HTML file.")]
		public string InputFile { get; set; }

		[Option('o', "output", Required = true,
		  HelpText = "Output HTML file.")]
		public string OutputFile { get; set; }

	}
}
