namespace AppRegaliApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class RegaloDtoInput
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RegaloDtoInput()
        {
        }

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

        public byte[] ImmagineRegalo { get; set; }

        public double? ImportoCollezionato { get; set; }
    }
}
