namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GotridofMascotandaddedDisplayName : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Teams", new[] { "Name" });
            AddColumn("dbo.Teams", "DisplayName", c => c.String(maxLength: 30));
            AlterColumn("dbo.Teams", "Name", c => c.String(nullable: false, maxLength: 150));
            CreateIndex("dbo.Teams", "Name", unique: true);
            DropColumn("dbo.Teams", "Mascot");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Teams", "Mascot", c => c.String(nullable: false, maxLength: 60));
            DropIndex("dbo.Teams", new[] { "Name" });
            AlterColumn("dbo.Teams", "Name", c => c.String(nullable: false, maxLength: 60));
            DropColumn("dbo.Teams", "DisplayName");
            CreateIndex("dbo.Teams", "Name", unique: true);
        }
    }
}
