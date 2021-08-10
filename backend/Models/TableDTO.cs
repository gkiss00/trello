using System;
using System.Collections.Generic;

namespace prid_2021_a06.Models {
    public class TableDTO {
        public int Id { get; set; }
        public int? teamId { get; set; }
        public int? organizationId { get; set; }
        public string[] usersInString { get; set; }
        public string Title { get; set; }
        public virtual User Owner { get; set; }
        public View View { get; set; }
        public int? UserId { get; set; }
        public string Action { get; set; }
        public virtual Team Team { get; set; }
        public virtual IList<SectionDTO> Sections { get; set; }
        public virtual IList<User> UsersIn { get; set; }
    }
}