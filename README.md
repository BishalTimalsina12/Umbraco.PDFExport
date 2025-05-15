# Umbraco.PDFExport

Easily export any content node in your Umbraco site to a well-formatted PDF — whether it’s a single page or your entire content tree

![Overview](https://raw.githubusercontent.com/BishalTimalsina12/Umbraco.PDFExport/refs/heads/main/example.png)
*PDF export in action*
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



## Need Help?

Found a bug? Want to suggest a feature or contribute?

Open an issue on GitHub

Submit a pull request

Login:
admin@example.com  
1234567890
