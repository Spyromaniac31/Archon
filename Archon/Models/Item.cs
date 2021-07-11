namespace Archon.Models
{
    public class Item : Entity
    {
        public bool Craftable { get; set; }
        public bool Harvestable { get; set; }

        public Item(string name, string id, bool craftable, bool harvestable) : base(name, id)
        {
            Craftable = craftable;
            Harvestable = harvestable;
        }
    }
}
