using System;
using System.Collections.Generic;

namespace CodigoTest.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? MemberId { get; set; }

    public double? TotalPrice { get; set; }

    public double? TotalPoint { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? Desciption { get; set; }

    public virtual Member? Member { get; set; }

    public virtual List<OrderItem> OrderItems { get; set;}
}
