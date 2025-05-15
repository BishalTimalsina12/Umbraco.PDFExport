using System;
using PdfSharpCore.Pdf;

namespace Umbraco.PdfExport.Services
{
	public interface IPdfGeneratorService
	{
		/// <summary>
		/// Generates a PDF for the specified content
		/// </summary>
		/// <param name="contentId">The ID of the content to generate a PDF for</param>
		/// <returns>A byte array containing the PDF data</returns>
		/// <exception cref="FileNotFoundException">Thrown when the content is not found</exception>
		byte[] GeneratePdfForContent(int contentId);

		/// <summary>
		/// Adds a content node's data to an existing PDF document (for multi-node export)
		/// </summary>
		/// <param name="document">The PDF document to add to</param>
		/// <param name="contentId">The ID of the content to add</param>
		void AddContentToPdfDocument(PdfDocument document, int contentId);
	}
}
