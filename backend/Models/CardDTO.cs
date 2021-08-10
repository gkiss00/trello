using System;
using System.Collections.Generic;

namespace prid_2021_a06.Models {
    public class CardDTO {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? Position { get; set; }
        public string Content { get; set; }
        public virtual User Author{ get; set; }
        public int? AuthorId { get; set; }
        public virtual Section Section { get; set; }
        public int? SectionId { get; set; }
        public string Action { get; set; }
        public int? userId { get; set; }
        public virtual IList<User> UsersIn { get; set; }
    }
}
