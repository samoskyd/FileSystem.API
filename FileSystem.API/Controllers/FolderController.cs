using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml;
using System.Xml.Serialization;

namespace FileSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        private readonly DataContext _context;

        public FolderController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<FolderDto>>> GetFolders()
        {
            var folders = await _context.Folders.Where(f => f.Parent == null).ToListAsync();
            if (folders == null)
                return NotFound();

            var res = new List<FolderDto>();
            res = FlistToDto(folders);
            return Ok(res);
        }

        [HttpGet("children/{id}")]
        public async Task<ActionResult<List<FolderDto>>> GetChildren(int id)
        {
            var folder = await _context.Folders
                .Include(f => f.Children)
                .FirstOrDefaultAsync(f => f.Id == id);
            
            if (folder == null)
                return NotFound();

            if (folder.Children == null)
                return NotFound();

            var children = new List<FolderDto>();
            children = FlistToDto(folder.Children);
            return Ok(children);
        }

        [HttpGet("neighbors/{id}")]
        public ActionResult<List<FolderDto>> GetNeighbors(int id)
        {
            var folder = _context.Folders
                .Include(f => f.Parent)
                .Where(f => f.Id == id)
                .FirstOrDefault();
            if (folder == null)
                return NotFound();

            List<Folder> neighbors = new List<Folder>();
            if (folder.Parent != null)
            {
                neighbors = _context.Folders
                    .Where(f => f.ParentId == folder.ParentId && f.Id != folder.Id)
                    .ToList();
            }

            var res = new List<FolderDto>();
            res = FlistToDto(neighbors);
            return Ok(res);
        }

        [HttpGet("parent/{id}")]
        public async Task<ActionResult<FolderDto>> GetParent(int id)
        {
            var folder = await _context.Folders
                .Include(f => f.Parent)
                .FirstOrDefaultAsync(f => f.Id == id);
            
            if (folder == null)
                return NotFound();

            if (folder.Parent == null)
                return NotFound();

            var res = new FolderDto(folder.Parent);
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FolderDto>> GetFolderById(int id)
        {
            var folder = await _context.Folders.FirstOrDefaultAsync(f => f.Id == id);

            if (folder == null)
                return NotFound();

            var res = new FolderDto(folder);
            return Ok(res);
        }

        [HttpPost]
        public ActionResult<Folder> CreateFolder(Folder folder)
        {
            if (folder == null) return BadRequest();
            _context.Folders.Add(folder);

            return Ok(folder);
        }

        //[HttpPost("import")]
        //public IActionResult ImportFoldersFromXml([FromBody] XmlDocument xmlDocument)
        //{
        //    try
        //    {
        //        var folders = new List<XmlFolderDto>();

        //        XmlNodeList folderNodes = xmlDocument.SelectNodes("//Folder");
        //        foreach (XmlNode folderNode in folderNodes)
        //        {
        //            var folder = new XmlFolderDto
        //            {
        //                Name = folderNode.SelectSingleNode("Name")?.InnerText,
        //                ParentId = folderNode.SelectSingleNode("ParentId") != null ? int.Parse(folderNode.SelectSingleNode("ParentId").InnerText) : null
        //            };
        //            folders.Add(folder);
        //        }

        //        var folderIdMapping = new Dictionary<int, Folder>();
        //        foreach (var folderDto in folders)
        //        {
        //            var folder = new Folder
        //            {
        //                Name = folderDto.Name,
        //                Parent = _context.Folders.FirstOrDefault(f => f.Id == folderDto.ParentId)
        //            };

        //            if (folderDto.ParentId.HasValue && folderIdMapping.ContainsKey(folderDto.ParentId.Value))
        //            {
        //                folder.Parent = folderIdMapping[folderDto.ParentId.Value];
        //                folder.Path = folder.Parent.Path + "/" + folder.Name;
        //                folder.Parent.Children ??= new List<Folder>();
        //                folder.Parent.Children.Add(folder);
        //            }
        //            else
        //            {
        //                folder.Path = folder.Name;
        //            }

        //            folderIdMapping[folderDto.Id] = folder;
        //            _context.Folders.Add(folder);
        //        }

        //        _context.SaveChanges();

        //        return Ok("Folders imported successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, "Error importing folders from XML.");
        //    }
        //}

        //[HttpGet("export")]
        //public IActionResult ExportFoldersToXml()
        //{
        //    try
        //    {
        //        var folders = _context.Folders.ToList();

        //        var xmlDtoList = folders.Select(folder => new XmlFolderDto
        //        {
        //            Id = folder.Id,
        //            Name = folder.Name,
        //            ParentId = folder.Parent?.Id
        //        }).ToList();

        //        var xmlSerializer = new XmlSerializer(typeof(List<XmlFolderDto>));

        //        using (var memoryStream = new MemoryStream())
        //        {
        //            using (var xmlWriter = XmlWriter.Create(memoryStream))
        //            {
        //                xmlSerializer.Serialize(xmlWriter, xmlDtoList);
        //                var xmlBytes = memoryStream.ToArray();

        //                return File(xmlBytes, "application/xml", "folders.xml");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, "Error exporting folders to XML.");
        //    }
        //}

        private List<FolderDto> FlistToDto(List<Folder> fl)
        {
            var dto_list = new List<FolderDto>();
            foreach (var item in fl)
            {
                var dto = new FolderDto(item);
                dto_list.Add(dto);
            }

            return dto_list;
        }
    }
}
