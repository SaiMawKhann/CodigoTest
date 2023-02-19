using System;
using System.Collections.Generic;

namespace CodigoTest.Models;

public partial class Member
{
    public int MemberId { get; set; }

    public string? MemberName { get; set; }

    public string? MobileNumber { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public double? MemberTotalPoint { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; } = new List<RefreshToken>();
}
