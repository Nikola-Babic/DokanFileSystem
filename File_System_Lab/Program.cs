using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DokanNet;

namespace File_System_Lab
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                FS_Lab fs = new FS_Lab('R');

                Console.WriteLine(@"Success");
            }
            catch (DokanException ex)
            {
                Console.WriteLine(@"Error: " + ex.Message);
            }
        }
    }
}
