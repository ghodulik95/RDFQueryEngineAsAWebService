using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System;


namespace GBE.QueryEngine
{
    public class NHContainment
    {
        public NHContainment()
        {
        }
        // check if graph node ID contains a specific query node neighborhood 
        public static bool containsNeighbor(Dictionary<int, List<NBEntry>> NBindex, Dictionary<string, List<int>> intervalForString, int ID, Dictionary<string, Dictionary<int, int>> queryNeighbor)
        {
            bool contained = true;
            //read index entries for this node ID
            List<NBEntry> resultIndexingNeighborhoodForID = NBindex[ID];
            if (resultIndexingNeighborhoodForID.Count() != 0)
            {
                foreach (KeyValuePair<string, Dictionary<int, int>> pair in queryNeighbor)
                {
                    string queryKeyword = pair.Key;
                    Dictionary<int, int> indexQuery = pair.Value;
                    bool forwardPass = false;
                    //get the number of this keywords in 1,2,3 forward levels and backward levels
                    int firstLevelF = 0;
                    int secondLevelF = 0;
                    int thirdLevelF = 0;
                    if (indexQuery.ContainsKey(1))
                    {
                        firstLevelF = indexQuery[1];
                    }
                    if (indexQuery.ContainsKey(2))
                    {
                        secondLevelF = indexQuery[2];
                    }
                    if (indexQuery.ContainsKey(3))
                    {
                        thirdLevelF = indexQuery[3];
                    }
                    int firstLevelB = 0;
                    int secondLevelB = 0;
                    int thirdLevelB = 0;
                    if (indexQuery.ContainsKey(-1))
                    {
                        firstLevelB = indexQuery[-1];
                    }
                    if (indexQuery.ContainsKey(-2))
                    {
                        secondLevelB = indexQuery[-2];
                    }
                    if (indexQuery.ContainsKey(-3))
                    {
                        thirdLevelB = indexQuery[-3];
                    }
                    //get the keyword integer range
                    List<int> interval = intervalForString[queryKeyword];
                    int upperbound = interval[0];
                    int lowerbound = interval[1];
                    byte[] upperByte = BitConverter.GetBytes(upperbound);
                    byte[] lowerByte = BitConverter.GetBytes(lowerbound);
                    byte upperSig = upperByte[2];
                    byte upperFirstByte = upperByte[1];
                    byte upperSecondByte = upperByte[0];
                    byte lowerSig = lowerByte[2];
                    byte lowerFirstByte = lowerByte[1];
                    byte lowerSecondByte = lowerByte[0];
                    //get the indexing which should be used for this keywords for forwarding neighbor
                    int countOfKeywords1 = 0;
                    int countOfKeywords2 = 0;
                    int countOfKeywords3 = 0;
                    //count the first level keywords
                    if (!(firstLevelF + secondLevelF + thirdLevelF == 0))
                    {
                        List<NBEntry> result1 = new List<NBEntry>();
                        result1 = (from s in resultIndexingNeighborhoodForID
                                   where s.Sigbyte.CompareTo(upperSig) <= 0 && s.Sigbyte.CompareTo(lowerSig) >= 0 && s.Distance == 1
                                   select s).ToList<NBEntry>();
                        int count1 = result1.Count();
                        if (count1 > 2)
                        {
                            for (int i = 1; i < count1 - 1; i++)
                            {
                                countOfKeywords1 = countOfKeywords1 + result1[i].Count;
                            }
                            if (countOfKeywords1 >= firstLevelF + secondLevelF + thirdLevelF)
                            {
                                if (firstLevelB + secondLevelB + thirdLevelB == 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    forwardPass = true;
                                }
                            }
                            foreach (byte[] i in result1[0].IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywords1++;
                                }
                            }
                            foreach (byte[] i in result1.Last().IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywords1++;
                                }
                            }
                            if (countOfKeywords1 >= firstLevelF + secondLevelF + thirdLevelF)
                            {
                                if (firstLevelB + secondLevelB + thirdLevelB == 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    forwardPass = true;
                                }
                            }
                        }
                        else if (count1 == 2)
                        {
                            foreach (byte[] i in result1[0].IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywords1++;
                                }
                            }
                            foreach (byte[] i in result1.Last().IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywords1++;
                                }
                            }
                            if (countOfKeywords1 >= firstLevelF + secondLevelF + thirdLevelF)
                            {
                                if (firstLevelB + secondLevelB + thirdLevelB == 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    forwardPass = true;
                                }
                            }
                        }
                        else if (count1 == 1)
                        {
                            foreach (byte[] i in result1[0].IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywords1++;
                                }
                            }
                            if (countOfKeywords1 >= firstLevelF + secondLevelF + thirdLevelF)
                            {
                                if (firstLevelB + secondLevelB + thirdLevelB == 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    forwardPass = true;
                                }
                            }
                        }
                        if (countOfKeywords1 < firstLevelF)
                        {
                            contained = false;
                            break;
                        }
                        // count the second level keywords
                        if (!forwardPass)
                        {
                            List<NBEntry> result2 = new List<NBEntry>();
                            result2 = (from s in resultIndexingNeighborhoodForID
                                       where s.Sigbyte.CompareTo(upperSig) <= 0 && s.Sigbyte.CompareTo(lowerSig) >= 0 && s.Distance == 2
                                       select s).ToList<NBEntry>();
                            int count2 = result2.Count();
                            if (count2 > 2)
                            {
                                for (int i = 1; i < count2 - 1; i++)
                                {
                                    countOfKeywords2 = countOfKeywords2 + result2[i].Count;
                                }
                                foreach (byte[] i in result2[0].IDs)
                                {
                                    if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                    {
                                        countOfKeywords2++;
                                    }
                                }
                                foreach (byte[] i in result2.Last().IDs)
                                {
                                    if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                    {
                                        countOfKeywords2++;
                                    }
                                }

                                if (countOfKeywords1 + countOfKeywords2 >= firstLevelF + secondLevelF + thirdLevelF)
                                {
                                    if (firstLevelB + secondLevelB + thirdLevelB == 0)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        forwardPass = true;
                                    }
                                }

                            }
                            else if (count2 == 2)
                            {
                                foreach (byte[] i in result2[0].IDs)
                                {
                                    if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                    {
                                        countOfKeywords2++;
                                    }
                                }
                                foreach (byte[] i in result2.Last().IDs)
                                {
                                    if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                    {
                                        countOfKeywords2++;
                                    }
                                }
                                if (countOfKeywords1 + countOfKeywords2 >= firstLevelF + secondLevelF + thirdLevelF)
                                {
                                    if (firstLevelB + secondLevelB + thirdLevelB == 0)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        forwardPass = true;
                                    }
                                }
                            }
                            else if (count2 == 1)
                            {
                                foreach (byte[] i in result2[0].IDs)
                                {
                                    if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                    {
                                        countOfKeywords2++;
                                    }
                                }
                                if (countOfKeywords1 + countOfKeywords2 >= firstLevelF + secondLevelF + thirdLevelF)
                                {
                                    if (firstLevelB + secondLevelB + thirdLevelB == 0)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        forwardPass = true;
                                    }
                                }
                            }
                            if (countOfKeywords1 + countOfKeywords2 < firstLevelF + secondLevelF)
                            {
                                contained = false;
                                break;
                            }
                            // count the third level keywords

                            if (!forwardPass)
                            {
                                List<NBEntry> result3 = new List<NBEntry>();
                                result3 = (from s in resultIndexingNeighborhoodForID
                                           where s.Sigbyte.CompareTo(upperSig) <= 0 && s.Sigbyte.CompareTo(lowerSig) >= 0 && s.Distance == 3
                                           select s).ToList<NBEntry>();
                                int count3 = result3.Count();
                                if (count3 > 2)
                                {
                                    for (int i = 1; i < count3 - 1; i++)
                                    {
                                        countOfKeywords3 = countOfKeywords3 + result3[i].Count;
                                    }
                                    foreach (byte[] i in result3[0].IDs)
                                    {
                                        if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                        {
                                            countOfKeywords3++;
                                        }
                                    }
                                    foreach (byte[] i in result3.Last().IDs)
                                    {
                                        if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                        {
                                            countOfKeywords3++;
                                        }
                                    }


                                    if (countOfKeywords1 + countOfKeywords2 + countOfKeywords3 >= firstLevelF + secondLevelF + thirdLevelF)
                                    {
                                        if (firstLevelB + secondLevelB + thirdLevelB == 0)
                                        {
                                            continue;
                                        }
                                    }
                                }
                                else if (count3 == 2)
                                {
                                    foreach (byte[] i in result3[0].IDs)
                                    {
                                        if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                        {
                                            countOfKeywords3++;
                                        }
                                    }
                                    foreach (byte[] i in result3.Last().IDs)
                                    {
                                        if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                        {
                                            countOfKeywords3++;
                                        }
                                    }

                                    if (countOfKeywords1 + countOfKeywords2 + countOfKeywords3 >= firstLevelF + secondLevelF + thirdLevelF)
                                    {
                                        if (firstLevelB + secondLevelB + thirdLevelB == 0)
                                        {
                                            continue;
                                        }
                                    }
                                }
                                else if (count3 == 1)
                                {
                                    foreach (byte[] i in result3[0].IDs)
                                    {
                                        if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                        {
                                            countOfKeywords3++;
                                        }
                                    }
                                    if (countOfKeywords1 + countOfKeywords2 + countOfKeywords3 >= firstLevelF + secondLevelF + thirdLevelF)
                                    {
                                        if (firstLevelB + secondLevelB + thirdLevelB == 0)
                                        {
                                            continue;
                                        }
                                    }
                                }

                                if (countOfKeywords1 + countOfKeywords2 + countOfKeywords3 < firstLevelF + secondLevelF + thirdLevelF)
                                {
                                    contained = false;
                                    break;
                                }
                            }
                        }
                    }
                    //get the indexing which should be used for this keywords for backwarding neighbors
                    int countOfKeywordsB1 = 0;
                    int countOfKeywordsB2 = 0;
                    int countOfKeywordsB3 = 0;
                    //count the first level keywords
                    if (!(firstLevelB + secondLevelB + thirdLevelB == 0))
                    {
                        List<NBEntry> resultB1 = new List<NBEntry>();
                        resultB1 = (from s in resultIndexingNeighborhoodForID
                                    where s.Sigbyte.CompareTo(upperSig) <= 0 && s.Sigbyte.CompareTo(lowerSig) >= 0 && s.Distance == -1
                                    select s).ToList<NBEntry>();
                        int countB1 = resultB1.Count();
                        if (countB1 > 2)
                        {
                            for (int i = 1; i < countB1 - 1; i++)
                            {
                                countOfKeywordsB1 = countOfKeywordsB1 + resultB1[i].Count;
                            }
                            foreach (byte[] i in resultB1[0].IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywordsB1++;
                                }
                            }
                            foreach (byte[] i in resultB1.Last().IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywordsB1++;
                                }
                            }
                            if (countOfKeywordsB1 >= firstLevelB + secondLevelB + thirdLevelB)
                            {
                                continue;
                            }
                        }
                        else if (countB1 == 2)
                        {
                            foreach (byte[] i in resultB1[0].IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywordsB1++;
                                }
                            }
                            foreach (byte[] i in resultB1.Last().IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywordsB1++;
                                }
                            }
                            if (countOfKeywordsB1 >= firstLevelB + secondLevelB + thirdLevelB)
                            {
                                continue;
                            }
                        }
                        else if (countB1 == 1)
                        {
                            foreach (byte[] i in resultB1[0].IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywordsB1++;
                                }
                            }
                            if (countOfKeywordsB1 >= firstLevelB + secondLevelB + thirdLevelB)
                            {
                                continue;
                            }
                        }
                        if (countOfKeywordsB1 < firstLevelB)
                        {
                            contained = false;
                            break;
                        }

                        // count the second level keywords
                        List<NBEntry> resultB2 = new List<NBEntry>();
                        resultB2 = (from s in resultIndexingNeighborhoodForID
                                    where s.Sigbyte.CompareTo(upperSig) <= 0 && s.Sigbyte.CompareTo(lowerSig) >= 0 && s.Distance == -2
                                    select s).ToList<NBEntry>();
                        int countB2 = resultB2.Count();
                        if (countB2 > 2)
                        {
                            for (int i = 1; i < countB2 - 1; i++)
                            {
                                countOfKeywordsB2 = countOfKeywordsB2 + resultB2[i].Count;
                            }
                            foreach (byte[] i in resultB2[0].IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywordsB2++;
                                }
                            }
                            foreach (byte[] i in resultB2.Last().IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywordsB2++;
                                }
                            }
                            if (countOfKeywordsB1 + countOfKeywordsB2 >= firstLevelB + secondLevelB + thirdLevelB)
                            {
                                continue;
                            }
                        }
                        else if (countB2 == 2)
                        {
                            foreach (byte[] i in resultB2[0].IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywordsB2++;
                                }
                            }
                            foreach (byte[] i in resultB2.Last().IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywordsB2++;
                                }
                            }
                            if (countOfKeywordsB1 + countOfKeywordsB2 >= firstLevelB + secondLevelB + thirdLevelB)
                            {
                                continue;
                            }
                        }
                        else if (countB2 == 1)
                        {
                            foreach (byte[] i in resultB2[0].IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywordsB2++;
                                }
                            }
                            if (countOfKeywordsB1 + countOfKeywordsB2 >= firstLevelB + secondLevelB + thirdLevelB)
                            {
                                continue;
                            }
                        }

                        if (countOfKeywordsB1 + countOfKeywordsB2 < firstLevelB + secondLevelB)
                        {
                            contained = false;
                            break;
                        }
                        // count the third level keywords

                        List<NBEntry> resultB3 = new List<NBEntry>();
                        resultB3 = (from s in resultIndexingNeighborhoodForID
                                    where s.Sigbyte.CompareTo(upperSig) <= 0 && s.Sigbyte.CompareTo(lowerSig) >= 0 && s.Distance == -3
                                    select s).ToList<NBEntry>();
                        int countB3 = resultB3.Count();
                        if (countB3 > 2)
                        {
                            for (int i = 1; i < countB3 - 1; i++)
                            {
                                countOfKeywordsB3 = countOfKeywordsB3 + resultB3[i].Count;
                            }
                            foreach (byte[] i in resultB3[0].IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywordsB3++;
                                }
                            }
                            foreach (byte[] i in resultB3.Last().IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywordsB3++;
                                }
                            }
                        }
                        else if (countB3 == 2)
                        {
                            foreach (byte[] i in resultB3[0].IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywordsB3++;
                                }
                            }
                            foreach (byte[] i in resultB3.Last().IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywordsB3++;
                                }
                            }
                        }
                        else if (countB3 == 1)
                        {
                            foreach (byte[] i in resultB3[0].IDs)
                            {
                                if (NHContainment.matchKeyword(i, lowerFirstByte, upperFirstByte, lowerSecondByte, upperSecondByte))
                                {
                                    countOfKeywordsB3++;
                                }
                            }
                        }

                        if (countOfKeywordsB1 + countOfKeywordsB2 + countOfKeywordsB3 < firstLevelB + secondLevelB + thirdLevelB)
                        {
                            contained = false;
                            break;
                        }

                    }

                }
                return contained;
            }
            else
            {
                if (queryNeighbor.Count != 0)
                {
                    contained = false;
                }
                return contained;
            }
        }

        public static bool matchKeyword(byte[] i, byte lowerFirstByte, byte upperFirstByte, byte lowerSecondByte, byte upperSecondByte)
        {
            bool countAsKeyword = false;
            if (i[0] > lowerFirstByte && i[0] < upperFirstByte)
            {
                countAsKeyword = true;
            }
            else if (i[0] > lowerFirstByte && i[0] == upperFirstByte)
            {
                if (i[1] <= upperSecondByte)
                {
                    countAsKeyword = true;
                }
            }
            else if (i[0] < upperFirstByte && i[0] == lowerFirstByte)
            {
                if (i[1] >= lowerSecondByte)
                {
                    countAsKeyword = true;
                }
            }
            else if (i[0] == lowerFirstByte && i[0] == upperFirstByte)
            {
                if (i[1] >= lowerSecondByte && i[1] <= upperSecondByte)
                {
                    countAsKeyword = true;
                }
            }
            return countAsKeyword;
        }
    }
}