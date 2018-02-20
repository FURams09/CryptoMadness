namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedTournamentTeamModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TournamentTeamModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Region = c.Int(nullable: false),
                        Seed = c.Int(nullable: false),
                        TeamId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TeamModels", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.TeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TournamentTeamModels", "TeamId", "dbo.TeamModels");
            DropIndex("dbo.TournamentTeamModels", new[] { "TeamId" });
            DropTable("dbo.TournamentTeamModels");
        }
    }
}
