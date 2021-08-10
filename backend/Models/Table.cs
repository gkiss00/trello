using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace prid_2021_a06.Models {
    public enum View {
        Public = 0, Organization = 1, Team = 2, Private = 3,
    }
    public class Table : IValidatableObject {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Title { get; set; }

        public virtual User Owner { get; set; }
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Required")]
        public View View { get; set; } = View.Private;

        public virtual Team Team { get; set;}
        public int? TeamId { get; set; }

        public virtual Organization Organization { get; set; }
        public int? OrganizationId { get; set; }

        //Liste des sections de la table 1-N
        public virtual IList<Section> Sections { get; set; } = new List<Section>();

        //Liste des users dans la table N-N
        public virtual IList<UserTable> UsersIn { get; set; } = new List<UserTable>();
        [NotMapped]
        private IList<User> users;
        [NotMapped]
        public IList<User> Users {
            get {
                return users;
            }
            set {
                if(value != null){
                    users = value;
                } else {
                    var list = new List<User>();
                    list.AddRange(UsersIn.Select(u => new User(){Id = u.User.Id, Pseudo = u.User.Pseudo}).ToList());
                    users = list;
                }
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            var currContext = validationContext.GetService(typeof(PridContext)) as PridContext;
            Debug.Assert(currContext != null);
            if (IsTitleEmpty())
                yield return new ValidationResult("Title is empty", new[] { nameof(Title) });
        }

        private bool IsTitleEmpty()
        {
            return Title == null || Title == "";
        }
    }
}