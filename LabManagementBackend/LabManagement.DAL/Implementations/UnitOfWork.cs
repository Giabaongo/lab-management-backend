using System.Threading;
using System.Threading.Tasks;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;

namespace LabManagement.DAL.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LabManagementDbContext _context;
        private IUserRepository? _userRepository;

        public UnitOfWork(LabManagementDbContext context)
        {
            _context = context;
        }

        public IUserRepository Users => _userRepository ??= new UserRepository(_context);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}


