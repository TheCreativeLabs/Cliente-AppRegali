namespace AppRegaliApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class EventoDtoOutput
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EventoDtoOutput()
        {
        }

        public String Id { get; set; }

        [Required]
        [StringLength(128)]
        public string Titolo { get; set; }

        [StringLength(4000)]
        public string Descrizione { get; set; }

        [Required]
        public DateTime DataEvento { get; set; }

        public DateTime? DataModifica { get; set; }

        public DateTime DataCreazione { get; set; }

        public bool? Cancellato { get; set; }

        [Required]
        public Guid IdCategoriaEvento { get; set; }

        public string CodiceCategoriaEvento { get; set; }

        public string IdImmagineEvento { get; set; }

        public byte[] ImmagineEvento { get; set; }

        public List<RegaloDtoOutput> Regali { get; set; }


        public byte[] ImmagineUserCreatoreEvento { get; set; }

        public string NomeUserCreatoreEvento { get; set; }

        public string CognomeUserCreatoreEvento { get; set; }
    }
}
