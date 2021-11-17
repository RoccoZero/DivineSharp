using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFinding
{
    enum tType : byte
    {
        Wall,
        Forest,
        Jungle,
        Road,
        Lava,
        Sands
    }
    enum AlgType : byte
    {
        BestFirst,
        Dijkstra,
        AStar
    }
    enum Status : byte
    {
        Running,
        Paused,
        Stopped
    }
    struct Cell
    {
        public int xIndex;
        public int yIndex;

        public Cell(int x, int y) { xIndex = x; yIndex = y; }
        public static Cell operator +(Cell cl1, Cell cl2)
        {
            Cell result = new Cell();
            result.xIndex = cl1.xIndex + cl2.xIndex;
            result.yIndex = cl1.yIndex + cl2.yIndex;
            return result;
        }
        public static bool operator ==(Cell cl1, Cell cl2)
        {
            return (cl1.xIndex == cl2.xIndex) && (cl1.yIndex == cl2.yIndex);
        }
        public static bool operator !=(Cell cl1, Cell cl2)
        {
            return !(cl1 == cl2);
        }
        public static explicit operator string(Cell cl)
        {
            return cl.xIndex + " " + cl.yIndex;
        }
        public static explicit operator Cell(string str)
        {
            char[] separator = { ' ' };
            string[] data = str.Split(separator, 2);
            Cell result = new Cell();
            result.xIndex = Convert.ToInt32(data[0]);
            result.yIndex = Convert.ToInt32(data[1]);
            return result;
        }
    }

    struct Map
    {
        public Cell parent;
        public Cell cell;
        public int gValue;
        public int fValue;
        public int StepCoast;
        public tType type;
        public static explicit operator string(Map map)
        {
            return (string)map.cell + " " + map.StepCoast + " " + (int)map.type;
        }
        public static explicit operator Map(string str)
        {
            char[] separator = { ' ' };
            string[] data = str.Split(separator, 4);
            Map result = new Map();
            result.cell.xIndex = Convert.ToInt32(data[0]);
            result.cell.yIndex = Convert.ToInt32(data[1]);
            result.StepCoast = Convert.ToInt32(data[2]);
            result.type = (tType)Convert.ToInt32(data[3]);
            return result;
        }
    }

    
    struct GrInfo
    {
        public bool terrTypeWasUsed,
            terrCoastWasChanged;
    }
}
