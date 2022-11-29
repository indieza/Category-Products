using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CategoryProducts.ViewModels
{
    public abstract class BaseDataViewModel
    {
        public string Id { get; set; }

        public DateTime CreateOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public DateTime? DeleteOn { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}