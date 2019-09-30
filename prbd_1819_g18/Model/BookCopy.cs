using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_1819_g18
{
    public class BookCopy : EntityBase<Model>
    {
        [Key]
        public int BookCopyId { get; set; }
        public DateTime AcquisitionDate { get; set; }
        public virtual User RentedBy
        {
            get
            {
                var u = (from r in RentalItems
                         select r.Rental.User).FirstOrDefault();
                return u;
             
            }
        }
        public virtual Book BookId { get; set; }
        

        public virtual ICollection<RentalItem> RentalItems { get; set; } = new HashSet<RentalItem>();




        public override string ToString()
        {
            return BookId.ToString();
        }



    }
}
