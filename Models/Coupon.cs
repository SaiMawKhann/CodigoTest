using System;
using System.Collections.Generic;

namespace CodigoTest.Models;

public partial class Coupon
{
    public int CouponId { get; set; }

    public string? CouponName { get; set; }

    public string? DiscountAmount { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public double? Quantity { get; set; }

    public string? Status { get; set; }

    public virtual List<MemberCoupon> MemberCoupons { get; set; }

}
