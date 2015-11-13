using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace AssetManagerAdmin.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
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

        private ServerConfig _selectedServer;

        public const string SelectedServerPropertyName = "SelectedServer";

        public ServerConfig SelectedServer
        {
            get { return _selectedServer; }

            set
            {
                _selectedServer = value;
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
            MessengerInstance.Send(SelectedServer, AppActions.LoggingIn);
        }

        public LoginViewModel()
        {
            ServersList = AppConfig.GetServersList().ToList();
            SelectedServer = ServersList.First();
        }
    }
}