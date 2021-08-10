using System;
using System.Collections.Generic;

namespace prid_2021_a06.Models {
    public class UserDTO {
        public int Id { get; set; }
        public string Pseudo { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public Role Role {get; set;}
        public virtual IList<UserTeam> TeamsIn { get; set; }
        public virtual IList<UserOrganization> OrganizationsIn { get; set; }
        public virtual IList<Table> TablesOwned {get; set; }
        public virtual IList<UserTable> TablesIn { get; set; }
        public virtual IList<UserCard> CardsIn { get; set; }
    }
}