using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonCounter
{
    public class InfoEntity : TableEntity
    {
        public InfoEntity()
        {
        }

        public string InfoParameter { get; set; }

        public string InfoValue { get; set; }
    }
}
