namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingBracketConfigHomeAwaytopickteam : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TournamentGamePicks", "HomeOrAway", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TournamentGamePicks", "HomeOrAway");
        }
    }
}
