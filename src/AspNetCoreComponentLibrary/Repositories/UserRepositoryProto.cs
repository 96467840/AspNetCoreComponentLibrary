using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreComponentLibrary
{
    // реализуем частичное кеширование. в кеше храним тока последних юзеров (за 1 день)
    public class UserRepositoryProto : RepositoryWithTempCache<long, Users>//, IUserRepository
    {
        protected new int TimeToLife = 6400;

    }
}
