﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksWeb.ViewModels
{
    public class ProductsListViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
    }
}
