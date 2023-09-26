namespace FileStorage.Models.Outcoming.Statistic
{
    public class StatisticTreeDto
    {
        public List<Node> Nodes { get; set; } = new List<Node>();
        public List<Link> Links { get; set; } = new List<Link>();
    }

    public class Node
    {
        public string id { get; set; } = null!;
        public int height { get; set; }
        public int size { get; set; }
        public string color { get; set; } = null!;
    }

     public class Link
    {
        public string source { get; set; } = null!;
        public string target { get; set; } = null!;
        public int distance { get; set; }
    }
}
