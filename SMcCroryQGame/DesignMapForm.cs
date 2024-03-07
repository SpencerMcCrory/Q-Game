/*
 * Program: SMcCroryQgame - Assignment 2
 * 
 * purpose:Create a game where a user can design maps/levels. Have two seperate forms.
 * 
 * 
 * Game Programming with Data Structures PROG2370 - Section 9
 * Created: by Spencer McCrory 8878450
 * November 18th, 2023
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMcCroryQGame
{
    public partial class DesignMapForm : Form
    {
        
        //Sizes for picture boxes
        private const int WIDTH = 50;
        private const int HEIGHT = 50;




        public DesignMapForm()
        {
            InitializeComponent();
        }


        /*This method generates all the picture boxes as long as there is valid input*/
        private void btnGenerate_Click(object sender, EventArgs e)
        {
        /*check if user already has a map generated*/
            if(panelMap.Controls.Count != 0)
            {
            /*ask if they want to override their current level*/
                DialogResult userChoice = MessageBox.Show("Do you want to create a new map?\n" +
                    "If you do, the current level will be lost",
                    "Already map generated",MessageBoxButtons.YesNo);
                if (userChoice == DialogResult.No)
                {
                    return;
                }
                else
                {
                    ClearMap();
                }
            }
            int rows = 0;
            int columns = 0;
            try
            {
                /*make sure rows and columns are ints within the try catch*/
                rows = int.Parse(txtRows.Text);
                columns = int.Parse(txtColumns.Text);

            /*check if rows and columns are both positve otherwise throw exception error*/
                if (rows < 0 || columns < 0)
                {
                    throw new ArgumentOutOfRangeException("Rows and columns must be non-negative.");
                }
                
             }
            catch(FormatException) {
                MessageBox.Show("Please provide valid data for rows and columns (Both must be integers)","User entered incorrect data.");

            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Rows and columns must be non-negative integer.", "User enter incorrect data");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            //Starting location of squares
            int x = 0;
            int y = 0;

            //build the grid and set up picture boxes to Nones
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Square pb = new Square(i, j, "None");
                    Square sb = new Square("None");
                    pb.Left = x;
                    pb.Top = y;
                    pb.Width = WIDTH;
                    pb.Height = HEIGHT;
                    pb.Text = i.ToString() + " " + j.ToString();
                    pb.Image = Properties.Resources.None_icon;
                    pb.SizeMode = PictureBoxSizeMode.StretchImage;
                    pb.BackColor = Color.FromArgb(172, 172, 172);
                    pb.BorderStyle = BorderStyle.Fixed3D;


                    panelMap.Controls.Add(pb); 

                    pb.Click += PB_Click;

                    x += WIDTH;
                }
                //reset column

                x = 0;
                y += HEIGHT;
                
            }





        }

        /*Event handler for all the picture boxes that were dynamically created*/
        private void PB_Click(object sender, EventArgs e)
        {
            /*save what square was selected*/
            Square pb = (Square)sender;

            /*change the square to the new selected square*/
            switch (pb.GetSelectedStructure())
            {
                case "RedDoor":
                    pb.Image = Properties.Resources.redDoor_icon;
                    pb.SetStructure("RedDoor");
                        break;
                case "None":
                    pb.Image = Properties.Resources.None_icon;
                    pb.SetStructure("None");
                    break;
                case "GreenDoor":
                    pb.Image = Properties.Resources.greenDoor_icon;
                    pb.SetStructure("GreenDoor");
                    break;
                case "RedBox":
                    pb.Image = Properties.Resources.redBox_icon;
                    pb.SetStructure("RedBox");
                    break;
                case "GreenBox":
                    pb.Image = Properties.Resources.greenBox_icon;
                    pb.SetStructure("GreenBox");
                    break;
                case "Wall":
                    pb.Image = Properties.Resources.wall;
                    pb.SetStructure("Wall");
                    break;


            }
        }

            /*All event handlers for tools, updates a static variable called selectedStructure*/
        private void btnNone_Click(object sender, EventArgs e)
        {
            Square sb = new Square("None");
        }

        private void btnWall_Click(object sender, EventArgs e)
        {
            Square sb = new Square("Wall");
        }

        private void btnRedDoor_Click(object sender, EventArgs e)
        {
            Square sb = new Square("RedDoor");
        }

        private void btnGreenDoor_Click(object sender, EventArgs e)
        {
            Square sb = new Square("GreenDoor");
        }

        private void btnRedBox_Click(object sender, EventArgs e)
        {
            Square sb = new Square("RedBox");
        }

        private void btnGreenBox_Click(object sender, EventArgs e)
        {
            Square sb = new Square("GreenBox");
        }
        /*end of tool event handlers*/

        
/*Event handler for save tool tip*/
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
        /*Check if user has a map or not, throw an exception if they dont*/
            try
            {
                if (panelMap.Controls.Count < 1)
                {
                    throw new Exception("Please add a map first to save.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            /*setting up save dialog box*/
            SaveFileDialog dlgSave = new SaveFileDialog();
            dlgSave.Filter = "QGame Files (*.QGame)|*.QGame";
            dlgSave.FileName = "Untitled.QGame";

            string relativePath = @"..\..\SavedFiles"; // Adjust the path as necessary
            string absolutePath = Path.GetFullPath(Path.Combine(Application.StartupPath, relativePath));

            if (Directory.Exists(absolutePath))
            {
                dlgSave.InitialDirectory = absolutePath;
            }
            

            //show dialog box
            DialogResult response = dlgSave.ShowDialog();

            switch (response)
            {
                case DialogResult.None:
                    break;
                case DialogResult.OK:
                    try
                    {/*try to save the file if they hit okay*/
                        
                        string fileName = dlgSave.FileName;
                        int numberOfSquares = panelMap.Controls.Count;
                        if (numberOfSquares < 0) {
                            throw new ArgumentNullException("You have not created a map yet. Please add a map to save.");
                        }
                        SaveMap(fileName);
                        SaveSuccessful();
                        

                    }
                    catch (ArgumentNullException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error in file save");
                    }

                    break;
                default:
                    break;
            }
        }

        /*Method that gets the total number of rows and columns and saves the map to a text file*/
        private void SaveMap(string fileName)
        {
            int totalCols = 0;
            int totalRows = 0;
            int currentCol = 0;
            int currentRow = 0;
            foreach (Square square in panelMap.Controls)
            {
/*have to add one because indexes start at 0, but I want a count here*/
                currentCol = square.GetCol() + 1;
                currentRow = square.GetRow() + 1;
                totalCols = totalCols < currentCol ? currentCol : totalCols;
                totalRows = totalRows < currentRow ? currentRow : totalRows;
            }


/*setting up the text file*/
            StreamWriter writer = new StreamWriter(fileName);
            writer.WriteLine(totalRows.ToString());
            writer.WriteLine(totalCols.ToString());
            foreach (Square square in panelMap.Controls)
            {
                /*writing each square info in the text file*/
                writer.WriteLine(square.GetRow().ToString()+","+ square.GetCol().ToString()+ "," + Square.StructureDictionary[square.GetStructure()]);
                

            }
            writer.Close();
        }
/*end of saveMethod*/

/*Clearing all the picture boxes*/
        private void ClearMap()
        {
            int numberOfSquares = panelMap.Controls.Count;

            //clear up memory storage rather manually than typing panelMap.Controls.Clear();
            /*for (int i = numberOfSquares - 1; i >= 0; i--)
            {
                var square = panelMap.Controls[i];
                panelMap.Controls.Remove(square);
                square.Dispose();
            }*/
            panelMap.Controls.Clear();
            GC.Collect();
        }

/*Success message when a save was complete*/
        private void SaveSuccessful()
        {
            int doors = 0;
            int boxes = 0;
            int walls = 0;
            foreach (Square square in panelMap.Controls)
            {
                if (square.GetStructure() == "Wall")
                {
                    walls++;
                }
                else if (square.GetStructure() == "RedDoor" || square.GetStructure() == "GreenDoor")
                {
                    doors++;
                }
                else if (square.GetStructure() == "RedBox" || square.GetStructure() == "GreenBox")
                {
                    boxes++;
                }

            }

            MessageBox.Show($"File saved successfully:\n" + $"Total number of walls: {walls}\n" + $"Total number of boxes: {boxes}\nTotal number of doors: {doors}");
        }

/*tool tip close, which closes the program*/
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.FindForm().Close();
        }


    }
}
