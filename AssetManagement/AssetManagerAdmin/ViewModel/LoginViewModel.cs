using System.Collections.Generic;
using System.Linq;
using AssetManagerAdmin.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace AssetManagerAdmin.ViewModel
{
    public class LoginViewModel : ViewModelBase, ICommonViewModel
    {
        private readonly IDataService _dataService;

        #region Properties

        public const string ServersListPropertyName = "ServersList";
        private List<ServerConfig> _serversList;

        public List<ServerConfig> ServersList
        {
            get { return _serversList; }

            set
            {
                _serversList = value;
                RaisePropertyChanged(ServersListPropertyName);
            }
        }

        public const string SelectedServerPropertyName = "SelectedServer";

        public ServerConfig SelectedServer
        {
            get { return _dataService.SelectedServer; }

            set
            {
                _dataService.SelectedServer = value;
                RaisePropertyChanged(SelectedServerPropertyName);
            }
        }

        #endregion

        private RelayCommand _loginCommand;

        public RelayCommand LoginCommand
        {
            get
            {
                return _loginCommand ??
                       (_loginCommand =
                           new RelayCommand(ExecuteLoginCommand));
            }
        }

        private void ExecuteLoginCommand()
        {
            MessengerInstance.Send(_dataService.SelectedServer, AppActions.LoggingIn);
        }

        public LoginViewModel(IDataService dataService)
        {
            _dataService = dataService;

            ServersList = AppConfig.GetServersList().ToList();

            SelectedServer = ServersList.First();

#if DEBUG_MODE
            SelectedServer = ServersList[1];
            ExecuteLoginCommand();
#endif
        }

        public bool IsActive { get; set; }
    }
}