using System;
using System.Collections.Generic;

namespace CauldronCodebase
{
    [Serializable]
    public class ListToSave<T> 
    {
        public List<T> list;

        public ListToSave(List<T> list)
        {
            this.list = list;
        }
    }
}