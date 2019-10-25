namespace AppRegaliApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;

    public partial class EventoMapper
    {
        public EventoMapper()
        {
        }

        RegaloMapper regaloMapper = new RegaloMapper();

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
            evento.DataCreazione = dto.DataCreazione;
            evento.DataModifica = dto.DataModifica;
            // else if (dto.IdImmagineEvento != null && dto.ImmagineEvento != null)
            //{
                //fixme updateImmagineEvento
            //}
            return evento;
        }

        public EventoDto EventoToEventoDto(Evento evento)
        {
            EventoDto dto = new EventoDto();
            if (evento.Id != null)
            {
                dto.Id = evento.Id.ToString();
            }
            if ( evento.Regalo != null)
            {
                dto.Regali = regaloMapper.RegaloToRegaloDtoList(evento.Regalo.Cast<Regalo>().ToList());
            }
            dto.Cancellato = evento.Cancellato;
            dto.DataEvento = evento.DataEvento;
            dto.Descrizione = evento.Descrizione;
            dto.IdCategoriaEvento = evento.IdCategoriaEvento;
            dto.DataCreazione = evento.DataCreazione;
            dto.DataModifica = evento.DataModifica;
            //dto.IdUtenteCreazione = evento.IdUtenteCreazione;
            //FIXME servono info sul creatore dell'evento?
            dto.Titolo = evento.Titolo;
            if (evento.IdImmagineEvento != null && evento.ImmagineEvento != null)
            {
                dto.ImmagineEvento = evento.ImmagineEvento.Immagine;
            }
            return dto;
        }

        public List<EventoDto> EventoToEventoDtoList(List<Evento> eventi)
        {
            List<EventoDto> listDto = new List<EventoDto>();
            eventi.ForEach(x => listDto.Add(EventoToEventoDto(x)));
            return listDto;
        }
    }
}
