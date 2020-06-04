using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SparkAuto.Models.ViewModel
{
    // ViewModel is created when the view needs data from more then one data model,
    // when view needs data model plus other data, or when the view needs non-db data
    // created outside of itself.  We package up all the data needed and add it to 
    // a viewmodel that can be given to the view.
    public class UsersListViewModel
    {
        // In our case we have a list of users and pagination data both going
        // to the User index view.
        public List<ApplicationUser> ApplicationUserList { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
