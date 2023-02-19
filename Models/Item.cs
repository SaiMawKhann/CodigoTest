using System;
using System.Collections.Generic;

namespace CodigoTest.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string? ItemName { get; set; }

    public string? ItemType { get; set; }

    public double? Price { get; set; }
}
