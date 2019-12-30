namespace AppRegaliApi
{
    using System;
    using System.Collections.Generic;

    public partial class PartecipazioneDtoOutput
    {
        public List<UtentePartecipazioneDtoOutput> UtentiPartecipanti { get; set; }

        public int NumeroAnonimi { get; set; }
    }
}
