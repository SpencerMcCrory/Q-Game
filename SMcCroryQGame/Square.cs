using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMcCroryQGame
{
    internal class Square : PictureBox
    {
        int row { get; set; }
        int col { get; set; }
        string structure { get; set; }

        static string selectedStructure;

        public static readonly Dictionary<string, string> StructureDictionary = new Dictionary<string, string> { {"None","0" },{"Wall","1" },{ "RedDoor", "2" },{"GreenDoor","3" },{"RedBox","6" },{"GreenBox","7" } };

        public static readonly Dictionary<string, string> StructureImageDictionary = new Dictionary<string, string> { { "None", "None_icon" }, { "Wall", "wall" }, { "RedDoor", "redDoor_icon" }, { "GreenDoor", "greenDoor_icon" }, { "RedBox", "redBox_icon" }, { "GreenBox", "greenBox_icon" } };

        /*Constructor for the squares that gets instantiated on generation*/
        public Square(int row, int col, string structure):base()
        {
            this.row = row;
            this.col = col;
            this.structure = structure;
        }



/*upating the currently selected structure (for the tools)*/
        public Square(string struc)
        {
            selectedStructure = struc;
        }

        public string GetSelectedStructure()
        {
            return selectedStructure;
        }

        public int GetRow()
        {
            return row;
        }
        public int GetCol()
        {
            return col;
        }
        public string GetStructure()
        {
            return structure;
        }

        /*updating picturebox current structure*/
        public void SetStructure(string struc)
        {
            structure = struc;
        }


       
    }
}
