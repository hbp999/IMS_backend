using System;
using System.Collections.Generic;

namespace IMS_PROD.Models;

public partial class Stock
{
    public int? ItemId { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public int? Mrp { get; set; }

    public int? PurchaseCost { get; set; }

    public int TId { get; set; }

    public string? BatchId { get; set; }

    public int? SItems { get; set; }
}
