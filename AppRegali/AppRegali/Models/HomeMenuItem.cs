using System;
using System.Collections.Generic;
using System.Text;

namespace AppRegali.Models
{
    public enum MenuItemType
    {
        Browse,
        About,
        Test,
        LogOut
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
