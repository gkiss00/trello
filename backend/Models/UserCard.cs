using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace prid_2021_a06.Models {
    public class UserCard {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int CardId { get; set; }
        public virtual Card Card { get; set; }
    }
}