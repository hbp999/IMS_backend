namespace IMS_PROD.Models
{
    public class Purchase
    {
        public string PurchaseID { get; set; }

        public int? ItemId { get; set; }

        public DateTime? PDate { get; set; }

        public int? PItems { get; set; }

        public virtual Item? ItemIDNavigation { get; set; }
    }
}
