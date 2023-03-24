using Prism.Commands;
using Prism.Mvvm;

namespace mutiWindowSync.ViewModels
{
    public class BaseViewModel : BindableBase
    {
        private DelegateCommand _loadingCommand;
        public DelegateCommand LoadingCommand =>
            _loadingCommand ??= new DelegateCommand(ExecuteLoadingCommandAsync);
        
        protected virtual void ExecuteLoadingCommandAsync()
        {
        }
    }
}