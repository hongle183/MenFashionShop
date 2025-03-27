//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopOnline.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Invoince
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Invoince()
        {
            this.InvoinceDetails = new HashSet<InvoinceDetail>();
        }
    
        public System.Guid invoinceId { get; set; }
        public System.Guid memberId { get; set; }
        public Nullable<int> totalMoney { get; set; }
        public string paymentMethod { get; set; }
        public string paymentStatus { get; set; }
        public string transactionId { get; set; }
        public string note { get; set; }
        public string meta { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> dateCreate { get; set; }
    
        public virtual Member Member { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoinceDetail> InvoinceDetails { get; set; }
    }
}
