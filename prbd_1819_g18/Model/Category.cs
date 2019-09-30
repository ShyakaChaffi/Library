using PRBD_Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace prbd_1819_g18
{
    public class Category : EntityBase<Model>
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new HashSet<Book>();

        protected Category()
        {

        }

        public bool HasBook(Book book)
        {
            //var q = from bc in Model.Categorys
            //        where bc.Equals(book)
            //        select bc;
            //return q != null;
            return true;
        }

        public void AddBook(Book book)
        {
            Books.Add(book);
          

        }

        public void RemoveBook(Book book)
        {
            if (Books.Contains(book))
            {
                Books.Remove(book);
            }

        }

        public void Delete()
        {
            Model.Categories.Remove(this);

        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
