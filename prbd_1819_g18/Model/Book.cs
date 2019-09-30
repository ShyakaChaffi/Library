using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_1819_g18
{

    public class Book: EntityBase<Model>
    {
        [Key]
        public int BookId { get; set; }
        public string Isbn { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Editor { get; set; }
        public string PicturePath { get; set; }
        public int NumAvailableCopies
        {
            get
            {
                return (
                    from c in this.Model.BookCopies
                    where c.BookId.BookId == BookId &&
                    (from i in c.RentalItems where i.ReturnDate == null select i).Count() == 0
                    select c
                ).Count();
            }
        }

        // public int Copies { get; set; }


        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();
        public virtual ICollection<BookCopy> Copies { get; set; } = new HashSet<BookCopy>();

        [NotMapped]
        public string AbsolutePicturePath
        {
            get { return PicturePath != null ? App.IMAGE_PATH + "\\" + PicturePath : null; }
        }

        public Book(string Isbn, string Author, string Title, string Editor, string PicturePath)
        {
            this.Isbn = Isbn;
            this.Author = Author;
            this.Title = Title;
            this.Editor = Editor;
            this.PicturePath = PicturePath;

        }

        public Book()
        {

        }

        public void AddCategory(Category[] category)
        {
            if (category != null) {
                foreach (Category c in category) {
                    Categories.Add(c);
                    
                }
               
                App.Model.SaveChanges();
            }

        }

        public void RemoveCategory(Category category)
        {
            if (Categories.Contains(category))
            {
                Categories.Remove(category);

            }

        }

        public void AddCopies(int quantity,DateTime date) {

            for (int i = 0; i < quantity; ++i)
            {
                var copy = Model.BookCopies.Create();
                copy.AcquisitionDate = date;
                copy.BookId = this;
                Copies.Add(copy);
            }
          


        }

        public void DeleteCopy(BookCopy copies) {
            if (Copies.Contains(copies) && copies!=null) {
                Copies.Remove(copies);
            }
        }

        public void Delete()
        {
            App.Model.Books.Remove(this);
        }


        public BookCopy GetAvailableCopy() {
            return (
                from c in this.Model.BookCopies
                where c.BookId.BookId == BookId &&
                (from i in c.RentalItems where i.ReturnDate == null select i).Count() == 0
                select c
            ).FirstOrDefault();

        }

        public override string ToString()
        {
            return Title.ToString();
        }

    }
}
