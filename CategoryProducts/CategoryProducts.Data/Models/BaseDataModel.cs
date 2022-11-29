namespace CategoryProducts.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public abstract class BaseDataModel
    {
        protected BaseDataModel()
        {
            this.Id = $"{Guid.NewGuid()}_{((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds()}";
            this.CreateOn = DateTime.UtcNow;
        }

        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public DateTime CreateOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public DateTime? DeleteOn { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}