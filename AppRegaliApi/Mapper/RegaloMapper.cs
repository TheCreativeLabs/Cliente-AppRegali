namespace AppRegaliApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.Security.Principal;

    public static class RegaloMapper
    {

        public static Regalo RegaloDtoInputToRegalo(RegaloDtoInput dto, Regalo regalo)
        {
            regalo.Cancellato = dto.Cancellato;
            regalo.Descrizione = dto.Descrizione;
            regalo.IdEvento = dto.IdEvento;
            regalo.Prezzo = dto.Prezzo;
            regalo.Titolo = dto.Titolo;
            regalo.ImportoCollezionato = 0;
            return regalo;
        }

        public static RegaloDtoOutput RegaloToRegaloDto(Regalo regalo)
        {
            RegaloDtoOutput dto = new RegaloDtoOutput();
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
                dto.IdImmagineRegalo = regalo.IdImmagineRegalo.ToString();
            }
            if ( regalo.ImmagineRegalo != null)
            {
                dto.ImmagineRegalo = regalo.ImmagineRegalo.Immagine;
            }
            return dto;
        }

        public static List<RegaloDtoOutput> RegaloToRegaloDtoList(List<Regalo> regalo)
        {
            List<RegaloDtoOutput> listDto = new List<RegaloDtoOutput>();
            regalo.ForEach( x => listDto.Add(RegaloToRegaloDto(x)));
            return listDto;
        }


    }
}
