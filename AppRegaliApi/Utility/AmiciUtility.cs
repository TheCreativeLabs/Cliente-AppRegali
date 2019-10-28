namespace AppRegaliApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class AmiciUtility
    {
        private static DbDataContext dbDataContext = new DbDataContext();

        public AmiciUtility()
        {
        }

        public static async Task<List<Guid>> GetIdAmiciOfUser(Guid idUser)
        {
            //Per trovare tutti gli amici del idUser, devo considerare tutte le righe di UserAmicizia 
            //sia nel caso in cui idUser è destinatario che nel caso in cui idUser è richiedente

            //current è destinatario, gli amici sono richiedenti
            //List<Guid> idAmiciRichiedenti = await dbDataContext.UserAmicizia
            //                                        .Where(x => ((x.IdUserDestinatario == idUser) && (x.Accettato)))
            //                                        .Select(x => x.IdUserRichiedente)
            //                                        .ToListAsync();

            //current è richiedente, gli amici sono destinatari
            //List<Guid> idAmiciDestinatari = await dbDataContext.UserAmicizia
            //                                        .Where(x => ((x.IdUserRichiedente == idUser) && (x.Accettato)))
            //                                        .Select(x => x.IdUserDestinatario)
            //                                        .ToListAsync();


            List<Guid> idAmiciAll = await dbDataContext.UserAmicizia
                                    .Where(x => (x.IdUserDestinatario == idUser || x.IdUserRichiedente == idUser) && x.Accettato == true)
                                    .Select(x => (x.IdUserDestinatario == idUser ? x.IdUserRichiedente : x.IdUserDestinatario)).ToListAsync();



            //List<Guid> idAmiciAll = idAmiciRichiedenti.Union(idAmiciDestinatari).ToListAsync();

            return idAmiciAll;
        }
    }
}
