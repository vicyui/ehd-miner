namespace EHDMiner
{
    class Node
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public Node(string name,string address)
        {
            Name = name;
            Address = address;
        }
    }
}
