namespace Archon.Models
{
    public class Entity
    {
        public string Name { get; set; }
        public string Id { get; set; }

        public Entity(string name, string id)
        {
            Name = name;
            Id = id;
        }
    }
}
