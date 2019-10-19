namespace AppRegaliApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EventoInputDto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EventoInputDto()
        {
        }

        [Required]
        [StringLength(128)]
        public string Titolo { get; set; }

        [StringLength(4000)]
        public string Descrizione { get; set; }

        [Required]
        public DateTime DataEvento { get; set; }

        public bool? Cancellato { get; set; }

        [Required]
        public Guid IdCategoriaEvento { get; set; }
    }
}
