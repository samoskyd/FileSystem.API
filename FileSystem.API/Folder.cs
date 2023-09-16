using System.ComponentModel.DataAnnotations.Schema;

namespace FileSystem.API
{
    public class Folder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Path { get; set; }
        public int? ParentId { get; set; }
        public Folder? Parent { get; set; }
        public virtual List<Folder> Children { get; set; } = new List<Folder>();
    }

    public class FolderDto
    {
        public FolderDto(Folder folder)
        {
            this.Id = folder.Id;
            this.Name = folder.Name;
            this.ParentId = folder.Parent?.Id;

            var ChildrenId = new List<int>();
            if (folder.Children != null)
                foreach (var i in folder.Children)
                {
                    ChildrenId.Add(i.Id);
                }
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public List<int>? ChildrenId { get; set; }
    }

    public class XmlFolderDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }

}
