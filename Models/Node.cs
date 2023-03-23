using System.ComponentModel.DataAnnotations;

namespace ZetaTrading.Models
{
    public class Node
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int? ParentId { get; set; }
        public Node Parent { get; set; }

        public int TreeId { get; set; }
        public Tree Tree { get; set; }
    }
}
