// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GenericEndpointRouting.Entities;
using GenericEndpointRouting.Services;

namespace GenericEndpointRouting.Middlewares
{
    public class EndpointRedirectingMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        private readonly RouteOptions _routeOptions;
        private readonly PageLoader _pageLoader;

        public EndpointRedirectingMiddleware(
            ILogger<EndpointRedirectingMiddleware> logger,
            RequestDelegate next,
            IOptions<RouteOptions> routeOptions,
            PageLoader pageLoader)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _routeOptions = routeOptions?.Value ?? throw new ArgumentNullException(nameof(routeOptions));
            _pageLoader = pageLoader ?? throw new ArgumentNullException(nameof(pageLoader));
        }

        public async Task Invoke(HttpContext httpContext, ISlugService slugService)
        {
            // get endpoint context that is set in Endpoint Routing middleware
            var endpointSelectorContext = httpContext.Features.Get<IEndpointFeature>() as EndpointSelectorContext;

            // check if it is generic endpoint
            if (endpointSelectorContext != null && endpointSelectorContext.Endpoint != null && endpointSelectorContext.Endpoint.DisplayName.Contains("GenericEndpoint"))
            {
                // get slug value 
                var genericSlug = endpointSelectorContext.RouteValues["genericSlug"] as string;
                if (!String.IsNullOrEmpty(genericSlug))
                {
                    string pageRouteValue = String.Empty;
                    var slug = slugService.GetSlugFromName(genericSlug);
                    if (slug != null)
                    {
                        switch(slug.Id)
                        {
                            case 1:
                                pageRouteValue = "/Blog/BlogPost";
                                break;
                            case 2:
                                pageRouteValue = "/Blog/BlogCategory";
                                break;
                            default:
                                break;
                        }
                        if (!String.IsNullOrEmpty(pageRouteValue))
                        {
                            foreach (var datasource in _routeOptions.EndpointDataSources)
                            {
                                foreach (var routeEndpoint in datasource.Endpoints)
                                {
                                    // redirect endpoint if endpoint found in datasource
                                    var pageActionDescriptor = routeEndpoint.Metadata.GetMetadata<PageActionDescriptor>();
                                    if (pageActionDescriptor != null && pageActionDescriptor.ViewEnginePath.Contains(pageRouteValue))
                                    {
                                        //endpointSelectorContext.Endpoint = routeEndpoint;
                                        // replace directly like above, it will produce error, need to compile first
                                        var compiled = await _pageLoader.LoadAsync(pageActionDescriptor);
                                        // replace endpoint and custom route values
                                        endpointSelectorContext.Endpoint = compiled.Endpoint;
                                        endpointSelectorContext.RouteValues["page"] = pageRouteValue;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            await _next(httpContext);
        }
    }
}
