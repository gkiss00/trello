using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using prid_2021_a06.Models;
using PRID_Framework;
using Microsoft.AspNetCore.Authorization;
using prid_2021_a06.Helpers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Collections;

namespace prid_2021_a06.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TablesController : ControllerBase {
        private readonly PridContext _context;

        public TablesController(PridContext context) {
            _context = context;
        }
        
        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpPost]
        public async Task<ActionResult<TableDTO>> createTableTeam(TableDTO data)
        {
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("You are not connected");
            //check data
            if (data.Title == null)
                return BadRequest(new ValidationErrors().Add("Title is not filled", "Title"));
            //create
            var newTable = new Table();
            newTable.Title = data.Title;
            newTable.Owner = currentUser;
            if (data.teamId != null)
            {
                newTable.View = View.Team;
                newTable.Team = this.getTeam(data.teamId);
            }
            else if (data.organizationId != null)
            {
                newTable.View = View.Organization;
                newTable.Organization = this.getOrganization(data.organizationId);
            }
            else
                newTable.View = View.Private;
            newTable.Sections = null;
            newTable.UsersIn = new List<UserTable>();
            _context.Tables.Add(newTable);
            var res = await _context.SaveChangesAsyncWithValidation();
            if (!res.IsEmpty)
                return BadRequest(res);

            newTable = this.getTableById(newTable.Id);
            List<User> usersToAdd = new List<User>();
            if (data.organizationId != null || (data.usersInString != null && data.usersInString.Length >= 1))
            {
                if (data.organizationId != null) {
                    usersToAdd = this.getUsersFromOrga(data.organizationId);
                }
                else {
                    foreach(string p in data.usersInString) {
                        usersToAdd.Add(this.getUserByPseudo(p));
                    }
                }
            }
            else {
                usersToAdd = getUsersFromTeam(data.teamId);
            }
            if (data.organizationId == null && data.teamId == null)
                usersToAdd.Add(this.GetCurrentUser(_context));
            
            foreach(User u in usersToAdd) {
                    newTable.UsersIn.Add(new UserTable(){UserId = u.Id, TableId = newTable.Id});
            }
            _context.Tables.Update(newTable);
            var res2 = await _context.SaveChangesAsyncWithValidation();
            if (!res2.IsEmpty)
                return BadRequest(res);
            var table = new Table(){ Id = newTable.Id, Title = newTable.Title};
            return table.ToDTO();
        }

        private Table getTableById(int id)
        {
            Table tab = new Table();
            tab = _context.Tables.FirstOrDefault(t => t.Id.Equals(id));
            return tab;
        }

        private List<User> getUsersFromOrga(int? id)
        {
            List<User> res = new List<User>();
            var orga = _context.UserOrganization.Where(o => o.OrganizationId.Equals(id)).ToList();
            foreach (UserOrganization o in orga)
            {
                res.Add(getUserById(o.UserId));
            }
            return res;
        }

        private List<User> getUsersFromTeam(int? id)
        {
            List<User> res = new List<User>();
            var teams = _context.UserTeam.Where(u => u.TeamId.Equals(id)).ToList();
            foreach(UserTeam t in teams)
            {
                res.Add(getUserById(t.UserId));
            }
            return res;
        }
        
        private User getUserByPseudo(string p)
        {
            User res = _context.Users.FirstOrDefault(u => u.Pseudo.Equals(p));
            return res;
        }

        private Team getTeam(int? id)
        {
            var team = _context.Teams.FirstOrDefault(t => t.Id.Equals(id));
            return team;
        }

        private Organization getOrganization(int? id) 
        {
            var organization = _context.Organizations.FirstOrDefault(o => o.Id.Equals(id));
            return organization;
        }

        private User getUserById(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id.Equals(id));
            return user;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TableDTO>> ReadTable(int id)
        {
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("You are not connected");
            //get table
            var Table = await _context.Tables.FirstOrDefaultAsync(s => s.Id == id);
            if (Table == null)
                return NotFound();
            //check permission
            if (!currentUser.Tables.Contains(Table))
                return BadRequest("Permission denied");
            //get
            return Table.ToDTO();
        }

        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet("user/{id}")]
        public async Task<ActionResult<IEnumerable<TableDTO>>> getUserTables(int id)
        {
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("You are not connected");
            //check for permission
            if (currentUser.Id != id)
                return BadRequest("Permission denied");
            var Table = (await _context.Users.FirstOrDefaultAsync(u => u.Id == id)).TablesIn.Select(t => t.Table);
            if (Table.Count<Table>() == 0)
                return NotFound();
            //getAll;
            return Table.Select(t => new Table(){Id = t.Id, Title = t.Title}).ToDTO();
        }

        [Authorized(Role.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TableDTO>>> ReadAllTables() 
        {
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("You are not connected");
            //check permissions
            if (currentUser.Role != Role.Admin)
                return BadRequest("Permission denied");
            //get
            return (await _context.Tables.ToListAsync()).ToDTO();
        }

        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpGet("user/table/{id}")]
        public async Task<ActionResult<TableDTO>> getUserTableContentById(int id){
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("You are not connected");
            //get table
            var table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == id);
            if (table == null)
                return NotFound();
            //check permission
            if (!currentUser.Tables.Contains(table))
                return BadRequest("Permission denied");
            //get
            return new Table(){Id = table.Id, Title = table.Title, Owner = new User(){Id = table.Owner.Id, Pseudo = table.Owner.Pseudo}, 
                    Users = table.UsersIn.Where(u => u.UserId != table.Owner.Id && u.UserId != currentUser.Id).OrderBy(u => u.User.Pseudo).Select(u => new User(){Id = u.User.Id, Pseudo = u.User.Pseudo}).ToList(), 
                    Sections = table.Sections.Select(s => new Section(){Id = s.Id, Title = s.Title, 
                    Cards = s.Cards.OrderBy(c => c.Position).Select(c => new Card(){Id = c.Id, Title = c.Title, UsersIn = c.UsersIn.
                Select(u => new UserCard(){User = new User(){Id = u.User.Id, Pseudo = u.User.Pseudo}}).
                OrderBy(u => u.User.Id != currentUser.Id).ThenBy(u => u.User.Id != table.Owner.Id).
                ThenBy(u => u.User.Pseudo).ToList()}).ToList()}
                ).ToList()
            }.ToDTO();
        }

        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpPut("{id}")]
        public async Task<ActionResult<Table>> UpdateTable(int id, TableDTO data) // ajouter regles metiers
        {
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("You are not connected");
            //get table
            var table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == id);
            if (table == null)
                return NotFound();
            //check permission
            if (!currentUser.Tables.Contains(table))
                return BadRequest("Permission denied");
            //put
            List<UserTable> usersIn = new List<UserTable>();

            if(data.Title != null && data.Title != ""){
                table.Title = data.Title;
            } if(data.UserId != null && data.Action == "Add"){
                UserTable tmp = new UserTable();
                tmp.UserId = data.UserId ?? default(int);
                tmp.TableId = table.Id;
                usersIn.Add(tmp);
                await _context.UserTable.AddRangeAsync(usersIn);
            } else if(data.UserId != null && data.Action == "Remove"){
                UserTable tmp = await _context.UserTable.FirstOrDefaultAsync(ut => ut.UserId == data.UserId);
                List<UserCard> tmp_usercard = new List<UserCard>();
                foreach(var section in table.Sections){
                    foreach(var card in section.Cards){
                        var tmpUser = card.UsersIn.FirstOrDefault(u => u.UserId == tmp.UserId);
                        if(tmpUser != null){
                            tmp_usercard.Add(tmpUser);
                        }
                    }
                }
                usersIn.Add(tmp);
                _context.UserTable.RemoveRange(usersIn);
                await _context.SaveChangesAsync();
                _context.UserCard.RemoveRange(tmp_usercard);
                await _context.SaveChangesAsync();
            }
            table.View = data.View;

            _context.Tables.Update(table);
            var res = await _context.SaveChangesAsyncWithValidation();
            if (!res.IsEmpty)
                return BadRequest(res);

            Table newTable = new Table(){Id = table.Id, Title = table.Title};
            return Ok(newTable);
        }
        
        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            //get current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("You are not connected");
            //get table
            var table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == id);
            if (table == null)
                return NotFound();
            //check permission
            if (!currentUser.Tables.Contains(table))
                return BadRequest("Permission denied");
            //delete
            List<Section> tmp_section = new List<Section>();
            List<Card> tmp_card = new List<Card>();
            List<UserCard> tmp_usercard = new List<UserCard>();
            foreach(var section in table.Sections){
                foreach(var card in section.Cards){
                    tmp_card.Add(card);
                    tmp_usercard.AddRange(card.UsersIn);
                }
                tmp_section.Add(section);
            }
            _context.UserCard.RemoveRange(tmp_usercard);
            _context.Cards.RemoveRange(tmp_card);
            _context.Sections.RemoveRange(tmp_section);

            //remove all userTable
            List<UserTable> tmp_usertable = new List<UserTable>();
            tmp_usertable.AddRange(table.UsersIn);
            
            _context.UserTable.RemoveRange(tmp_usertable);
            await _context.SaveChangesAsync();

            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}