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
using System.ComponentModel;
using PRBD_Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace prbd_1819_g18
{
    /// <summary>
    /// Logique d'interaction pour BookList.xamlc
    /// </summary>
    public partial class BookList : UserControlBase, INotifyPropertyChanged
    {






        public string filter;
        public string Filter { get => filter; set => SetProperty<string>(ref filter, value, () => ApplyFilterAction()); }

        public ICommand ApplyFilter { get; set; }

        public ICommand Clear { get; set; }


        private ObservableCollection<Book> books;
        public ObservableCollection<Book> Books
        {
            get => books;
            set => SetProperty<ObservableCollection<Book>>(ref books, value);

        }

        public string BookCategrys { get; set; } = "coucou";
        private ObservableCollection<String> categorys;
        public ObservableCollection<String> Categorys
        {
            get => categorys;
            set => SetProperty<ObservableCollection<String>>(ref categorys, value, () =>
            {
            });
        }
        public ICommand CategorysFilter { get; set; }
        public Book test { get; set; }

        public ICommand NewBook { get; set; }

        public ICommand DisplayMemberDetails { get; set; }

        public ICommand CategoryFilter { get; set; }

        public ICommand AddBasket { get; set; }

        public ICommand LinkCategory { get; set; }

        public Book SelectedBook { get; set; }

        //public String SelectedValue { get; set; }
        public String selectedValue;
        //private List<Book> newBokList;

        public String SelectedValue { get => selectedValue; set => SetProperty<String>(ref selectedValue, value); }

        public bool Enabled { get; set; }

        public string visible { get; set; }

        // contructeur
        public BookList()
        {
            InitializeComponent();
            DataContext = this;

            if (App.CurrentUser.Role != Role.Admin)
                visible = "Hidden";
            SelectedValue = "(All)";
            Msg(categorys);


            Console.WriteLine(DisplayMemberDetails);
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            DataContext = this;
            ApplyFilter = new RelayCommand(ApplyFilterAction);
            DisplayMemberDetails = new RelayCommand<Book>(books =>
            {
                //App.Msg(books.PicturePath);
                App.NotifyColleagues(AppMessages.MSG_DETAIL_BOOK, books);
                Console.WriteLine();
            });

            Clear = new RelayCommand(() =>
            {
                Filter = "";
                SelectedValue = "(All)";

            });
            NewBook = new RelayCommand(() =>
            {
                App.NotifyColleagues(AppMessages.NEW_BOOK);
            });

            CategoryFilter = new RelayCommand<String>(SelectedValue =>
            {
                GetBookCat(SelectedValue);
            });

            AddBasket = new RelayCommand<Book>(book=> {
                App.NotifyColleagues(AppMessages.MSG_ADDBASKET,book);
                Books = new ObservableCollection<Book>(App.Model.Books);



            });

            LinkCategory = new RelayCommand<Category>(cats => {
                App.NotifyColleagues(AppMessages.DISPLAY_CATEGORY,cats);
            });
            App.Register<Book>(this, AppMessages.MSG_BOOK_CHANGED,
                        book => { ApplyFilterAction(); });

            Books = new ObservableCollection<Book>(App.Model.Books);
            Categorys = new ObservableCollection<String>();
            AddCategorys();


            App.Msg(CategoryFilter);

            App.Register(this, AppMessages.MSG_REFRESH_BOOK_LIST, () =>
            {
                Books = new ObservableCollection<Book>(App.Model.Books);

            });

            App.Register(this, AppMessages.UPDATE_CATEGORY, () =>
            {
                Categorys.Clear();
                AddCategorys();

            });

        }

        private void AddCategorys()
        {
            foreach (var c in App.Model.Categories)
            {
                Categorys.Add(c.Name);
            }
            Categorys.Add("(All)");
        }

        public void ApplyFilterAction()
        {
            if (!string.IsNullOrEmpty(Filter) && selectedValue == "(All)")
            {
                Books = new ObservableCollection<Book>(getBookOnlyFilter());
            }
            if (string.IsNullOrEmpty(Filter) && selectedValue != "(All)")
            {
                Books = new ObservableCollection<Book>(getBookOnlyCategory());
            }
            if (!string.IsNullOrEmpty(Filter) && selectedValue != "(All)")
            {
                Books = new ObservableCollection<Book>(getBookOnlyFilterAndCategory());
            }
            if (string.IsNullOrEmpty(Filter) && selectedValue == "(All)")
            {
                Books = new ObservableCollection<Book>(App.Model.Books);
            }

            //Books = new ObservableCollection<Book>(getBookOnlyFilterAndCategory());
            //IEnumerable<Book> query = App.Model.Books;
            //if (!string.IsNullOrEmpty(Filter))
            //{

            //    query = from m in App.Model.Books
            //            where
            //                m.Title.Contains(Filter) || m.Author.Contains(Filter)
            //            select m;
            //    Books = new ObservableCollection<Book>(query);
            //    Console.WriteLine(Books);
            //}
            //else
            //{
            //    Books = new ObservableCollection<Book>(App.Model.Books);
            //}

        }

        public void Msg(Object elem)
        {
            Console.WriteLine(elem);
        }


        public void GetBookCat(String S)
        {
            if (S != "(All)")
            {
                Books = new ObservableCollection<Book>();
                foreach (var bk in App.Model.Books)
                {
                    foreach (var c in bk.Categories)
                    {
                        if (c.Name == S && bk.Title.Contains(Filter) && bk.Author.Contains(Filter))
                        {
                            Books.Add(bk);
                        }
                    }
                }
            }
            else
                Books = new ObservableCollection<Book>(App.Model.Books);
        }

        public List<Book> getBookOnlyFilter()
        {

            return (from b in App.Model.Books
                    where b.Title.Contains(Filter) || b.Author.Contains(Filter)
                    select b
                    ).ToList();
        }


        public List<Book> getBookOnlyFilterAndCategory()
        {
            List<Book> list = new List<Book>();
            var query = (from b in App.Model.Books
                         where b.Title.Contains(Filter) || b.Author.Contains(Filter)
                         select b
                    ).ToList();
            foreach (var l in query)
            {
                foreach (var c in l.Categories)
                {
                    if (c.Name == selectedValue)
                    {
                        list.Add(l);
                    }
                }
            }
            return list;
        }


        public List<Book> getBookOnlyCategory()
        {
            List<Book> list = new List<Book>();
            foreach (var t in App.Model.Books)
            {
                foreach (var c in t.Categories)
                {
                    if (c.Name == selectedValue)
                    {
                        list.Add(t);
                    }
                }
            }

            return list;
        }





        private void AddBasketButton_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }




    }
}


    

