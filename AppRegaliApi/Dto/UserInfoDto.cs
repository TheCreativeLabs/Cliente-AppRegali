namespace AppRegaliApi
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class UserInfoDto
    {

        public class UserRelation
        {
            private UserRelation(string value) { Value = value; }

            public string Value { get; set; }

            //utilizzare quando si resituisce l'userInfo dell'utente corrente
            public static UserRelation ME { get { return new UserRelation("ME"); } }

            //utilizzare quando si restituisce l'userInfo di un utente AMICO dell'utente corrente
            public static UserRelation CONTACT { get { return new UserRelation("CONTACT"); } }

            //utilizzare quando si restituisce l'userInfo di un utente che ha chiesto l'amicizia all'utente corrente (richiesta IN ENTRATA)
            public static UserRelation REQUEST_IN { get { return new UserRelation("REQUEST_IN"); } }

            //utilizzare quando si restituisce l'userInfo di un utente a cui l'utente corrente ha chiesto l'amicizia  (richiesta IN USCITA)
            public static UserRelation REQUEST_OUT { get { return new UserRelation("REQUEST_OUT"); } }

            //utilizzare quando si restituisce l'userInfo di un utente che NON è IN RELAZIONE con l'utente corrente
            public static UserRelation STRANGER { get { return new UserRelation("STRANGER"); } }
        }

        [StringLength(256)]
        public string Nome { get; set; }

        [StringLength(256)]
        public string Cognome { get; set; }

        public DateTime? DataDiNascita { get; set; }

        public byte[] FotoProfilo { get; set; }

        [Required]
        public Guid IdAspNetUser { get; set; }

        public string Email { get; set; }

        public string PhotoUrl { get; set; }

        public UserRelation Relation { get; set; }
    }
}
