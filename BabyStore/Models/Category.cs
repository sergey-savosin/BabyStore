using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BabyStore.Models
{
    public class Category
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Название категории должно быть заполнено")]
        [StringLength(50, MinimumLength =3, ErrorMessage = "Название должно содержать от 3-х до 50-ти символов")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""-'\s]*$", ErrorMessage = "Название должно содержать символы латиницы и начинаться с большой буквы")]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}