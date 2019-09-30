using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace prbd_1819_g18
{
    /// <summary>
    /// Logique d'interaction pour LoginView.xaml
    /// </summary>
    public partial class LoginView : WindowBase
    {

        public string pseudo;

        public string Pseudo { get => pseudo; set => SetProperty<string>(ref pseudo, value, () => Validate()); }

        public string password;

        public string Password { get => password; set => SetProperty<string>(ref password, value, () => Validate()); }

        public ICommand Login { get; set; }
        public ICommand Cancel { get; set; }

        public LoginView()
        {
            InitializeComponent();
            DataContext = this;
            Login = new RelayCommand(LoginAction,() =>
                      { return pseudo != null && password != null && !HasErrors; });
            Cancel = new RelayCommand(() => Close());
        }

        


        public override bool Validate (){

            ClearErrors();
            if (string.IsNullOrEmpty(Pseudo))
            {
                AddError("Pseudo", Properties.Resources.Error_Required);
            }
            else
            {
                if (Pseudo.Length < 3)
                {
                    AddError("Pseudo", Properties.Resources.Error_LengthGreaterEqual3);
                }
                else
                {
                    if (ValidateUser())
                    {
                        AddError("Pseudo", Properties.Resources.Error_DoesNotExist);
                      
                    }
                    if (ValidatePassword()) {
                         AddError("Password", Properties.Resources.Error_Wrong_Pass);
                    }

                }
                
            }

           

            RaiseErrors();
            return !HasErrors;

        }


        public bool ValidateUser() {

            return (from m in App.Model.Users
                    where m.UserName == pseudo 
                    select m).FirstOrDefault()==null;

          
        }


        public bool ValidatePassword()
        {

            return (from m in App.Model.Users
                    where m.UserName == pseudo && m.Password==password
                    select m).FirstOrDefault() == null;


        }
        private void LoginAction()
        {
            if (Validate())
            {
                var member = App.Model.Users.Find(getUserId());
                App.CurrentUser = member;
                ShowMainView();
                Close();
            }
        }

        private static void ShowMainView()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            Application.Current.MainWindow = mainWindow;
        }

        public int getUserId()
        {
            return (from m in App.Model.Users
                    where m.UserName == pseudo 
                    select m.UserId).FirstOrDefault();

        }

    }
}
