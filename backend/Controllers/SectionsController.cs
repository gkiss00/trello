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
    public class SectionsController : ControllerBase {
        private readonly PridContext _context;

        public SectionsController(PridContext context) {
            _context = context;
        }

        [Authorized(Role.Admin, Role.Manager, Role.Member)]
        [HttpPost]
        public async Task<ActionResult<Section>> CreateSection(SectionDTO data) // ajouter les regles metiers
        {
            //get the current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("You are not connected");
            //check for the permission
            var tableTmp = await _context.Tables.FirstOrDefaultAsync(t => t.Id == data.TableId);
            if(tableTmp == null)
                return NotFound();

            if (!currentUser.Tables.Contains(tableTmp))
                return BadRequest("You are not allowed to do that");
            //post
            var newSection = new Section();
            newSection.Title = data.Title;
            newSection.TableId = data.TableId;

            _context.Sections.Add(newSection);
            var res = await _context.SaveChangesAsyncWithValidation();
            if (!res.IsEmpty)
                return BadRequest(res);

            var section = new Section(){Id = newSection.Id, Title = newSection.Title};

            return Ok(section);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SectionDTO>> ReadSection(int id)
        {
            //get the current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("You are not connected");
            //get section
            var section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == id);
            if (section == null)
                return NotFound();
            //check for the permission
            if (!currentUser.Tables.Contains(section.Table))
                return BadRequest("You are not allowed to do that");
            return section.ToDTO();
        }

        [Authorized(Role.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SectionDTO>>> ReadAllSections() 
        {
            //get the current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("You are not connected");
            //check for the permission
            if (currentUser.Role != Role.Admin)
                 return BadRequest("You are not allowed to do tht");
            return (await _context.Sections.ToListAsync()).ToDTO();
        }

        [Authorized(Role.Admin, Role.Manager, Role.Member)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSection(int id, SectionDTO sectionDTO)// ajouter regles metiers
        {
            //get the current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("You are not connected");
            //get the section
            var section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == id);
            if(section == null)
                return NotFound();
            //check the permission
            if (!currentUser.Tables.Contains(section.Table))
                return BadRequest("Not allowed to modify this Section");
            //update
            section.Title = sectionDTO.Title;
            var res = await _context.SaveChangesAsyncWithValidation();
            if (!res.IsEmpty)
                return BadRequest(res);
            return NoContent();
        }
        
        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSection(int id)
        {
            
            //get the current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                    return BadRequest("You are not connected");
            //get the section
            var section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == id);
            if(section == null)
                return NotFound();
            //check the permission
            if (!currentUser.Tables.Contains(section.Table))
                return BadRequest("Not allowed to modify this Section");

            List<Card> tmp_card = new List<Card>();
            List<UserCard> tmp_usercard = new List<UserCard>();
            foreach(var card in section.Cards){
                tmp_card.Add(card);
                tmp_usercard.AddRange(card.UsersIn);
            }
            _context.UserCard.RemoveRange(tmp_usercard);
            await _context.SaveChangesAsync();
            _context.Cards.RemoveRange(tmp_card);
            await _context.SaveChangesAsync();

            //delete
            _context.Sections.Remove(section);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
