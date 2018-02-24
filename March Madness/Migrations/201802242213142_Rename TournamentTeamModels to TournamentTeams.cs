namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameTournamentTeamModelstoTournamentTeams : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TournamentTeamModels", newName: "TournamentTeams");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.TournamentTeams", newName: "TournamentTeamModels");
        }
    }
}
