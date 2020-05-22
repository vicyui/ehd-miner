namespace EHDMiner
{
    public class Node
    {
        public string  NodeId { get; set; }
        public string Zh_name { get; set; }
        public string En_name { get; set; }
        public string Address { get; set; }
        public bool Access { get; set; }
        public Node(string id ,string zh_name, string en_name, string address)
        {
            NodeId = id;
            Zh_name = zh_name;
            En_name = en_name;
            Address = address;
            Access = false;
        }
        public Node(string id, string zh_name, string en_name, string address,bool flag)
        {
            NodeId = id;
            Zh_name = zh_name;
            En_name = en_name;
            Address = address;
            Access = flag;
        }
    }
}
