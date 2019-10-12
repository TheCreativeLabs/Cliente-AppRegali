namespace AppRegaliApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ImmagineRegalo")]
    public partial class ImmagineRegalo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ImmagineRegalo()
        {
            Regaloes = new HashSet<Regalo>();
            Regaloes1 = new HashSet<Regalo>();
        }

        public Guid Id { get; set; }

        [Required]
        public byte[] Immagine { get; set; }

        public Guid IdRegalo { get; set; }

        public virtual Regalo Regalo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Regalo> Regaloes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Regalo> Regaloes1 { get; set; }
    }
}
