namespace AppRegaliApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserAmicizia")]
    public partial class UserAmicizia
    {
        [Key]
        [Column(Order = 0)]
        public Guid IdUserRichiedente { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid IdUserDestinatario { get; set; }

        public bool Accettato { get; set; }
    }
}
