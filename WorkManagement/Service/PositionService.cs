using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkManagement.Data;
using WorkManagement.Interfaces;
using WorkManagement.Models;

namespace WorkManagement.Service
{
    public class PositionService : IPositionService
    {
        private readonly WorkManagementContext _context;

        public PositionService(WorkManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Position>> GetAll()
        {
            return await _context.Positions.ToListAsync();
        }

        public async Task<Position> GetById(int id)
        {
            return await _context.Positions.Include(a => a.Users).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task Add(Position position)
        {
            await _context.Set<Position>().AddAsync(position);
            await _context.SaveChangesAsync();
        }

        public async Task Update(int id, Position position)
        {
            var existingPosition = await _context.Positions.FindAsync(position.Id);
            if (existingPosition != null)
            {
                existingPosition.Name = position.Name;
                existingPosition.Description = position.Description;
                await _context.SaveChangesAsync();
            }
        }

        public async Task Delete(int id)
        {
            var position = await _context.Set<Position>().FindAsync(id);
            if (position != null)
            {
                _context.Set<Position>().Remove(position);
                await _context.SaveChangesAsync();
            }
        }
    }
}
