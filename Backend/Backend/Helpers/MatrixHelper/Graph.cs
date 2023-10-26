namespace Backend.Helpers.MatrixHelper
{
    public class Graph
    {
        public Node Root;
        public HashSet<Node> AllNodes = new HashSet<Node>();

        public Node CreateRoot(string id)
        {
            Root = CreateNode(id);
            return Root;
        }

        public Node CreateNode(string id)
        {
            var n = new Node(id);
            AllNodes.Add(n);
            return n;
        }

        public int?[,] CreateAdjMatrix()
        {
            int?[,] adj = new int?[AllNodes.Count, AllNodes.Count];

            for (int i = 0; i < AllNodes.Count; i++)
            {
                Node n1 = AllNodes.ElementAt(i);

                for (int j = 0; j < AllNodes.Count; j++)
                {
                    Node n2 = AllNodes.ElementAt(j);

                    var arc = n1.Arcs.FirstOrDefault(a => a.Child == n2);

                    if (arc != null)
                    {
                        adj[i, j] = arc.Weigth;
                    }
                }
            }
            return adj;
        }

        public HashSet<Node> SortAdjMatrix()
        {
            List<Node> listNodeNotSorted = new List<Node>();
            List<Node> listNodeSorted = new List<Node>();

            // copy list node
            listNodeNotSorted.AddRange(AllNodes);
            listNodeSorted.AddRange(listNodeNotSorted.OrderByDescending(x => x.Arcs.Count)
                .ThenByDescending(x => x.MaxWeight())
                .ThenBy(x => x.Id));

            return listNodeSorted.ToHashSet();
        }
    }
}
