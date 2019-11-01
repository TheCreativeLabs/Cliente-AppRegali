namespace AppRegaliApi.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class FacebookImageData
    {
        public String data { get; set; }

        public class Data
        {
            public String height { get; set; }
            public String is_silhouette { get; set; }
            public String url { get; set; }
            public String width { get; set; }
        }
    }
}
