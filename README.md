# Umbraco.PDFExport

Umbraco package that enables content editors and developers to export Umbraco content nodes as professionally formatted PDF documents.
<img src="https://raw.githubusercontent.com/BishalTimalsina12/Umbraco.PDFExport/refs/heads/main/example.png" alt="Umbraco PDF Export" style="max-width:100%; height:auto;" />

## Features

- **Content Apps Integration**: Access PDF export functionality directly from any content node via a dedicated tab
- **Individual and Bulk Export**: Generate PDFs for single nodes or export your entire content tree as one comprehensive document
- **Complete Property Support**: Handles all common Umbraco property types:
  - Block List items (with element type labels)
  - Rich Text Editor content (with HTML stripping)
  - Media and images
  - Content Pickers (single and multi-node)
  - Link pickers
  - Arrays and complex objects
  - Boolean values
  - DateTime values
- **Professional Formatting**:
  - Bold property names and regular text for values
  - Intelligent text wrapping for long content
  - Proper indentation and spacing for nested items
  - Multi-page support with automatic pagination
  - Clean HTML tag stripping for RTE content
  - 
- **Developer-Friendly API**: Built with a clean architecture for easy extension and customization

## Stack

- Built for **Umbraco 13+**
- **PDFsharp and PdfSharpCore**: Industry-standard PDF generation libraries for .NET
- **Clean Service Architecture**: Well-structured services with dependency injection

## Installation

### NuGet (Recommended)
```
dotnet add package Umbraco.PDFExport
```

### Manual Installation
1. Clone the repository
2. Build the solution
3. Copy the `Umbraco.PdfExport` project into your Umbraco solution
4. Reference the project in your Umbraco site
5. Build and run your Umbraco site

## Configuration

The package works out of the box without configuration, but offers optional settings:

```json
{
  "PdfExport": {
    "ApiKey": "your-secret-key-for-api-access"
  }
}
```

Add this to your `appsettings.json` file to enable API key authentication for the export endpoints.

## Usage

### Through the Umbraco Backoffice

1. **Content App**: 
   - Navigate to any content node
   - Click the **PDF Export** tab
   - Click **Generate PDF** to create and preview
   - Download the PDF using the provided button

2. **API Endpoints**:
   - `GET /umbraco/backoffice/PdfExport/PdfExportDashboard/GeneratePdf?contentId={id}` - Generate PDF for a single node
   - `GET /umbraco/backoffice/PdfExport/PdfExportDashboard/ExportAllNodes?apiKey={key}` - Generate a single PDF with all content nodes

## Extending the Package

### Custom Property Renderers

The `PdfGeneratorService` uses a robust approach to handle different property types. To extend:

1. Fork the repository
2. Add your custom property type handling in the `GeneratePdfForContent` and `AddContentToPdfDocument` methods
3. Build and deploy



## Support

For issues, feature requests, or contributions:
- Open an issue on the project repository
- Submit a pull request 



---
**Author:** Bishal Tim 
**License:** MIT 
