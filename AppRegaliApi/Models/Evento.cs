namespace AppRegaliApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Evento")]
    public partial class Evento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Evento()
        {
            Regalo = new HashSet<Regalo>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [StringLength(128)]
        public string Titolo { get; set; }

        [StringLength(4000)]
        public string Descrizione { get; set; }

        [Required]
        public Guid IdUtenteCreazione { get; set; }

        [Required]
        public DateTime DataCreazione { get; set; }

        public DateTime? DataModifica { get; set; }

        [Required]
        public DateTime DataEvento { get; set; }

        public bool? Cancellato { get; set; }

        public Guid? IdImmagineEvento { get; set; }

        [Required]
        public Guid IdCategoriaEvento { get; set; }

        public virtual EventoCategoria EventoCategoria { get; set; }

        public virtual ImmagineEvento ImmagineEvento { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Regalo> Regalo { get; set; }
    }
}
