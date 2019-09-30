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
    public class BookBis
    {

        public Book book { get; set; }
        public string Title { get; set; }
        public bool Ischecked { get; set; }

        public BookBis(Book book, bool IsCheckeds)
        {

            this.book = book;
            this.Title = book.Title+"("+book.Author+")";
            this.Ischecked = IsCheckeds;
        }

    }
    /// <summary>
    /// Logique d'interaction pour RentalItemsByBook.xaml
    /// </summary>
    public partial class RentalItemsByBook : UserControlBase
    {
        private ObservableCollection<BookBis> bookList;
        public ObservableCollection<BookBis> BookList
        {
            get => bookList;
            set => SetProperty<ObservableCollection<BookBis>>(ref bookList, value);

        }
        private ObservableCollection<RentalItem> rentalItemsSelected;
        public ObservableCollection<RentalItem> RentalItemsSelected
        {
            get => rentalItemsSelected;
            set => SetProperty<ObservableCollection<RentalItem>>(ref rentalItemsSelected, value);

        }

        public BookBis BookSelected { get; set; }

      

        private void AddToBookList()
        {
            foreach (var b in App.Model.Books)
            {
                BookList.Add(new BookBis(b, false));
            }
        }

        public ICommand Change { get; set; }


        private void FillRentalItemsSelected(BookBis b)
        {
            var query = (from r in App.Model.RentalItems
                        where r.BookCopy.BookId.BookId == b.book.BookId && r.ReturnDate != null
                        select r).ToList();
            foreach (var ri in query)
            {
                if (!RentalItemsSelected.Contains(ri))
                    RentalItemsSelected.Add(ri);
                else
                    RentalItemsSelected.Remove(ri);
            }

        }

        public RentalItemsByBook()
        {
            if(App.CurrentUser.Role == Role.Admin)
            {
                InitializeComponent();

                DataContext = this;
                BookList = new ObservableCollection<BookBis>();
                rentalItemsSelected = new ObservableCollection<RentalItem>();
                AddToBookList();


                Change = new RelayCommand<BookBis>(BookSelected => {

                    Console.WriteLine("BookSelected : " + BookSelected.book.Title);
                    FillRentalItemsSelected(BookSelected);
                });
            }
           
        }
    }
}
