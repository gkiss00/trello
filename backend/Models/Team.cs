using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace prid_2021_a06.Models {
    public class Team : IValidatableObject {
        [Key]
        public int Id {get; set;}

        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }

        //Liste des users dans l'équipe N-N
        public virtual IList<UserTeam> UsersIn { get; set; } = new List<UserTeam>();

        //Organization of the team
        public virtual Organization Organization { get; set; }
        public int? OrganizationId { get; set; }

        public virtual IList<Table> TablesTeam { get; set; } = new List<Table>();

        [NotMapped]
        public IEnumerable<User> Users {
            get => UsersIn.Select(ui => ui.User);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            var currContext = validationContext.GetService(typeof(PridContext)) as PridContext;
            Debug.Assert(currContext != null);
            if (isNameEmpty())
                yield return new ValidationResult("Name is empty", new[] { nameof(Name) });
        }

        private Boolean isNameEmpty(){
            return Name == null || Name == "";
        }
    }
}