namespace AppRegaliApi
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class UserInfoDto
    {

        [StringLength(256)]
        public string Nome { get; set; }

        [StringLength(256)]
        public string Cognome { get; set; }

        public DateTime? DataDiNascita { get; set; }

        public byte[] FotoProfilo { get; set; }

        [Required]
        public Guid IdAspNetUser { get; set; }

        public string Email { get; set; }
    }
}
