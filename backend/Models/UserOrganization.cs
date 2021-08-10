using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace prid_2021_a06.Models {
    public class UserOrganization {
        public int UserId {get; set;}
        public virtual User User {get; set; }
        public int OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
    }
}