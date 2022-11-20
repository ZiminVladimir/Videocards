using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Videocards.Models
{
    public class Videocard
    {
        public int Id { get; set; }

        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Компания")]
        public string Company { get; set; }

        [Display(Name = "Производитель")]
        public string VendorName { get; set; }

        [Display(Name = "Изображение")]
        public string ImageUrl { get; set; }

        public Vendor Vendor { get; set; }

        [Display(Name = "Цена")]
        public int Price { get; set; }

        [Display(Name = "Обзор")]
        public string OverView { get; set; }

    }
}
