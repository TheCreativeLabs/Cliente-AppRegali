namespace AppRegaliApi.Models
{
    using System;
    using System.Security.Principal;

    public partial class EventoMapper
    {
        public EventoMapper()
        {
        }

        public Evento EventoInputDtoToEvnto(EventoInputDto dto, Guid CurrentUserId )
        {
            Evento evento = new Evento();
            evento.Cancellato = dto.Cancellato;
            evento.DataEvento = dto.DataEvento;
            evento.Descrizione = dto.Descrizione;
            evento.IdCategoriaEvento = dto.IdCategoriaEvento;
            evento.IdUtenteCreazione = CurrentUserId;
            evento.Titolo = dto.Titolo;
            return evento;
        }
    }
}
