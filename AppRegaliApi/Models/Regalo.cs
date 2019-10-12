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
            ImmagineRegaloes = new HashSet<ImmagineRegalo>();
            RegaloUserPartecipaziones = new HashSet<RegaloUserPartecipazione>();
        }

        public Guid Id { get; set; }

        public Guid? IdEvento { get; set; }

        [StringLength(156)]
        public string Titolo { get; set; }

        [StringLength(4000)]
        public string Descrizione { get; set; }

        public double? Prezzo { get; set; }

        public bool? Cancellato { get; set; }

        public double? ImportoCollezionato { get; set; }

        public Guid? IdImmagineRegalo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImmagineRegalo> ImmagineRegaloes { get; set; }

        public virtual ImmagineRegalo ImmagineRegalo { get; set; }

        public virtual ImmagineRegalo ImmagineRegalo1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RegaloUserPartecipazione> RegaloUserPartecipaziones { get; set; }
    }
}
