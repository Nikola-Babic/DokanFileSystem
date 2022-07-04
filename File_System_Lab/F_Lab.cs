using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DokanNet;

namespace File_System_Lab
{
    class F_Lab
    {
        private F_Lab data;

        public F_Lab(F_Lab data)
        {
            Name = data.Name;
            Data = data.Data;
            Attributes = data.Attributes;
            DataSet = new List<byte>();
            Size = data.Size;
            DateCreated = data.DateCreated;
            DateModified = data.DateModified;
        }

        public F_Lab(string name, byte[] data, FileAttributes fileAttributes)
        {
            Name = name;
            Data = data;
            Attributes = fileAttributes;
            DataSet = new List<byte>();
        }

        public int BufferSize { get; set; }

        public String Name { get; set; }
        public byte[] Data { get; set; }
        public List<byte> DataSet { get; set; }
        public int Size { get; set; }

        public FileAttributes Attributes { get; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
    }
}
