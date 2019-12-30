namespace AppRegaliApi
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class UtentePartecipazioneDtoOutput
    {
        [StringLength(256)]
        public string NomePartecipante { get; set; }

        [StringLength(256)]
        public string CognomePartecipante { get; set; }

        [Required]
        public Guid IdUserPartecipante { get; set; }

        [Required]
        public Guid IdRegalo { get; set; }

        public bool Anonimo { get; set; }


    }
}
