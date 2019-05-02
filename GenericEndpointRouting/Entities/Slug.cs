using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericEndpointRouting.Entities
{
    public class Slug
    {
        public Slug(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }


    }
}
