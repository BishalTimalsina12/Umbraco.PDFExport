using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.PdfExport.Services;

namespace Umbraco.PdfExport.Composers
{
	public class PdfComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
			builder.Services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();
		}
	}
}