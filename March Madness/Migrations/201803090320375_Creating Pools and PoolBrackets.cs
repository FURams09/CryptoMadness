namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatingPoolsandPoolBrackets : DbMigration
    {
        public override void Up()
        {
			CreateTable(
			   "dbo.Pools",
			   c => new
			   {
				   Id = c.Int(nullable: false, identity: true),
				   UserId = c.String(),
				   Nickname = c.String(),
			   })
			   .PrimaryKey(t => t.Id);

			CreateTable(
                "dbo.PoolBrackets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BracketId = c.String(),
                        Bracket_Id = c.Int(),
                        Pool_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BracketEntries", t => t.Bracket_Id)
                .ForeignKey("dbo.Pools", t => t.Pool_Id)
                .Index(t => t.Bracket_Id)
                .Index(t => t.Pool_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PoolBrackets", "Pool_Id", "dbo.Pools");
            DropForeignKey("dbo.PoolBrackets", "Bracket_Id", "dbo.BracketEntries");
            DropIndex("dbo.PoolBrackets", new[] { "Pool_Id" });
            DropIndex("dbo.PoolBrackets", new[] { "Bracket_Id" });
            DropTable("dbo.PoolBrackets");
			DropTable("dbo.Pools");
        }
    }
}
