namespace test9.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Archives",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StationIndex = c.Int(nullable: false),
                        GreenwichYear = c.Int(nullable: false),
                        GreenwichMonth = c.Int(nullable: false),
                        GreenwichDay = c.Int(nullable: false),
                        GreenwichMeanTime = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        Month = c.Int(nullable: false),
                        Day = c.Int(nullable: false),
                        Time = c.Int(nullable: false),
                        Cloudiness = c.Int(nullable: false),
                        Index = c.Int(nullable: false),
                        WindDirection = c.Int(nullable: false),
                        WindSpeed = c.Int(nullable: false),
                        PrecipitationSum = c.Double(nullable: false),
                        Temperature = c.Double(nullable: false),
                        Humidity = c.Int(nullable: false),
                        Pressure = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Extrema",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Day_Month = c.String(),
                        Minimum = c.Double(nullable: false),
                        Minimum_Year = c.Int(nullable: false),
                        Maximum = c.Double(nullable: false),
                        Maximum_Year = c.Int(nullable: false),
                        SignText = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Extrema");
            DropTable("dbo.Archives");
        }
    }
}
