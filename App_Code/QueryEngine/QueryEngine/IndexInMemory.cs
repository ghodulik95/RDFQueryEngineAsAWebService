using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;

namespace GBE.QueryEngine
{
    public struct NBEntry
    {
        public int Distance;
        public byte Sigbyte;
        public int Count;
        public List<byte[]> IDs;
    }


    public class IndexInMemory
    {

        public IndexInMemory()
        {
        }

        public static Dictionary<int, List<NBEntry>> loadNBIndex(string NBindexTable)
        {
            int NBIDlength = 3;
            string sql = "select * from " + NBindexTable + " order by ID";
            DataTable NBTable = DataHelpe.GetDataTable(sql);
            Dictionary<int, List<NBEntry>> NBindex = new Dictionary<int, List<NBEntry>>();
            int currentID = 0;
            List<NBEntry> indexforCurrentID = new List<NBEntry>();
            foreach (DataRow di in NBTable.Rows)
            {
                byte[] binaryNodeID = di.Field<byte[]>("ID");
                Array.Reverse(binaryNodeID, 0, binaryNodeID.Length);
                int rowID = BitConverter.ToInt32(binaryNodeID, 0);
                if (rowID != currentID)
                {
                    //when the node ID changes, copy all the index for this ID and move to next one
                    if (indexforCurrentID.Count != 0)
                    {
                        List<NBEntry> indexEntryCopy = new List<NBEntry>();
                        indexEntryCopy.AddRange(indexforCurrentID);
                        NBindex.Add(currentID, indexEntryCopy);
                        indexforCurrentID.Clear();
                    }
                    currentID = rowID;
                    NBEntry entry = new NBEntry();
                    entry.Distance = di.Field<int>("Distance");
                    byte[] SigbyteField = di.Field<byte[]>("Sigbyte");
                    entry.Sigbyte = SigbyteField[0];
                    entry.Count = di.Field<int>("Count");
                    byte[] IDsField = di.Field<byte[]>("IDs");
                    List<byte[]> ListOfIDs = new List<byte[]>();
                    for (int i = 0; i < IDsField.Count() / (NBIDlength - 1); i++)
                    {
                        byte[] singleID = new byte[2];
                        singleID[0] = IDsField[2 * i];
                        singleID[1] = IDsField[2 * i + 1];
                        ListOfIDs.Add(singleID);
                    }
                    entry.IDs = ListOfIDs;
                    indexforCurrentID.Add(entry);
                }
                else
                {
                    NBEntry entry = new NBEntry();
                    entry.Distance = di.Field<int>("Distance");
                    byte[] SigbyteField = di.Field<byte[]>("Sigbyte");
                    entry.Sigbyte = SigbyteField[0];
                    entry.Count = di.Field<int>("Count");
                    byte[] IDsField = di.Field<byte[]>("IDs");
                    List<byte[]> ListOfIDs = new List<byte[]>();
                    for (int i = 0; i < IDsField.Count() / (NBIDlength - 1); i++)
                    {
                        byte[] singleID = new byte[2];
                        singleID[0] = IDsField[2 * i];
                        singleID[1] = IDsField[2 * i + 1];
                        ListOfIDs.Add(singleID);
                    }
                    entry.IDs = ListOfIDs;
                    indexforCurrentID.Add(entry);
                }
            }
            NBTable.Clear();
            return NBindex;
        }
        public static List<int> byteIDstoInt(byte Sigbyte, List<byte[]> IDs)
        {
            List<int> IntegerIDs = new List<int>();
            foreach (byte[] id in IDs)
            {
                byte[] oneIntegerID = new byte[4];
                oneIntegerID[0] = id[1];
                oneIntegerID[1] = id[0];
                oneIntegerID[2] = Sigbyte;
                oneIntegerID[3] = 0;
                IntegerIDs.Add(BitConverter.ToInt32(oneIntegerID, 0));
            }
            return IntegerIDs;
        }
    }
}