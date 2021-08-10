using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace prid_2021_a06.Models {
    public class Card : IValidatableObject {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Title { get; set; }

        public string Content { get; set; }

        public virtual User Author { get; set; }
        public int? AuthorId { get; set; }

        //Table of the card 1-N
        public virtual Section Section { get; set; }
        public int? SectionId { get; set; }

        public int? Position { get ; set; }

        //Liste de ses users N-N
        public virtual IList<UserCard> UsersIn { get; set; } = new List<UserCard>();
        [NotMapped]
        public IEnumerable<User> Users {
            get {
                List<User> tmp = new List<User>();
                tmp.AddRange(UsersIn.Select(u => new User(){Id = u.User.Id, Pseudo = u.User.Pseudo}));
                return tmp;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            var currContext = validationContext.GetService(typeof(PridContext)) as PridContext;
            Debug.Assert(currContext != null);
            if (IsTitleEmpty())
                yield return new ValidationResult("Title is empty", new[] { nameof(Title) });
            if (IsTitleNotUniqueInTable(currContext))
                yield return new ValidationResult("Title is not unique", new[] { nameof(Title) });
        }

        private bool IsTitleEmpty()
        {
            return Title == null || Title == "";
        }

        private bool IsTitleNotUniqueInTable(PridContext context){
            var section = context.Sections.FirstOrDefault(s => s.Id == SectionId);
            var tmp = context.Cards.Where(c => c.Title == Title && c.Section.Table.Id == section.Table.Id && c.Id != Id); //&& c.Section.Table.Id != Section.Table.Id
            if(tmp != null){
                return tmp.Count<Card>() > 0;
            }
            return false;
        }
    }
}