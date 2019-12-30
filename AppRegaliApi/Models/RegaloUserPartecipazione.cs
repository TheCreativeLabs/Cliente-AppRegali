namespace AppRegaliApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RegaloUserPartecipazione")]
    public partial class RegaloUserPartecipazione
    {

        [Key]
        public Guid Id { get; set; }

        public Guid IdRegalo { get; set; }
        
        public Guid IdUser { get; set; }

        public double Importo { get; set; }

        public bool Anonimo { get; set; }

        public virtual Regalo Regalo { get; set; }

        public virtual UserInfo UserInfoPartecipante{ get; set; }
    }
}
