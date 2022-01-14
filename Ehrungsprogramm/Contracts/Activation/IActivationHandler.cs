using System.Threading.Tasks;

namespace Ehrungsprogramm.Contracts.Activation
{
    public interface IActivationHandler
    {
        bool CanHandle();

        Task HandleAsync();
    }
}
