namespace AppRegaliApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.Security.Principal;

    public partial class RegaloMapper
    {
        public RegaloMapper()
        {
        }

        public Regalo RegaloDtoToRegalo(RegaloDto dto)
        {
            Regalo regalo = new Regalo();
            if (dto.Id != null)
            {
                regalo.Id = new Guid(dto.Id);
            }
            regalo.Cancellato = dto.Cancellato;
            regalo.Descrizione = dto.Descrizione;
            regalo.IdEvento = dto.IdEvento;
            regalo.Prezzo = dto.Prezzo;
            regalo.Titolo = dto.Titolo;
            regalo.ImportoCollezionato = 0;
            if (dto.IdImmagineRegalo == null && dto.ImmagineRegalo != null)
            {
                ImmagineRegalo immRegalo = new ImmagineRegalo();
                immRegalo.Immagine = dto.ImmagineRegalo;
                regalo.ImmagineRegalo = immRegalo;
            } else if (dto.IdImmagineRegalo != null && dto.ImmagineRegalo != null)
            {
                //fixme updateImmagineEvento
            }
            return regalo;
        }

        public RegaloDto RegaloToRegaloDto(Regalo regalo)
        {
            RegaloDto dto = new RegaloDto();
            if (regalo.Id != null)
            {
                dto.Id = regalo.Id.ToString();
            }
            dto.Cancellato = regalo.Cancellato;
            dto.Descrizione = regalo.Descrizione;
            dto.IdEvento = regalo.IdEvento;
            dto.Prezzo = regalo.Prezzo;
            dto.Titolo = regalo.Titolo;
            dto.ImportoCollezionato = regalo.ImportoCollezionato;
            if (regalo.IdImmagineRegalo != null)
            {
                dto.IdImmagineRegalo = regalo.IdImmagineRegalo;
            }
            if ( regalo.ImmagineRegalo != null)
            {
                dto.ImmagineRegalo = regalo.ImmagineRegalo.Immagine;
            }
            return dto;
        }

        public List<RegaloDto> RegaloToRegaloDtoList(List<Regalo> regalo)
        {
            List<RegaloDto> listDto = new List<RegaloDto>();
            regalo.ForEach( x => listDto.Add(RegaloToRegaloDto(x)));
            return listDto;
        }


    }
}
