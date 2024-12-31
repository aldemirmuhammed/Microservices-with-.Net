using System.Collections.Generic;

namespace FreeCourse.Web.Models.SignalR
{
    public class UserHubModels
    {
        public string UserName { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }
}
