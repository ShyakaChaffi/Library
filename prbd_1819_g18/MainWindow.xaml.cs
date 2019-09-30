using System;
using System.Collections.Generic;
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

namespace prbd_1819_g18
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBase
    {


        public ICommand Logout { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Logout = new RelayCommand(() => {
                App.CurrentUser = null;
              new LoginView().Show();
              
                this.Close();
               

            });
            newBookMessage();
            BookDetail();
            TabItems();



            //App.Register<UserControlBase>(this, AppMessages.MSG_BOOK_CHANGED, ctl =>
            //{
            //    var tab = (from TabItem t in tabControl.Items where t.Content == ctl select t).SingleOrDefault();
            //    ctl.Dispose();
            //    tabControl.Items.Remove(tab);
            //});

            App.Register<UserControlBase>(this, AppMessages.MSG_CLOSE_TAB, ctl => {
                var tab = (from TabItem t in tabControl.Items where t.Content == ctl select t).SingleOrDefault();
                ctl.Dispose();
                tabControl.Items.Remove(tab);
            });

        }

       

        public void newBookMessage()
        {
            App.Register(this, AppMessages.NEW_BOOK, () =>
            {
                var book = App.Model.Books.Create();
                var tab = new TabItem()
                {
                    Header = "NEW BOOK  ",
                    Content = new NewBook(book, true)
                };
                tabControl.Items.Add(tab);
                Dispatcher.InvokeAsync(() => tab.Focus());
                closeAnglet(tab);
              
            });

            App.Register<Category>(this, AppMessages.DISPLAY_CATEGORY, category=> {

                var tab = catTab;
                tab.Focus();
               App.NotifyColleagues(AppMessages.PRESELECTE_CAT, category);
               

            });


            
        }

      


        public void BookDetail()
        {

            App.Register<Book>(this, AppMessages.MSG_DETAIL_BOOK, books =>
            {
                if (books != null)
                {
                    var tab = new TabItem()
                    {
                        Header = books.Title,
                        Content = new NewBook(books, false)

                    };
                    tabControl.Items.Add(tab);
                    Dispatcher.InvokeAsync(() => tab.Focus());
                    closeAnglet(tab);

                }
            });
        
            
        }

        public void closeAnglet(TabItem tab)
        {
            Console.WriteLine("TAB : " + tab.Content);
            tab.MouseDown += (o, e) => {
                if (e.ChangedButton == MouseButton.Middle &&
                    e.ButtonState == MouseButtonState.Pressed)
                {
                    tabControl.Items.Remove(o);
                    (tab.Content as UserControlBase).Dispose();
                }
            };
            tab.PreviewKeyDown += (o, e) => {
                if (e.Key == Key.W && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    tabControl.Items.Remove(o);
                    (tab.Content as UserControlBase).Dispose();
                }
            };

        }

        private void AddTab(Book book, bool isNew)
        {
            var ctl = new NewBook(book, isNew);
            var tab = new TabItem()
            {
                Header = isNew ? "<new member>" : book.Title,
                Content = ctl
            };

        }

        public void TabItems() {

          
        }


    }
}

