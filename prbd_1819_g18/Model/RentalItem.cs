using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;
using System.ComponentModel.DataAnnotations;

namespace prbd_1819_g18
{
    public class RentalItem: EntityBase<Model>
    {
        [Key]
        public int RentalItemId { get; set; }
        public DateTime? ReturnDate { get; set; }

        
        public virtual  BookCopy BookCopy { get; set; }

      
        public virtual Rental Rental { get; set; }


        public void DoReturn()
        {
            this.ReturnDate = DateTime.Now ;
           // Rental.RemoveItem(this);

        }

        public void CancelReturn() {
            this.ReturnDate = null;
        }

        public override string ToString()
        {
            return (this.RentalItemId.ToString());
        }


    }

    
}
