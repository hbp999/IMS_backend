
using IMS_PROD.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;


namespace IMS_PROD.Controllers
{
    public class StVal
    {
        public int? ItemId { get; set; }

        public int? Mrp { get; set; }

        public int? PurchaseCost { get; set; }

        public int TId { get; set; }

        public string? BatchId { get; set; }

        public DateTime ExpiryDate { get; set; }

        public int? SItems { get; set; }
    }

    public class PcVal
    {
        public string PurchaseID { get; set; }

        public int? ItemId { get; set; }

        public DateTime? PDate { get; set; }

        public int? PItems { get; set; }
    }

    public class ItmVal
    {
        public int ItemId { get; set; }

        public string? ItemName { get; set; }
    }

    public class Pdval
    {
        public int UId { get; set; }
        public string? MailId { get; set; }
        public string? Password { get; set; }

    }
        [ApiController]
    public class IMSController : ControllerBase
    {
        InventoryContext db = new InventoryContext();

        [HttpGet]
        [Route("api/[controller]/StockGet")]
        public dynamic Sts()
        {
            var val = (from a in db.Stocks
                       join b in db.Items on a.ItemId equals b.ItemId
                       select new
                       {
                           a.ItemId,
                           b.ItemName,
                           a.BatchId,
                           a.SItems,
                           a.ExpiryDate,
                           a.Mrp
                       }
                       );
            return val;
        }
        /* public List<Stock> Sts()
         {
             var res = db.Stocks.ToList();
             return res;
         }*/

        [HttpGet]
        [Route("api/[controller]/DashGet")]
        public dynamic Dsa()
        {
            
            var ds = (from a in db.Stocks
                      join b in db.Items on a.ItemId equals b.ItemId
                      select new
                      {
                          b.ItemName,
                          a.SItems,
                          stval = a.SItems * a.Mrp

                      });
            return ds;

        }

        [HttpGet]
        [Route("api/[controller]/DateGet")]
        public dynamic Dt()
        {
            DateTime CurrentDate = DateTime.Now;
            var dt = (from a in db.Stocks
                      join b in db.Items on a.ItemId equals b.ItemId
                      where (CurrentDate > a.ExpiryDate)
                      select new
                      {
                          b.ItemName,
                          a.SItems,
                          a.Mrp
                      }

                 );
            return dt;
        }

        [HttpGet]
        [Route("api/[controller]/ItemGet")]
        /*public dynamic Its()
        {
            var res = (from a in db.Items
                       join b in db.Stocks on a.ItemId equals b.ItemId

                       select new
                       {
                           b.ItemId,
                           a.ItemName,
                           
                       });


            return res;
        }*/
        public dynamic Its()
        {
            var res = db.Items.ToList();
            return res;
        }

        [HttpGet]
        [Route("api/[controller]/PurchaseGet")]
        public dynamic Pts([FromQuery] DateTime FromDate, DateTime ToDate)
        {
            

                var to = (from a in db.Items
                          join b in db.Purchase on a.ItemId equals b.ItemId
                          /*where (b.PDate >= FromDate && b.PDate <= ToDate)*/
                          select new
                          {
                              a.ItemName,
                              b.PDate,
                              b.PItems
                          })/*OrderBy(d => d.ItemName).ToList()*/;
                return to;
            
            
          
        }

      

        [HttpGet]
        [Route("api/[controller]/UpdateGet")]
        public dynamic Uts([FromQuery] int id)
        {

            var val = (from a in db.Items
                       join b in db.Updates on a.ItemId equals b.ItemId
                       where(b.ItemId == id)
                       select new
                       {
                           a.ItemName,
                           b.ItemRem
                       });
            return val;
        }


        [HttpGet]
        [Route("api/[controller]/UserGet")]
        public List<User> Urs()
        {
            /*string original=encrypt(ps);
            var val=db.Users.Where(l => l.MailId == user && l.Password == original).FirstOrDefault();
            if (val != null )
            {
                return val;
            }
            return Ok("password matches");*/
            var res = db.Users.ToList();
            return res;

        }

        [HttpPost]
        [Route("api/[controller]/PostItem")]
        public ActionResult ItemCreate([FromBody] ItmVal it)
        {


            Item cl = new Item();
            int maxresults1 = db.Items.ToList().Count != 0 ? db.Items.Max(d => d.ItemId) : 0;
            cl.ItemName = it.ItemName;
            cl.ItemId = maxresults1 + 1;
            db.Items.Add(cl);
            db.SaveChanges();



            return Ok("ok");


        }
        [HttpPost]
        [Route("api/[controller]/PostStock")]
        public ActionResult StockCreate([FromBody] StVal st)
        {


          
           
            Stock pl = new Stock();
            pl.ItemId = st.ItemId;
            pl.SItems = st.SItems;

            var res = db.Updates.Where(d => d.ItemId == st.ItemId).FirstOrDefault();
            if (res == null)
            {
                

                Update ut = new Update();
                int maxresults3 = db.Updates.ToList().Count != 0 ? db.Updates.Max(d => d.UpdateToken) : 0;
                ut.UpdateToken = maxresults3 + 1;
                int maxresults4 = db.Stocks.ToList().Count != 0 ? db.Stocks.Max(d => d.TId) : 0;
                pl.TId = maxresults4 + 1;
                ut.ItemId = st.ItemId;
                ut.ItemRem = (Int32)st.SItems;
                db.Updates.Add(ut);
                db.SaveChanges();

            }
            else
            {
                res.ItemRem = res.ItemRem + (Int32)st.SItems;

            }
            pl.Mrp = st.Mrp;
            pl.ExpiryDate = st.ExpiryDate;
            pl.PurchaseCost = st.PurchaseCost;
            pl.BatchId = st.BatchId;
            db.Stocks.Add(pl);
            db.SaveChanges();



            return Ok("ok");






        }

        [HttpPost]
        [Route("api/[controller]/PostPurchase")]
        public dynamic PurchaseCreate([FromBody] PcVal pc)
        {
            var res = db.Updates.Where(d => d.ItemId == pc.ItemId).FirstOrDefault();

            if (res != null && res.ItemRem >= pc.PItems)
            {

                Purchase sl = new Purchase();
                sl.PurchaseID = pc.PurchaseID;
                sl.ItemId = pc.ItemId;
                sl.PItems = pc.PItems;
                sl.PDate = pc.PDate;
                res.ItemRem = res.ItemRem - (Int32)sl.PItems;
                db.Purchase.Add(sl);
                db.SaveChanges();
                return Ok("ok");
            }
            else if(res != null && res.ItemRem < pc.PItems) 
            {
            
                var aval = (Int32)pc.PItems - res.ItemRem;
                return "Purchase Exceed By : " + aval;
            }

            else
            {
                return BadRequest("Item not Available");
            }

          

        }

        [HttpPost]
        [Route("api/[controller]/PostUpdate")]
        public ActionResult UpdateCreate([FromBody] Update up)
        {


            Update ut = new Update();
            int maxresults3 = db.Updates.ToList().Count != 0 ? db.Updates.Max(d => d.UpdateToken) : 0;
            ut.UpdateToken = maxresults3 + 1;
            db.Updates.Add(up);
            db.SaveChanges();

            return Ok("ok");

        }
        /*
            ut.ItemId = up.ItemId;
            *//*if (ut.ItemId == up.ItemId && st.SItems > pc.PItems)
            {
                var rem = st.SItems - pc.PItems;
                ut.ItemRem = (Int32)rem;
            }*//*
            
            ut.UpdateToken = maxresults3 + 1;
            db.Updates.Add(up);
            db.SaveChanges();

         }*/
        [HttpPost]
        [Route("api/[controller]/PostUser")]
        public async Task<object> Encrypt([FromBody] User us)
        {
            User strmsg = new User();
            int maxresults5 = db.Users.ToList().Count != 0 ? db.Users.Max(d => d.UId) : 0;
            strmsg.UId = maxresults5 + 1;
            us.UId = strmsg.UId;
            using (db)
            {
                using (var _ctxTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        
                        strmsg.MailId = us.MailId;
                        strmsg.Password = encrypt(us.Password);
                        us.Password = strmsg.Password;
                        db.Users.Add(us);
                        await db.SaveChangesAsync();
                        _ctxTransaction.Commit();
                        return strmsg;


                    }


                    catch (Exception e)
                    {



                        _ctxTransaction.Rollback();
                        e.ToString();
                        throw;
                        //message = (int)responseMessage.Error;
                    }
                }
                

            }

        }
        [HttpPost]
        [Route("api/[controller]/PostLogin")]
        public ActionResult LoginCreate([FromBody] Pdval pd)
        {
             var pass = encrypt(pd.Password);
            if(db.Users.Any(g=>g.MailId == pd.MailId && g.Password == pass))
            {
                return Ok("Ok");
            }
            else
            {
                return BadRequest();
            }
           
        }


        private string encrypt(string pass)
        {

            string strmsg = string.Empty;
            byte[] encode = new byte[pass.Length];
            encode = Encoding.UTF8.GetBytes(pass);
            strmsg = Convert.ToBase64String(encode);
            return strmsg;

        }


        private string Decryptdata(string encryptpwd)
        {
            string decryptpwd = string.Empty;
            UTF8Encoding encodepwd = new UTF8Encoding();
            Decoder Decode = encodepwd.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encryptpwd);
            int charCount = Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            decryptpwd = new String(decoded_char);
            return decryptpwd;
        }






    }
}
