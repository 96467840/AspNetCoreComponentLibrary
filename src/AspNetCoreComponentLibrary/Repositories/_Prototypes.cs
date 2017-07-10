using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    public class LanguageRepositoryProto : RepositoryWithCache<long, Languages>
    //public class LanguageRepositoryProto : Repository<long, Languages>
    {
    }

    //public class MenuRepositoryProto : RepositoryWithCache<long, Menus>//, IMenuRepository
    public class MenuRepositoryProto : Repository<long, Menus>//, IMenuRepository
    {
    }


    public class UserSiteRepositoryProto : RepositoryForRelations<UserSites>
    {
    }
}
