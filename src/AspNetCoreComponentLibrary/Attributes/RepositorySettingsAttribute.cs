﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    /// <summary>
    /// Настройка репозитория для админки
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RepositorySettingsAttribute : Attribute
    {
    }
}
