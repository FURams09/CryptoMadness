namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TeamNameMadeUnique : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.TeamModels", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.TeamModels", new[] { "Name" });
        }
    }
}
