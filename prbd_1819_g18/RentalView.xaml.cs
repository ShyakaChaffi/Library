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

namespace prbd_1819_g18
{
    /// <summary>
    /// Logique d'interaction pour Rental.xaml
    /// </summary>
    public partial class RentalView : UserControlBase {

        //public bool IsAdmin
        //{
        //    get { return App.CheckAdmin(); }
        //}


        public bool isClicked = true;
        private ObservableCollection<Rental> rented;
    public ObservableCollection<Rental> Rented
    {
        get => rented;
        set => SetProperty<ObservableCollection<Rental>>(ref rented, value);

    }

        private ObservableCollection<RentalItem> rentedItem;
        public ObservableCollection<RentalItem> RentedItem
        {
            get => rentedItem;
            set => SetProperty<ObservableCollection<RentalItem>>(ref rentedItem, value);

        }
         public string Enable { get; set; }

        public ICommand Test { get; set; }
        public ICommand Return { get; set; }
        public ICommand Delete { get; set; }
        public ICommand SelectedBasket { get; set; }
        public ICommand SelectedRentalItems { get; set; }
        public Rental Selected { get; set; }

        
        public RentalView()
        {

            InitializeComponent();
            DataContext = this;

            if (App.CurrentUser.Role != Role.Admin)
            {
                Enable = "Hidden";
                Rented = new ObservableCollection<Rental>(selectUserItem());
                


            }
            else {
                Rented = new ObservableCollection<Rental>(selectRightItem());
                Test = new RelayCommand(() =>
                {
                    Console.WriteLine("coucou");
                });

               

                Return = new RelayCommand<RentalItem>(item => {
                    if (isClicked)
                    {
                        item.DoReturn();
                        isClicked = false;
                    }
                    else {
                        item.CancelReturn();
                        isClicked = true;
                    }
                    RentedItem = new ObservableCollection<RentalItem>(Selected.Items);
                    Rented = new ObservableCollection<Rental>(selectRightItem());
                    App.Model.SaveChanges();

                });

                Delete = new RelayCommand<RentalItem>(item => {
                    App.Model.RentalItems.Remove(item);
                    App.NotifyColleagues(AppMessages.MSG_REFRESH_BOOK_LIST);
                    Empty();
                    Rented = new ObservableCollection<Rental>(selectRightItem());
                    RentedItem = new ObservableCollection<RentalItem>();

                    if(Rented.Count() != 0 && Selected != null)
                    {
                        RentedItem = new ObservableCollection<RentalItem>(Selected.Items);
                    }
                    App.Model.SaveChanges();
                });


                SelectedRentalItems = new RelayCommand(() => {
                    Console.WriteLine("coucou");
                });

            }

            SelectedBasket = new RelayCommand(() =>
            {
                if(Rented.Count()>0)
                {
                    RentedItem = new ObservableCollection<RentalItem>(Selected.Items);
                }

            });

            App.Register(this, AppMessages.MSG_REFRESH_RENTAL, () => {
                if (App.CurrentUser.Role == Role.Admin)
                {
                    Rented = new ObservableCollection<Rental>(selectRightItem());
                }

                else
                {
                    Console.WriteLine("MAJ");
                    Rented = new ObservableCollection<Rental>(selectUserItem());
                }

            });


        }


        public void refresh() {

            

        }

        public List<Rental> selectRightItem() {
            var query = from t in App.Model.Rentals
                        where t.RentalDate != null
                        select t;

            return query.ToList(); 
        }

        public List<Rental> selectUserItem()
        {
            var query = from t in App.Model.Rentals
                        where t.RentalDate != null && t.User.UserName == App.CurrentUser.UserName
                        select t;

            return query.ToList();
        }

        private void Empty()
        {
            var Empt = new List<Rental>();
            foreach(var r in Rented)
            {
                if(r.Items.Count()==0)
                {
                    App.Model.Rentals.Remove(r);
                }
            }
            App.Model.SaveChanges();
        }
    }
}
