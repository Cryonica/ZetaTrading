using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZetaTrading.Exceptions;
using ZetaTrading.Models;
using ZetaTrading.Services;

namespace ZetaTrading.Controllers
{
    public class NodeController : ControllerBase
    {

        private readonly IJournalService _journalService;
        private readonly TreeService _treeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public NodeController(IJournalService journalService, IHttpContextAccessor httpContextAccessor, TreeService treeService)
        {
            _journalService = journalService;
            _treeService = treeService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet("Node{id}")]
        public async Task<IActionResult> GetNode(int id)
        {
            try
            {
                var node = await _treeService.GetNodeByIdAsync(id);
                if (node == null) throw new Exception($"no node with ID= {id}");
                return Ok(node);
            }
            catch (Exception ex)
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                var requestString = $"{request?.Path}{request?.QueryString}";
                SecureException journal = _journalService.AddExceptionToJournal(ex, new string[] { "Bad request", requestString });
                return StatusCode(500, journal);
            }
        }
        [HttpGet("TreeByNode{id}")]
        public async Task<IActionResult> GetTree(int id)
        {
            try
            {
                var nodes = await _treeService.GetTreeByNodeAsync(id);
                if (nodes == null) throw new Exception($"no node with ID= {id}");
                return Ok(nodes);
            }
            catch (Exception ex)
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                var requestString = $"{request?.Path}{request?.QueryString}";
                SecureException journal = _journalService.AddExceptionToJournal(ex, new string[] { "Bad request", requestString });
                return StatusCode(500, journal);
            }

        }


        [HttpPut("Update{id:int}")]
        public async Task<IActionResult> Update(int id, string newDescription)
        {
            
            try
            {
                var nodeForUpdate = await _treeService.Update(id, newDescription);
                if (nodeForUpdate == null) throw new Exception($"no node with ID= {id}");
                return Ok(nodeForUpdate);
            }
            catch (Exception ex)
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                var requestString = $"{request?.Path}{request?.QueryString}";
                SecureException journal = _journalService.AddExceptionToJournal(ex, new string[] { "Notyfy", requestString });
                return StatusCode(500, journal);
            }
        }

        [HttpDelete("Delete{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            
            try
            {
                var deleteStatus = await _treeService.Delete(id);

                if (deleteStatus == TreeService.DeleteStatus.PresentChilds)
                {
                    throw new Exception($"You have to delete all children nodes first");
                }
                if (deleteStatus == TreeService.DeleteStatus.NotFound) return NotFound();
                return StatusCode(200, deleteStatus.ToString());
            }
            catch (Exception ex)
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                var requestString = $"{request?.Path}{request?.QueryString}";
                //SecureException journal = _journalService.AddExceptionToJournal(ex, new string[] { "Secure", requestString });
                return StatusCode(500, "Secure");
            }
            return Ok();
        }

        [HttpPost("CreateNode")]
        public async Task<IActionResult> Create(string NodeName, string Description, int ParentId, string TreeName, int TreeId )
        {
            try
            {
                NodeConfiguration nodeConfiguration;
                if (ParentId == 0)
                {
                    nodeConfiguration = new NodeConfiguration { Name = NodeName, Description = Description, Tree = new Tree { Id = TreeId, Name = TreeName } };
                }
                else
                {
                    nodeConfiguration = new NodeConfiguration { Name = NodeName, Description = Description, ParentId = ParentId, Tree = new Tree { Id = TreeId, Name = TreeName } };
                }
                if (nodeConfiguration == null) throw new Exception("unable to create node");
                await _treeService.CreateNode(nodeConfiguration);
                return Ok(nodeConfiguration);
            }
            catch (Exception ex)
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                var requestString = $"{request?.Path}{request?.QueryString}";
                SecureException journal = _journalService.AddExceptionToJournal(ex, new string[] { "Bad request", requestString });
                return StatusCode(500, journal);
            }
        }

    }
}
