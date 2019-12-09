﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace Monkeys_Timetable
{
    class DataManager  //读入到发时刻及车站等数据的类，封装读取列车、车站等方法
    {
        private List<Train> x_TtrainList; //列车列表
        public List<Train> TrainList
        {
            get
            {
                return x_TtrainList;
            }
            set
            {
                x_TtrainList = value;
            }
        }
        private Dictionary<string, Train> m_TrainDic; //列车字典
        //列车字典
        public Dictionary<string, Train> TrainDic
        {
            get
            {
                if (m_TrainDic == null)
                    m_TrainDic = new Dictionary<string, Train>();
                return m_TrainDic;
            }
            set { m_TrainDic = value; }
        }
        private Dictionary<string, Train> m_UpTrainDic; //上行列车字典
        //列车字典
        public Dictionary<string, Train> UpTrainDic
        {
            get
            {
                if (m_UpTrainDic == null)
                    m_UpTrainDic = new Dictionary<string, Train>();
                return m_UpTrainDic;
            }
            set { m_UpTrainDic = value; }
        }
        private Dictionary<string, Train> m_DownTrainDic; //列车字典
        //列车字典
        public Dictionary<string, Train> DownTrainDic
        {
            get
            {
                if (m_DownTrainDic == null)
                    m_DownTrainDic = new Dictionary<string, Train>();
                return m_DownTrainDic;
            }
            set { m_DownTrainDic = value; }
        }
        private List<Station> x_stationList;//车站列表
        public List<Station> stationList
        {
            get
            {
                return x_stationList;
            }
            set
            {
                x_stationList = value;
            }
        }

        private List<String> x_stationStringList;//车站列表
        public List<String> stationStringList
        {
            get
            {
                return x_stationStringList;
            }
            set
            {
                x_stationStringList = value;
            }
        }

        private Dictionary<string, Dictionary<string,int>> x_HeadwayDic; //通过"站名+上下行+间隔类型"索引间隔时间标准
        public Dictionary<string, Dictionary<string, int>> HeadwayDic
        {
            get
            {
                return x_HeadwayDic;
            }
            set
            {
                x_HeadwayDic = value;
            }
        }

        public void ReadTrain(string Filename)
        {
            TrainDic.Clear();
            StreamReader sr = new StreamReader(Filename, Encoding.UTF8);
            sr.ReadLine();
            string str = sr.ReadLine();
            while (str != null)
            {
                Train tra = new Train();
                str = str.Replace("\r", string.Empty).Replace("\"", string.Empty).Replace("\t", string.Empty).Replace("'", string.Empty).Replace("\\", string.Empty).Replace("\0", string.Empty).Replace("?", string.Empty).Replace("*", string.Empty);
                String[] strr = str.Split(',');
                tra.trainNo = strr[0];
                string staname = strr[2];
                if (!TrainDic.ContainsKey(tra.trainNo))
                {
                    tra.staTimeDic = new Dictionary<string, List<string>>();
                    if (!tra.staTimeDic.ContainsKey(staname))
                    {
                        List<string> timelist = new List<string>();
                        timelist.Add(strr[3]);
                        timelist.Add(strr[4]);
                        timelist.Add(strr[5]);
                        tra.staTimeDic.Add(staname, timelist);
                    }
                    TrainDic.Add(tra.trainNo, tra);
                }
                else
                {
                    if (!TrainDic[tra.trainNo].staTimeDic.ContainsKey(staname))
                    {
                        List<string> timelist = new List<string>();
                        timelist.Add(strr[3]);
                        timelist.Add(strr[4]);
                        timelist.Add(strr[5]);
                        TrainDic[tra.trainNo].staTimeDic.Add(staname, timelist);
                    }
                }
                str = sr.ReadLine();
            }
            sr.Close();
            TrainList = new List<Train>();

            foreach (KeyValuePair<string, Train> trainNumber in TrainDic)//给trainList赋值
            {
                TrainList.Add(TrainDic[trainNumber.Key]);
            }
            for(int i = 0; i < TrainList.Count(); i++)
            {
                TrainList[i].staList = new List<string>();
                foreach (KeyValuePair<string, List<string>> trainNumber in TrainList[i].staTimeDic)
                {
                    TrainList[i].staList.Add(trainNumber.Key);
                }
            }
        }
        public void DivideUpDown()
        {
            UpTrainDic = new Dictionary<string, Train>();
            DownTrainDic = new Dictionary<string, Train>();
            for (int i = 0; i < TrainList.Count; i++)
            {
                string LastNumber = TrainList[i].trainNo.Substring(TrainList[i].trainNo.Length - 1, 1);
                if (( LastNumber == "0")||(LastNumber == "2") || (LastNumber == "4") || (LastNumber == "6") || (LastNumber == "8"))
                {
                    UpTrainDic.Add(TrainList[i].trainNo, TrainList[i]);
                }
                else if ((LastNumber == "1") || (LastNumber == "3") || (LastNumber == "5") || (LastNumber == "7") || (LastNumber == "9"))
                {
                    DownTrainDic.Add(TrainList[i].trainNo, TrainList[i]);
                }
            }
        }

        public void ReadStation(string Filename)
        {
            StreamReader sr = new StreamReader(Filename, Encoding.UTF8);
            string str = sr.ReadLine();
            stationList = new List<Station>();
            stationStringList = new List<String>();
            while (str != null)
            {
                Station sta = new Station();
                str = str.Replace("\r", string.Empty).Replace("\"", string.Empty).Replace("\t", string.Empty).Replace("'", string.Empty).Replace("\\", string.Empty).Replace("\0", string.Empty).Replace("?", string.Empty).Replace("*", string.Empty);
                String[] strr = str.Split(',');
                sta.stationName = strr[1];
                sta.totalMile = int.Parse(strr[2]);
                stationList.Add(sta);
                stationStringList.Add(sta.stationName);
                str = sr.ReadLine();
            }
            sr.Close();
        }
        public void OutPutTimetable(List<Train> trainList,List<string> stationlist)
        {
            FileStream fs = new FileStream(Environment.CurrentDirectory + @"\\运行图输出.csv", System.IO.FileMode.Open, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

            sw.Write("车次,");
            for (int i = 0; i < stationlist.Count; i++)
            {
                if (i == 0)
                {
                    sw.Write(stationlist[i] + "出发,");
                }
                else if (i == (stationlist.Count() - 1))
                {
                    sw.Write(stationlist[i] + "到达,");
                }
                else
                {
                    sw.Write(stationlist[i] + "到达," + stationlist[i] + "出发,");
                }
            }
            sw.Write("\r\n");
            for (int i = 0; i < trainList.Count(); i++)
            {               
                sw.Write(trainList[i].trainNo + ",");
                int begin = stationStringList.IndexOf(trainList[i].staList[0]);
                int end = stationStringList.IndexOf(trainList[i].staList[trainList[i].staList.Count() - 1]);
                string front = "";
                for (int j = 0; j < begin; j++)
                {
                    if (j == 1)
                    {
                        front += "0,";
                    }
                    else if (j != 1)
                    {
                        front += "0,0,";
                    }
                }
                foreach (string station in trainList[i].staList)
                {
                    sw.Write(trainList[i].staTimeDic[station][0] + ",");
                    sw.Write(trainList[i].staTimeDic[station][1] + ",");                                  
                }              
                sw.Write("\r\n");
            }
            sw.Close();
            fs.Close();
        }        
        public void ReadHeadway(string FileName)//读取车站列车安全间隔
        {
            HeadwayDic = new Dictionary<string, Dictionary<string,int>>();
            StreamReader sr = new StreamReader(FileName, Encoding.Default);
            string[] speed = sr.ReadLine().Replace("\r", string.Empty).Replace("\"", string.Empty).Replace("\t", string.Empty).Replace("'", string.Empty).Replace("\\", string.Empty).Replace("\0", string.Empty).Replace("?", string.Empty).Replace("*", string.Empty).Split(',');

            string speed1 = speed[2].Substring(2, 3);
            string speed2 = speed[11].Substring(2, 3);
            sr.ReadLine();
            string str = sr.ReadLine();
            while (str != null)
            {
                str = str.Replace("\r", string.Empty).Replace("\"", string.Empty).Replace("\t", string.Empty).Replace("'", string.Empty).Replace("\\", string.Empty).Replace("\0", string.Empty).Replace("?", string.Empty).Replace("*", string.Empty);
                string[] strr = str.Split(',');
                if (Convert.ToInt32(strr[1]) == 1) //判断上下行，存到不同的索引值中
                {
                    string key = ConcatenateAllString(strr[0], "down", speed1);
                    if (!HeadwayDic.ContainsKey(key))
                    {
                        Dictionary<string, int> TrainHeadway = new Dictionary<string, int>();
                        int dd = Convert.ToInt32(strr[2]);
                        int df = Convert.ToInt32(strr[3]);
                        int dt = Convert.ToInt32(strr[4]);
                        int fd = Convert.ToInt32(strr[5]);
                        int ff = Convert.ToInt32(strr[6]);
                        int ft = Convert.ToInt32(strr[7]);
                        int td = Convert.ToInt32(strr[8]);
                        int tf = Convert.ToInt32(strr[9]);
                        int tt = Convert.ToInt32(strr[10]);

                        TrainHeadway.Add("到到", dd);
                        TrainHeadway.Add("到发", df);
                        TrainHeadway.Add("到通", dt);
                        TrainHeadway.Add("发到", fd);
                        TrainHeadway.Add("发发", ff);
                        TrainHeadway.Add("发通", ft);
                        TrainHeadway.Add("通到", td);
                        TrainHeadway.Add("通发", tf);
                        TrainHeadway.Add("通通", tt);
                        HeadwayDic.Add(key, TrainHeadway);
                    }
                    string key1 = ConcatenateAllString(strr[0], "down", speed2);//存入第二个速度等级的间隔时分
                    if (!HeadwayDic.ContainsKey(key1))
                    {
                        Dictionary<string, int> TrainHeadway = new Dictionary<string, int>();
                        int dd = Convert.ToInt32(strr[11]);
                        int df = Convert.ToInt32(strr[12]);
                        int dt = Convert.ToInt32(strr[13]);
                        int fd = Convert.ToInt32(strr[14]);
                        int ff = Convert.ToInt32(strr[15]);
                        int ft = Convert.ToInt32(strr[16]);
                        int td = Convert.ToInt32(strr[17]);
                        int tf = Convert.ToInt32(strr[18]);
                        int tt = Convert.ToInt32(strr[19]);

                        TrainHeadway.Add("到到", dd);
                        TrainHeadway.Add("到发", df);
                        TrainHeadway.Add("到通", dt);
                        TrainHeadway.Add("发到", fd);
                        TrainHeadway.Add("发发", ff);
                        TrainHeadway.Add("发通", ft);
                        TrainHeadway.Add("通到", td);
                        TrainHeadway.Add("通发", tf);
                        TrainHeadway.Add("通通", tt);
                        HeadwayDic.Add(key1, TrainHeadway);
                    }
                }
                else if (Convert.ToInt32(strr[1]) == 0)
                {
                    string key = ConcatenateAllString(strr[0], "up", speed1);
                    if (!HeadwayDic.ContainsKey(key))
                    {
                        Dictionary<string, int> TrainHeadway = new Dictionary<string, int>();
                        int dd = Convert.ToInt32(strr[2]);
                        int df = Convert.ToInt32(strr[3]);
                        int dt = Convert.ToInt32(strr[4]);
                        int fd = Convert.ToInt32(strr[5]);
                        int ff = Convert.ToInt32(strr[6]);
                        int ft = Convert.ToInt32(strr[7]);
                        int td = Convert.ToInt32(strr[8]);
                        int tf = Convert.ToInt32(strr[9]);
                        int tt = Convert.ToInt32(strr[10]);
                        TrainHeadway.Add("到到", dd);
                        TrainHeadway.Add("到发", df);
                        TrainHeadway.Add("到通", dt);
                        TrainHeadway.Add("发到", fd);
                        TrainHeadway.Add("发发", ff);
                        TrainHeadway.Add("发通", ft);
                        TrainHeadway.Add("通到", td);
                        TrainHeadway.Add("通发", tf);
                        TrainHeadway.Add("通通", tt);
                        HeadwayDic.Add(key, TrainHeadway);
                    }
                    string key1 = ConcatenateAllString(strr[0], "up", speed2);
                    if (!HeadwayDic.ContainsKey(key1))
                    {
                        Dictionary<string, int> TrainHeadway = new Dictionary<string, int>();
                        int dd = Convert.ToInt32(strr[11]);
                        int df = Convert.ToInt32(strr[12]);
                        int dt = Convert.ToInt32(strr[13]);
                        int fd = Convert.ToInt32(strr[14]);
                        int ff = Convert.ToInt32(strr[15]);
                        int ft = Convert.ToInt32(strr[16]);
                        int td = Convert.ToInt32(strr[17]);
                        int tf = Convert.ToInt32(strr[18]);
                        int tt = Convert.ToInt32(strr[19]);
                        TrainHeadway.Add("到到", dd);
                        TrainHeadway.Add("到发", df);
                        TrainHeadway.Add("到通", dt);
                        TrainHeadway.Add("发到", fd);
                        TrainHeadway.Add("发发", ff);
                        TrainHeadway.Add("发通", ft);
                        TrainHeadway.Add("通到", td);
                        TrainHeadway.Add("通发", tf);
                        TrainHeadway.Add("通通", tt);
                        HeadwayDic.Add(key1, TrainHeadway);
                    }
                }
                str = sr.ReadLine();
            }
            sr.Close();
        }

        public string ConcatenateAllString(string a1,string a2,string a3)
        {
            string s = a1 + a2 + a3;
            return s;
        }
    }
}
