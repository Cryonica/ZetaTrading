using Microsoft.EntityFrameworkCore;
using ZetaTrading.Data;
using ZetaTrading.Models;

namespace ZetaTrading.Services
{
    public class TreeService
    {
        private readonly ApplicationDbContext _dbContext;
        public enum DeleteStatus
        {
            PresentChilds,
            NotFound,
            Ok
        }

        public TreeService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void CreateNodes(List<NodeConfiguration> nodeConfigurations)
        {
            var trees = new Dictionary<int, Tree>();

            foreach (var configuration in nodeConfigurations)
            {
                if (!trees.TryGetValue(configuration.Tree.Id, out var tree))
                {
                    tree = new Tree { Name = configuration.Tree.Name };
                    trees.Add(configuration.Tree.Id, tree);
                    _dbContext.Set<Tree>().Add(tree);
                }

                var node = new Node
                {
                    Name = configuration.Name,
                    Description = configuration.Description,
                    Tree = tree
                };

                if (configuration.ParentId.HasValue)
                {
                    node.ParentId = configuration.ParentId.Value;
                }

                _dbContext.Set<Node>().Add(node);
            }

            _dbContext.SaveChanges();
        }
        public IList<Node> GetNodes(int parentId)
        {
            var result = new List<Node>();

            using (var context = new ApplicationDbContext())
            {
                // retrieve nodes with the specified parent ID
                var nodes = context.Nodes.Where(n => n.ParentId == parentId).ToList();

                // iterate over the retrieved nodes and add them to the resulting list
                foreach (var node in nodes)
                {
                    result.Add(node);

                    // recursively call GetNodes to add child nodes
                    var childNodes = GetNodes(node.Id);
                    if (childNodes.Any())
                    {
                        result.AddRange(childNodes);
                    }
                }
            }

            return result;
        }
        public async Task<Node?> GetNodeByIdAsync(int nodeId)
        {
            using (var context = new ApplicationDbContext())
            {
                var node = await context.Nodes.FindAsync(nodeId);
                if (node == null)
                {
                    return null;
                }
                return node;
            }
        }
        public async Task<List<TreeNodes>?> GetTreeByNodeAsync(int nodeId)
        {
            using (var context = new ApplicationDbContext())
            {
                var node = await context.Nodes.FindAsync(nodeId);
                if (node == null)
                {
                    return null;
                }
                var tree = await context.Trees.FindAsync(node.TreeId);

                if (tree == null) return null;

                var rootNodes = await context.Nodes
                    .Where(n => n.TreeId == tree.Id)
                    .OrderBy(n => n.Id)
                    .ToArrayAsync();

                List<TreeNodes> treeNodes = new();

                foreach (var rootNode in rootNodes)
                {
                    var childNodes = await context.Nodes.Where(c => c.ParentId == rootNode.Id).ToArrayAsync();
                    if (childNodes != null)
                    {
                        if (childNodes.Count()>1)
                        {
                            var branch = childNodes.Where(cn => cn.TreeId != rootNode.TreeId).FirstOrDefault();
                            if (branch != null)
                            {
                                treeNodes.Add(
                                 new TreeNodes { Description = rootNode.Description, Name = rootNode.Name, ParentId = rootNode.ParentId, MainTreeID = tree.Id, BranchID = branch.TreeId }
                                );
                            }
                        }
                        else
                        {

                            var childNode = childNodes.FirstOrDefault();
                            if (childNode != null && childNode.TreeId != rootNode.TreeId)
                            {
                                treeNodes.Add(
                                    new TreeNodes { Description = rootNode.Description, Name = rootNode.Name, ParentId = rootNode.ParentId, MainTreeID = tree.Id, BranchID = childNode.TreeId });
                            }

                            treeNodes.Add(
                                    new TreeNodes { Description = rootNode.Description, Name = rootNode.Name, ParentId = rootNode.ParentId, MainTreeID = tree.Id });

                        }
                        
                    }                   

                }
                return treeNodes;
            }
        }
        public async Task<DeleteStatus> Delete(int nodeId)
        {
            using (var context = new ApplicationDbContext())
            {

                //check child node

                var childNode = await context.Nodes
                    .Where(n => n.ParentId == nodeId)
                    .FirstOrDefaultAsync();
                if (childNode != null) return DeleteStatus.PresentChilds;

                //find node for delete

                var nodeForDelete = context.Find<Node>(nodeId);
                if (nodeForDelete != null)
                {
                    context.Remove(nodeForDelete);
                    context.SaveChanges();
                    return DeleteStatus.Ok;
                }
                else
                {
                    return DeleteStatus.NotFound;
                }


            }
        }
        public async Task<Node> Update(int nodeId, string newVal)
        {
            using (var context = new ApplicationDbContext())
            {
                var nodeForUpdate = context.Find<Node>(nodeId);
                if (nodeForUpdate != null)
                {
                    nodeForUpdate.Description = newVal;
                    context.Entry(nodeForUpdate).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    return nodeForUpdate;
                }
                else return null;

            }


            return null;
        }
        public async Task<bool> CreateNode(NodeConfiguration nodeConfiguration)
        {
            await Task.Factory.StartNew(() => CreateNodes(new List<NodeConfiguration> { nodeConfiguration }));
            return true;
        }
    }
}
