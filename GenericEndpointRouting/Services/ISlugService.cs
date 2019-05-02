using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericEndpointRouting.Entities;

namespace GenericEndpointRouting.Services
{
    public interface ISlugService
    {
        Slug GetSlugFromName(string name);
    }
}
