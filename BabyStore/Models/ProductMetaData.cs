using System.ComponentModel.DataAnnotations;

namespace BabyStore.Models
{
    [MetadataType(typeof(ProductMetaData))]
    public partial class Product
    {
    }

    public class ProductMetaData
    {
        [Required(ErrorMessage = "Название продукта дожно быть заполнено")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина названия от 3 до 50 символов")]
        [RegularExpression(@"^[a-zA-Z0-9""-'\s]*$", ErrorMessage = "Название категории должно состоять из латиницы и цифр")]
        [Display(Name = "Product Name")]
        public string Name;

        [Required(ErrorMessage = "Описание продукта должно быть заполнено")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Описание должно содержать от 10 до 200 символов")]
        [RegularExpression(@"^[,;a-zA-Z0-9""-'\s]*$", ErrorMessage = "Описание должно состоять из латиницы и цифр")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Цена - обязательное поле")]
        [Range(0.10, 10000, ErrorMessage = "Значение цены от 0.10 до 10000.00")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:c}")]
        [RegularExpression(@"[0-9]+(\.[0-9][0-9]?)?", ErrorMessage = "Цена должна содержать число и не более чем 2 дробные цифры")]
        public decimal Price { get; set; }
    }
}