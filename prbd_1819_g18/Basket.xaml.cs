using PRBD_Framework;
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
using System.Windows.Shapes;

namespace prbd_1819_g18
{
    /// <summary>
    /// Logique d'interaction pour Basket.xaml
    /// </summary>
    public partial class Basket : UserControlBase
    {

        //public bool IsAdmin
        //{ get { return App.CheckAdmin(); } }



        private List<User> userList;
        public List<User> UserList
        {
            get => userList;
            set => SetProperty<List<User>>(ref userList, value);

        }

        private List<RentalItem> rentalItems;
        public List<RentalItem> RentalItems
        {
            get => rentalItems;
            set => SetProperty<List<RentalItem>>(ref rentalItems, value);

        }

        public User userSelected;
        public User UserSelected { get => userSelected; set => SetProperty<User>(ref userSelected, value, () => echo()); }

        public string Visibile { get; set; }

        public ICommand UserBasketFilter { get; set; }
        public ICommand Clear { get; set; }
        public ICommand Confirm { get; set; }
        public ICommand Delete { get; set; }

        public Basket()
        {
            InitializeComponent();

            DataContext = this;

            getRental();

            UserSelected = App.CurrentUser;
            if (App.CurrentUser.Role == Role.Admin)
            {
                
                UserList = App.Model.GetUsers();
                Visibile = "Visible";
              

            }
            else
            {
                UserList = new List<User>();
                UserList.Add(App.CurrentUser);
                Visibile = "Hidden";
            }


            refrechRental();

            UserBasketFilter = new RelayCommand(()=> {
                if (BasketNull())
                {
                    UserSelected.CreateBasket();

                }
                
                    RentalItems = new List<RentalItem>(UserSelected.Basket.Items);
                
            
            });

            Clear = new RelayCommand(()=> {
                UserSelected.ClearBasket();
                RentalItems = new List<RentalItem>(UserSelected.Basket.Items);
                App.NotifyColleagues(AppMessages.MSG_REFRESH_BOOK_LIST);
                App.Model.SaveChanges();
            });

            Confirm = new RelayCommand(()=> {
                UserSelected.ConfirmBasket();
                UserSelected.CreateBasket();
                RentalItems = new List<RentalItem>(UserSelected.Basket.Items);
                App.NotifyColleagues(AppMessages.MSG_REFRESH_RENTAL);
            });

            Delete = new RelayCommand<RentalItem>(item =>
            {

                UserSelected.RemoveFromBasket(item);
                App.Model.RentalItems.Remove(item);
                RentalItems = new List<RentalItem>(UserSelected.Basket.Items);
                App.NotifyColleagues(AppMessages.MSG_REFRESH_BOOK_LIST);
                App.Model.SaveChanges();
            });
        }

        public void echo()
        {
            Console.WriteLine("USer : " + UserSelected.UserName);
        }


        private bool BasketNull()
        {
            return UserSelected.Basket == null;
        }

        public void getRentalItems() {

        }

        public void getRental()
        {
            foreach(var t in App.Model.RentalItems) {
                Console.WriteLine("LES RENTALITEMS:"+t);
            }
        }

        public void refrechRental()
        {
            App.Register<Book>(this, AppMessages.MSG_ADDBASKET, book =>
            {
                UserSelected.AddToBasket(book);
                RentalItems = new List<RentalItem>(UserSelected.Basket.Items);
            });


        }

        


    }

   




}
