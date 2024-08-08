using System;
using System.Collections.Generic;

namespace IMS_PROD.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string? ItemName { get; set; }

    public virtual ICollection<Purchase> Purchase { get; set; }
    public virtual ICollection<Update> Updates { get; set; }
}
