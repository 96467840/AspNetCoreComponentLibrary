using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary.Abstractions
{
    public interface IBlockable
    {
        bool IsBlocked { get; set; }
    }
}
