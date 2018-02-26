namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangingTournamentTeamkeyinTournamentGamePicktoPickedTeam : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TournamentGamePicks", "PickTeam_Id", "dbo.TournamentTeams");
            DropIndex("dbo.TournamentGamePicks", new[] { "PickTeam_Id" });
            RenameColumn(table: "dbo.TournamentGamePicks", name: "PickTeam_Id", newName: "PickedTeamId");
            AlterColumn("dbo.TournamentGamePicks", "PickedTeamId", c => c.Int(nullable: false));
            CreateIndex("dbo.TournamentGamePicks", "PickedTeamId");
            AddForeignKey("dbo.TournamentGamePicks", "PickedTeamId", "dbo.TournamentTeams", "Id", cascadeDelete: true);
            DropColumn("dbo.TournamentGamePicks", "TournamentTeamId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TournamentGamePicks", "TournamentTeamId", c => c.Int(nullable: false));
            DropForeignKey("dbo.TournamentGamePicks", "PickedTeamId", "dbo.TournamentTeams");
            DropIndex("dbo.TournamentGamePicks", new[] { "PickedTeamId" });
            AlterColumn("dbo.TournamentGamePicks", "PickedTeamId", c => c.Int());
            RenameColumn(table: "dbo.TournamentGamePicks", name: "PickedTeamId", newName: "PickTeam_Id");
            CreateIndex("dbo.TournamentGamePicks", "PickTeam_Id");
            AddForeignKey("dbo.TournamentGamePicks", "PickTeam_Id", "dbo.TournamentTeams", "Id");
        }
    }
}
