using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramB.Model;

namespace ProgramB.Repositories
{
    public interface IHandRepository
    {
        Hand GetHandById(string handId);
        Task<IEnumerable<Hand>> GetAllAsync();
        void Add(Hand hand);
        void Save();
    }
}