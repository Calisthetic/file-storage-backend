using FileStorage.Data;
using FileStorage.Models.Db;
using FileStorage.Models.Outcoming.File;
using FileStorage.Models.Outcoming.Folder;
using FileStorage.Models.Outcoming.Statistic;
using FileStorage.Services;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.Controllers
{
    [Route("api/statistic")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public StatisticController(ApiDbContext context, IMapper mapper, IUserService userService)
        {
            _context = context;
            _mapper = mapper;
            _userService = userService;
        }

        // GET: api/statistic/tree
        [HttpGet("tree")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<StatisticTreeDto>> GetFoldersTree()
        {
            if (_context.Files == null || _context.Folders == null)
            {
                return NotFound();
            }
            
            // If user unauthorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            var files = await _mapper.From(
                _context.Files.Where(x => x.UserId == userId && x.IsDeleted == false && x.FolderId == null)
            ).ProjectToType<FileTreeDto>().ToListAsync();
            var folders = await _mapper.From(
                _context.Folders.Include(x => x.InverseUpperFolder).Include(x => x.Files)
                .Where(x => x.UserId == userId && x.IsDeleted == false && x.UpperFolderId == null)
            ).ProjectToType<FolderTreeDto>().ToListAsync();
            if (folders == null) { return NotFound(); }

            return Ok(ConvertToTree(new FolderTreeDto()
            {
                Name = "main",
                Folders = folders,
                Files = files,
            }, 1));
        }
        private static StatisticTreeDto ConvertToTree(FolderTreeDto folder, int depth)
        {
            var result = new StatisticTreeDto() { Nodes = new List<Node>() { new Node() { 
                id = depth.ToString() + ": " + folder.Name,
                height = 1,
                size = folder.Name == "main" ? 32 : 24,
                color = folder.Name == "main" ? "var(--treeMain)" : "var(--treeFolder)"
            } } };
            foreach (FileTreeDto f in folder.Files)
            {
                result.Nodes.Add(new Node() { id = f.Name, height = 1, size = 18, color = "var(--treeFile)" });
                result.Links.Add(new Link() { source = depth.ToString() + ": " + folder.Name, target = f.Name, distance = 40 });
            }
            foreach (FolderTreeDto f in folder.Folders)
            {
                var temp = ConvertToTree(f, depth + 1);
                result.Nodes.AddRange(temp.Nodes);
                result.Links.AddRange(temp.Links);
                result.Links.Add(new Link() { 
                    source = depth.ToString() + ": " + folder.Name, 
                    target = (depth + 1).ToString() + ": " + f.Name, 
                    distance = 80 
                });
            }
            return result;
        }

        // GET: api/statistic/pie
        [HttpGet("pie")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<StatisticTreeDto>> GetFilesPie()
        {
            if (_context.FileTypes == null)
            {
                return NotFound();
            }

            // If user unauthorized
            if (!int.TryParse(_userService.GetUserId(), out int userId))
            {
                return Unauthorized();
            }

            var files = await _context.Files.Where(x => x.UserId == userId && x.IsDeleted == false).ToListAsync();
            if (files == null) { return NotFound(); }

            var result = new List<PieDto>();
            for (int i = 0; i < files.Count; i++)
            {
                string type = files[i].Name[files[i].Name.LastIndexOf('.')..];
                if (result.Where(x => x.Id == type).Count() == 0)
                {
                    result.Add(new PieDto()
                    {
                        Id = type,
                        Label = type,
                        Value = files[i].FileSize
                    });
                }
            }
            return Ok(files);
        }
    }
}
