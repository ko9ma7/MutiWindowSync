using System;
using Prism.Mvvm;

namespace mutiWindowSync.Entity
{
    public class BaseDto :BindableBase
    {
        private static int nextId = 1;
        private int id = nextId++;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private DateTime createDate;

        public DateTime CreateDate
        {
            get { return createDate; }
            set { createDate = value; }
        }

        private DateTime updateDate;

        public DateTime UpdateDate
        {
            get { return updateDate; }
            set { updateDate = value; }
        }
    }
}