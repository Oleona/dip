namespace test9.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class archiveone : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Archives", "StationIndex", c => c.Int());
            AlterColumn("dbo.Archives", "GreenwichYear", c => c.Int());
            AlterColumn("dbo.Archives", "GreenwichMonth", c => c.Int());
            AlterColumn("dbo.Archives", "GreenwichDay", c => c.Int());
            AlterColumn("dbo.Archives", "GreenwichMeanTime", c => c.Int());
            AlterColumn("dbo.Archives", "Year", c => c.Int());
            AlterColumn("dbo.Archives", "Month", c => c.Int());
            AlterColumn("dbo.Archives", "Day", c => c.Int());
            AlterColumn("dbo.Archives", "Time", c => c.Int());
            AlterColumn("dbo.Archives", "Cloudiness", c => c.Int());
            AlterColumn("dbo.Archives", "Index", c => c.Int());
            AlterColumn("dbo.Archives", "WindDirection", c => c.Int());
            AlterColumn("dbo.Archives", "WindSpeed", c => c.Int());
            AlterColumn("dbo.Archives", "PrecipitationSum", c => c.Double());
            AlterColumn("dbo.Archives", "Temperature", c => c.Double());
            AlterColumn("dbo.Archives", "Humidity", c => c.Int());
            AlterColumn("dbo.Archives", "Pressure", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Archives", "Pressure", c => c.Double(nullable: false));
            AlterColumn("dbo.Archives", "Humidity", c => c.Int(nullable: false));
            AlterColumn("dbo.Archives", "Temperature", c => c.Double(nullable: false));
            AlterColumn("dbo.Archives", "PrecipitationSum", c => c.Double(nullable: false));
            AlterColumn("dbo.Archives", "WindSpeed", c => c.Int(nullable: false));
            AlterColumn("dbo.Archives", "WindDirection", c => c.Int(nullable: false));
            AlterColumn("dbo.Archives", "Index", c => c.Int(nullable: false));
            AlterColumn("dbo.Archives", "Cloudiness", c => c.Int(nullable: false));
            AlterColumn("dbo.Archives", "Time", c => c.Int(nullable: false));
            AlterColumn("dbo.Archives", "Day", c => c.Int(nullable: false));
            AlterColumn("dbo.Archives", "Month", c => c.Int(nullable: false));
            AlterColumn("dbo.Archives", "Year", c => c.Int(nullable: false));
            AlterColumn("dbo.Archives", "GreenwichMeanTime", c => c.Int(nullable: false));
            AlterColumn("dbo.Archives", "GreenwichDay", c => c.Int(nullable: false));
            AlterColumn("dbo.Archives", "GreenwichMonth", c => c.Int(nullable: false));
            AlterColumn("dbo.Archives", "GreenwichYear", c => c.Int(nullable: false));
            AlterColumn("dbo.Archives", "StationIndex", c => c.Int(nullable: false));
        }
    }
}
