namespace EHDMiner
{
    public class Node
    {
        public int Id { get; set; }
        public string Name_zh { get; set; }
        public string Name_en { get; set; }
        public string Address { get; set; }
        public int Access { get; set; }
        public int On_used { get; set; }
        public string End_date { get; set; }
        /*public Node(int id ,string name_zh, string name_en, string address)
        {
            Id = id;
            Name_zh = name_zh;
            Name_en = name_en;
            Address = address;
            Access = 0;
        }
        public Node(int id, string name_zh, string name_en, string address,int flag)
        {
            Id = id;
            Name_zh = name_zh;
            Name_en = name_en;
            Address = address;
            Access = flag;
            On_used = 1;
        }*/
    }
}
