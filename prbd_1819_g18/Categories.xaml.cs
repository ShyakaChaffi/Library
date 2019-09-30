using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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

namespace prbd_1819_g18
{
    /// <summary>
    /// Logique d'interaction pour Categories.xaml
    /// </summary>
    public partial class Categories : UserControlBase
    {
        //public NewBook vuBook = new NewBook();
        //public bool IsAdmin
        //{ get { return App.CheckAdmin(); } }


        private ObservableCollection<Category> category;
        public ObservableCollection<Category> Category
        {
            get => category;
            set => SetProperty<ObservableCollection<Category>>(ref category, value, () => {
            });
        }

        public string  editCategory;
        public string EditCategory { get => editCategory; set => SetProperty<string>(ref editCategory, value, () => Validate()); }



        public Category categorySelected;
        public Category CategorySelected { get => categorySelected; set => SetProperty<Category>(ref categorySelected, value); }

        public string SelectedItem { get; set; }

        public ICommand Add { get; set; }
        public ICommand Update { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Delete { get; set; }
        public ICommand DisplayCategoryName { get; set; }


        private bool enableTextbox;
        public bool EnableTextbox
        {
            get { return enableTextbox; }
            set
            {
                enableTextbox = value;
                RaisePropertyChanged(nameof(EnableTextbox));
            }
        }

        public bool Enable { get; set; }

        public override bool Validate()
        {

            ClearErrors();
            if (string.IsNullOrEmpty(EditCategory))
            {
                Console.WriteLine("first");
                AddError("EditCategory", Properties.Resources.Error_Required);
            }
            else
            {
                if (CategoryExist(EditCategory))
                {
                    AddError("EditCategory", Properties.Resources.Category_exist);
                }                    
            }  
            RaiseErrors();
            return !HasErrors;
        }

        public Categories()
        {
           
            InitializeComponent();

            DataContext = this;

            CountBooksByCategory();
           
            RefreshCategories();

            Message();

            

            if (App.CurrentUser.Role == Role.Admin)
            {
                EnableTextbox = true;
                Enable = true;

                DisplayCategoryName = new RelayCommand(() =>
                {              
                    if(CategorySelected != null)
                    EditCategory = CategorySelected.Name;
                 
                });

                Cancel = new RelayCommand(CancelCategory,() =>
                {
                    return !string.IsNullOrEmpty(EditCategory);

                });

                Update = new RelayCommand(UpdateCategory,() =>
                {
                    return !HasErrors && !CategorySelectNUll();

                });

                Add = new RelayCommand(AddCategory,() =>
                {
                    return !HasErrors && !EditCategoryNotNUll();
                    
                    
                });

                Delete = new RelayCommand(DeleteCategory,() =>
                {
                    return !CategorySelectNUll() && EditCatEquCatSelect();
                });
            }
            else
            {
                EnableTextbox = false;
                Enable = false;
            }
                

            App.Register(this, AppMessages.UPDATE_CATEGORY, () =>
            {
                Console.WriteLine("kkk");
                Category = new ObservableCollection<Category>(App.Model.Categories);


            });
        }

        private bool CompareEditToCategories()
        {
            foreach (var c in App.Model.Categories)
            {
                if (c.Name == EditCategory)
                    return true;
            }
            return false;
        }


        private bool CategorySelectNUll()
        {
            return (CategorySelected == null);
        }

        private bool EditCatEquCatSelect()
        {
            return (CategorySelected.Name == EditCategory);
        }

        private bool EditCategoryNotNUll()
        {
            return (EditCategory == null);
        }

        private void UpdateCategory()
        {          
            var query = (from c in App.Model.Categories
                         where c.Name == CategorySelected.Name
                         select c).FirstOrDefault();
            query.Name = EditCategory;
            App.Model.SaveChanges();
            RefreshCategories();
            EditCategory = "";
            SetCategorySelectedNUll();
            App.NotifyColleagues(AppMessages.UPDATE_CATEGORY);
        }

        private void CancelCategory()
        {
            EditCategory = "";
            SetCategorySelectedNUll();
        }

        private void AddCategory()
        {
            if (MessageBox.Show("Confirm?", "Are You Sure?", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                CancelCategory();
            }
            else
            {
                App.Model.CreateCategory(EditCategory);
                RefreshCategories();
                EditCategory = "";
                SetCategorySelectedNUll();
                App.NotifyColleagues(AppMessages.UPDATE_CATEGORY);

            }
        }

        private void DeleteCategory()
        {
            var query = (from c in App.Model.Categories
                         where c.Name == CategorySelected.Name
                        select c).FirstOrDefault();
            App.Msg(query);
            App.Model.Categories.Remove(query);
            App.Model.SaveChanges();            
            RefreshCategories();
            SetCategorySelectedNUll();
            CancelCategory();
            App.NotifyColleagues(AppMessages.MSG_REFRESH_BOOK_LIST);

        }

        private List<int> CountBooksByCategory()
        {
            List<int> listumberOfBooks = new List<int>();
            foreach (var category in App.Model.Categories)
            {
                listumberOfBooks.Add(category.Books.Count());
            }
            return listumberOfBooks;
        }


        private void RefreshCategories()
        {
            Category = new ObservableCollection<Category>(App.Model.Categories);
            Console.WriteLine("REFRESH CATEGORY LIST");
        }

        private void SetCategorySelectedNUll()
        {
            CategorySelected = null;
        }

        private bool CategoryExist(String cat)
        {
            bool b = false;
            foreach (var cate in App.Model.Categories)
            {
                if (cat.ToUpper() == cate.Name.ToUpper())
                {
                    return true;
                }
            }

            return b;
        }

        public Category GetCategoriesByString()
        {
            var query =( from c in App.Model.Categories
                         where c.Name == EditCategory
                        select c).FirstOrDefault();

            return query;
        }


        public void Message() {

            App.Register<Category>(this, AppMessages.PRESELECTE_CAT, cat =>
            {
                CancelCategory();
                EditCategory = cat.Name;
                CategorySelected = cat;
            });

        }





    }
}
