using System;

namespace AppCommon
{
    public class SecuritySettings
    {
        public string Authority { get; set; }
        public string RedirectUri { get; set; }
        public string PostLogoutRedirectUri { get; set; }
    }
}
