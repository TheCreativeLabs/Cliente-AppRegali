namespace AppRegaliApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Threading.Tasks;

    public static partial class EventoMapper
    {
        public static Evento EventoDtoToEvento(EventoDtoOutput dto, Guid UserId )
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
            if (dto.IdImmagineEvento != null)
            {
                evento.IdImmagineEvento = new Guid(dto.IdImmagineEvento);
            }
            evento.IdUtenteCreazione = UserId;
            evento.Titolo = dto.Titolo;
            evento.DataCreazione = dto.DataCreazione;
            evento.DataModifica = dto.DataModifica;
            return evento;
        }

        public static EventoDtoOutput EventoToEventoDto(Evento evento)
        {
            if(evento == null)
            {
                return null;
            }
            EventoDtoOutput dto = new EventoDtoOutput();
            if (evento.Id != null)
            {
                dto.Id = evento.Id.ToString();
            }
            if ( evento.Regalo != null)
            {
                dto.Regali = RegaloMapper.RegaloToRegaloDtoList(evento.Regalo.Cast<Regalo>().ToList());
            }
            dto.Cancellato = evento.Cancellato;
            dto.DataEvento = evento.DataEvento;
            dto.Descrizione = evento.Descrizione;
            dto.IdCategoriaEvento = evento.IdCategoriaEvento;
            dto.DataCreazione = evento.DataCreazione;
            dto.DataModifica = evento.DataModifica;
            dto.IdImmagineEvento = evento.IdImmagineEvento.ToString();
            //dto.IdUtenteCreazione = evento.IdUtenteCreazione;
            //FIXME servono info sul creatore dell'evento?
            dto.Titolo = evento.Titolo;
            if (evento.IdImmagineEvento != null && evento.ImmagineEvento != null)
            {
                dto.ImmagineEvento = evento.ImmagineEvento.Immagine;
            }
            return dto;
        }

        public static List<EventoDtoOutput> EventoToEventoDtoList(List<Evento> eventi)
        {
            if (eventi == null || eventi.Count() == 0)
            {
                return new List<EventoDtoOutput>();
            }
            List<EventoDtoOutput> listDto = new List<EventoDtoOutput>();
            eventi.ForEach(x => listDto.Add(EventoToEventoDto(x)));
            return listDto;
        }
    }
}
