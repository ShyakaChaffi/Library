using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace prbd_1819_g18
{
    public class Rental: EntityBase<Model>
    {
        [Key]
        public int RentalId { get; set; }
        public DateTime? RentalDate { get; set; }
        public int RentalItems { get; }
        public  virtual User User { get; set; }
        public int NumOpenItems
        {
            get
            {
                return (
                        from ri in Items
                        where ri.ReturnDate.Equals(null)
                        select ri
                        ).Count();
            }
        }

        public virtual ICollection<RentalItem> Items { get; set; } = new HashSet<RentalItem>();

        

        public void RemoveCopy(BookCopy copy) {
            Model.BookCopies.Remove(copy);

        }

        public RentalItem RentCopy(BookCopy copy) {
            RentalItem rental = Model.RentalItems.Create();
            rental.ReturnDate = null;
            rental.BookCopy = copy;
            rental.Rental = this;
            copy.RentalItems.Add(rental);
            this.Items.Add(rental);
            Model.RentalItems.Add(rental);
            App.Model.SaveChanges();

            return rental;
        }

        public void RemoveItem(RentalItem copy)
        {
            if (Items.Contains(copy)) {
                Items.Remove(copy);
            }
        }

        public void Return(RentalItem item) {
            item.DoReturn();
        }

        public void Confirm() {
                if (RentalDate == null)
                {
                    RentalDate = DateTime.Now;
                }
        }

        public void Clear() {
            if (Items.Count() > 0) {
                Model.RentalItems.RemoveRange(Items);
                Items.Clear();
            }
        }


        public override string ToString()
        {
            return RentalId.ToString()+NumOpenItems;
        }
    }
}
