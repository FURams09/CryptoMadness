namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropoldTournamentEntryColumninBrcketEntries : DbMigration
    {
        public override void Up()
        {
			DropColumn("dbo.BracketGamePicks", "TournamentEntryID");

		}
        
        public override void Down()
        {
			AddColumn("BracketGamePicks", "TournamentEntryID", c => c.Int(nullable: false));
        }
    }
}
