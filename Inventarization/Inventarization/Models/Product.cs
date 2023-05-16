using System;
using System.Collections.Generic;

namespace Inventarization.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? ArticleNumber { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Photo { get; set; } = null!;

    public string Manufacturer { get; set; } = null!;

    public decimal Cost { get; set; }

    public decimal DiscountAmount { get; set; }

    public int QuantityInStock { get; set; }

    public virtual string? ImagePath { get { return System.IO.Path.Combine(Environment.CurrentDirectory, $"images/{Photo}"); } }
}
