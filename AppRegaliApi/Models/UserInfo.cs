namespace AppRegaliApi
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserInfo")]
    public partial class UserInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(256)]
        public string Nome { get; set; }

        [StringLength(256)]
        public string Cognome { get; set; }

        public DateTime? DataDiNascita { get; set; }

        public byte[] FotoProfilo { get; set; }

        [Required]
        public Guid IdAspNetUser { get; set; }
    }
}
