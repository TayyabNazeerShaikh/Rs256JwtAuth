using Rs256JwtAuth.Models;

namespace Rs256JwtAuth.Repositories
{
    public class InventoryRepository
    {
        private readonly List<InventoryItem> _items = new()
        {
        new InventoryItem{ Id = 1, Name = "Mouse", Quantity = 100 },
        new InventoryItem{ Id = 2, Name = "Keyboard", Quantity = 50 }
        };


        public List<InventoryItem> GetAll() => _items;


        public void Add(InventoryItem item)
        {
            item.Id = _items.Any() ? _items.Max(x => x.Id) + 1 : 1;
            _items.Add(item);
        }


        public bool Remove(int id)
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            return item != null && _items.Remove(item);
        }
    }
}
