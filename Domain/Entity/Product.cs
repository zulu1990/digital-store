﻿using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Product : BaseEntity
    {

        public string Name { get; set; }
        public decimal Price { get; set; }

        public ProductCategory Category { get; set; }
        public int ProductIdentifier { get; set; }

        public Guid? OrderId { get; set; }

        public bool Sold { get; set; }
    }
}
