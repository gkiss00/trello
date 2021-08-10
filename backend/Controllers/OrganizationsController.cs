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
    public class OrganizationsController : ControllerBase {
        private readonly PridContext _context;

        public OrganizationsController(PridContext context) {
            _context = context;
        }

        //create organization
        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpPost]
        public async Task<ActionResult<OrganizationDTO>> CreateOrganization(OrganizationDTO data) // ajouter les regles metiers
        {            
          
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("No user connected");
            if (data.Name == null)
                return BadRequest(new ValidationErrors().Add("Title is not filled", "Title"));
            //creating with simple datas
            var newOrga = new Organization();
            newOrga.Name = data.Name;
            newOrga.Teams = new List<Team>();
            newOrga.UsersIn = new List<UserOrganization>();
            _context.Organizations.Add(newOrga);
            var res = await _context.SaveChangesAsyncWithValidation();
            if (!res.IsEmpty)
                return BadRequest(res);

            newOrga = this.getOrganizationById(newOrga.Id);
        
            //putting all users to add into one var as they're from teams but also from a list potentially created by the user
            List<User> usersToAdd = new List<User>();
            if (data.Teams != null && data.Teams.Count != 0)
            {
                foreach(Team t in data.Teams){
                    newOrga.Teams.Add(this.getTeamById(t.Id));
                    var members = this.getMembersByTeamId(t.Id);
                    usersToAdd.AddRange(members);
                }
            }

            // Adding users to the organization
            if (data.UserList != null)
            {
                foreach(string p in data.UserList)
                    usersToAdd.Add(this.getUserByPseudo(p));
            }
            usersToAdd = this.distinct(usersToAdd);
            foreach(User u in usersToAdd)
                newOrga.UsersIn.Add(new UserOrganization(){UserId = u.Id, OrganizationId = newOrga.Id});
            _context.Organizations.Update(newOrga);
            var res2 = await _context.SaveChangesAsyncWithValidation();
            if (!res2.IsEmpty)
                return BadRequest(res2);
            var orga = new Organization(){ Id = newOrga.Id, Name = newOrga.Name};
            return orga.ToDTO();
        }

        private Team getTeamById(int id)
        {
            Team team = new Team();
            team = _context.Teams.FirstOrDefault(t => t.Id.Equals(id));
            return team;
        }

        private Organization getOrganizationById(int id)
        {
            Organization orga = new Organization();
            orga = _context.Organizations.FirstOrDefault(o => o.Id.Equals(id));
            return orga;
        }     

        private User getUserByPseudo(string p)
        {
            User res = _context.Users.FirstOrDefault(u => u.Pseudo.Equals(p));
            return res;
        }

        private User getUserById(int id)
        {
            User res = _context.Users.FirstOrDefault(u => u.Id.Equals(id));
            return res;
        }

        private List<User> getMembersByTeamId(int id)
        {
            List<User> res = new List<User>();
            var teams = _context.UserTeam.Where(u => u.TeamId.Equals(id)).ToList();
            foreach(UserTeam t in teams)
            {
                res.Add(getUserById(t.UserId));
            }
            return res;
        }
        
        private List<User> distinct(List<User> users)
        {
            List<User> res = new List<User>();
            foreach (User u in users)
            {
                bool found = false;
                foreach (User r in res)
                {
                    if (r.Pseudo.CompareTo(u.Pseudo) == 0)
                        found = true;
                }
                if (found == false)
                    res.Add(u);
            }
            return (res);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrganizationDTO>> ReadOrganization(int id)
        {
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("No user connected");
            var Organization = await _context.Organizations.FirstOrDefaultAsync(s => s.Id == id);
            if (Organization == null)
                return NotFound();
            return new Organization(){Id = Organization.Id, Name = Organization.Name}.ToDTO();
        }

        [Authorized(Role.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrganizationDTO>>> ReadAllOrganizations() 
        {
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("No user connected");
            return (await _context.Organizations.ToListAsync()).ToDTO();
        }

        //get all the organizations the user is in
        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet("user/{id}/organizations")]
        public async Task<ActionResult<IEnumerable<OrganizationDTO>>> getUserOrganizations(int id)
        {
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("No user connected");
            var orga = await getOrganizationsTables(id);
            if (orga == null)
                return NotFound();
            return orga.Select(o => new Organization(){Id = o.Id, Name = o.Name}).ToDTO();
        }

        private async Task<IEnumerable<Organization>> getOrganizationsTables(int id){
            var orga = (await _context.Users.FirstOrDefaultAsync(u => u.Id == id)).OrganizationsIn.Select(o => o.Organization).OrderBy(o => o.Name);
            if (orga.Count<Organization>() == 0)
                return null;
            return orga;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrganization(int id, OrganizationDTO OrganizationDTO) // ajouter regles metiers
        {
            var Organization = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == id);

            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("No user connected");

            var found = 0;
            foreach (var act in Organization.UsersIn)
            {
                if (act.UserId == currentUser.Id)
                {
                    found = 1;
                    break;
                }
            }
            if (found == 0)
                return BadRequest("Not allowed to modify this Organization");
            else
            {
                Organization.Name = OrganizationDTO.Name;
                Organization.UsersIn = OrganizationDTO.UsersIn;
            }
            var res = await _context.SaveChangesAsyncWithValidation();
            if (!res.IsEmpty)
                return BadRequest(res);
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrganization(int id)
        {
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("No user connected");
            var query = _context.UserOrganization.FirstOrDefault(uo => uo.OrganizationId.Equals(id) && uo.UserId.Equals(currentUser.Id));
            if (query == null)
                return BadRequest("Not allowed to delete this organization");
            var uo = _context.UserOrganization
                .Where(uo => uo.OrganizationId.Equals(id)).ToList();
            foreach(var u in uo)
                _context.UserOrganization.Remove(u);

            var teams = _context.Teams.Where(t => t.OrganizationId.Equals(id)).ToList();
            foreach(var t in teams)
            {
                t.OrganizationId = null;
                _context.Teams.Update(t);
            }

            var Organization = _context.Organizations.FirstOrDefault(u => u.Id.Equals(id));
            _context.Organizations.Remove(Organization);
            var res = await _context.SaveChangesAsyncWithValidation();
            if (!res.IsEmpty)
                return BadRequest(res);
            return Ok();
        }


        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet("{id}/members")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> getOrganizationMembers(int id)
        {            
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("No user connected");
            var org = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == id);
            if (org == null)
                return NotFound();
            return org.Users.Select(u => new User(){Id = u.Id, Pseudo = u.Pseudo}).ToDTO();
        }


        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet("{id}/teams")]
        public async Task<ActionResult<IEnumerable<TeamDTO>>> getOrganizationTeams(int id)
        {        
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("No user connected");
            var teams = await _context.Teams.Where(t => t.OrganizationId.Equals(id)).OrderBy(t => t.Name).ToListAsync();
            if (teams == null)
                return NotFound();
            return teams.Select(t => new Team(){Id = t.Id, Name = t.Name}).ToDTO();
        }        
        
        // get all the teams that can be added to an organization (those who are already in are not selected)
        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet("{orgId}/teams_addable")]
        public async Task<ActionResult<IEnumerable<TeamDTO>>> getTeamsAddable(int orgId)
        {        
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("No user connected");
            var teams = await _context.Teams.Where(t => t.OrganizationId != orgId).OrderBy(t => t.Name).ToListAsync();
            if (teams == null)
                return NotFound();
            return teams.Select(t => new Team(){Id = t.Id, Name = t.Name}).ToDTO();
        }


        // remove user from organization
        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet("rem_user/{pseudo}/{id}")]
        public async Task<IActionResult> removeUser(string pseudo, int id)
        {        
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("No user connected");
            var entit = _context.UserOrganization
                .Where(uo => uo.User.Pseudo.Equals(pseudo))
                .FirstOrDefault(uo => uo.OrganizationId.Equals(id));
            if (entit == null)
                return BadRequest();
            var user = getUserByPseudo(pseudo);
            var uo = await _context.UserOrganization
                .FirstOrDefaultAsync(uo => uo.UserId.Equals(user.Id) && uo.OrganizationId.Equals(id));
            _context.UserOrganization.Remove(uo);
            var res = await _context.SaveChangesAsyncWithValidation();
            if (!res.IsEmpty)
                return BadRequest(res);
            return Ok();
        }

        // remove team from organization
        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet("{orgId}/{teamId}")]
        public async Task<IActionResult> removeTeam(int orgId, int teamId)
        {         
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("No user connected");
            List<UserTeam> usersToDelete = new List<UserTeam>();
            usersToDelete = _context.UserTeam.Where(t => t.TeamId.Equals(teamId)).ToList();
            List<UserOrganization> usersInOrga = new List<UserOrganization>();
            usersInOrga = _context.UserOrganization.Where(ut => ut.OrganizationId.Equals(orgId)).ToList();

            var team = _context.Teams.FirstOrDefault(t => t.Id.Equals(teamId));
            team.OrganizationId = null;
            _context.Teams.Update(team);  
            var orga = _context.Organizations.FirstOrDefault(o => o.Id.Equals(orgId));

            foreach(UserOrganization uo in usersInOrga)
            {
                if (usersToDelete.Any(u => u.UserId.Equals(uo.UserId)))
                {
                    var isExisting = _context.UserTeam.Where(ut => (ut.UserId.Equals(uo.UserId) && ut.TeamId != teamId)).ToList();

                    List<UserTeam> cpy = new List<UserTeam>(isExisting.Count());
                    isExisting.ForEach(el => {
                        cpy.Add(new UserTeam(el));
                    });
                    foreach(UserTeam us in cpy)
                    {
                        if (orga.Teams.Any(t => t.Id.Equals(us.TeamId)))
                            NoContent();
                        else 
                            isExisting.RemoveAll(ie => ie.UserId == us.UserId && ie.TeamId == us.TeamId);
                    }
                    if (isExisting.Count() == 0) 
                        _context.UserOrganization.Remove(uo);
                }
            }
            
            var res = await _context.SaveChangesAsyncWithValidation();
            if (!res.IsEmpty)
                return BadRequest(res);

            usersInOrga = _context.UserOrganization.Where(ut => ut.OrganizationId.Equals(orgId)).ToList();
            if (usersInOrga == null)
            {
                _context.Organizations.Remove(orga);
            } 
            var res2 = await _context.SaveChangesAsyncWithValidation();
            if (!res2.IsEmpty)
                return BadRequest(res2);
            return Ok();
        }

        // add a user to an organization
        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet("add/{orgId}/{userId}")]
        public async Task<ActionResult<UserDTO>> addUserToOrga(int orgId, int userId)
        {        
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("No user connected");
            User user = _context.Users.FirstOrDefault(u => u.Id.Equals(userId));
            Organization orga = _context.Organizations.FirstOrDefault(o => o.Id.Equals(orgId));

            UserOrganization uo = new UserOrganization(){User = user, UserId = userId, Organization = orga, OrganizationId = orgId};
            _context.UserOrganization.Add(uo);
            var res = await _context.SaveChangesAsyncWithValidation();
            if (!res.IsEmpty)
                return NotFound();
            return new User(){Id = user.Id, Pseudo = user.Pseudo}.ToDTO();
        }        
        
        // add team to an organization
        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet("{orgId}/add_team/{teamId}")]
        public async Task<ActionResult<TeamDTO>> addTeamToOrga(int orgId, int teamId)
        {        
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("No user connected");
            Team t = _context.Teams.FirstOrDefault(t => t.Id.Equals(teamId));
            t.OrganizationId = orgId;

            Organization orga = _context.Organizations.FirstOrDefault(o => o.Id.Equals(orgId));
            orga.Teams.Add(this.getTeamById(t.Id));
            var members = this.getMembersByTeamId(t.Id);
            foreach(User u in members) {
                if (orga.UsersIn.Any(uo => uo.UserId.Equals(u.Id)))
                    NoContent();
                else
                    orga.UsersIn.Add(new UserOrganization(){UserId = u.Id, OrganizationId = orga.Id});
            }
            _context.Update(t);
            _context.Update(orga);
            var res = await _context.SaveChangesAsyncWithValidation();
            if (!res.IsEmpty)
                return NotFound();
            return Ok();
        }
    }
}