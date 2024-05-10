using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Mess.core;
using Mess.mvvm.model;
using Mess.Net;

namespace Mess.mvvm.viewmodel
{
  
    class Mainviewmodel :ObservableObject
        
    { public string Message { get; set; }
        public ObservableCollection<UserModel> Users { get; set; }
        public string Username { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }

        private Server _server;
        
       // public RelayCommand SendMessageCommand { get; set; }
        private ContactModel _selectedContact;
        public ContactModel SelectedContact
        {
            get { return _selectedContact; }
            set { _selectedContact = value; OnPropertyChanged(); }
        }
        public ObservableCollection<string> Messages { get; set; }
        public ObservableCollection<ContactModel> Contacts { get; set; }
        
        
       

       // private String _message;

       /* public String Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged(); }
            
        }*/

        public Mainviewmodel()

        { Users = new ObservableCollection<UserModel>(); 
            _server = new Server();
            _server.connectedEvent += UserConnected;
            _server.msgRecivedEvent += MessageRecived;
            _server.UserDisconnectEvent += RemoveUser;
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username));
            SendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(Message));
            Messages = new ObservableCollection<string>();
            Contacts = new ObservableCollection<ContactModel>();
        
         
        }
        private void MessageRecived()
        {
            var msg = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }
        private void RemoveUser()
        {
            var uid =_server.PacketReader.ReadMessage();
            var user = Users.Where(x => x.UID== uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }
        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.PacketReader.ReadMessage(),
                UID = _server.PacketReader.ReadMessage(),

            };
            if (!Users.Any(x => x.UID == user.UID)) { 
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            
            
            }

        }
    }

}
