namespace AppRegaliApi
{
    using AppRegaliApi.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserInfo")]
    public partial class UserInfo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserInfo()
        {
            RegaloUserPartecipazioni = new HashSet<RegaloUserPartecipazione>();
        }

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

        public string PhotoUrl { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RegaloUserPartecipazione> RegaloUserPartecipazioni { get; set; }
    }
}
