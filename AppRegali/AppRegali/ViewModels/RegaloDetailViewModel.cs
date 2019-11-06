using System;
using Api;
using AppRegali.Models;

namespace AppRegali.ViewModels
{
    public class RegaloDetailViewModel : BaseViewModel
    {
        public RegaloDtoOutput Item { get; set; }
        public RegaloDetailViewModel(RegaloDtoOutput item = null)
        {
            Title = item?.Titolo;
            Item = item;
        }
    }
}
