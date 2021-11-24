using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGraph
{
    internal class Account : IAccount
    {
        public string Username => "hmohammed.c@tetco.sa";

        public string Environment => "login.windows.net";

        public AccountId HomeAccountId => new ("90d608a-9e08-49cd-b082-b8a104f73b4f.8c7c7c28-320f-4385-aa6b-19348f852df0",
            "d90d608a-9e08-49cd-b082-b8a104f73b4f",
            "8c7c7c28-320f-4385-aa6b-19348f852df0"
            );
    }
}
