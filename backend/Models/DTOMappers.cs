using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace prid_2021_a06.Models {
    public static class DTOMappers {
        
        public static UserDTO ToDTO(this User user) {
            return new UserDTO {
                Id = user.Id,
                Pseudo = user.Pseudo,
                // we don't put the password in the DTO for security reasons
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Role = user.Role,
                TeamsIn = user.TeamsIn,
                OrganizationsIn = user.OrganizationsIn,
                TablesOwned = user.TablesOwned,
                TablesIn = user.TablesIn,
                CardsIn = user.CardsIn
            };
        }

        public static CardDTO ToDTO(this Card card) {
            return new CardDTO {
                Id = card.Id,
                Title = card.Title,
                Content = card.Content,
                Author = card.Author,
                AuthorId = card.AuthorId,
                Position = card.Position,
                Section = card.Section,
                SectionId = card.SectionId,
                UsersIn = card.Users.ToList(),
            };
        }

        public static OrganizationDTO ToDTO(this Organization org) {
            return new OrganizationDTO {
                Id = org.Id,
                Name = org.Name,
                UsersIn = org.UsersIn,
            };
        }

        public static SectionDTO ToDTO(this Section sec) {
            return new SectionDTO {
                Id = sec.Id,
                Title = sec.Title,
                Table = sec.Table,
                Cards = sec.Cards.ToDTO(),
            };
        }

        public static TableDTO ToDTO(this Table table) {
            return new TableDTO {
                Id = table.Id,
                Title = table.Title,
                Owner = table.Owner,
                View = table.View,
                Sections = table.Sections.ToDTO(),
                UsersIn = table.Users,
                Team = table.Team,
            };
        }

        public static TeamDTO ToDTO(this Team team) {
            return new TeamDTO {
                Id = team.Id,
                Name = team.Name,
                UsersIn = team.UsersIn,
                TablesTeam = team.TablesTeam,
                OrganizationId = team.OrganizationId
            };
        }

        public static List<UserDTO> ToDTO(this IEnumerable<User> users) {
            return users.Select(m => m.ToDTO()).ToList();
        }

        public static List<CardDTO> ToDTO(this IEnumerable<Card> cards) {
            return cards.Select(c => c.ToDTO()).ToList();
        }

        public static List<OrganizationDTO> ToDTO(this IEnumerable<Organization> orgs) {
            return orgs.Select(o => o.ToDTO()).ToList();
        }

        public static List<SectionDTO> ToDTO(this IEnumerable<Section> secs) {
            return secs.Select(s => s.ToDTO()).ToList();
        }

        public static List<TableDTO> ToDTO(this IEnumerable<Table> tables) {
            return tables.Select(t => t.ToDTO()).ToList();
        }

        public static List<TeamDTO> ToDTO(this IEnumerable<Team> teams) {
            return teams.Select(t => t.ToDTO()).ToList();
        }
    }
}
