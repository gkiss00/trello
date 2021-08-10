using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace prid_2021_a06.Models {
    public class Section : IValidatableObject {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Title { get; set; }

        //La table dans laquel il est 1-N
        public virtual Table Table {get; set;}
        public int? TableId { get; set; }

        //Les cartes qu'elle contient 1-N
        public virtual List<Card> Cards { get; set; } = new List<Card>();

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