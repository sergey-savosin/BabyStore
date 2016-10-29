using System.Collections.Generic;
using System.ComponentModel;

namespace BabyStore.Models
{
    public class Category
    {
        public int ID { get; set; }
        [DisplayName("Category Name")]
        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}