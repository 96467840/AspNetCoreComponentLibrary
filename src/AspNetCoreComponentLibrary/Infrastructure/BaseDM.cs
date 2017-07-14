using AspNetCoreComponentLibrary.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCoreComponentLibrary
{
    // никаких пропертей тока методы, иначе могут быть глюки с БД (EF че-то пытается сделать с пропертями и у него это не очень получается)
    public interface IBaseDM
    {
        bool IsBlockable();
        bool IsSortable();
        string IdStr();
        Dictionary<PropertyInfo, ListColumnBaseAttribute> MassSave();
        Dictionary<PropertyInfo, ListColumnBaseAttribute> Columns();
        //List<int> ColSM();
        /**/
        //Form ToForm<FA>(Controller2Garin controller) where FA:FieldBaseAttribute;
    }

    public abstract class BaseDM<K> : IBaseDM
    {
        [OrderBy(Priority = int.MaxValue)]
        [ListColumn(Width = "50")]
        public K Id { get; set; }

        public bool IsBlockable()
        {
            return GetType().IsImplementsInterface(typeof(IBlockable));
        }

        public bool IsSortable() { return GetType().IsImplementsInterface(typeof(ISortable)); }

        Dictionary<PropertyInfo, ListColumnBaseAttribute> massSave;
        public Dictionary<PropertyInfo, ListColumnBaseAttribute> MassSave()
        {
            if (massSave == null)
                massSave = GetType().GetPropertiesWithAttribute<MassSaveAttribute>().ToDictionary(i=>i.Key, i=> (ListColumnBaseAttribute)i.Value);
            return massSave;
        }

        Dictionary<PropertyInfo, ListColumnBaseAttribute> columns;
        public Dictionary<PropertyInfo, ListColumnBaseAttribute> Columns()
        {
            if (columns == null)
                columns = GetType().GetPropertiesWithAttribute<ListColumnAttribute>().ToDictionary(i => i.Key, i => (ListColumnBaseAttribute)i.Value);
            return columns;
        }

        /*List<int> colSM;
        public List<int> ColSM()
        {
            if (colSM == null)
            {
                colSM = new List<int> { 0, 0, 2 };
                if (IsBlockable())
                {
                    colSM[2] = 3;
                }

                if (MassSave().Any())
                {
                    colSM[1] = 1;
                }
                colSM[0] = 12 - colSM[1] - colSM[2];
            }
            return colSM;
        }*/

        public string IdStr() { return Id == null ? "null" : GetType().ToStringVM(Id); }

    }
}
