// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using GenericEndpointRouting.Services;

namespace GenericEndpointRouting.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IEndpointRouteBuilder"/> to add endpoints.
    /// </summary>
    public static class FallbackEndpointRouteBuilderExtensions
    {

        /// <summary>
        /// Adds a generic endpoint to the <see cref="IEndpointRouteBuilder"/> that will match
        /// the template "{genericSlug}" with the lowest possible priority.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
        /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
        /// <remarks>
        /// <para>
        /// The order of the registered endpoint will be <c>int.MaxValue</c>.
        /// </para>
        /// </remarks>
        public static IEndpointConventionBuilder MapFallbackToGenericPage(
            this IEndpointRouteBuilder endpoints)
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            var conventionBuilder = endpoints.Map("{genericSlug}", async context =>
            {
                // get slug value 
                var genericSlug = context.GetRouteValue("genericSlug") as string;
                if (!String.IsNullOrEmpty(genericSlug))
                {
                    string pageRouteValue = String.Empty;
                    var slugService = context.RequestServices.GetRequiredService<ISlugService>();
                    var slug = slugService.GetSlugFromName(genericSlug);
                    if (slug != null)
                    {
                        switch (slug.Id)
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
                            // get page action descriptor
                            var actionDescriptors = context.RequestServices.GetRequiredService<IActionDescriptorCollectionProvider>().ActionDescriptors;
                            var action = actionDescriptors.Items.OfType<PageActionDescriptor>().Where(item => item.ViewEnginePath.Contains(pageRouteValue)).FirstOrDefault();
                            if (action != null)
                            {
                                // get endpoint context, then custom route values
                                var endpointSelectorContext = context.Features.Get<IEndpointFeature>() as EndpointSelectorContext;
                                endpointSelectorContext.RouteValues["page"] = pageRouteValue;

                                // pass route data to action context
                                var routeData = context.GetRouteData();
                                //var actionContext = new ActionContext(context, routeData, action);

                                // should load compiled page action descriptor into action context, if not (like above) it will produce error
                                var compiledAction = await context.RequestServices.GetRequiredService<PageLoader>().LoadAsync(action);
                                var actionContext = new ActionContext(context, routeData, compiledAction);

                                var invokerFactory = context.RequestServices.GetRequiredService<IActionInvokerFactory>();
                                var invoker = invokerFactory.CreateInvoker(actionContext);
                                await invoker.InvokeAsync();
                            }
                        }
                    }
                }
            });
            conventionBuilder.WithDisplayName("GenericEndpoint");
            conventionBuilder.Add(b => ((RouteEndpointBuilder)b).Order = int.MaxValue);
            return conventionBuilder;
        }
    }
}
