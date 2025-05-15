using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using System.Text.RegularExpressions;

namespace Umbraco.PdfExport.Services
{
	public class PdfGeneratorService : IPdfGeneratorService
	{
		private readonly IUmbracoContextFactory _contextFactory;
		private const int MARGIN = 40;
		private const int LINE_HEIGHT = 20;
		private const int SECTION_SPACING = 30;

		public PdfGeneratorService(IUmbracoContextFactory contextFactory)
		{
			_contextFactory = contextFactory;
		}

		public byte[] GeneratePdfForContent(int contentId)
		{
			using var contextRef = _contextFactory.EnsureUmbracoContext();
			var content = contextRef.UmbracoContext.Content.GetById(contentId);
			if (content == null)
				throw new FileNotFoundException($"Content with ID {contentId} not found.");

			using var stream = new MemoryStream();
			using (var document = new PdfDocument())
			{
				document.Info.Title = $"{content.Name} - {DateTime.Now:yyyy-MM-dd}";
				document.Info.CreationDate = DateTime.Now;

				var page = document.AddPage();
				var gfx = XGraphics.FromPdfPage(page);

				var titleFont = new XFont("Arial", 24, XFontStyle.Bold);
				var propertyFont = new XFont("Arial", 12, XFontStyle.Bold);
				var valueFont = new XFont("Arial", 12);
				var blockFont = new XFont("Arial", 11);
				var propFontBold = new XFont("Arial", 12, XFontStyle.Bold);
				var propFont = new XFont("Arial", 12, XFontStyle.Regular);

				gfx.DrawString(content.Name, titleFont, XBrushes.Black,
					new XRect(MARGIN, MARGIN, page.Width - (MARGIN * 2), LINE_HEIGHT * 2),
					XStringFormats.TopLeft);

				var y = MARGIN + (LINE_HEIGHT * 3);

				foreach (var prop in content.Properties)
				{
					var value = prop.GetValue();
					if (value is BlockListModel blockList)
					{
						gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
						y += 25;
						foreach (var block in blockList)
						{
							if (block?.Content != null)
							{
								var contentType = block.Content.ContentType;
								string elementTypeName = contentType != null ? contentType.Alias : "Unknown";
								gfx.DrawString($"  - {elementTypeName}", propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
								y += 20;
							}
						}
						y += 5;
					}
					else if (value is IPublishedContent pickedContent)
					{
						gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
						y += 20;
						gfx.DrawString($"{pickedContent.Name}", propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
						y += 20;
					}
					else if (value is IEnumerable<IPublishedContent> pickedContents)
					{
						gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
						y += 20;
						var names = string.Join(", ", pickedContents.Select(c => c.Name));
						gfx.DrawString($"{names}", propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
						y += 20;
					}
					else if (value is IEnumerable<Umbraco.Cms.Core.Models.Link> links)
					{
						gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
						y += 20;
						foreach (var link in links)
						{
							gfx.DrawString($"{link.Name} ({link.Url})", propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
							y += 20;
						}
						y += 5;
					}
					else if (value is Umbraco.Cms.Core.Models.Link link)
					{
						gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
						y += 20;
						gfx.DrawString($"{link.Name} ({link.Url})", propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
						y += 20;
					}
					else if (value is IPublishedContent media)
					{
						gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
						y += 20;
						gfx.DrawString($"{media.Name}", propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
						y += 20;
					}
					else if (value is IEnumerable<IPublishedContent> mediaItems)
					{
						gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
						y += 20;
						var names = string.Join(", ", mediaItems.Select(m => m.Name));
						gfx.DrawString($"{names}", propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
						y += 20;
					}
					else if (value is string[] stringArray)
					{
						gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
						y += 20;
						var joinedString = string.Join(", ", stringArray);
						gfx.DrawString($"{joinedString}", propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
						y += 20;
					}
					else if (value != null)
					{
						string displayValue = value.ToString();
						if (string.IsNullOrEmpty(displayValue))
						{
							displayValue = "N/A";
						}
						else if (displayValue.Contains("<") && displayValue.Contains(">"))
						{
							displayValue = StripHtml(displayValue);
						}
						gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
						y += 20;
						var lines = SplitTextIntoLines(displayValue, propFont, page.Width - 100, gfx);
						foreach (var line in lines)
						{
							gfx.DrawString(line, propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
							y += 20;
						}
					}

					if (y > page.Height - MARGIN)
					{
						page = document.AddPage();
						gfx = XGraphics.FromPdfPage(page);
						y = MARGIN;
					}
				}

				document.Save(stream, false);
			}

			return stream.ToArray();
		}

		private string FormatPropertyValue(object value)
		{
			if (value == null) return string.Empty;

			if (value is IEnumerable<IPublishedContent> list && value is not IPublishedContent)
			{
				return string.Join(", ", list.Select(i => i.Name));
			}

			return value switch
			{
				string s => s,

				string[] stringArray => string.Join(", ", stringArray.Where(s => !string.IsNullOrEmpty(s))),

				MediaWithCrops media => media.LocalCrops?.Src ?? string.Empty,

				IPublishedContent pc => pc.Name,

				DateTime dt => dt.ToString("g"),

				bool b => b ? "Yes" : "No",

				_ => value.ToString() ?? string.Empty
			};
		}

		private List<string> SplitTextIntoLines(string text, XFont font, double maxWidth, XGraphics gfx)
		{
			var lines = new List<string>();
			var words = text.Split(' ');
			var currentLine = new StringBuilder();

			foreach (var word in words)
			{
				var testLine = currentLine.Length == 0 ? word : currentLine + " " + word;
				var size = gfx.MeasureString(testLine, font);

				if (size.Width <= maxWidth)
				{
					currentLine.Append(currentLine.Length == 0 ? word : " " + word);
				}
				else
				{
					lines.Add(currentLine.ToString());
					currentLine.Clear().Append(word);
				}
			}

			if (currentLine.Length > 0)
				lines.Add(currentLine.ToString());

			return lines;
		}

		private string StripHtml(string input)
		{
			if (string.IsNullOrEmpty(input)) return string.Empty;
			return Regex.Replace(input, "<.*?>", string.Empty);
		}

		public void AddContentToPdfDocument(PdfDocument document, int contentId)
		{
			using var contextRef = _contextFactory.EnsureUmbracoContext();
			var content = contextRef.UmbracoContext.Content.GetById(contentId);
			if (content == null)
				return;

			var page = document.AddPage();
			var gfx = XGraphics.FromPdfPage(page);
			var titleFont = new XFont("Arial", 24, XFontStyle.Bold);
			var propFontBold = new XFont("Arial", 12, XFontStyle.Bold);
			var propFont = new XFont("Arial", 12, XFontStyle.Regular);

			gfx.DrawString(content.Name, titleFont, XBrushes.Black, new XRect(40, 40, page.Width - 80, 40), XStringFormats.TopLeft);
			int y = 100;

			foreach (var prop in content.Properties)
			{
				var value = prop.GetValue();
				if (value is BlockListModel blockList)
				{
					gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
					y += 25;
					foreach (var block in blockList)
					{
						if (block?.Content != null)
						{
							var contentType = block.Content.ContentType;
							string elementTypeName = contentType != null ? contentType.Alias : "Unknown";
							gfx.DrawString($"  - {elementTypeName}", propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
							y += 20;
						}
					}
					y += 5;
				}
				else if (value is IPublishedContent pickedContent)
				{
					gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
					y += 20;
					gfx.DrawString($"{pickedContent.Name}", propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
					y += 20;
				}
				else if (value is IEnumerable<IPublishedContent> pickedContents)
				{
					gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
					y += 20;
					var names = string.Join(", ", pickedContents.Select(c => c.Name));
					gfx.DrawString($"{names}", propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
					y += 20;
				}
				else if (value is IEnumerable<Umbraco.Cms.Core.Models.Link> links)
				{
					gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
					y += 20;
					foreach (var link in links)
					{
						gfx.DrawString($"{link.Name} ({link.Url})", propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
						y += 20;
					}
					y += 5;
				}
				else if (value is Umbraco.Cms.Core.Models.Link link)
				{
					gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
					y += 20;
					gfx.DrawString($"{link.Name} ({link.Url})", propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
					y += 20;
				}
				else if (value is IPublishedContent media)
				{
					gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
					y += 20;
					gfx.DrawString($"{media.Name}", propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
					y += 20;
				}
				else if (value is IEnumerable<IPublishedContent> mediaItems)
				{
					gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
					y += 20;
					var names = string.Join(", ", mediaItems.Select(m => m.Name));
					gfx.DrawString($"{names}", propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
					y += 20;
				}
				else if (value is string[] stringArray)
				{
					gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
					y += 20;
					var joinedString = string.Join(", ", stringArray);
					gfx.DrawString($"{joinedString}", propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
					y += 20;
				}
				else if (value != null)
				{
					string displayValue = value.ToString();
					if (string.IsNullOrEmpty(displayValue))
					{
						displayValue = "N/A";
					}
					else if (displayValue.Contains("<") && displayValue.Contains(">"))
					{
						displayValue = StripHtml(displayValue);
					}
					gfx.DrawString($"{prop.Alias}:", propFontBold, XBrushes.Black, new XRect(40, y, page.Width - 80, 20), null);
					y += 20;
					var lines = SplitTextIntoLines(displayValue, propFont, page.Width - 100, gfx);
					foreach (var line in lines)
					{
						gfx.DrawString(line, propFont, XBrushes.Black, new XRect(60, y, page.Width - 100, 20), null);
						y += 20;
					}
				}
			}
		}
	}
}



