using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBE.Core
{
    public class VertexID//for later component usage.
    {
        int id;
        string label;

        public VertexID(int id, string label)
        {
            this.ID = id;
            this.Label = label;
        }

        public int ID
        {
            get { return id; }
            set { id = ID; }
        }
        public string Label
        {
            get { return label; }
            set { label = Label; }
        }
    }

}
