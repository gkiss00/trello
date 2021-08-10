using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace prid_2021_a06.Models {
    public class PridContext : DbContext {
        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<UserOrganization> UserOrganization {get; set; }
        public DbSet<UserTeam> UserTeam { get; set; }
        public DbSet<UserTable> UserTable { get; set; }
        public DbSet<UserCard> UserCard { get; set; }

        public PridContext(DbContextOptions<PridContext> options)
            : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Pseudo)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            
            ConfigureForeignKey(modelBuilder);
            AddData(modelBuilder);
        }

        private void ConfigureForeignKey(ModelBuilder modelBuilder)
        {
            ConfigUserTable(modelBuilder);
            ConfigUserCard(modelBuilder);
            ConfigUserTeam(modelBuilder);
            ConfigUserOrganization(modelBuilder);
        }

        private void AddData(ModelBuilder modelBuilder) {
            Hasher hasher = new Hasher();
            var admin = new User() { Id = 1, Pseudo = "admin", Password = hasher.Hash("admin"), Email = "Admin@epfc.eu", FirstName = "Ad", LastName = "Min", Role = Role.Admin };
            var ben = new User() { Id = 2, Pseudo = "ben", Password = hasher.Hash("ben"), Email = "Benoit@epfc.eu", FirstName = "Benoit", LastName = "Penelle"};
            var bruno = new User() { Id = 3, Pseudo = "bruno", Password = hasher.Hash("bruno"), Email = "Bruno@epfc.eu", FirstName = "Bruno", LastName = "Lacroix" };
            var gautier = new User() { Id = 4, Pseudo = "gautier", Password = hasher.Hash("gautier"), Email = "Gautier@epfc.eu", FirstName = "Gautier", LastName = "Kiss"};
            var quentin = new User() { Id = 5, Pseudo = "quentin", Password = hasher.Hash("quentin"), Email = "Quentin@epfc.eu", FirstName = "Quentin", LastName = "Locht", Role = Role.Admin };
            
            modelBuilder.Entity<User>().HasData(
                admin, ben, bruno, gautier, quentin
            );


            //TEAMS
            Team one = new Team() { Id = 1, Name = "Projekt-Team" };
            Team two = new Team() { Id = 2, Name = "A-Team" };

            //BOARDS
            Table first = new Table() { Id = 1, UserId = quentin.Id, Title = "Club de pétanque", View = View.Public };
            Table second = new Table() { Id = 2, UserId = gautier.Id, Title = "Club des nudistes fous", View = View.Private };
            Table third = new Table() { Id = 3, UserId = admin.Id, Title = "App de rageux", TeamId = one.Id, View = View.Team };
            Table fourth = new Table() { Id = 4, UserId = ben.Id, Title = "Call auf duty", TeamId = two.Id, View = View.Organization };
            Table fifth = new Table() { Id = 5, UserId = bruno.Id, Title = "Stellar astronomy", TeamId = one.Id, View = View.Team };

            //SECTION FOR ASTRONOMIE TABLE
            Section sectionOne = new Section() {Id = 1, Title = "Backlog", TableId = fifth.Id};
            Section sectionTwo = new Section() {Id = 2, Title = "Dev in progress", TableId = fifth.Id};
            Section sectionThree = new Section() {Id = 3, Title = "To test", TableId = fifth.Id};
            Section sectionFour =  new Section() {Id = 4, Title = "Testing in progress", TableId = fifth.Id};
            Section sectionFive = new Section() {Id = 5, Title = "Ready for release", TableId = fifth.Id};
            Section sectionSix = new Section() {Id = 6, Title = "Release in progress", TableId = fifth.Id};

            //SECTION FOR CALL OF DUTY TABLE
            Section sectionSeven = new Section() {Id = 7, Title = "Things to do", TableId = fourth.Id};
            Section sectionEight = new Section() {Id = 8, Title = "In progress", TableId = fourth.Id};
            Section sectionNine = new Section() {Id = 9, Title = "Completed", TableId = fourth.Id};
            Section sectionTen = new Section() {Id = 10, Title = "Bug Fixing", TableId = fourth.Id};
            Section sectionEleven = new Section() {Id = 11, Title = "Bug fixed", TableId = fourth.Id};

            //CARDS FOR ASTRONOMIE TABLE
            Card cardOne = new Card() {Id = 1, AuthorId = admin.Id, Title = "Stars: what makes up a star? Nucleosynthesis? HR Diagrams...what happens in the life of a star?", Content = "U ll find it by yourself", SectionId = sectionOne.Id, Position = 0};
            Card cardTwo = new Card() {Id = 2, AuthorId = bruno.Id, Title = "Planets", Content = "Uranus", SectionId = sectionOne.Id, Position = 1};
            Card cardThree = new Card() {Id = 3, AuthorId = bruno.Id, Title = "Dark Matter / Dark Energy", Content = "Dark Sidious", SectionId = sectionTwo.Id, Position = 0};
            Card cardFour = new Card() {Id = 4, AuthorId = ben.Id, Title = "SETI", Content = "ITES", SectionId = sectionThree.Id, Position = 0};
            Card cardFive = new Card() {Id = 5, AuthorId = admin.Id, Title = "Size - SS, Galaxy, Universe", Content = "Les gardiens de la galaxie", SectionId = sectionThree.Id, Position = 1};
            Card cardSix = new Card() {Id = 6, AuthorId = ben.Id, Title = "Constellations: Patterns/myths/stories - go to a planetarium show. UTD, UNT, UTA - We should research at TAS", Content = "Lauch in 3 2 1...", SectionId = sectionThree.Id, Position = 2};
            Card cardSeven = new Card() {Id = 7, AuthorId = admin.Id, Title = "Poster for Observation Day", Content = "And the sun was", SectionId = sectionThree.Id, Position = 3};
            Card cardEight = new Card() {Id = 8, AuthorId = quentin.Id, Title = "Observatories map, light pollution map", Content = "Let' write something here", SectionId = sectionThree.Id, Position = 4};
            Card cardNine = new Card() {Id = 9, AuthorId = ben.Id, Title = "Deep Sky Stacker", Content = "Hpw do u call someone with no bady and no noze ? Nobody knows", SectionId = sectionFour.Id, Position = 0};
            Card cardTen = new Card() {Id = 10, AuthorId = bruno.Id, Title = "Astrometrica", Content = "Don't tell me that", SectionId = sectionFour.Id, Position = 1};
            Card cardEleven = new Card() {Id = 11, AuthorId = admin.Id, Title = "Deep Sky Objects Browser", Content = "Aliens are coming for us", SectionId = sectionFive.Id, Position = 0};
            Card cardTwelve = new Card() {Id = 12, AuthorId = quentin.Id, Title = "Vstar", Content = "Astar", SectionId = sectionFive.Id, Position = 1};
            Card cardThirteen = new Card() {Id = 13, AuthorId = ben.Id, Title = "Field of View Calculator", Content = "Y = aX + b", SectionId = sectionFive.Id, Position = 2};
            Card cardFourteen = new Card() {Id = 14, AuthorId = quentin.Id, Title = "Update comets", Content = "Make a wish", SectionId = sectionSix.Id, Position = 0};
            Card cardFifteen = new Card() {Id = 15, AuthorId = quentin.Id, Title = "Supernovae sightings", Content = "Boummmmm", SectionId = sectionSix.Id, Position = 1};
            Card cardSixteen = new Card() {Id = 16, AuthorId = quentin.Id, Title = "Photometric", Content = "Astrometric", SectionId = sectionSix.Id, Position = 2};
            Card cardSeventeen = new Card() {Id = 17, AuthorId = bruno.Id, Title = "Astronomy with the naked eye", Content = "Naked body", SectionId = sectionSix.Id, Position = 3};

            //CARDS FOR CALL OF DUTY TABLE
            Card cardThirtySix = new Card() {Id = 18, AuthorId = ben.Id, Title = "Captured vehicles feature", Content = "", SectionId = sectionSeven.Id, Position = 0};
            Card cardEighteen = new Card() {Id = 19, AuthorId = ben.Id, Title = "Action game UI redisign", Content = "", SectionId = sectionSeven.Id, Position = 1};
            Card cardNineteen = new Card() {Id = 20, AuthorId = gautier.Id, Title = "Dynamic staged battles", Content = "", SectionId = sectionSeven.Id, Position = 2};
            Card cardTwenty = new Card() {Id = 21, AuthorId = admin.Id, Title = "Armor plates", Content = "", SectionId = sectionSeven.Id, Position = 3};
            Card cardTwentyOne = new Card() {Id = 22, AuthorId = bruno.Id, Title = "Skill based matchmaking", Content = "", SectionId = sectionSeven.Id, Position = 4};
            Card cardTwentyTwo = new Card() {Id = 23, AuthorId = bruno.Id, Title = "New faces for soldiers", Content = "", SectionId = sectionEight.Id, Position = 0};
            Card cardTwentyThree = new Card() {Id = 24, AuthorId = bruno.Id, Title = "Battle report upgrade", Content = "", SectionId = sectionEight.Id, Position = 1};
            Card cardTwentyFour = new Card() {Id = 25, AuthorId = admin.Id, Title = "Clan system", Content = "", SectionId = sectionNine.Id, Position = 0};
            Card cardTwentyFive = new Card() {Id = 26, AuthorId = gautier.Id, Title = "Weapons cone fire rework", Content = "", SectionId = sectionNine.Id, Position = 1};
            Card cardTwentySix = new Card() {Id = 27, AuthorId = admin.Id, Title = "New evolved AI for assault battles and skirmish", Content = "", SectionId = sectionNine.Id, Position = 2};
            Card cardTwentySeven = new Card() {Id = 28, AuthorId = ben.Id, Title = "New terrain textures", Content = "", SectionId = sectionNine.Id, Position = 3};
            Card cardTwentyEight = new Card() {Id = 29, AuthorId = bruno.Id, Title = "Airfield map rework", Content = "", SectionId = sectionNine.Id, Position = 4};
            Card cardTwentyNine = new Card() {Id = 30, AuthorId = gautier.Id, Title = "Graphic assets rework", Content = "", SectionId = sectionNine.Id, Position = 5};
            Card cardThirty = new Card() {Id = 31, AuthorId = gautier.Id, Title = "Tech upgrade: Evaluating alternative hosting solutions to provide better performance and stability for our players", Content = "", SectionId = sectionNine.Id, Position = 6};
            Card cardThirtyOne = new Card() {Id = 32, AuthorId = ben.Id, Title = "Stability: issues related to ray tracing on next gen consoles", Content = "", SectionId = sectionTen.Id, Position = 0};
            Card cardThirtyTwo = new Card() {Id = 33, AuthorId = ben.Id, Title = "Splitscreen: visual issues", Content = "", SectionId = sectionTen.Id, Position = 1};
            Card cardThirtyThree = new Card() {Id = 34, AuthorId = admin.Id, Title = "Animations: leveling up", Content = "", SectionId = sectionTen.Id, Position = 2};
            Card cardThirtyFour = new Card() {Id = 35, AuthorId = bruno.Id, Title = "Ring of fire: unintended behavior after update", Content = "", SectionId = sectionEleven.Id, Position = 0};
            Card cardThirtyFive = new Card() {Id = 36, AuthorId = admin.Id, Title = "Trapper challenge: progress stops at 255/500", Content = "", SectionId = sectionEleven.Id, Position = 1};

            //SECTION FOR CLUB DE PETANQUE TABLE
            Section s12 = new Section() {Id = 12, Title = "Lancer le cochonet", TableId = first.Id};
            Section s13 = new Section() {Id = 13, Title = "Joueur la partie", TableId = first.Id};
            Section s14 = new Section() {Id = 14, Title = "Changer de coté en prenant du temps pcq on est vieux", TableId = first.Id};
            Section s15 = new Section() {Id = 15, Title = "Gagner la partie", TableId = first.Id};
            Section s16 = new Section() {Id = 16, Title = "C'est l'heure du ricard", TableId = first.Id};

            //CARDS FOR CLUB DE PETANQUE TABLE
            Card c37 = new Card() {Id = 37, AuthorId = gautier.Id, Title = "Marie-jeanne lance le cochonet", Content = "", SectionId = s12.Id, Position = 0};
            Card c38 = new Card() {Id = 38, AuthorId = gautier.Id, Title = "Jean ben guigui lance la premiere boule et rate le terrain", Content = "", SectionId = s13.Id, Position = 0};
            Card c39 = new Card() {Id = 39, AuthorId = ben.Id, Title = "Papy fourmi lance une boule et tombe par terre", Content = "", SectionId = s13.Id, Position = 1};
            Card c40 = new Card() {Id = 40, AuthorId = ben.Id, Title = "Marie-jeanne perds son dentier ", Content = "", SectionId = s13.Id, Position = 2};
            Card c41 = new Card() {Id = 41, AuthorId = gautier.Id, Title = "Michel lance sa canne au lieu de sa boule", Content = "", SectionId = s13.Id, Position = 3};
            Card c42 = new Card() {Id = 42, AuthorId = quentin.Id, Title = "Marrie-Jeanne part aux toilettes", Content = "", SectionId = s13.Id, Position = 4};
            Card c43 = new Card() {Id = 43, AuthorId = quentin.Id, Title = "Jean ben guigui : Mais elle est ou Jeanne", Content = "", SectionId = s13.Id, Position = 5};
            Card c44 = new Card() {Id = 44, AuthorId = admin.Id, Title = "10 minutes plus tard les seniors changent de coté", Content = "", SectionId = s14.Id, Position = 0};
            Card c45 = new Card() {Id = 45, AuthorId = ben.Id, Title = "Patrick Sébastion nous fait une de ses chasons prefs", Content = "", SectionId = s15.Id, Position = 0};
            Card c46 = new Card() {Id = 46, AuthorId = admin.Id, Title = "La maison de repos se prends une quitte", Content = "", SectionId = s16.Id, Position = 0};

            //SECTION FOR CLUB DE NUDISTES FOUS
            Section s17 = new Section() {Id = 17, Title = "Les cabines", TableId = second.Id};
            Section s18 = new Section() {Id = 18, Title = "La plage nudiste", TableId = second.Id};
            Section s19 = new Section() {Id = 19, Title = "Le restaurant nudiste", TableId = second.Id};
            Section s20 = new Section() {Id = 20, Title = "Le carrefour nudist", TableId = second.Id};
            Section s21 = new Section() {Id = 21, Title = "Le club nudist", TableId = second.Id};

            //CARDS FOR CLUB DE NUDISTS FOUS
            Card c47 = new Card() {Id = 47, AuthorId = quentin.Id, Title = "Jean bernard entre dans la cabine pour retirer ses vetements", Content = "", SectionId = s17.Id, Position = 0};
            Card c48 = new Card() {Id = 48, AuthorId = gautier.Id, Title = "Suzanne marche sur la plage, et ses seins aussi", Content = "", SectionId = s18.Id, Position = 0};
            Card c49 = new Card() {Id = 49, AuthorId = quentin.Id, Title = "Gorge joue a la raquette, et marche sur le seins de Suzanne", Content = "", SectionId = s18.Id, Position = 1};
            Card c50 = new Card() {Id = 50, AuthorId = gautier.Id, Title = "Harmand et Madeleine jouent aux boules, mais pas la pétanque", Content = "", SectionId = s18.Id, Position = 2};
            Card c51 = new Card() {Id = 51, AuthorId = gautier.Id, Title = "Maintenant 3h que nicole bave dans une huitre et la remange", Content = "", SectionId = s19.Id, Position = 0};
            Card c52 = new Card() {Id = 52, AuthorId = gautier.Id, Title = "Gaston déguste un bon steak", Content = "", SectionId = s19.Id, Position = 1};
            Card c53 = new Card() {Id = 53, AuthorId = quentin.Id, Title = "Kevin est parti acheté une baguette, maheureusement la caissière n'a pas squanné la bonne", Content = "", SectionId = s20.Id, Position = 0};
            Card c54 = new Card() {Id = 54, AuthorId = gautier.Id, Title = "Francoise faisait ses courses, mes n'a pas trouvé de crème solaire", Content = "", SectionId = s20.Id, Position = 1};
            Card c55 = new Card() {Id = 55, AuthorId = quentin.Id, Title = "Patrick Sébastion nous fait une de ses chasons prefs", Content = "", SectionId = s20.Id, Position = 2};
            Card c56 = new Card() {Id = 56, AuthorId = gautier.Id, Title = "Tout le monde se déanche, c'est la fête ce soir", Content = "", SectionId = s21.Id, Position = 0};

            //SECTION FOR APP DE RAGEUX
            Section s22 = new Section() {Id = 22, Title = "ON DOIT PAS FAIRE COMME CA", TableId = third.Id};
            Section s23 = new Section() {Id = 23, Title = "Toi personne t'ecoute", TableId = third.Id};
            Section s24 = new Section() {Id = 24, Title = "ON VEUT PAS DE TON AVIS", TableId = third.Id};
            Section s25 = new Section() {Id = 25, Title = "Ta mère est moche d'abord", TableId = third.Id};
            Section s26 = new Section() {Id = 26, Title = "Tu veux du pain ????", TableId = third.Id};

            //CARDS FOR APP DE RAGEAUX
            Card c57 = new Card() {Id = 57, AuthorId = admin.Id, Title = "Gnegnegne monsieur je sais tout", Content = "", SectionId = s22.Id, Position = 0};
            Card c58 = new Card() {Id = 58, AuthorId = ben.Id, Title = "Ouais bha au moins je sais ce qu'il faut faire", Content = "", SectionId = s23.Id, Position = 0};
            Card c59 = new Card() {Id = 59, AuthorId = gautier.Id, Title = "On veut pas t'entendre toi", Content = "", SectionId = s23.Id, Position = 1};
            Card c60 = new Card() {Id = 60, AuthorId = bruno.Id, Title = "mais t'es qui toi d'abord", Content = "", SectionId = s23.Id, Position = 2};
            Card c61 = new Card() {Id = 61, AuthorId = ben.Id, Title = "Mais juste ftg enfait", Content = "", SectionId = s24.Id, Position = 0};
            Card c62 = new Card() {Id = 62, AuthorId = bruno.Id, Title = "Ok je me casse", Content = "", SectionId = s24.Id, Position = 1};
            Card c63 = new Card() {Id = 63, AuthorId = gautier.Id, Title = "On pare pas sur les mères", Content = "", SectionId = s25.Id, Position = 0};
            Card c64 = new Card() {Id = 64, AuthorId = ben.Id, Title = "Me donne pas d'ordres", Content = "", SectionId = s25.Id, Position = 1};
            Card c65 = new Card() {Id = 65, AuthorId = bruno.Id, Title = "Tu vas te calmer toi", Content = "", SectionId = s25.Id, Position = 2};
            Card c66 = new Card() {Id = 66, AuthorId = gautier.Id, Title = "Parfait pour une fondue", Content = "", SectionId = s26.Id, Position = 0};

            //ORGANIZATIONS
            Organization oneo = new Organization() {Id = 1, Name = "EPFC"};
            Organization twoo = new Organization() {Id = 2, Name = "ULB"};

            //INSERT TABLE
            modelBuilder.Entity<Table>().HasData(
                first, second, third, fourth, fifth
            );

            //INSERT USER-TABLE
            modelBuilder.Entity<UserTable>().HasData(
                new { UserId = ben.Id, TableId = 1},
                new { UserId = ben.Id, TableId = 3},  
                new { UserId = ben.Id, TableId = 4},  
                new { UserId = ben.Id, TableId = 5},               
                new { UserId = admin.Id, TableId = 1},
                new { UserId = admin.Id, TableId = 3},
                new { UserId = admin.Id, TableId = 4},
                new { UserId = admin.Id, TableId = 5}, 
                new { UserId = gautier.Id, TableId = 1},
                new { UserId = gautier.Id, TableId = 2},
                new { UserId = gautier.Id, TableId = 4},
                new { UserId = bruno.Id, TableId = 3},
                new { UserId = bruno.Id, TableId = 4},
                new { UserId = bruno.Id, TableId = 5},
                new { UserId = quentin.Id, TableId = 1},
                new { UserId = quentin.Id, TableId = 2},
                new { UserId = quentin.Id, TableId = 3},
                new { UserId = quentin.Id, TableId = 5}
            );

            //INSERT TEAM
            modelBuilder.Entity<Team>().HasData(
                one, two
            );

            //INSERT USER-TEAM
            modelBuilder.Entity<UserTeam>().HasData(
                new { UserId = ben.Id, TeamId = 1},
                new { UserId = ben.Id, TeamId = 2},
                new { UserId = admin.Id, TeamId = 1},     
                new { UserId = admin.Id, TeamId = 2},
                new { UserId = gautier.Id, TeamId = 2},
                new { UserId = bruno.Id, TeamId = 2 },
                new { UserId = quentin.Id, TeamId = 1}
            );

            //INSERT ORGANIZATION
            modelBuilder.Entity<Organization>().HasData(
                oneo, twoo
            );

            //INSERT USER-ORGANIZATION
            modelBuilder.Entity<UserOrganization>().HasData(
                new { UserId = admin.Id, OrganizationId = 1},
                new { UserId = admin.Id, OrganizationId = 2}
            );

            //INSERT SECTION
            modelBuilder.Entity<Section>().HasData(
                sectionOne, sectionTwo, sectionThree, sectionFour, sectionFive, sectionSix, sectionSeven, 
                sectionEight, sectionNine, sectionTen, sectionEleven, s12, s13, s14, s15, s16, s17, s18, s19, s20, s21,
                s22, s23, s24, s25, s26
            );

            //INSERT CARDS
            modelBuilder.Entity<Card>().HasData(
                cardOne, cardTwo, cardThree, cardFour, cardFive, cardSix, cardSeven, cardEight, cardNine, cardTen, cardEleven,
                cardTwelve, cardThirteen, cardFourteen, cardFifteen, cardSixteen, cardSeventeen, cardEighteen, cardNineteen,
                cardTwenty, cardTwentyOne, cardTwentyTwo, cardTwentyThree, cardTwentyFour, cardTwentyFive, cardTwentySix, cardTwentySeven,
                cardTwentyEight, cardTwentyNine, cardThirty, cardThirtyOne, cardThirtyTwo, cardThirtyThree, cardThirtyFour, cardThirtyFive, 
                cardThirtySix, c37, c38, c39, c40, c41, c42, c43, c44, c45, c46, c47, c48, c49, c50, c51, c52, c53, c54, c55, c56, c57,
                c58, c59, c60, c61, c62, c63, c64, c65, c66
            );

            //INSERT USERCARDS
            //cards 1 to 17 are from table 5 with users 1 = admin, 2 = ben, 3 = bruno, 5 = quentin
            //cards 18 to 36 from tbl 4 with users 1 = admin, 2 = ben, 3 = bruno, 4 = gautier
            //cards 37 to 46 from tbl 1 with users 1, 2, 4, 5
            //cards 47 to 56 from tbl 2 with users 4, 5
            //cards 57 to 66 from tbl 3 with users 1, 2, 3, 4
            modelBuilder.Entity<UserCard>().HasData( 
                new { UserId = admin.Id, CardId = 3},
                new { UserId = admin.Id, CardId = 6},
                new { UserId = admin.Id, CardId = 7},
                new { UserId = admin.Id, CardId = 9},
                new { UserId = admin.Id, CardId = 18},
                new { UserId = admin.Id, CardId = 20},
                new { UserId = admin.Id, CardId = 29},
                new { UserId = admin.Id, CardId = 33},
                new { UserId = admin.Id, CardId = 37},
                new { UserId = admin.Id, CardId = 44},
                new { UserId = admin.Id, CardId = 45},
                new { UserId = admin.Id, CardId = 60},
                new { UserId = admin.Id, CardId = 63},
                new { UserId = ben.Id, CardId = 1}, 
                new { UserId = ben.Id, CardId = 8},
                new { UserId = ben.Id, CardId = 9},
                new { UserId = ben.Id, CardId = 10},
                new { UserId = ben.Id, CardId = 21},
                new { UserId = ben.Id, CardId = 23},
                new { UserId = ben.Id, CardId = 26},
                new { UserId = ben.Id, CardId = 36},
                new { UserId = ben.Id, CardId = 41},
                new { UserId = ben.Id, CardId = 43},
                new { UserId = ben.Id, CardId = 57},
                new { UserId = ben.Id, CardId = 60},
                new { UserId = ben.Id, CardId = 63},
                new { UserId = ben.Id, CardId = 66},
                new { UserId = bruno.Id, CardId = 1},
                new { UserId = bruno.Id, CardId = 8},
                new { UserId = bruno.Id, CardId = 10},
                new { UserId = bruno.Id, CardId = 15},
                new { UserId = bruno.Id, CardId = 20},
                new { UserId = bruno.Id, CardId = 26},
                new { UserId = bruno.Id, CardId = 35},
                new { UserId = bruno.Id, CardId = 58},
                new { UserId = bruno.Id, CardId = 61},
                new { UserId = bruno.Id, CardId = 62},
                new { UserId = gautier.Id, CardId = 18},
                new { UserId = gautier.Id, CardId = 21},
                new { UserId = gautier.Id, CardId = 27},
                new { UserId = gautier.Id, CardId = 28},
                new { UserId = gautier.Id, CardId = 33},
                new { UserId = gautier.Id, CardId = 39},
                new { UserId = gautier.Id, CardId = 44},
                new { UserId = gautier.Id, CardId = 46},
                new { UserId = gautier.Id, CardId = 51},
                new { UserId = gautier.Id, CardId = 55},
                new { UserId = gautier.Id, CardId = 62},
                new { UserId = quentin.Id, CardId = 2}, 
                new { UserId = quentin.Id, CardId = 3},
                new { UserId = quentin.Id, CardId = 6},
                new { UserId = quentin.Id, CardId = 9},
                new { UserId = quentin.Id, CardId = 11},
                new { UserId = quentin.Id, CardId = 13},
                new { UserId = quentin.Id, CardId = 17},
                new { UserId = quentin.Id, CardId = 38},
                new { UserId = quentin.Id, CardId = 41},
                new { UserId = quentin.Id, CardId = 42},
                new { UserId = quentin.Id, CardId = 51},
                new { UserId = quentin.Id, CardId = 55},
                new { UserId = quentin.Id, CardId = 56}
            );
        }

        //RELATION BETWEEN USER AND TABLE
        private void ConfigUserTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTable>().HasKey(ut => new { ut.UserId, ut.TableId });

            modelBuilder.Entity<UserTable>()
                .HasOne<User>(ut => ut.User)
                .WithMany(m => m.TablesIn)
                .HasForeignKey(fk => fk.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserTable>()
                .HasOne<Table>(ut => ut.Table)
                .WithMany(m => m.UsersIn)
                .HasForeignKey(fk => fk.TableId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        //RELATION BETWEEN USER AND CARD
        private void ConfigUserCard(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserCard>().HasKey(ut => new { ut.UserId, ut.CardId });

            modelBuilder.Entity<UserCard>()
                .HasOne<User>(ut => ut.User)
                .WithMany(m => m.CardsIn)
                .HasForeignKey(fk => fk.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserCard>()
                .HasOne<Card>(ut => ut.Card)
                .WithMany(m => m.UsersIn)
                .HasForeignKey(fk => fk.CardId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        //RELATION BETWEEN USER AND TEAM
        private void ConfigUserTeam(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTeam>().HasKey(ut => new { ut.UserId, ut.TeamId });

            modelBuilder.Entity<UserTeam>()
                .HasOne<User>(ut => ut.User)
                .WithMany(m => m.TeamsIn)
                .HasForeignKey(fk => fk.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserTeam>()
                .HasOne<Team>(ut => ut.Team)
                .WithMany(m => m.UsersIn)
                .HasForeignKey(fk => fk.TeamId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        //RELATION BETWEEN USER AND ORGANIZATION
        private void ConfigUserOrganization(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserOrganization>().HasKey(ut => new { ut.UserId, ut.OrganizationId });

            modelBuilder.Entity<UserOrganization>()
                .HasOne<User>(ut => ut.User)
                .WithMany(m => m.OrganizationsIn)
                .HasForeignKey(fk => fk.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserOrganization>()
                .HasOne<Organization>(ut => ut.Organization)
                .WithMany(m => m.UsersIn)
                .HasForeignKey(fk => fk.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}