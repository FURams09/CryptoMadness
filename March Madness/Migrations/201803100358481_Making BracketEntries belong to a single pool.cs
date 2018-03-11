namespace March_Madness.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakingBracketEntriesbelongtoasinglepool : DbMigration
    {
        public override void Up()
        {
			Sql("create table #tmk_BracketEntries (ID int, UserID nvarchar(400), name nvarchar(50));");
			Sql("insert into #tmk_BracketEntries select * from bracketentries;");
			Sql("Delete from PoolBrackets");
			Sql("Delete from BracketEntries");
            DropForeignKey("dbo.PoolBrackets", "Bracket_Id", "dbo.BracketEntries");
            DropForeignKey("dbo.PoolBrackets", "Pool_Id", "dbo.Pools");
            DropIndex("dbo.PoolBrackets", new[] { "Bracket_Id" });
            DropIndex("dbo.PoolBrackets", new[] { "Pool_Id" });
            AddColumn("dbo.BracketEntries", "PoolId", c => c.Int(nullable: false));
            CreateIndex("dbo.BracketEntries", "PoolId");
            AddForeignKey("dbo.BracketEntries", "PoolId", "dbo.Pools", "Id", cascadeDelete: true);
            DropTable("dbo.PoolBrackets");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PoolBrackets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BracketId = c.String(),
                        Bracket_Id = c.Int(),
                        Pool_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.BracketEntries", "PoolId", "dbo.Pools");
            DropIndex("dbo.BracketEntries", new[] { "PoolId" });
            DropColumn("dbo.BracketEntries", "PoolId");
            CreateIndex("dbo.PoolBrackets", "Pool_Id");
            CreateIndex("dbo.PoolBrackets", "Bracket_Id");
            AddForeignKey("dbo.PoolBrackets", "Pool_Id", "dbo.Pools", "Id");
            AddForeignKey("dbo.PoolBrackets", "Bracket_Id", "dbo.BracketEntries", "Id");
        }
    }
}
