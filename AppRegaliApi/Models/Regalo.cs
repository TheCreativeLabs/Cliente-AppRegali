namespace AppRegaliApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Regalo")]
    public partial class Regalo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Regalo()
        {
            RegaloUserPartecipazione = new HashSet<RegaloUserPartecipazione>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid? IdEvento { get; set; }

        [Required]
        [StringLength(156)]
        public string Titolo { get; set; }

        [StringLength(4000)]
        public string Descrizione { get; set; }

        [Required]
        public double? Prezzo { get; set; }

        public bool? Cancellato { get; set; }

        public double? ImportoCollezionato { get; set; }

        public Guid? IdImmagineRegalo { get; set; }

        public virtual Evento Evento { get; set; }

        public virtual ImmagineRegalo ImmagineRegalo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RegaloUserPartecipazione> RegaloUserPartecipazione { get; set; }
    }
}
