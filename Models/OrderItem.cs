using System;
using System.Collections.Generic;

namespace CodigoTest.Models;

public partial class OrderItem
{
    public int OrderId { get; set; }

    public int ItemId { get; set; }

    public double? Quantity { get; set; }

    public double? Total { get; set; }

    public double? Point { get; set; }
}
