using System;
using System.Collections.Generic;
using System.Text;

namespace AppRegali.Models
{
    public enum MenuItemType
    {
        Home,
        EventiPersonali,
        Amici,
        Account
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }

        public string Icon { get; set; }
    }
}
