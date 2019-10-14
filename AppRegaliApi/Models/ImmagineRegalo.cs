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
            Regalo = new HashSet<Regalo>();
        }

        public Guid Id { get; set; }

        [Required]
        public byte[] Immagine { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Regalo> Regalo { get; set; }
    }
}
