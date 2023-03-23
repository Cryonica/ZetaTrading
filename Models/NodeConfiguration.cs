namespace ZetaTrading.Models
{
    public class NodeConfiguration
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ParentId { get; set; }
        public Tree Tree { get; set; }
    }
}
