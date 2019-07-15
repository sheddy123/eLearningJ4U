namespace eLearningLMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DetailFile : DbMigration
    {
        public override void Up()
        {
            //RenameTable(name: "dbo.FileDetailds", newName: "DetailFiles");
            //DropForeignKey("dbo.FileDetailds", "Support_SupportId1", "dbo.Supports");
            //DropForeignKey("dbo.Supports", "FileDetail_Id", "dbo.FileDetailds");
            //DropForeignKey("dbo.FileDetailds", "Support_SupportId", "dbo.Supports");
            //DropIndex("dbo.Supports", new[] { "FileDetail_Id" });
            //DropIndex("dbo.DetailFiles", new[] { "Support_SupportId" });
            //DropIndex("dbo.DetailFiles", new[] { "Support_SupportId1" });
            //DropColumn("dbo.DetailFiles", "SupportId");
            //DropColumn("dbo.DetailFiles", "SupportId");
            //RenameColumn(table: "dbo.DetailFiles", name: "FileDetail_Id", newName: "SupportId");
            //RenameColumn(table: "dbo.DetailFiles", name: "Support_SupportId", newName: "SupportId");
            //AlterColumn("dbo.DetailFiles", "SupportId", c => c.Int(nullable: false));
            //CreateIndex("dbo.DetailFiles", "SupportId");
            //AddForeignKey("dbo.DetailFiles", "SupportId", "dbo.Supports", "SupportId", cascadeDelete: true);
            //DropColumn("dbo.Supports", "FileDetail_Id");
            //DropColumn("dbo.DetailFiles", "Support_SupportId1");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.DetailFiles", "Support_SupportId1", c => c.Int());
            //AddColumn("dbo.Supports", "FileDetail_Id", c => c.Guid());
            //DropForeignKey("dbo.DetailFiles", "SupportId", "dbo.Supports");
            //DropIndex("dbo.DetailFiles", new[] { "SupportId" });
            //AlterColumn("dbo.DetailFiles", "SupportId", c => c.Int());
            //RenameColumn(table: "dbo.DetailFiles", name: "SupportId", newName: "Support_SupportId");
            //RenameColumn(table: "dbo.DetailFiles", name: "SupportId", newName: "FileDetail_Id");
            //AddColumn("dbo.DetailFiles", "SupportId", c => c.Int(nullable: false));
            //AddColumn("dbo.DetailFiles", "SupportId", c => c.Int(nullable: false));
            //CreateIndex("dbo.DetailFiles", "Support_SupportId1");
            //CreateIndex("dbo.DetailFiles", "Support_SupportId");
            //CreateIndex("dbo.Supports", "FileDetail_Id");
            //AddForeignKey("dbo.FileDetailds", "Support_SupportId", "dbo.Supports", "SupportId");
            //AddForeignKey("dbo.Supports", "FileDetail_Id", "dbo.FileDetailds", "Id");
            //AddForeignKey("dbo.FileDetailds", "Support_SupportId1", "dbo.Supports", "SupportId");
            //RenameTable(name: "dbo.DetailFiles", newName: "FileDetailds");
        }
    }
}
