using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace prid_2021_a06.Models {
    public class Organization : IValidatableObject {
        [Key]
        public int Id {get; set;}

        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }

        //Liste des users dans l'organisation N-N
        public virtual IList<UserOrganization> UsersIn { get; set; } = new List<UserOrganization>();

        //Liste des teams dans l'organisation 1-N
        public virtual IList<Team> Teams { get; set; } = new List<Team>();

        [NotMapped]
        public IEnumerable<User> Users {
            get => UsersIn.Select(ui => ui.User);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            var currContext = validationContext.GetService(typeof(PridContext)) as PridContext;
            Debug.Assert(currContext != null);
            if (IsNameEmpty())
                yield return new ValidationResult("Name is empty", new[] { nameof(Name) });
        }

        private bool IsNameEmpty()
        {
            return Name == null || Name == "";
        }
    }
}