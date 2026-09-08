using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Foundation.State
{
    public interface ITransition
    {
        public bool CheckTransition();
    }
}
