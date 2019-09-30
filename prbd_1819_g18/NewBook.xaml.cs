using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
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
using Microsoft.Win32;
using PRBD_Framework;
using System.Drawing;

namespace prbd_1819_g18
{
    public class CategoryChecked
    {

        public Category cat { get; set; }
        public string Name { get; set; }
        public bool Ischecked { get; set; }

        public CategoryChecked(Category cats, bool IsCheckeds)
        {

            this.cat = cats;
            this.Name = cats.Name;
            this.Ischecked = IsCheckeds;
        }

    }

    //}
    /// <summary>
    /// Interaction logic for NewBook.xaml
    /// </summary>
    public partial class NewBook : UserControlBase
    {

        //public bool IsAdmin
        //{ get { return App.CheckAdmin(); } }

        public Book book { get; set; }
        private ImageHelper imageHelper;



        private bool isNew;
        public bool IsNew
        {
            get { return isNew; }
            set
            {
                isNew = value;
                RaisePropertyChanged(nameof(IsNew));
            }
        }

        public string iSBN;
        public string ISBN { get => iSBN; set => SetProperty<string>(ref iSBN, value, () => Validate()); }

        public string title;
        public string Title { get => title; set => SetProperty<string>(ref title, value, () => Validate()); }


        private string author;
        public string Author
        {
            get { return author; }
            set
            {
                author = value;
                RaisePropertyChanged(nameof(Author));
            }
        }

        public string editor;
        public string Editor
        {
            get { return editor; }
            set
            {
                editor = value;
                RaisePropertyChanged(nameof(Editor));
            }
        }

        public string PicturePath
        {
            get { return book.AbsolutePicturePath; }
            set
            {
                book.PicturePath = value;
                RaisePropertyChanged(nameof(PicturePath));
            }
        }

        private int numberOfCopy;
        public int NumberOfCopy
        {
            get { return numberOfCopy; }
            set
            {
                numberOfCopy = value;
                RaisePropertyChanged(nameof(NumberOfCopy));
            }
        }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                RaisePropertyChanged(nameof(Date));
            }
        }


        public bool Enable { get; set; }
        public string BoutonText { get; set; }


        public ICommand Save { get; set; }
        public ICommand Delete { get; set; }

        public ICommand Add { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand ClearImage { get; set; }
        public ICommand LoadImage { get; set; }
        // public bool Checked { get; set; }



        private ObservableCollection<Category> category;
        public ObservableCollection<Category> Category
        {
            get => category;
            set => SetProperty<ObservableCollection<Category>>(ref category, value);

        }

        private ObservableCollection<CategoryChecked> newCAt;
        public ObservableCollection<CategoryChecked> NewCAt
        {
            get => newCAt;
            set => SetProperty<ObservableCollection<CategoryChecked>>(ref newCAt, value);

        }


        private ObservableCollection<BookCopy> bookCopies;
        public ObservableCollection<BookCopy> BookCopies
        {
            get => bookCopies;
            set => SetProperty<ObservableCollection<BookCopy>>(ref bookCopies, value);

        }

        // constructeur
        public NewBook(Book book, bool isNewBook)
        {

            if (App.CurrentUser.Role == Role.Admin)
            {
                Enable = true;
            }
            else
            {
                Enable = false;
            }



            Category = new ObservableCollection<Category>(App.Model.Categories);

            NewCAt = new ObservableCollection<CategoryChecked>();

            BookCopies = new ObservableCollection<BookCopy>(book.Copies);

            Date = DateTime.Now;

            

            this.isNew = isNewBook;
            if (!isNew)
            {
                this.book = book;
                BoutonText = "Update";
                ISBN = book.Isbn;
                Title = book.Title;
                Author = book.Author;
                editor = book.Editor;
            }
            else
            {
                this.book = book;
                BoutonText = "Ajout";
                Title = "";

            }


            foreach (var cat in NewCAt)
            {
                App.Msg(cat.Name);

                ;
            }


            initCommand();
            InitializeComponent();
            updateCategory();
            checkCattegory(NewCAt);

            imageHelper = new ImageHelper(App.IMAGE_PATH, book.PicturePath);

            DataContext = this;
        }

        public override bool Validate()
        {

            ClearErrors();
            if (string.IsNullOrEmpty(ISBN))
            {
                AddError("ISBN", Properties.Resources.Error_Required);
            }
            else
            {
                if (ISBNExist(ISBN) && isNew)
                {
                    AddError("ISBN", Properties.Resources.ISBN_exist);
                }
                if (ISBNExist2(book, ISBN) && !isNew)
                {
                    AddError("ISBN", Properties.Resources.ISBN_exist);
                }
            }
            RaiseErrors();
            return !HasErrors;
        }

        public bool ISBNExist(string ISBN)
        {
            bool i = false;
            foreach (var b in App.Model.Books)
            {
                if (b.Isbn == ISBN)
                    i = true;
            }
            return i;
        }

        public bool ISBNExist2(Book book,string ISBN)
        {
            bool i = false;
            foreach (var b in App.Model.Books)
            {
                if (b.Isbn == ISBN && b!=book)
                    i = true;
            }
            return i;
        }

        public void ClearElm()
        {
            if (!isNew)
            {
                ISBN = book.Isbn;
                Title = book.Title;
                Author = book.Author;
                Editor = book.Editor;
                Date = DateTime.Now;
                NumberOfCopy = 0;
                NewCAt.Clear();
                checkCattegory(NewCAt);
                imageHelper.Cancel();
            }
            else
            {
                ISBN = "";
                Title = "";
                Author = "";
                Editor = "";
                Date = DateTime.Now;
                NumberOfCopy = 0;
                NewCAt.Clear();
                checkCattegory(NewCAt);
            }


        }

        public void checkCattegory(ObservableCollection<CategoryChecked> CC)
        {
            if (book.Categories.Count() > 0)
            {
                foreach (var c in book.Categories)
                {
                    CC.Add(new CategoryChecked(c, true));
                }
                foreach (var i in Category)
                {
                    if (!ExistInCategoryChecked(CC, i.Name))
                    {
                        CC.Add(new CategoryChecked(i, false));
                    }
                }
            }
            else
                foreach (var i in Category)
                {
                    CC.Add(new CategoryChecked(i, false));
                }
        }

        private bool ExistInCategoryChecked(ObservableCollection<CategoryChecked> CC, string str)
        {
            foreach (var t in CC)
            {
                if (t.Name.Equals(str))
                    return true;
            }
            return false;
        }

        public void initCommand()
        {

            //Cancel = new RelayCommand(ClearElm);

            Cancel = new RelayCommand(ClearElm, () =>
            {
                if(!IsNew)
                {
                    return IsAdmin() && BookPropertiesChanged() || CanSaveOrCancelAction();
                }
                else
                    return  IsAdmin() && BookPropertiesChanged();
            });

            Save = new RelayCommand(SaveBook, () =>
            {
                if (!IsNew)
                {
                    return IsAdmin() && Validate() && BookPropertiesChanged() || CanSaveOrCancelAction();
                }
                else
                    return IsAdmin() && Validate();


            });


            Add = new RelayCommand(AddCopy, () =>
            {
                return !isNew && IsAdmin();
            });

            ClearImage = new RelayCommand(Clear,() => {
                return IsAdmin() && PicturePath != null;
            });

            Delete = new RelayCommand(deleteBook, ()=> {
                return IsAdmin() && !IsNew;
            });

            LoadImage = new RelayCommand(UploadButton_Click, () => {
                return IsAdmin();
            });

        }

        private void Clear()
        {
            imageHelper.Clear();
            PicturePath = imageHelper.CurrentFile;
        }

        private bool IsAdmin()
        {
            if (App.CurrentUser != null)
                return App.CurrentUser.Role == Role.Admin;
            else
                return false;
        }

        public void deleteBook() {
            App.Model.Books.Remove(book);
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_REFRESH_BOOK_LIST);
            App.NotifyColleagues(AppMessages.MSG_CLOSE_TAB, this);
        }
        public void AddCopy()
        {
            book.AddCopies(NumberOfCopy, date);
            App.Model.SaveChanges();
            BookCopies = new ObservableCollection<BookCopy>(book.Copies);
            App.NotifyColleagues(AppMessages.MSG_REFRESH_BOOK_LIST);
        }

        public void SaveBook()
        {
            if (isNew)
            {
                book = new Book();
                App.Model.CreateBook(ISBN, Title, Author, Editor, NumberOfCopy);
                Category[] cat = new Category[0];
                List<Category> categ = new List<Category>();
                foreach (var cc in NewCAt)
                {
                    if (cc.Ischecked)
                    {
                        categ.Add(cc.cat);
                    }
                }
                book.Categories.Clear();
                book.AddCategory(categ.ToArray());
                PicturePath = imageHelper.CurrentFile;
            }
            else
            {
                book.Isbn = ISBN;
                book.Title = Title;
                book.Author = Author;
                book.Editor = Editor;
                book.Categories.Clear();
                Category[] cat = new Category[0];
                List<Category> categ = new List<Category>();
                foreach (var cc in NewCAt)
                {
                    if (cc.Ischecked)
                    {
                        categ.Add(cc.cat);
                    }

                }
                book.AddCategory(categ.ToArray());
            }
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_REFRESH_BOOK_LIST);
            //App.NotifyColleagues(AppMessages.UPDATE_CATEGORY);
        }

        private bool BookPropertiesChanged()
        {
            if (!isNew)
            {
                if (Title != book.Title || ISBN != book.Isbn || Author != book.Author || Editor != book.Editor || !NewCatEqualsBookCategories())
                {
                    return true;
                }
            }
            else
            {
                if (Title != null || Author != null || Editor != null)
                {
                    return true;
                }
            }
            return false;
        }

        private bool NewCatEqualsBookCategories()
        {
            bool b = true;
            foreach (var c in NewCAt)
            {
                if (!c.Ischecked && book.Categories.Contains(c.cat))
                {
                    b = false;
                }
                else
                {
                    if (c.Ischecked && !book.Categories.Contains(c.cat))
                        b = false;
                }
            }
            return b;
        }




        public bool attribNotNull()
        {
            if (IsAttrbutOk(Title) && IsAttrbutOk(Author) && IsAttrbutOk(Editor) && IsAttrbutOk(ISBN))
            {
                return true;
            }
            return false;
        }

        public bool IsAttrbutOk(string param)
        {

            return !string.IsNullOrEmpty(param) && string.IsNullOrWhiteSpace(param);
        }

        private bool CanSaveOrCancelAction()
        {
            if (IsNew)
            {
                return !string.IsNullOrEmpty(Title) && !HasErrors;
            }
            var change = (from c in App.Model.ChangeTracker.Entries<Book>()
                          where c.Entity == book
                          select c).FirstOrDefault();
            return change != null && change.State != EntityState.Unchanged;
        }


        public void UploadButton_Click()
        {
            //var fd = new OpenFileDialog();
            //if (fd.ShowDialog() == true)
            //{
            //    var filename = fd.FileName;
            //    if (filename != null && File.Exists(filename))
            //    {

            //        // a retravailler
            //        imageHelper.Load(fd.FileName);
            //        App.Msg(PicturePath);
            //        this.PicturePath = PicturePath;

            //        App.Msg(book.PicturePath);
            //    }
            //}
            var fd = new OpenFileDialog();
            if (fd.ShowDialog() == true)
            {
                imageHelper.Load(fd.FileName);
                PicturePath = imageHelper.CurrentFile;
            }
        }

        public void updateCategory()
        {
            App.Register(this, AppMessages.UPDATE_CATEGORY, () =>
            {

                Category = new ObservableCollection<Category>(App.Model.Categories);
                NewCAt.Clear();
                checkCattegory(NewCAt);

            });
        }






    }
}
