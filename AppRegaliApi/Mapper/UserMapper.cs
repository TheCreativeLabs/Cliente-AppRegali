namespace AppRegaliApi.Models
{
    using System;
    using System.Security.Principal;

    public partial class UserInfoMapper
    {
        public UserInfoMapper()
        {
        }

        public Evento EventoDtoToEvento(EventoDto dto, Guid CurrentUserId )
        {
            Evento evento = new Evento();
            if (dto.Id != null)
            {
                evento.Id = new Guid(dto.Id);
            }
            evento.Cancellato = dto.Cancellato;
            evento.DataEvento = dto.DataEvento;
            evento.Descrizione = dto.Descrizione;
            evento.IdCategoriaEvento = dto.IdCategoriaEvento;
            evento.IdUtenteCreazione = CurrentUserId;
            evento.Titolo = dto.Titolo;
            if (dto.IdImmagineEvento == null && dto.ImmagineEvento != null)
            {
                //fixme createImmagineEvento();
                ImmagineEvento immEvento = new ImmagineEvento();
                immEvento.Immagine = dto.ImmagineEvento;
                evento.ImmagineEvento = immEvento;
            } else if (dto.IdImmagineEvento != null && dto.ImmagineEvento != null)
            {
                //fixme updateImmagineEvento
            }
            return evento;
        }
    }
}
