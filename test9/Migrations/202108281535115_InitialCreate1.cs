namespace test9.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Forecasts",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Prediction = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Requests", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Requests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ForecastId = c.Int(nullable: false),
                        ExtremumId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Forecasts", "Id", "dbo.Requests");
            DropForeignKey("dbo.Requests", "UserId", "dbo.Users");
            DropIndex("dbo.Requests", new[] { "UserId" });
            DropIndex("dbo.Forecasts", new[] { "Id" });
            DropTable("dbo.Users");
            DropTable("dbo.Requests");
            DropTable("dbo.Forecasts");
        }
    }
}
