﻿using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public abstract class BaseDM<K>
    {
        public K Id { get; set; }
    }
}
