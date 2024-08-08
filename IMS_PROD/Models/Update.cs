namespace IMS_PROD.Models
{
    public class Update
    {
        public int UpdateToken { get; set; }
        public int? ItemId { get; set; }
        public int ItemRem {  get; set; }

        public virtual Item? ItemIDNavigation { get; set; }
    }
}
