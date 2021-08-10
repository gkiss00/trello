namespace prid_2021_a06.Models {
    public class UserTable {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int TableId { get; set; }
        public virtual Table Table { get; set; }
    }
}