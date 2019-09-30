using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using PRBD_Framework;
using prbd_1819_g18;
using System.ComponentModel;

namespace prbd_1819_g18
{
    /// <summary>
    /// Interaction logic for UserList.xaml
    /// </summary>
    /// 

   
    public partial class UserList : UserControlBase
    {

        private string filter;
        public string Filter
        {
            get => filter;
            set => SetProperty<string>(ref filter, value, ApplyFilterAction);
        }

        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            get => users;
            set => SetProperty<ObservableCollection<User>>(ref users, value, () => {
            });
        }

        

        private User selectdItem;
        public User SelectdItem
        {
            get => selectdItem;
            set => SetProperty<User>(ref selectdItem, value, ApplyFilterAction);
        }


        public ICommand Clear { get; set; }

        public ICommand Remove { get; set; }
        public ICommand action { get; set; }

        public UserList()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            DataContext = this;
            Users = new ObservableCollection<User>(App.Model.Users);
            GetCommand();
           

        }


        public void GetCommand()
        {
            Clear = new RelayCommand(ClearFilter);
            Remove = new RelayCommand(RemoveUser);
        }


        public void ApplyFilterAction() {

            IEnumerable<User> query = App.Model.Users;
            if (!string.IsNullOrEmpty(Filter))
            {

                query = from m in App.Model.Users
                        where
                            m.UserName.Contains(Filter) 
                        select m;
                Users = new ObservableCollection<User>(query);
            }
            else
            {
                Users = new ObservableCollection<User>(App.Model.Users);
            }
        }

        public void ClearFilter()
        {

            Filter = "";
            Users = new ObservableCollection<User>(App.Model.Users);
            Console.WriteLine(selectdItem);
        }

        public void  RemoveUser() {
            User.RemoveUser(selectdItem);
            Users = new ObservableCollection<User>(App.Model.Users); 
            ApplyFilterAction();
        }
    }
}
