using System;
using System.Collections.Generic;

namespace prid_2021_a06.Models {
    public class OrganizationDTO {
        public int Id {get; set;}
        public string Name { get; set; }
        public string[] UserList {get; set;}
        public virtual IList<UserOrganization> UsersIn { get; set; }
        public virtual IList<Team> Teams {get;set;}
    }
}