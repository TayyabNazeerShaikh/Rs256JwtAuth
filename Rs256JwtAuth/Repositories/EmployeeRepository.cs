using Rs256JwtAuth.Models;

namespace Rs256JwtAuth.Repositories
{
    public class EmployeeRepository
    {
        private readonly List<Employee> _employees = new()
{
new Employee{ Id = 1, Name = "Admin User", Email = "admin@test.com", Role = "Admin" },
new Employee{ Id = 2, Name = "Normal User", Email = "user@test.com", Role = "Employee" }
};


        public List<Employee> GetAll() => _employees;


        public Employee? Get(int id) => _employees.FirstOrDefault(x => x.Id == id);


        public void Add(Employee e)
        {
            e.Id = _employees.Any() ? _employees.Max(x => x.Id) + 1 : 1;
            _employees.Add(e);
        }


        public bool Update(Employee e)
        {
            var existing = Get(e.Id);
            if (existing == null) return false;
            existing.Name = e.Name;
            existing.Email = e.Email;
            existing.Role = e.Role;
            return true;
        }


        public bool Delete(int id)
        {
            var emp = Get(id);
            return emp != null && _employees.Remove(emp);
        }
    }
}
