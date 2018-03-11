namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingReftoOwnerfromBracketEntry : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Pools", name: "UserId", newName: "OwnerId");
			AlterColumn("dbo.Pools", "OwnerId", c => c.String(maxLength: 128));
			CreateIndex("dbo.Pools", "OwnerId");
            AlterColumn("dbo.BracketEntries", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.BracketEntries", "UserId");
            AddForeignKey("dbo.BracketEntries", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BracketEntries", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.BracketEntries", new[] { "UserId" });
            AlterColumn("dbo.BracketEntries", "UserId", c => c.String());
            DropIndex("dbo.Pools", new[] { "OwnerId" });
            RenameColumn(table: "dbo.Pools", name: "OwnerId", newName: "UserId");
        }
    }
}
