using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using System.Collections.Generic;
using System.Linq;
using Umbraco.PdfExport.Services;
using System.IO;
using System;
using Microsoft.Extensions.Configuration;

namespace Umbraco.PdfExport.Controllers
{
	[PluginController("PdfExport")]
	public class PdfExportDashboardController : UmbracoAuthorizedApiController
	{
		private readonly IContentService _contentService;
		private readonly IPdfGeneratorService _pdfGeneratorService;
		private readonly string _apiKey;

		public PdfExportDashboardController(
			IContentService contentService,
			IPdfGeneratorService pdfGeneratorService,
			IConfiguration config)
		{
			_contentService = contentService;
			_pdfGeneratorService = pdfGeneratorService;
			_apiKey = config["PdfExport:ApiKey"];
		}

		[HttpGet]
		public IActionResult GetContentTree()
		{
			var rootContent = _contentService.GetRootContent();
			var tree = new List<object>();

			foreach (var content in rootContent)
			{
				tree.Add(new
				{
					id = content.Id,
					name = content.Name,
					icon = "icon-document",
					children = GetChildren(content.Id)
				});
			}

			return Ok(tree);
		}

		private List<object> GetChildren(int parentId)
		{
			var children = _contentService.GetPagedChildren(parentId, 0, int.MaxValue, out _);
			return children.Select(c => new
			{
				id = c.Id,
				name = c.Name,
				icon = "icon-document",
				children = GetChildren(c.Id)
			}).ToList<object>();
		}

		[HttpGet]
		public IActionResult GeneratePdf(int contentId)
		{
			try
			{
				var content = _contentService.GetById(contentId);
				if (content == null)
					return NotFound("Content not found.");

				var pdfBytes = _pdfGeneratorService.GeneratePdfForContent(contentId);

				string safeName = string.Concat(content.Name.Split(Path.GetInvalidFileNameChars()));
				string fileName = $"{safeName}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

				return File(pdfBytes, "application/pdf", fileName);
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}


		[HttpGet]
		public IActionResult GetNodeName(int id)
		{
			var content = _contentService.GetById(id);
			if (content == null)
				return NotFound();
			return Ok(content.Name);
		}

		[HttpGet]
		public IActionResult ExportAllNodes([FromQuery(Name = "apiKey")] string apiKey)
		{
			if (!string.IsNullOrEmpty(_apiKey) && apiKey != _apiKey)
				return Unauthorized("Invalid API key.");

			var allNodes = _contentService.GetRootContent().SelectMany(GetAllDescendantsAndSelf).ToList();
			using var stream = new MemoryStream();
			using (var document = new PdfSharpCore.Pdf.PdfDocument())
			{
				foreach (var node in allNodes)
				{
					_pdfGeneratorService.AddContentToPdfDocument(document, node.Id);
				}
				document.Save(stream, false);
			}

			string rootNodeName = _contentService.GetRootContent().FirstOrDefault()?.Name ?? "Export";

			string safeName = string.Concat(rootNodeName.Split(Path.GetInvalidFileNameChars()));
			string fileName = $"{safeName}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

			return File(stream.ToArray(), "application/pdf", fileName);
		}


		private IEnumerable<IContent> GetAllDescendantsAndSelf(IContent content)
		{
			yield return content;
			foreach (var child in _contentService.GetPagedChildren(content.Id, 0, int.MaxValue, out _))
			{
				foreach (var descendant in GetAllDescendantsAndSelf(child))
					yield return descendant;
			}
		}
	}
}