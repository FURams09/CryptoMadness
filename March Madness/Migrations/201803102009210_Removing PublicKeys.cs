namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovingPublicKeys : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PublicKeys", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.PublicKeys", new[] { "UserId" });
            AddColumn("dbo.BracketEntries", "OwnerAddress", c => c.String(maxLength: 42));
            AddColumn("dbo.Pools", "OwnerAddress", c => c.String(maxLength: 42));
            DropTable("dbo.PublicKeys");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PublicKeys",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        PublicKey = c.String(maxLength: 40),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.Pools", "OwnerAddress");
            DropColumn("dbo.BracketEntries", "OwnerAddress");
            CreateIndex("dbo.PublicKeys", "UserId");
            AddForeignKey("dbo.PublicKeys", "UserId", "dbo.AspNetUsers", "Id");
        }
    }
}
