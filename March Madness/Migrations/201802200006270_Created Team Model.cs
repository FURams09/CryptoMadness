namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedTeamModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TeamModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 60),
                        Mascot = c.String(nullable: false, maxLength: 60),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TeamModels");
        }
    }
}
