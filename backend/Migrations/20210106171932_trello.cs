using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace prid_2021_a06.Migrations
{
    public partial class trello : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Pseudo = table.Column<string>(nullable: false),
                    Password = table.Column<string>(maxLength: 10, nullable: false),
                    Email = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: true),
                    LastName = table.Column<string>(maxLength: 50, nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: true),
                    Role = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    OrganizationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserOrganization",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    OrganizationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrganization", x => new { x.UserId, x.OrganizationId });
                    table.ForeignKey(
                        name: "FK_UserOrganization_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserOrganization_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    View = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: true),
                    OrganizationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tables_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tables_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tables_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserTeam",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    TeamId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTeam", x => new { x.UserId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_UserTeam_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserTeam_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: false),
                    TableId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sections_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserTable",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    TableId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTable", x => new { x.UserId, x.TableId });
                    table.ForeignKey(
                        name: "FK_UserTable_Tables_TableId",
                        column: x => x.TableId,
                        principalTable: "Tables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserTable_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    AuthorId = table.Column<int>(nullable: true),
                    SectionId = table.Column<int>(nullable: true),
                    Position = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cards_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserCard",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    CardId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCard", x => new { x.UserId, x.CardId });
                    table.ForeignKey(
                        name: "FK_UserCard_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCard_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "EPFC" },
                    { 2, "ULB" }
                });

            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "Name", "OrganizationId" },
                values: new object[,]
                {
                    { 1, "Projekt-Team", null },
                    { 2, "A-Team", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "Email", "FirstName", "LastName", "Password", "Pseudo", "Role" },
                values: new object[,]
                {
                    { 1, null, "Admin@epfc.eu", "Ad", "Min", "21232f297a57a5a743894a0e4a801fc3", "admin", 2 },
                    { 2, null, "Benoit@epfc.eu", "Benoit", "Penelle", "7fe4771c008a22eb763df47d19e2c6aa", "ben", 0 },
                    { 3, null, "Bruno@epfc.eu", "Bruno", "Lacroix", "e3928a3bc4be46516aa33a79bbdfdb08", "bruno", 0 },
                    { 4, null, "Gautier@epfc.eu", "Gautier", "Kiss", "bef19c64c8126fdc78911de3c7e50f1e", "gautier", 0 },
                    { 5, null, "Quentin@epfc.eu", "Quentin", "Locht", "c7f413a4f6f4a658c24f0a437666089e", "quentin", 2 }
                });

            migrationBuilder.InsertData(
                table: "Tables",
                columns: new[] { "Id", "OrganizationId", "TeamId", "Title", "UserId", "View" },
                values: new object[,]
                {
                    { 3, null, 1, "App de rageux", 1, 2 },
                    { 4, null, 2, "Call auf duty", 2, 1 },
                    { 5, null, 1, "Stellar astronomy", 3, 2 },
                    { 2, null, null, "Club des nudistes fous", 4, 3 },
                    { 1, null, null, "Club de pétanque", 5, 0 }
                });

            migrationBuilder.InsertData(
                table: "UserOrganization",
                columns: new[] { "UserId", "OrganizationId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 }
                });

            migrationBuilder.InsertData(
                table: "UserTeam",
                columns: new[] { "UserId", "TeamId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 1 },
                    { 2, 2 },
                    { 3, 2 },
                    { 4, 2 },
                    { 5, 1 }
                });

            migrationBuilder.InsertData(
                table: "Sections",
                columns: new[] { "Id", "TableId", "Title" },
                values: new object[,]
                {
                    { 22, 3, "ON DOIT PAS FAIRE COMME CA" },
                    { 16, 1, "C'est l'heure du ricard" },
                    { 15, 1, "Gagner la partie" },
                    { 14, 1, "Changer de coté en prenant du temps pcq on est vieux" },
                    { 13, 1, "Joueur la partie" },
                    { 12, 1, "Lancer le cochonet" },
                    { 21, 2, "Le club nudist" },
                    { 20, 2, "Le carrefour nudist" },
                    { 19, 2, "Le restaurant nudiste" },
                    { 18, 2, "La plage nudiste" },
                    { 17, 2, "Les cabines" },
                    { 6, 5, "Release in progress" },
                    { 5, 5, "Ready for release" },
                    { 3, 5, "To test" },
                    { 2, 5, "Dev in progress" },
                    { 1, 5, "Backlog" },
                    { 4, 5, "Testing in progress" },
                    { 11, 4, "Bug fixed" },
                    { 23, 3, "Toi personne t'ecoute" },
                    { 10, 4, "Bug Fixing" },
                    { 9, 4, "Completed" },
                    { 24, 3, "ON VEUT PAS DE TON AVIS" },
                    { 25, 3, "Ta mère est moche d'abord" },
                    { 26, 3, "Tu veux du pain ????" },
                    { 8, 4, "In progress" },
                    { 7, 4, "Things to do" }
                });

            migrationBuilder.InsertData(
                table: "UserTable",
                columns: new[] { "UserId", "TableId" },
                values: new object[,]
                {
                    { 3, 4 },
                    { 1, 1 },
                    { 2, 1 },
                    { 2, 3 },
                    { 5, 2 },
                    { 4, 2 },
                    { 5, 3 },
                    { 3, 3 },
                    { 4, 4 },
                    { 5, 5 },
                    { 3, 5 },
                    { 1, 5 },
                    { 2, 5 },
                    { 4, 1 },
                    { 2, 4 },
                    { 1, 4 },
                    { 1, 3 },
                    { 5, 1 }
                });

            migrationBuilder.InsertData(
                table: "Cards",
                columns: new[] { "Id", "AuthorId", "Content", "Position", "SectionId", "Title" },
                values: new object[,]
                {
                    { 57, 1, "", 0, 22, "Gnegnegne monsieur je sais tout" },
                    { 7, 1, "And the sun was", 3, 3, "Poster for Observation Day" },
                    { 8, 5, "Let' write something here", 4, 3, "Observatories map, light pollution map" },
                    { 9, 2, "Hpw do u call someone with no bady and no noze ? Nobody knows", 0, 4, "Deep Sky Stacker" },
                    { 10, 3, "Don't tell me that", 1, 4, "Astrometrica" },
                    { 11, 1, "Aliens are coming for us", 0, 5, "Deep Sky Objects Browser" },
                    { 12, 5, "Astar", 1, 5, "Vstar" },
                    { 13, 2, "Y = aX + b", 2, 5, "Field of View Calculator" },
                    { 14, 5, "Make a wish", 0, 6, "Update comets" },
                    { 15, 5, "Boummmmm", 1, 6, "Supernovae sightings" },
                    { 16, 5, "Astrometric", 2, 6, "Photometric" },
                    { 17, 3, "Naked body", 3, 6, "Astronomy with the naked eye" },
                    { 47, 5, "", 0, 17, "Jean bernard entre dans la cabine pour retirer ses vetements" },
                    { 48, 4, "", 0, 18, "Suzanne marche sur la plage, et ses seins aussi" },
                    { 49, 5, "", 1, 18, "Gorge joue a la raquette, et marche sur le seins de Suzanne" },
                    { 50, 4, "", 2, 18, "Harmand et Madeleine jouent aux boules, mais pas la pétanque" },
                    { 51, 4, "", 0, 19, "Maintenant 3h que nicole bave dans une huitre et la remange" },
                    { 52, 4, "", 1, 19, "Gaston déguste un bon steak" },
                    { 53, 5, "", 0, 20, "Kevin est parti acheté une baguette, maheureusement la caissière n'a pas squanné la bonne" },
                    { 54, 4, "", 1, 20, "Francoise faisait ses courses, mes n'a pas trouvé de crème solaire" },
                    { 55, 5, "", 2, 20, "Patrick Sébastion nous fait une de ses chasons prefs" },
                    { 56, 4, "", 0, 21, "Tout le monde se déanche, c'est la fête ce soir" },
                    { 37, 4, "", 0, 12, "Marie-jeanne lance le cochonet" },
                    { 38, 4, "", 0, 13, "Jean ben guigui lance la premiere boule et rate le terrain" },
                    { 39, 2, "", 1, 13, "Papy fourmi lance une boule et tombe par terre" },
                    { 40, 2, "", 2, 13, "Marie-jeanne perds son dentier " },
                    { 41, 4, "", 3, 13, "Michel lance sa canne au lieu de sa boule" },
                    { 42, 5, "", 4, 13, "Marrie-Jeanne part aux toilettes" },
                    { 43, 5, "", 5, 13, "Jean ben guigui : Mais elle est ou Jeanne" },
                    { 44, 1, "", 0, 14, "10 minutes plus tard les seniors changent de coté" },
                    { 6, 2, "Lauch in 3 2 1...", 2, 3, "Constellations: Patterns/myths/stories - go to a planetarium show. UTD, UNT, UTA - We should research at TAS" },
                    { 5, 1, "Les gardiens de la galaxie", 1, 3, "Size - SS, Galaxy, Universe" },
                    { 4, 2, "ITES", 0, 3, "SETI" },
                    { 3, 3, "Dark Sidious", 0, 2, "Dark Matter / Dark Energy" },
                    { 58, 2, "", 0, 23, "Ouais bha au moins je sais ce qu'il faut faire" },
                    { 59, 4, "", 1, 23, "On veut pas t'entendre toi" },
                    { 60, 3, "", 2, 23, "mais t'es qui toi d'abord" },
                    { 61, 2, "", 0, 24, "Mais juste ftg enfait" },
                    { 62, 3, "", 1, 24, "Ok je me casse" },
                    { 63, 4, "", 0, 25, "On pare pas sur les mères" },
                    { 64, 2, "", 1, 25, "Me donne pas d'ordres" },
                    { 65, 3, "", 2, 25, "Tu vas te calmer toi" },
                    { 66, 4, "", 0, 26, "Parfait pour une fondue" },
                    { 19, 2, "", 1, 7, "Action game UI redisign" },
                    { 20, 4, "", 2, 7, "Dynamic staged battles" },
                    { 21, 1, "", 3, 7, "Armor plates" },
                    { 22, 3, "", 4, 7, "Skill based matchmaking" },
                    { 18, 2, "", 0, 7, "Captured vehicles feature" },
                    { 45, 2, "", 0, 15, "Patrick Sébastion nous fait une de ses chasons prefs" },
                    { 23, 3, "", 0, 8, "New faces for soldiers" },
                    { 25, 1, "", 0, 9, "Clan system" },
                    { 26, 4, "", 1, 9, "Weapons cone fire rework" },
                    { 27, 1, "", 2, 9, "New evolved AI for assault battles and skirmish" },
                    { 28, 2, "", 3, 9, "New terrain textures" },
                    { 29, 3, "", 4, 9, "Airfield map rework" },
                    { 30, 4, "", 5, 9, "Graphic assets rework" },
                    { 31, 4, "", 6, 9, "Tech upgrade: Evaluating alternative hosting solutions to provide better performance and stability for our players" },
                    { 32, 2, "", 0, 10, "Stability: issues related to ray tracing on next gen consoles" },
                    { 33, 2, "", 1, 10, "Splitscreen: visual issues" },
                    { 34, 1, "", 2, 10, "Animations: leveling up" },
                    { 35, 3, "", 0, 11, "Ring of fire: unintended behavior after update" },
                    { 36, 1, "", 1, 11, "Trapper challenge: progress stops at 255/500" },
                    { 1, 1, "U ll find it by yourself", 0, 1, "Stars: what makes up a star? Nucleosynthesis? HR Diagrams...what happens in the life of a star?" },
                    { 2, 3, "Uranus", 1, 1, "Planets" },
                    { 24, 3, "", 1, 8, "Battle report upgrade" },
                    { 46, 1, "", 0, 16, "La maison de repos se prends une quitte" }
                });

            migrationBuilder.InsertData(
                table: "UserCard",
                columns: new[] { "UserId", "CardId" },
                values: new object[,]
                {
                    { 2, 57 },
                    { 5, 6 },
                    { 1, 7 },
                    { 2, 8 },
                    { 3, 8 },
                    { 1, 9 },
                    { 2, 9 },
                    { 5, 9 },
                    { 2, 10 },
                    { 3, 10 },
                    { 5, 11 },
                    { 5, 13 },
                    { 3, 15 },
                    { 5, 17 },
                    { 4, 51 },
                    { 5, 51 },
                    { 4, 55 },
                    { 5, 55 },
                    { 5, 56 },
                    { 1, 37 },
                    { 5, 38 },
                    { 4, 39 },
                    { 2, 41 },
                    { 5, 41 },
                    { 5, 42 },
                    { 2, 43 },
                    { 1, 44 },
                    { 4, 44 },
                    { 1, 6 },
                    { 1, 45 },
                    { 5, 3 },
                    { 5, 2 },
                    { 3, 58 },
                    { 1, 60 },
                    { 2, 60 },
                    { 3, 61 },
                    { 3, 62 },
                    { 4, 62 },
                    { 1, 63 },
                    { 2, 63 },
                    { 2, 66 },
                    { 1, 20 },
                    { 3, 20 },
                    { 2, 21 },
                    { 4, 21 },
                    { 1, 18 },
                    { 4, 18 },
                    { 2, 23 },
                    { 2, 26 },
                    { 3, 26 },
                    { 4, 27 },
                    { 4, 28 },
                    { 1, 29 },
                    { 1, 33 },
                    { 4, 33 },
                    { 3, 35 },
                    { 2, 36 },
                    { 2, 1 },
                    { 3, 1 },
                    { 1, 3 },
                    { 4, 46 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_AuthorId",
                table: "Cards",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_SectionId",
                table: "Cards",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_TableId",
                table: "Sections",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_OrganizationId",
                table: "Tables",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_TeamId",
                table: "Tables",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_UserId",
                table: "Tables",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_OrganizationId",
                table: "Teams",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCard_CardId",
                table: "UserCard",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganization_OrganizationId",
                table: "UserOrganization",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Pseudo",
                table: "Users",
                column: "Pseudo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTable_TableId",
                table: "UserTable",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTeam_TeamId",
                table: "UserTeam",
                column: "TeamId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCard");

            migrationBuilder.DropTable(
                name: "UserOrganization");

            migrationBuilder.DropTable(
                name: "UserTable");

            migrationBuilder.DropTable(
                name: "UserTeam");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "Tables");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
