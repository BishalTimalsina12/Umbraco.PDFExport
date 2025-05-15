using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.PdfExport.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Umbraco.PdfExport.Composers
{
	public class PdfExportComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			// Register the PDF generator service with its interface
			builder.Services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();

			// Register the startup service
			builder.Services.AddHostedService<PdfExportStartup>();
		}
	}

	public class PdfExportStartup : IHostedService
	{
		public PdfExportStartup(Umbraco.Cms.Core.Services.IUserService userService) { }

		public Task StartAsync(CancellationToken cancellationToken)
		{
			// No permission logic needed
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}