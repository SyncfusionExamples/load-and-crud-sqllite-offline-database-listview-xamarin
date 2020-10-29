using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ListViewXamarin
{
    #region Contacts
    public class Contacts
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string ContactName { get; set; }
        public string ContactNumber { get; set; }
    }
    #endregion
}
