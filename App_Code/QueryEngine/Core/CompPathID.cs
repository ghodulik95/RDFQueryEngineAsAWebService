using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GBE.Core
{
    public class CompPathID
    {
        int compID1;
        int compID2;
        int nodeID1;
        int nodeID2;
        string constraint;

        public CompPathID(int compID1, int nodeID1, int compID2, int nodeID2, string constraint)
        {
            this.compID1 = compID1;
            this.nodeID1 = nodeID1;
            this.compID2 = compID2;
            this.nodeID2 = nodeID2;
            this.constraint = constraint;
        }

        public int CompID1
        {
            get { return compID1; }
            set { compID1 = CompID1; }
        }

        public int CompID2
        {
            get { return compID2; }
            set { compID2 = CompID2; }
        }

        public int NodeID1
        {
            get { return nodeID1; }
            set { nodeID1 = NodeID1; }
        }

        public int NodeID2
        {
            get { return nodeID2; }
            set { nodeID2 = NodeID2; }
        }

        public string Constraint
        {
            get { return constraint; }
            set { constraint = Constraint; }
        }
    }
}
