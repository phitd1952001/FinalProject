namespace Backend.Helpers.MatrixHelper
{
    public class Node
    {
        public string Id;
        public int CL = 1;
        public int Color = -1;
        public HashSet<Arc> Arcs = new HashSet<Arc>();

        public Node(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Create a new arc, connecting this Node to the Nod passed in the parameter
        /// Also, it creates the inversed node in the passed node
        /// </summary>
        public Node AddArc(Node child, int w)
        {
            if (!Arcs.Any(a => a.Child == child))
            {
                Arcs.Add(new Arc
                {
                    Parent = this,
                    Child = child,
                    Weigth = w
                });
            }

            if (!child.Arcs.Any(a => a.Parent == child && a.Child == this))
            {
                child.AddArc(this, w);
            }

            return this;
        }

        public int MaxWeight()
        {
            if (!Arcs.Any())
            {
                return 0;
            }
            var maxWeight = Arcs.OrderByDescending(i => i.Weigth).First().Weigth;
            return maxWeight;
        }
    }
}
