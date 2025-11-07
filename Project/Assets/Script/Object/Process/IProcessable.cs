using System.Collections;

namespace Backend.Object.Process
{
    public interface IProcessable
    {
        public IEnumerator Running();
    }
}
