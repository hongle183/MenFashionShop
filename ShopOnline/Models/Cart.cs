using System;
using System.Linq;

namespace ShopOnline.Models
{
    public class Cart
    {
        private readonly menfsEntities db = new menfsEntities();
        public Guid productId { get; set; }
        public Guid sizeId { get; set; }
        public Guid colorId { get; set; }
        public string nameItem { get; set; }
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
        public Cart(Guid productId, Guid sizeId, Guid colorId)
        {
            this.productId = productId;
            this.sizeId = sizeId;
            this.colorId = colorId;
            Variant item = db.Variants.Single(model => model.productId == productId && model.sizeId == sizeId && model.colorId == colorId);
            this.nameItem = item.Product.productName;
            this.unitPrice = int.Parse(item.Product.price.ToString());
            this.discountItem = this.unitPrice * int.Parse(item.Product.discount.ToString()) / 100;
            this.priceItem = this.unitPrice - (this.unitPrice * int.Parse(item.Product.discount.ToString()) / 100);
            this.quantity = 1;
        }
    }
}