using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_QBSI.Clients.NHC
{
    public class AccessToDatabase_NHC
    {
        public static string GetAccessConnectionString() // Access
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string fileName = "CustomQBSIDatabase.accdb";
            string resourcePath = Path.Combine(baseDirectory, fileName);
            string accessConnectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={resourcePath};Persist Security Info=False;";

            return accessConnectionString;
        }
    }
}
