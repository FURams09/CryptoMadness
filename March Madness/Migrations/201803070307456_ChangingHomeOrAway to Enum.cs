namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangingHomeOrAwaytoEnum : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TournamentGamePicks", "HomeOrAway", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TournamentGamePicks", "HomeOrAway", c => c.Boolean(nullable: false));
        }
    }
}
