using System.Threading.Tasks;
using WorkManagement.Models;

namespace WorkManagement.Interfaces
{
    public interface IPositionService
    {
        Task<IEnumerable<Position>> GetAll();
        Task<Position> GetById(int id);
        Task Add(Position position);
        Task Update(int id, Position position);
        Task Delete(int id);
    }
}
