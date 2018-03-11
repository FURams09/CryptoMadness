namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingEntryFeeToPoolandAddingPublicKeys : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PublicKeys",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        PublicKey = c.String(maxLength: 40),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Pools", "EntryFee", c => c.Short(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PublicKeys", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.PublicKeys", new[] { "UserId" });
            DropColumn("dbo.Pools", "EntryFee");
            DropTable("dbo.PublicKeys");
        }
    }
}
