using System.Windows.Controls;
using mutiWindowSync.ViewModels;

namespace mutiWindowSync.Views
{
    public partial class HandleShow : UserControl
    {
        public HandleShow()
        {
            InitializeComponent();
        }
        
        private  HandleShowViewModel _dataContext;
        private HandleShowViewModel MDataContext
        {
            get { return _dataContext ??= (HandleShowViewModel) DataContext; }
        }
    }
}