using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using prid_2021_a06.Models;
using System.ComponentModel.DataAnnotations;
using PRID_Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Security.Claims;
using prid_2021_a06.Helpers;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace prid_2021_a06.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase {
        private readonly PridContext _context;

        public TeamsController(PridContext context) {
            _context = context;
        }

        //POST A NEW TEAM WITH ID NAME AND USERSIN
        [HttpPost]
        public async Task<ActionResult<Team>> CreateTeam(TeamDTO data) // ajouter les regles metiers
        {
            //get current user from frontend;
            User current_user_frontend = await  _context.Users.FirstOrDefaultAsync(uu => uu.Pseudo == data.MemberPseudo);
            //get current user from backend;
            User current_user_backend = this.GetCurrentUser(_context);
            if (current_user_frontend == null || current_user_backend == null)
                return NotFound();
            if (current_user_backend.Id != current_user_frontend.Id)
                return BadRequest("You are not allowed to do this");
            //check the team name
            if (data.Name == null)
                return BadRequest(new ValidationErrors().Add("Title is not filled", "Title"));
            //create new team
            var newTeam = new Team();
            newTeam.Id = data.Id;
            newTeam.Name = data.Name;
            //create the userteam
            UserTeam userTeam= new UserTeam();
            userTeam.UserId = current_user_frontend.Id;
            userTeam.TeamId = newTeam.Id;
            //add to the context
            _context.Teams.Add(newTeam);
            _context.UserTeam.Add(userTeam);
            var res = await _context.SaveChangesAsyncWithValidation();

            if (!res.IsEmpty)
                return BadRequest(res);
            var team = new Team(){Id = newTeam.Id, Name = newTeam.Name};
            return team;
        }

        //RETURN THE A TEAM WITH HER NAME AND ID
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDTO>> ReadTeam(int id)
        {
            //get the current user
            User current_user_backend = this.GetCurrentUser(_context);
            if (current_user_backend == null)
                return BadRequest("You are not connected");
            //get the team
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == id);
            if (team == null)
                return NotFound();
            //check permission
            if(!current_user_backend.Teams.Contains(team))
                return BadRequest("Permission denied");
            //get
            return new Team(){Id = team.Id, Name = team.Name}.ToDTO();
        }

        //RETURN ALL MEMBERS OF A TEAM
        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet("{id}/members")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> getTeamMembers(int id)
        {
            //get the current user
            User current_user_backend = this.GetCurrentUser(_context);
            if (current_user_backend == null)
                return BadRequest("You are not connected");
            //get the team
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == id);
            //chek if it exists
            if (team == null)
                return NotFound();
            //check if the user is part of the team
            var ui = team.Users;
            foreach(var u in ui){
                if (u.Id == current_user_backend.Id)
                    return team.Users.Select(u => new User(){Id = u.Id, Pseudo = u.Pseudo, FirstName=u.FirstName, LastName=u.LastName}).ToDTO();
            }
            return BadRequest("You are not allowed to do that");
        }

        //RETURN ???
        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet("user/{id}/teams")]
        public async Task<ActionResult<IEnumerable<TeamDTO>>> getUserTeams(int id)
        {
            //get the current user
            User current_user_backend = this.GetCurrentUser(_context);
            //check if you are connected
            if (current_user_backend == null)
                return BadRequest("You are not connected");
            var teams = await getTeamsTables(id);
            if (teams == null)
                return NotFound();
            return teams.Select(t => new Team(){Id = t.Id, Name = t.Name}).ToDTO();
        }       

        //returns teams with organization for the userid
        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet("user/{id}/teams/orga")]
        public async Task<ActionResult<IEnumerable<TeamDTO>>> getUserTeamsWithOrga(int id)
        {
            //get the current user
            User current_user_backend = this.GetCurrentUser(_context);
            //check if you are connected
            if (current_user_backend == null)
                return BadRequest("You are not connected");
            var teams = await getTeamsTables(id);
            if (teams == null)
                return NotFound();
            return teams.Where(t => t.OrganizationId == null).Select(t => new Team(){Id = t.Id, Name = t.Name}).ToDTO();
        }
        


        //returns currentUser teams and tables of teams
        private async Task<IEnumerable<Team>> getTeamsTables(int id){
            //get the current user
            User current_user_backend = this.GetCurrentUser(_context);
            //check if you are connected
            if (current_user_backend.Id != id)
                return null;
            var teams = (await _context.Users.FirstOrDefaultAsync(u => u.Id == id)).TeamsIn.Select(t => t.Team).OrderBy(t => t.Name);
            if (teams.Count<Team>() == 0)
                return null;
            return teams;
        }    
        
        //returns team by specified team.Id and its tables
        private async Task<Team> getTeamTables(int id){
            var team = (await _context.Teams.FirstOrDefaultAsync(t => t.Id == id));
            return team;
        }

        private async Task<IEnumerable<Team>> getTeamsOfOrga(int orgId){
            var teams = await _context.Teams.Where(o => o.OrganizationId.Equals(orgId)).ToListAsync();
            return teams;
        }

        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet("orga/{orgId}")]
        public async Task<ActionResult<IEnumerable<TeamDTO>>> getAllTeamsFromOrga(int orgId)
        {       
            //get the current user
            User current_user_backend = this.GetCurrentUser(_context);
            //check if you are connected
            if (current_user_backend == null)
                return BadRequest("You are not connected");
            var teams = await getTeamsOfOrga(orgId);
            foreach (var t in teams) {
                var tmp = await getTeam(t.Id);
                t.Id = tmp.Value.Id;
                t.Name = tmp.Value.Name;
                t.TablesTeam = tmp.Value.TablesTeam;
            }
            return teams.Select(team => new Team(){Id = team.Id, Name = team.Name, TablesTeam = team.TablesTeam
                .Select(ta => new Table(){Id = ta.Id, Title = ta.Title}).ToList()}).ToDTO();
        }


        //RETURN A TEAM WITH THEIR TABLES
        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet("team/{id}")]
        public async Task<ActionResult<TeamDTO>> getTeam(int id){
            //get the current user
            User current_user_backend = this.GetCurrentUser(_context);
            //check if you are connected
            if (current_user_backend == null)
                return BadRequest("You are not connected");
            var team = await getTeamTables(id);
            if (team == null || team.UsersIn.FirstOrDefault(u => u.UserId == current_user_backend.Id) == null)
                return NotFound();
            return new Team(){Id = team.Id, Name = team.Name, TablesTeam = team.TablesTeam.Select(ta => new Table(){Id = ta.Id, Title = ta.Title}).ToList()}.ToDTO();
        }

        //RETURN ???
        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet("user/{id}")]
        public async Task<ActionResult<IEnumerable<TeamDTO>>> getUserTableOfTeams(int id){
            //get the current user
            User current_user_backend = this.GetCurrentUser(_context);
            //check if you are connected
            if (current_user_backend == null)
                return BadRequest("You are not connected");
            var Team = await getTeamsTables(id);
            if (Team == null)
                return NotFound();
            return Team.Select(t => new Team(){Id = t.Id, Name = t.Name, TablesTeam = t.TablesTeam.Select(ta => new Table(){Id = ta.Id, Title = ta.Title}).ToList()}).ToDTO();
        }

        //RETURN ALL TEAMS
        [Authorized(Role.Admin, Role.Member, Role.Manager)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDTO>>> ReadAllTeams() 
        {
            //get the current user
            User current_user_backend = await _context.Users.FirstOrDefaultAsync(uu => uu.Pseudo == User.Identity.Name);
            //check if you are connected
            if (current_user_backend == null)
                return BadRequest("You are not connected");
            return (await _context.Teams.Select(t => new Team(){Id = t.Id, Name = t.Name}).ToListAsync()).ToDTO();
        }

        [Authorized(Role.Admin, Role.Member, Role.Manager)]
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateTeam(int id, TeamDTO TeamDTO) // ajouter regles metiers
        {
            //get the team we are working on
            var Team = await _context.Teams.FirstOrDefaultAsync(o => o.Id == id);
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("No user connected");
            //check if the user is in the team
            var found = 0;
            foreach (var act in Team.UsersIn)
            {
                if (act.UserId == currentUser.Id)
                {
                    found = 1;
                    break;
                }
            }
            if (found == 0)
                return BadRequest("Not allowed to modify this Team");
            //ADD MEMBER
            if(TeamDTO.Action == "add"){
                //check if user exist
                User u = await  _context.Users.FirstOrDefaultAsync(uu => uu.Pseudo == TeamDTO.MemberPseudo);
                if (u == null)
                    return NotFound();
                //check if the user is already in
                foreach (var act in Team.UsersIn)
                {
                    if (act.UserId == u.Id){
                        return BadRequest("User already in this team");
                    }
                }
                UserTeam tmp = new UserTeam();
                tmp.TeamId = Team.Id;
                tmp.UserId = u.Id;
                _context.UserTeam.Add(tmp);
                _context.Teams.Update(Team);
                await _context.SaveChangesAsyncWithValidation();
                return Ok(new User(){Pseudo=u.Pseudo, Id=u.Id, FirstName=u.FirstName, LastName=u.LastName});
            }
            //REMOVE MEMBER
            if (TeamDTO.Action == "remove"){
                //check if user exist
                User u = await  _context.Users.FirstOrDefaultAsync(uu => uu.Pseudo == TeamDTO.MemberPseudo);
                if (u == null)
                    return NotFound();
                //check if the user is in
                var f = 0;
                foreach (var act in Team.UsersIn)
                {
                    if (act.UserId == u.Id){
                        f = 1;
                    }
                }
                if (f == 0)
                    return BadRequest("User not in the team");
                UserTeam tmp = null;
                foreach(var ut in _context.UserTeam){
                    if (ut.TeamId == Team.Id && ut.UserId == u.Id){
                        tmp = ut;
                        break;
                    }
                }
                _context.UserTeam.Remove(tmp);
                await _context.SaveChangesAsyncWithValidation();
            }
            //Team.Name = TeamDTO.Name;

            //Team.UsersIn = TeamDTO.UsersIn;
            var res = await _context.SaveChangesAsyncWithValidation();
            if (!res.IsEmpty)
                return BadRequest(res);
            return NoContent();
        }
        
        [Authorized(Role.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            //get team
            var Team = await _context.Teams.FirstOrDefaultAsync(o => o.Id == id);
            if (Team == null)
                return NotFound();
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("No user connected");
            //check if user is part of the team
            if(!currentUser.Teams.Contains(Team))
                return BadRequest("Not allowed to delete this Team");
            //delete
            List<Table> tmp_table = new List<Table>();
            List<Section> tmp_section = new List<Section>();
            List<Card> tmp_card = new List<Card>();
            List<UserCard> tmp_usercard = new List<UserCard>();
            List<UserTable> tmp_usertable = new List<UserTable>();
            foreach(var table in Team.TablesTeam){
                foreach(var section in table.Sections){
                    foreach(var card in section.Cards){
                        tmp_card.Add(card);
                        tmp_usercard.AddRange(card.UsersIn);
                    }
                    tmp_section.Add(section);
                }
                tmp_table.Add(table);
                tmp_usertable.AddRange(table.UsersIn);
            }
            _context.UserCard.RemoveRange(tmp_usercard);
            _context.Cards.RemoveRange(tmp_card);
            _context.Sections.RemoveRange(tmp_section);
            
            _context.UserTable.RemoveRange(tmp_usertable);
            await _context.SaveChangesAsync();

            _context.Tables.RemoveRange(tmp_table);
            await _context.SaveChangesAsync();
            
            //remove all userteam relation
            List<UserTeam> tmp_userteam = new List<UserTeam>();
            foreach(var ut in _context.UserTeam){
                if (ut.TeamId == Team.Id)
                    tmp_userteam.Add(ut);
            }
            _context.UserTeam.RemoveRange(tmp_userteam);
            await _context.SaveChangesAsync();

            _context.Teams.Remove(Team);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}