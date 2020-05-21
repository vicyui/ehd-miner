namespace EHDMiner
{
    class Node
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public bool Access { get; set; }
        public Node(string name,string address)
        {
            Name = name;
            Address = address;
            Access = false;
        }
    }
}
