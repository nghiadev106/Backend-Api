using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class OrderDetailViewModel
    {
        public int OrderID { set; get; }

        public int ID { set; get; }
        public int Quantity { set; get; }
        public int Price { set; get; }
        public int? PromotionPrice { set; get; }
        public int ColorId { get; set; }
        public int SizeId { get; set; }
    }
}
