using System;
using System.Collections.Generic;

namespace prid_2021_a06.Models {
    public class SectionDTO {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual Table Table { get; set;}
        public int? TableId { get; set; }
        public virtual List<CardDTO> Cards { get; set; }
    }
}