using System.Collections.Generic;

namespace dotnetPortalOkta.Models
{
    public class AppViewModel
    {
        public string Username { get; set; }

        public bool SdkAvailable { get; set; }

        public dynamic UserInfo { get; set; }

        public List<Okta.Sdk.IAppLink> Applications { get; set; }
    }
}
