namespace AppRegaliApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ImmagineEvento")]
    public partial class ImmagineEvento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ImmagineEvento()
        {
            Eventoes = new HashSet<Evento>();
        }

        public Guid Id { get; set; }

        [Required]
        public byte[] Immagine { get; set; }

        public Guid IdEvento { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Evento> Eventoes { get; set; }

        public virtual Evento Evento { get; set; }
    }
}
