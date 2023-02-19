using System;
using System.Collections.Generic;

namespace CodigoTest.Models;

public partial class MemberCoupon
{
    public int MemberId { get; set; }

    public int CouponId { get; set; }

    public int OrderId { get; set; }

    public double? OrderTotalPrice { get; set; }

    public double? DiscountTotalPrice { get; set; }

    public double? TotalPrice { get; set; }

    public DateTime? UsedDate { get; set; }
}
