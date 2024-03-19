using System;
using System.Linq;

namespace ShopOnline.Models
{
    public class Cart
    {
        menfsEntities1 db = new menfsEntities1();
        public Guid idItem { get; set; }
        public string nameItem { get; set; }
        public string imageItem { get; set; }
        public int priceItem { get; set; }
        public int unitPrice { get; set; }
        public int quantity { get; set; }
        public int discount { get; set; }
        public Double priceTotal
        {
            get
            {
                return quantity * priceItem;
            }
        }
        public Cart(Guid idProduct)
        {
            this.idItem = idProduct;
            Product item = db.Products.Single(model => model.productId == idItem);
            this.nameItem = item.productName;
            this.imageItem = item.image;
            this.unitPrice = int.Parse(item.price.ToString());
            this.priceItem = int.Parse(item.price.ToString()) - ((int.Parse(item.price.ToString()) * int.Parse(item.discount.ToString())) / 100);
            this.quantity = 1;
            this.discount = ((int.Parse(item.price.ToString()) * int.Parse(item.discount.ToString())) / 100) * this.quantity;
        }
    }
}