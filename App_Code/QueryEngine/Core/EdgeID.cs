using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBE.Core
{
    public class EdgeID
    {
        int iD1;
        int iD2;
        string labelOfEdge;
        public EdgeID(int iD1,int iD2, string label)
        {
            this.iD1 = iD1;
            this.iD2 = iD2;
            this.labelOfEdge = label;
        }

        public int ID1
        {
            get
            {
                return iD1;
            }
            set
            {
                iD1 = value;
            }
        }

        public int ID2 {
            get
            {
                return iD2;
            }
            set
            {
                iD2 = value;
            }
        }

        public string LabelOfEdge
        {
            get { return labelOfEdge; }
            set { labelOfEdge = value; }
        }

        public override string ToString()
        {
            return ID1 + "        " + ID2 + "        " + LabelOfEdge;
        }

    }
}
