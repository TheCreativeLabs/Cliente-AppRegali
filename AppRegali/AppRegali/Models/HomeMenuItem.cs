using System;
using System.Collections.Generic;
using System.Text;

namespace AppRegali.Models
{
    public enum MenuItemType
    {
        Browse,
        Account,
        LogOut
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
