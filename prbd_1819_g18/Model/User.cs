using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace prbd_1819_g18
{

    public enum Role { Admin, Manager, Membre }
    public class User : EntityBase<Model>
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public Role Role { get; set; }
        public Rental Basket
        {
            get
            {
                var op =( from ri in Rentals
                         where ri.RentalDate == null
                         select ri).FirstOrDefault();
                return op;
            }
        }

        public RentalItem[] ActiveRentalItems { get; }

        public virtual ICollection<Rental> Rentals { get; set; } = new HashSet<Rental>();


        protected User()
        {

        }

        public Rental CreateBasket()
        {
            Rental rent = Model.Rentals.Create();
            rent.User = this;
            Rentals.Add(rent);
            Model.Rentals.Add(rent);
            App.Model.SaveChanges();
            return rent;
        }



        public RentalItem AddToBasket(Book book)
        {
            if (Basket == null)
            {
                CreateBasket();
            }

            RentalItem rent = null;
            BookCopy copy = book.GetAvailableCopy();
            if (copy != null)
            {
                rent = Basket.RentCopy(copy);
            }

            App.Model.SaveChanges();
            
            return rent;
        }

        public void RemoveFromBasket(RentalItem item)
        {
            if (Rentals.Count > 0)
            {
                Basket.RemoveItem(item);
                App.Model.SaveChanges();
            }

        }

        public void ClearBasket()
        {
            if (Basket != null) {
                this.Basket.Clear();
            }
           
        }




        public void ConfirmBasket()
        {
            if (Rentals.Count > 0)
            {
                this.Basket.Confirm();
            }
            App.Model.SaveChanges();
        }

        public static void RemoveUser(User user)
        {
            App.Model.Users.Remove(user);
            App.Model.SaveChanges();
        }

        public override string ToString()
        {
            return UserName.ToString();
        }

        public void Return(BookCopy copy)
        {
            RentalItem rentalItem = copy.RentalItems.FirstOrDefault(r => r.ReturnDate == null);
            if (rentalItem != null)
            {
                rentalItem.ReturnDate = DateTime.Now;
                Model.SaveChanges();
            }
        }

    }
}
