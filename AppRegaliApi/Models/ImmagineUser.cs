namespace AppRegaliApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ImmagineUser")]
    public partial class ImmagineUser
    {
        public Guid Id { get; set; }

        [Required]
        public byte[] Immagine { get; set; }

        public Guid IdUser { get; set; }
    }
}
