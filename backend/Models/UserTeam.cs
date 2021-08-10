namespace prid_2021_a06.Models {
    public class UserTeam {
        public int UserId {get; set;}
        public virtual User User {get; set; }
        public int TeamId { get; set; }
        public virtual Team Team { get; set; }


        public UserTeam() {}
        public UserTeam(UserTeam ut)
        {
            this.UserId = ut.UserId;
            this.User = ut.User;
            this.TeamId = ut.TeamId;
            this.Team = ut.Team;
        }
    }
}