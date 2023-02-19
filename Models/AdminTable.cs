using System;
using System.Collections.Generic;

namespace CodigoTest.Models;

public partial class AdminTable
{
    public int AdminId { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? MobileNumber { get; set; }

    public string? Password { get; set; }

    public bool? IsActive { get; set; }
}
