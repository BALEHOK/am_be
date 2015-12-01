using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using AssetManagerAdmin.Services;

namespace AssetManagerAdmin.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IFrameNavigationService _navigationService;

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
            _navigationService.NavigateTo(ViewModelLocator.AuthViewKey, SelectedServer);
        }

        public LoginViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
            ServersList = AppConfig.GetServersList().ToList();
            SelectedServer = ServersList.First();
        }
    }
}