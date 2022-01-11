using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APP.Base.Model.Model
{
    public class AuthenticationModel
    {
        public AuthenticationModel()
        {
            this.TokenExpiresIn = 30;
            this.TokenExpiresStartTime = DateTime.Now;
            this.TokenExpiresEndTime = this.TokenExpiresStartTime.AddMinutes(30);
        }
        public string ApplicationCode { get; set; }
        public string UserName { get; set; }
        public int TokenExpiresIn { get; set; }
        public DateTime TokenExpiresStartTime { get; set; }
        public DateTime TokenExpiresEndTime { get; set; }
        public string Role { get; set; }
        public string NameSurname { get; set; }
        public string TokenCipherText { get; set; }
        public string IPAddress { get; set; }
    }
}
