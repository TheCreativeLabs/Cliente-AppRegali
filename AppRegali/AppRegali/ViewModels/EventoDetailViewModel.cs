using System;
using Api;
using AppRegali.Models;

namespace AppRegali.ViewModels
{
    public class EventoDetailViewModel : BaseViewModel
    {
        public Evento Item { get; set; }
        public EventoDetailViewModel(Evento item = null)
        {
            Title = item?.Titolo;
            Item = item;
        }
    }
}
