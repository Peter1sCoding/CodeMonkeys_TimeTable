﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monkeys_Timetable
{
    class Station
    {
        private string x_stationNo;//车站序号
        public string stationNo
        {
            get
            {
                return x_stationNo;
            }
            set
            {
                x_stationNo = value;
            }
        }

        private string x_stationName;//车站名称
        public string stationName
        {
            get
            {
                return x_stationName;
            }
            set
            {
                x_stationName = value;
            }
        }

        private int x_totalMile; //车站累计里程
        public int totalMile
        {
            get
            {
                return x_totalMile;
            }
            set
            {
                x_totalMile = value;
            }
        }
        private List<Train> x_upStaTraArrList;
        public List<Train> upStaTraArrList
        {
            get
            {
                return x_upStaTraArrList;
            }
            set
            {
                x_upStaTraArrList = value;
            }
        }
        private List<Train> x_upStaTraDepList;
        public List<Train> upStaTraDepList
        {
            get
            {
                return x_upStaTraDepList;
            }
            set
            {
                x_upStaTraDepList = value;
            }
        }
        private List<Train> x_downStaTraArrList;
        public List<Train> downStaTraArrList
        {
            get
            {
                return x_downStaTraArrList;
            }
            set
            {
                x_downStaTraArrList = value;
            }
        }
        private List<Train> x_downStaTraDepList;
        public List<Train> downStaTraDepList
        {
            get
            {
                return x_downStaTraDepList;
            }
            set
            {
                x_downStaTraDepList = value;
            }
        }
    }
}
