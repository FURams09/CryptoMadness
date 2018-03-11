namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NamingImprovementsandAddingPoolModel : DbMigration
    {
        public override void Up()
        {
			DropForeignKey("dbo.TournamentGamePicks", "TournamentEntryID", "dbo.TournamentEntries");
            RenameTable(name: "dbo.TournamentEntries", newName: "BracketEntries");
            RenameTable(name: "dbo.TournamentGamePicks", newName: "BracketGamePicks");
            
            DropIndex("dbo.BracketGamePicks", new[] { "TournamentEntryID" });
            AddColumn("dbo.BracketGamePicks", "BracketEntry_Id", c => c.Int());
            CreateIndex("dbo.BracketGamePicks", "BracketEntry_Id");
            AddForeignKey("dbo.BracketGamePicks", "BracketEntry_Id", "dbo.BracketEntries", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BracketGamePicks", "BracketEntry_Id", "dbo.BracketEntries");
            DropIndex("dbo.BracketGamePicks", new[] { "BracketEntry_Id" });
            DropColumn("dbo.BracketGamePicks", "BracketEntry_Id");
          
            RenameTable(name: "dbo.BracketGamePicks", newName: "TournamentGamePicks");
            RenameTable(name: "dbo.BracketEntries", newName: "TournamentEntries");
			CreateIndex("dbo.TournamentGamePicks", "TournamentEntryID");
			AddForeignKey("dbo.TournamentGamePicks", "TournamentEntryID", "dbo.TournamentEntries", "Id", cascadeDelete: true);
		}
    }
}
