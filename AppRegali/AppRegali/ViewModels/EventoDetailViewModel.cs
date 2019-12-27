using System;
using Api;
using AppRegali.Models;

namespace AppRegali.ViewModels
{
    public class EventoDetailViewModel : BaseViewModel
    {
        public EventoDtoOutput Item { get; set; }

        public int HeightViewRegali { get; set; }

        public EventoDetailViewModel()
        {
        }
    }
}
