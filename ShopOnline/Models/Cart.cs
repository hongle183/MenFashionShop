using System;
using System.Linq;

namespace ShopOnline.Models
{
    public class Cart
    {
        menfsEntities db = new menfsEntities();
        public Guid idItem { get; set; }
        public string nameItem { get; set; }
        public string metaItem { get; set; }
        public string imageItem { get; set; }
        public int priceItem { get; set; }
        public int unitPrice { get; set; }
        public int quantity { get; set; }
        public int discountItem { get; set; }
        public int priceTotal
        {
            get
            {
                return quantity * priceItem;
            }
        }
        public int discountTotal
        {
            get
            {
                return discountItem * quantity;
            }
        }
        public Cart(Guid idProduct)
        {
            this.idItem = idProduct;
            Product item = db.Products.Single(model => model.productId == idItem);
            this.nameItem = item.productName;
            this.metaItem = item.meta;
            this.imageItem = item.image;
            this.unitPrice = int.Parse(item.price.ToString());
            this.priceItem = this.unitPrice - (this.unitPrice * int.Parse(item.discount.ToString()) / 100);
            this.quantity = 1;
            this.discountItem = this.unitPrice * int.Parse(item.discount.ToString()) / 100;
        }
    }
}