﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Monkeys_Timetable
{
    /// <summary>
    /// 显示开行方案
    /// </summary>
    public partial class LinePlan : Form
    {
        /// <summary>
        /// DataManager对象，用以读取与生成开行方案相关的数据
        /// </summary>
        DataManager dm;
        /// <summary>
        /// 存入上行开行方案的字典，key为车站名，int中0为不停站，1为停站
        /// </summary>
        Dictionary<string, int> UpPlanDic;
        /// <summary>
        /// 存入下行开行方案的字典，key为车站名，int中0为不停站，1为停站
        /// </summary>
        Dictionary<string, int> DownPlanDic;
        Dictionary<int, List<String>> StaLists;
        private List<Dictionary<string, float>> m_StaX;
        public List<Dictionary<string, float>> StaX
        {
            get
            {
                if(m_StaX == null)
                {
                    m_StaX = new List<Dictionary<string, float>>();
                }
                return m_StaX;
            }
            set
            {
                m_StaX = value;
            }
        }
        Bitmap bmp = new Bitmap(1240, 600);
        Pen pen = new Pen(Color.Black);
        SolidBrush brush = new SolidBrush(Color.Black);
        Font font = new Font("宋体", 8f);
        float curY;
        float curX;

        public LinePlan(Dictionary<int, List<String>> staLists)
        {
            InitializeComponent();
            dm = new DataManager();
            dm.ReadHeadway(Application.StartupPath + @"\\车站列车安全间隔.csv");
            dm.ReadStation(Application.StartupPath + @"\\沪宁车站信息.csv");
            dm.ReadTrain(Application.StartupPath + @"\\沪宁时刻图.csv");
            dm.DivideUpDown();
            dm.AddTra2sta();
            dm.GetStop();
            this.StaLists = staLists;
            Graphics g = Graphics.FromImage(bmp);
            GetPlan();
            DrawFrame(pictureBox1.Width, pictureBox1.Height, g);
            DrawLine(g);
        }
        /// <summary>
        /// 生成列车开行方案，并存入开行方案字典和DataTable
        /// </summary>
        public void GetPlan()
        {
            UpPlanDic = new Dictionary<string, int>();
            DownPlanDic = new Dictionary<string, int>();
            for (int i = 0; i < dm.upTrainList.Count; i++)
            {
                string StaConList = "";
                for (int j = 0; j < dm.upTrainList[i].staList.Count; j++)
                {
                    if ((j == 0) && (dm.upTrainList[i].isStopDic[dm.upTrainList[i].staList[j]] == true))
                    {
                        StaConList = dm.upTrainList[i].staList[j];
                    }
                    else if (dm.upTrainList[i].isStopDic[dm.upTrainList[i].staList[j]] == true)
                    {
                        StaConList = StaConList + "," + dm.upTrainList[i].staList[j];
                    }
                }
                if (!UpPlanDic.Keys.Contains(StaConList))
                {
                    UpPlanDic.Add(StaConList, 1);
                }
                else if (UpPlanDic.Keys.Contains(StaConList))
                {
                    UpPlanDic[StaConList] += 1;
                }
            }
            for (int i = 0; i < dm.downTrainList.Count; i++)
            {
                string StaConList = "";
                for (int j = 0; j < dm.downTrainList[i].staList.Count; j++)
                {
                    if ((j == 0) && (dm.downTrainList[i].isStopDic[dm.downTrainList[i].staList[j]] == true))
                    {
                        StaConList = dm.downTrainList[i].staList[j];
                    }
                    else if (dm.downTrainList[i].isStopDic[dm.downTrainList[i].staList[j]] == true)
                    {
                        StaConList = StaConList + "," + dm.downTrainList[i].staList[j];
                    }
                }
                if (!DownPlanDic.Keys.Contains(StaConList))
                {
                    DownPlanDic.Add(StaConList, 1);
                }
                else if (DownPlanDic.Keys.Contains(StaConList))
                {
                    DownPlanDic[StaConList] += 1;
                }
            }
        }
        public void DrawFrame(float Width,float Height,Graphics g)
        {          
            float Left = 30;
            float Right = 5;
            float Up = 30;
            float Down = 5;
            int totalSta = 0;
            for(int i = 1; i < StaLists.Count + 1; i++)
            {
                for(int j = 0; j < StaLists[i].Count; j++)
                {
                    totalSta += 1;
                }
            }
            float gapBetweenSta = (Width - Left - Right) / ((float)totalSta + (float)(StaLists.Count - 1));
            float gapBetweenSec = gapBetweenSta;
            curX = 0;
            curY = Up;
            bool UpDown = true;
            g.DrawString("数量", font, brush, curX, curY);
            curX = Left;
            for (int i = 1; i < StaLists.Count + 1; i++)
            {
                Dictionary<string, float> LocDic = new Dictionary<string, float>();
                StaX.Add(LocDic);
                UpDown = true;              
                for (int j = 0; j < StaLists[i].Count; j++)
                {
                    g.DrawEllipse(pen, curX, Up, 5, 5);
                    if (UpDown)
                    {
                        g.DrawString(StaLists[i][j], font, brush, curX - 10, curY + 15);
                        StaX[i - 1].Add(StaLists[i][j], curX);
                    }
                    if (!UpDown)
                    {
                        g.DrawString(StaLists[i][j], font, brush, curX - 10, curY - 20);
                        StaX[i - 1].Add(StaLists[i][j], curX);
                    }
                    UpDown = !UpDown;
                    curX += gapBetweenSta;
                }
                curX += gapBetweenSec;            
            }
            curY += 40;
            this.pictureBox1.BackgroundImage = bmp;
        }
        public void DrawLine(Graphics g)
        {
            foreach(KeyValuePair<string,int> Line in UpPlanDic)
            {
                string[] stas = Line.Key.Split(',');
                g.DrawString(Line.Value.ToString(), font, brush, 0, curY);
                for (int i = 1; i < StaLists.Count + 1; i++)
                {
                    for (int j = 0; j < StaLists[i].Count; j++)
                    {
                        if (stas.Contains(StaLists[i][j]))
                        {
                            g.FillEllipse(brush, StaX[i - 1][StaLists[i][j]], curY, 7, 7);
                        }
                    }
                }
                //for(int i = 0; i < stas.Count(); i++)
                //{
                //    for(int j = 1; j < StaLists.Count; j++)
                //    {
                //        if(StaLists[j].Contains(stas[i])&&(!StaLists[j + 1].Contains(stas[i])))
                //        {
                //            g.FillEllipse(brush, StaX[j - 1][stas[i]], curY, 7, 7);
                //        }
                //    }
                //}
                //for (int i = 0; i < stas.Count(); i++)
                //{
                //    if ((StaLists[StaLists.Count].Contains(stas[i])&& (!StaLists[StaLists.Count - 1].Contains(stas[i]))))
                //    {
                //        g.FillEllipse(brush, StaX[StaLists.Count - 1][stas[i]], curY, 7, 7);
                //    }                   
                //}
                for (int i = 1; i < StaLists.Count + 1; i++)
                {
                    for (int j = 0; j < stas.Count() - 1; j++)
                    {
                        if ((StaLists[i].Contains(stas[j]) && (StaLists[i].Contains(stas[j + 1]))))
                        {
                            g.DrawLine(pen, StaX[i - 1][stas[j]], curY + 3, StaX[i - 1][stas[j + 1]], curY + 3);
                        }
                    }
                }
                Pen pen1 = new Pen(Color.Black);
                pen1.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                pen1.DashPattern = new float[] { 5, 5 };
                for (int i = 1; i < StaLists.Count; i++)
                {
                    for(int j = 0; j < stas.Count() - 1; j++)
                    {
                        if((StaLists[i].Contains(stas[j]) && (StaLists[i + 1].Contains(stas[j + 1]))))
                        {
                            g.DrawLine(pen1, StaX[i - 1][stas[j]], curY, StaX[i][stas[j + 1]], curY);
                        }
                    }
                }
                curY += 10;
            }
        }
        /// <summary>
        /// 将开行方案DataTable以DataGridView形式显示于窗体中
        /// </summary>    
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void LinePlan_Load(object sender, EventArgs e)
        {

        }
    }
}
