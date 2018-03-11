namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HopefullygettingridofTournamentEntryIDforBracketEntry_idinmodeltomatchcurrentstateofDB : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BracketGamePicks", "BracketEntry_Id", "dbo.BracketEntries");
            DropIndex("dbo.BracketGamePicks", new[] { "BracketEntry_Id" });
            RenameColumn(table: "dbo.BracketGamePicks", name: "BracketEntry_Id", newName: "BracketEntryId");
            AlterColumn("dbo.BracketGamePicks", "BracketEntryId", c => c.Int(nullable: false));
            CreateIndex("dbo.BracketGamePicks", "BracketEntryId");
            AddForeignKey("dbo.BracketGamePicks", "BracketEntryId", "dbo.BracketEntries", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {

            DropForeignKey("dbo.BracketGamePicks", "BracketEntryId", "dbo.BracketEntries");
            DropIndex("dbo.BracketGamePicks", new[] { "BracketEntryId" });
            AlterColumn("dbo.BracketGamePicks", "BracketEntryId", c => c.Int());
            RenameColumn(table: "dbo.BracketGamePicks", name: "BracketEntryId", newName: "BracketEntry_Id");
            CreateIndex("dbo.BracketGamePicks", "BracketEntry_Id");
            AddForeignKey("dbo.BracketGamePicks", "BracketEntry_Id", "dbo.BracketEntries", "Id");
        }
    }
}
