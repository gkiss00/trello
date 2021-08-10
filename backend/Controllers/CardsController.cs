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
using prid_2021_a06.Controllers;

namespace prid_2021_a06.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase {
        private readonly PridContext _context;

        public CardsController(PridContext context) {
            _context = context;
        }

        [Authorized(Role.Admin, Role.Manager, Role.Member)]
        [HttpPost]
        public async Task<ActionResult<Card>> CreateCard(CardDTO data) // ajouter les regles metiers
        {
            //get the current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("You are not connected");
            var newCard = new Card();
            newCard.Title = data.Title;
            newCard.Content = data.Content != null ? data.Content : "";
            if(data.Section != null){
                newCard.Section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == data.Section.Id);
            }
            newCard.SectionId = data.SectionId;
            newCard.Position = data.Position; 
            newCard.AuthorId = data.AuthorId;                                      

            _context.Cards.Add(newCard);                                            
            var res = await _context.SaveChangesAsyncWithValidation();
            if (!res.IsEmpty)
                return BadRequest(res);

            var card = new Card(){Id = newCard.Id, Title = newCard.Title, Content = newCard.Content};

            return Ok(card);
        }

        [Authorized(Role.Admin, Role.Manager, Role.Member)]
        [HttpGet("{id}")]
        public async Task<ActionResult<CardDTO>> ReadCard(int id)
        {
            //get the current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("You are not connected");
            //get the card
            var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == id);
            if (card == null)
                return NotFound();
            //return the card
            return new CardDTO(){Id = card.Id, Title = card.Title, Content = card.Content, 
                Author = new User(){Id = card.Author.Id, Pseudo = card.Author.Pseudo},
                Section = new Section(){Title = card.Section.Title}, UsersIn = card.UsersIn.
                OrderBy(u => u.User.Id != currentUser.Id).ThenBy(u => u.UserId != card.Section.Table.Owner.Id).
                ThenBy(u => u.User.Pseudo).Select(u => new User(){Id = u.User.Id, Pseudo = u.User.Pseudo}).ToList()};
        }

        [Authorized(Role.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CardDTO>>> ReadAllCards() 
        {
            //get the current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("You are not connected");
            return (await _context.Cards.ToListAsync()).ToDTO();
        }

        [Authorized(Role.Admin, Role.Manager, Role.Member)]
        [HttpPut("{id}")]
        public async Task<ActionResult<Card>> UpdateCard(CardDTO data, int id) // ajouter regles metiers
        {
            //get the current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("You are not connected");
            //get card
            var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == id);
            if (card == null)
                return BadRequest("No card");
            //check for the permission
            if (!currentUser.Tables.Contains(card.Section.Table))
                return BadRequest("You are not allowed to do that");

            var tempCard = new Card(){Id = card.Id, Position = card.Position, SectionId = card.SectionId};
            IList<Card> updateCardsList = new List<Card>();
            UserCard member = new UserCard();

            if(data.Section != null){
                card.Section = data.Section;
            } if(data.SectionId != null){
                card.SectionId = data.SectionId;
                updateCardsList = await updateSectionsCardsPositions(card, tempCard, data);
            } else {
                updateCardsList = await updateSectionCardsPositions(data, tempCard);
            } if(data.Position != null){
                card.Position = data.Position;
            } if(data.Title != null){
                card.Title = data.Title;
            } if(data.Content != null){
                card.Content = data.Content;
            } if(data.userId != null && data.Action == "Add"){
                member.CardId = card.Id;
                member.UserId = data.userId ?? default(int);
                _context.UserCard.Add(member);
            } else if(data.userId != null && data.Action == "Remove"){
                member.CardId = card.Id;
                member.UserId = data.userId ?? default(int);
                _context.UserCard.Remove(member);
            }

            updateCardsList.Add(card);
            _context.Cards.UpdateRange(updateCardsList);
            var res = await _context.SaveChangesAsyncWithValidation();
            if (!res.IsEmpty)
                return BadRequest(res);

            var updatedCard = new Card(){Id = card.Id, Title = card.Title, Content = card.Content};

            return Ok(updatedCard);
        }

        // met à jour les positions des cartes des sections lorsqu'une carte est déplacée d'une à l'autre
        private async Task<IList<Card>> updateSectionsCardsPositions(Card card, Card tempCard, CardDTO data){
            var sectionListCardRemoved = await _context.Cards.Where(c => c.SectionId == tempCard.SectionId && c.Position > tempCard.Position).ToListAsync();
            sectionListCardRemoved.ForEach(c => c.Position -= 1);
            var sectionListCardAdded = await _context.Cards.Where(c => c.SectionId == card.SectionId && c.Position >= data.Position).ToListAsync();
            sectionListCardAdded.ForEach(c => c.Position += 1);
            sectionListCardAdded.AddRange(sectionListCardRemoved);
            return sectionListCardRemoved;
        }

        private async Task<IList<Card>> updateSectionCardsPositions(CardDTO data, Card tempCard){
            List<Card> section;
            if(tempCard.Position > data.Position){
                section = await _context.Cards.Where(c => c.SectionId == tempCard.SectionId && c.Position >= data.Position && c.Position < tempCard.Position).ToListAsync();
                section.ForEach(c => c.Position++);
            } else {
                section = await _context.Cards.Where(c => c.SectionId == tempCard.SectionId && c.Position <= data.Position && c.Position > tempCard.Position).ToListAsync();
                section.ForEach(c => c.Position--);
            }
            return section;
        }

        [Authorized(Role.Member, Role.Manager, Role.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            //get the current user
            var currentUser = this.GetCurrentUser(_context);
            if (currentUser == null)
                return BadRequest("You are not connected");
            //get card
            var card = await _context.Cards.FirstOrDefaultAsync(c => c.Id == id);
            if (card == null)
                return BadRequest("No card");
            //check for the permission
            if (!currentUser.Tables.Contains(card.Section.Table))
                return BadRequest("You are not allowed to do that");
            //delete
            List<UserCard> tmp_usercard = new List<UserCard>();
            tmp_usercard.AddRange(card.UsersIn);
            _context.UserCard.RemoveRange(tmp_usercard);
            await _context.SaveChangesAsync();
            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
