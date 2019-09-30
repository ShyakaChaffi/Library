using MySql.Data.EntityFramework;
using prbd_1819_g18;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using PRBD_Framework;

namespace prbd_1819_g18
{
    public enum DbType { MsSQL, MySQL }
    public enum EFDatabaseInitMode { CreateIfNotExists, DropCreateIfChanges, DropCreateAlways }

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class MySqlModel : Model
    {
        public MySqlModel(EFDatabaseInitMode initMode) : base("name=library-mysql")
        {
            switch (initMode)
            {
                case EFDatabaseInitMode.CreateIfNotExists:
                    Database.SetInitializer<MySqlModel>(new CreateDatabaseIfNotExists<MySqlModel>());
                    break;
                case EFDatabaseInitMode.DropCreateIfChanges:
                    Database.SetInitializer<MySqlModel>(new DropCreateDatabaseIfModelChanges<MySqlModel>());
                    break;
                case EFDatabaseInitMode.DropCreateAlways:
                    Database.SetInitializer<MySqlModel>(new DropCreateDatabaseAlways<MySqlModel>());
                    break;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // see: https://blog.craigtp.co.uk/Post/2017/04/05/Entity_Framework_with_MySQL_-_Booleans,_Bits_and_%22String_was_not_recognized_as_a_valid_boolean%22_errors.
            modelBuilder.Properties<bool>().Configure(c => c.HasColumnType("bit"));
        }

        public override void Reseed(string tableName)
        {
            Database.ExecuteSqlCommand($"ALTER TABLE {tableName} AUTO_INCREMENT=1");
        }
    }

    public class MsSqlModel : Model
    {
        public MsSqlModel(EFDatabaseInitMode initMode) : base("name=library-mssql")
        {
            switch (initMode)
            {
                case EFDatabaseInitMode.CreateIfNotExists:
                    Database.SetInitializer<MsSqlModel>(new CreateDatabaseIfNotExists<MsSqlModel>());
                    break;
                case EFDatabaseInitMode.DropCreateIfChanges:
                    Database.SetInitializer<MsSqlModel>(new DropCreateDatabaseIfModelChanges<MsSqlModel>());
                    break;
                case EFDatabaseInitMode.DropCreateAlways:
                    Database.SetInitializer<MsSqlModel>(new DropCreateDatabaseAlways<MsSqlModel>());
                    break;
            }
        }

        public override void Reseed(string tableName)
        {
            Database.ExecuteSqlCommand($"DBCC CHECKIDENT('{tableName}', RESEED, 0)");
        }
    }

    public abstract class Model : DbContext
    {
        protected Model(string name) : base(name) { }

        public static Model CreateModel(DbType type, EFDatabaseInitMode initMode = EFDatabaseInitMode.DropCreateIfChanges)
        {
            Console.WriteLine($"Creating model for {type}\n");
            switch (type)
            {
                case DbType.MsSQL:
                    return new MsSqlModel(initMode);
                case DbType.MySQL:
                    return new MySqlModel(initMode);
                default:
                    throw new ApplicationException("Undefined database type");
            }
        }


        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<RentalItem> RentalItems { get; set; }

        public void ClearDatabase()
        {
#if MSSQL

            Console.WriteLine("qfklqsjdfkljqsdlfjiljsdfkljqsdkldfjklsqdjdhfjsqdhfjqsdhljkfklqsdjfklqsdjfkljsqddlkfjlsqdkjf");
            RentalItems.RemoveRange(RentalItems);
            Rentals.RemoveRange(Rentals);
            Users.RemoveRange(Users);
            BookCopies.RemoveRange(BookCopies);
            Books.RemoveRange(Books);
            Categories.RemoveRange(Categories);


#else
            
#endif
            SaveChanges();
            Reseed(nameof(Users));
            Reseed(nameof(Books));
            Reseed(nameof(Rentals));
            Reseed(nameof(BookCopies));
            Reseed(nameof(Categories));
            Reseed(nameof(RentalItems));

           

            /**
             * Décommenter la ligne suivante pour réinitialiser le compteur de clef autoincrementée
             */
            //Reseed(nameof(Messages));
        }

        public abstract void Reseed(string tableName);

        public void CreateTestData()
        {
            new TestDatas(DbType.MsSQL); ;
        }

        public User CreateMember(string userName, string passWord, string fullName, string email, DateTime? birthdate = null, Role role = Role.Membre)
        {

            User newUser = null;
            newUser = Users.Create();
            newUser.UserName = userName;
            newUser.Password = passWord;
            newUser.FullName = fullName;
            newUser.Email = email;
            newUser.BirthDate = birthdate;
            newUser.Role = role;

            Users.Add(newUser);
            SaveChanges();

            return newUser;

        }

        public Category CreateCategory(string category)
        {

            Category cat = Categories.Create();
            cat.Name = category;

            Categories.Add(cat);
            SaveChanges();
            return cat;
        }


        public Book CreateBook(string isbn, string title, string author, string editor, int numCopies)
        {

            Book book = Books.Create();
            book.Isbn = isbn;
            book.Title = title;
            book.Author = author;
            book.Editor = editor;
            
            Books.Add(book);
            BookCopy copy = App.Model.BookCopies.Create();
            copy.BookId = book;
            copy.AcquisitionDate = DateTime.Now;
            BookCopies.Add(copy);
            SaveChanges();

            return book;
        }

        public List<Book> FindBooksByText(string txt)
        {

            List<Book> list = new List<Book>();
            foreach (Book b in Books)
            {
                if (b.Title.Contains(txt) ||
                              b.Author.Contains(txt) ||
                                    b.Editor.Contains(txt))
                {
                    list.Add(b);
                }

            }
            return list;
        }

        public List<RentalItem> GetActiveRentalItems()
        {

            List<RentalItem> list = new List<RentalItem>();
            foreach (RentalItem r in RentalItems)
            {
                if (r.ReturnDate == null)
                {
                    list.Add(r);
                }
            }
            return list;
        }

        public List<User> GetUsers()
        {

            List<User> list = new List<User>();
            foreach (User r in Users)
            {
                    list.Add(r);                
            }
            return list;
        }
    }
}

