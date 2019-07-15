namespace eLearningLMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FileDetai : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileDetais",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FileName = c.String(),
                        Extension = c.String(),
                        InstructorId = c.Int(nullable: false),
                        SupportId = c.Int(nullable: false),
                        CourseCode = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Instructors", t => t.InstructorId, cascadeDelete: true)
                .ForeignKey("dbo.Supports", t => t.SupportId, cascadeDelete: true)
                .Index(t => t.InstructorId)
                .Index(t => t.SupportId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FileDetais", "SupportId", "dbo.Supports");
            DropForeignKey("dbo.FileDetais", "InstructorId", "dbo.Instructors");
            DropIndex("dbo.FileDetais", new[] { "SupportId" });
            DropIndex("dbo.FileDetais", new[] { "InstructorId" });
            DropTable("dbo.FileDetais");
        }
    }
}
