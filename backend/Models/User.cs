using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace prid_2021_a06.Models {
    public enum Role {
        Admin = 2, Manager = 1, Member = 0
    }
    public class User : IValidatableObject {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^[a-zA-Z][_a-zA-Z0-9]{2,9}$", ErrorMessage = "Pseudo is not valid")]
        public string Pseudo { get; set; }

        [Required(ErrorMessage = "Required")]
        [MinLength(3, ErrorMessage = "Minimum 3 characters"), MaxLength(10, ErrorMessage = "Maximum 10 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,5})$", ErrorMessage = "Email is not valid")]
        public string Email { get; set; }

        [MaxLength(50, ErrorMessage = "The firstname is too long")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "The lastname is too long")]
        public string LastName { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")] // a checker dans le validate?
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Required")]
        public Role Role {get; set;} = Role.Member;

        [NotMapped]
        public string Token { get; set; }

        [NotMapped]
        public string PasswordConfirm { get; set; }

        //Liste des teams ou je suis N-N
        public virtual IList<UserTeam> TeamsIn { get; set; } = new List<UserTeam>();
        [NotMapped]
        public IEnumerable<Team> Teams {
            get => TeamsIn.Select(ui => ui.Team);
        }

        //Liste des users dans l'organisation N-N
        public virtual IList<UserOrganization> OrganizationsIn { get; set; } = new List<UserOrganization>();
        [NotMapped]
        public IEnumerable<Organization> Users {
            get => OrganizationsIn.Select(ui => ui.Organization);
        }

        //Liste de tables possédées 1-N
        public virtual IList<Table> TablesOwned {get; set; } = new List<Table>();

        //Liste de tables ou je suis N-N
        public virtual IList<UserTable> TablesIn { get; set; } = new List<UserTable>();
        [NotMapped]
        public IEnumerable<Table> Tables {
            get => TablesIn.Select(ut => ut.Table);
        }

        //Liste des cartes ou je suis N-N
        public virtual IList<UserCard> CardsIn { get; set; } = new List<UserCard>();
        [NotMapped]
        public IEnumerable<Card> Cards {
            get => CardsIn.Select(o => o.Card);
        }

        public int? Age {
            get {
                if (!BirthDate.HasValue)
                    return null;
                var today = DateTime.Today;
                var age = today.Year - BirthDate.Value.Year;
                if (BirthDate.Value.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            var currContext = validationContext.GetService(typeof(PridContext)) as PridContext;
            Debug.Assert(currContext != null);
            if (IsPseudoUsed(currContext))
                yield return new ValidationResult("Pseudo already used", new[] { nameof(Pseudo) });
            if (PasswordMismatch(currContext))
                yield return new ValidationResult("Passwords dont match", new[] { nameof(Password) });
            if (IsMailUsed(currContext))
                yield return new ValidationResult("Email already used", new[] { nameof(Email) });
            if (BirthDate.HasValue && BirthDate.Value.Date > DateTime.Today)
                yield return new ValidationResult("Can't be born in the future in this reality", new[] { nameof(BirthDate) });
            else if (Age.HasValue && Age < 18)
                yield return new ValidationResult("Must be 18 years old", new[] { nameof(BirthDate) });
            if (!AreNamesValid()){
                yield return new ValidationResult("Lastname and firstname must both be filled or left empty", new[] {nameof(FirstName), nameof(LastName) });
            }
        }

        private bool AreNamesValid()
        {
            return ((string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName)) || (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName)));
        }

        private bool IsPseudoUsed(PridContext context)
        {
            var user = context.Users.SingleOrDefault(u => u.Pseudo == Pseudo && u.Id != Id);
            return user != null;
        }

        private bool IsMailUsed(PridContext context)
        {
            var user = context.Users.SingleOrDefault(u => u.Email == Email && u.Id != Id);
            return user != null;
        }

        private bool PasswordMismatch(PridContext context)
        {
            return PasswordConfirm != Password;
        }
    }
}