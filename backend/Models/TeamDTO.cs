using System;
using System.Collections.Generic;

namespace prid_2021_a06.Models {
    public class TeamDTO {
        public int Id {get; set;}
        public string Name { get; set; }
        public string Action {get; set;}
        public string MemberPseudo {get; set;}
        public virtual IList<UserTeam> UsersIn { get; set; }
        public virtual IList<Table> TablesTeam { get; set; }
        public int? OrganizationId { get; set; }
    }
}