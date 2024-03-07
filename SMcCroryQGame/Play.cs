/*
 * Program: SMcCroryQgame - Assignment 3
 * 
 * purpose:Create a game where a user can interact with boxes and have them be destroyed when
 *          they run into the same coloured door. Count the number of moves and how many boxes are left.
 *          Be able to a load a QGame file.
 * 
 * 
 * Game Programming with Data Structures PROG2370 - Section 9
 * Created: by Spencer McCrory 8878450
 * December 8th, 2023
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMcCroryQGame
{

    public partial class Play : Form
    {
        //Enum that will be used for what direction the square is going
        private enum Direction { Up, Down, Left, Right }


        //Sizes for picture boxes
        private const int WIDTH = 50;
        private const int HEIGHT = 50;
        private string[,] mapGrid;
        Square lastClickedSquare;

        public Play()
        {
            InitializeComponent();
            
        }

        //opens file dialog box where user can open a QGAME file, tries opening the file and tells user of any
        //errors if it occurs
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {

            /*setting up save dialog box*/
            // Opens the relative path based on each user
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.Filter = "QGame Files (*.QGame)|*.QGame";
            dlgOpen.RestoreDirectory = false;
            string relativePath = @"..\..\SavedFiles"; 
            string absolutePath = Path.GetFullPath(Path.Combine(Application.StartupPath, relativePath));

            if (Directory.Exists(absolutePath))
            {
                dlgOpen.InitialDirectory = absolutePath;
            }
            else
            {
                MessageBox.Show("SaveFiles directory does not exist. Opening default directory.");
            }

            //show dialog box
            DialogResult response = dlgOpen.ShowDialog();

            switch (response)
            {
                case DialogResult.OK:
                    //exception handling if the file 
                    try
                    {/*try to load the file if they hit okay*/

                        string fileName = dlgOpen.FileName;
                        LoadMap(fileName); // Method to load and process the file
                    }
                    catch (ArgumentNullException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    break;
                default:
                    MessageBox.Show("You selected no file. \nYou must select a file if you want to play a map.", "QGame");
                    break;
            }
        }
        //This takes a file name, reads the file and generates the map based on the file
        //also creates a grid map that will be used to check for collision
        private void LoadMap(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);

            // Read total rows and columns
            int totalRows = int.Parse(reader.ReadLine());
            int totalCols = int.Parse(reader.ReadLine());
            mapGrid = new string[totalRows, totalCols];

            // Clear the existing map before loading new one
            panelMap.Controls.Clear();

            int x = 0;
            int y = 0;

            // Read and create each Square
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] parts = line.Split(',');

                // Ensure that there are exactly three parts for row, col, and structure
                if (parts.Length == 3)
                {
                    int row = int.Parse(parts[0]);
                    int col = int.Parse(parts[1]);
                    string structureValue = parts[2];
                    string structure = Square.StructureDictionary.FirstOrDefault(s => s.Value == structureValue).Key;

                    Square square = new Square(row, col, structure); // Create new Square with row, col, and structure
                    square.Left = x;
                    square.Top = y;
                    square.Width = WIDTH;
                    square.Height = HEIGHT;
                    square.SizeMode = PictureBoxSizeMode.StretchImage;
                    square.BackColor = Color.FromArgb(172, 172, 172);
                    square.BorderStyle = BorderStyle.Fixed3D;
                    square.Image = GetStructureImage(structure);
                    panelMap.Controls.Add(square); // Add the square to your panel or map
                    mapGrid[row, col] = structure;
                    square.Click += Square_Click;

                    x += WIDTH;
                    if (x >= WIDTH * totalCols)
                    {
                        x = 0;
                        y += HEIGHT;
                    }
                    if (x >= WIDTH * totalCols) // Check if end of row is reached
                    {
                        x = 0;        // Reset X to start of next row
                        y += HEIGHT;  // Move Y down to start a new row
                    }
                }
                else
                {
                    // Handle the error or ignore the line
                }
            }

            reader.Close();
            txtNumberOfMoves.Text = "0";
            CheckIfPlayerWon();
        }
        //Event Handler when user clicks a square
        //updates the border style of the selected square
        //removes styling if the square moved
        private void Square_Click(object sender, EventArgs e)
        {
            if (lastClickedSquare != null)
            {
                lastClickedSquare.BorderStyle = BorderStyle.Fixed3D; // Or any default style you've been using
            }

            // Set lastClickedSquare to the new square and update its BorderStyle
            lastClickedSquare = (Square)sender;
            lastClickedSquare.BorderStyle = BorderStyle.FixedSingle;
            
        }
        // Method returns the structures image
        public Image GetStructureImage(string structure)
        {
            switch (structure)
            {
                case "Wall":
                    return Properties.Resources.wall;
                case "RedDoor":
                    return Properties.Resources.redDoor_icon;
                case "GreenDoor":
                    return Properties.Resources.greenDoor_icon;
                case "RedBox":
                    return Properties.Resources.redBox_icon;
                case "GreenBox":
                    return Properties.Resources.greenBox_icon;
                default:
                    return Properties.Resources.None_icon;
            }
        }
        // Checks if user selected a square and either returns the value of the last selected square or returns null
        private Square VerifySquare(Square lastClickedSquare)
        {
            if (lastClickedSquare != null && (lastClickedSquare.GetStructure() == "RedBox" || lastClickedSquare.GetStructure() == "GreenBox"))
            {
                return lastClickedSquare;
            }
            return null; // Return null if the square is not valid for movement
        }

        private void UpdateSquare(Square square, string structure, Image image)
        {
            square.SetStructure(structure);
            square.Image = image;
        }

        //returns the square object based on the x y coordinates
        private Square GetSquareAtPosition(int row, int col)
        {
            // Find the square at the specified position
            foreach (Control control in panelMap.Controls)
            {
                //check that it's a square and match the row and col of the selected square
                if (control is Square square && square.GetRow() == row && square.GetCol() == col)
                {
                    return square;
                }
            }
            return null;
        }

        // Moves the selected square in the direction of the users choice until something other than a none tile
        //gets in the way
        //if the selected tile runs in the it's same colour door, the square will be deleted
        //then once square is moved, it checks if the player won
        private void MoveSquare(Direction direction, Square square)
        {
            //get all the characteristics of the old square
            int oldRow = square.GetRow();
            int oldCol = square.GetCol();
            Image oldImage = square.Image;
            string oldStructure = square.GetStructure();
            int newRow = oldRow;
            int newCol = oldCol;
            //this will help make the square only move till it gets blocked
            bool canMove = true;
            while (canMove)
            {
                switch (direction)
                {
                    case Direction.Up: newRow--; break;
                    case Direction.Down: newRow++; break;
                    case Direction.Left: newCol--; break;
                    case Direction.Right: newCol++; break;
                }

                // Check if the new position is valid
                if (newCol >= 0 && newCol < mapGrid.GetLength(1) &&
                    newRow >= 0 && newRow < mapGrid.GetLength(0) &&
                    mapGrid[newRow, newCol] == "None")

                {
                    // Continue moving
                }
                else
                {
                    canMove = false;
                    //ran into the same colour door, keep the extended version
                    if ((mapGrid[newRow, newCol] == "GreenDoor" && mapGrid[oldRow,oldCol] == "GreenBox")
                        ||
                        (mapGrid[newRow, newCol] == "RedDoor" && mapGrid[oldRow, oldCol] == "RedBox"))
                    {
                        //keep extended position
                        mapGrid[oldRow, oldCol] ="None"; // Delete the box
                        UpdateSquare(square, "None", Properties.Resources.None_icon);
                        lastClickedSquare = null;
                        canMove = false;
                    }
                    // Ran into an obsticle so adjust the extended position by 1
                    else 
                    {
                        switch (direction)
                        {
                            case Direction.Up: newRow++; break;
                            case Direction.Down: newRow--; break;
                            case Direction.Left: newCol++; break;
                            case Direction.Right: newCol--; break;
                        }
                    }
                    
                    
                }
            }
            //this means user got rid of a box
            if(mapGrid[oldRow, oldCol] == "None")
            {
                square.BorderStyle = BorderStyle.Fixed3D;
                AddMove();
            }
            //this means user ran into something and actually moves squares
            else if ((oldRow != newRow || oldCol != newCol)&& mapGrid[oldRow, oldCol] != "None")
            {
                
                Square newSquare = GetSquareAtPosition(newRow, newCol);
                if (newSquare != null)
                {
                    // Update the map grid
                    mapGrid[oldRow, oldCol] = "None";
                    mapGrid[newRow, newCol] = oldStructure;
                    square.BorderStyle = BorderStyle.Fixed3D; // Remove outline from old position

                    newSquare.BorderStyle = BorderStyle.FixedSingle;

                    // Update the UI
                    UpdateSquare(square, "None", Properties.Resources.None_icon); // Clear the old position
                    UpdateSquare(newSquare, oldStructure, oldImage); // Set the new position

                    // Update lastClickedSquare to the new square
                    lastClickedSquare = newSquare;
                    AddMove();
                }
            }

            CheckIfPlayerWon();

        }

        //checks if there is any boxes remaining, if there isn't tell the user they won and clear the 
        //panel on their okay
        private void CheckIfPlayerWon()
        {
            int numberOfBoxes = 0;
            foreach (Square square in panelMap.Controls)
            {
                if (square.GetStructure() == "RedBox" || square.GetStructure() == "GreenBox")
                {
                    numberOfBoxes++;
                    
                }
            }
            txtNumberOfBoxes.Text = numberOfBoxes.ToString();
            if (numberOfBoxes == 0)
            {
                MessageBox.Show("Congratulations you finished the game with "+ txtNumberOfMoves.Text +" moves!\nGame is over. QGame will reset map once you click okay.", "QGame");
                panelMap.Controls.Clear();
                txtNumberOfBoxes.Text = "";
                txtNumberOfMoves.Text = "";
            }
        }

        //adds a move to the move counter
        private void AddMove()
        {
            int moves = int.Parse(txtNumberOfMoves.Text);
            moves++;
            txtNumberOfMoves.Text = moves.ToString();
        }


        //controller that moves a square right if a square was selected
        private void btnRight_Click(object sender, EventArgs e)
        {
            Square square = VerifySquare(lastClickedSquare);
            if(square != null)
            {
                MoveSquare(Direction.Right, lastClickedSquare);
            }
            else
            {
                MessageBox.Show("No Square was selected.\nPlease click a square you would like to move","QGame");
            }
            

        }

        //controller that moves a square down if a square was selected
        private void btnDown_Click(object sender, EventArgs e)
        {
            Square square = VerifySquare(lastClickedSquare);
            if(square != null)
            {
               MoveSquare(Direction.Down, lastClickedSquare);
            }
            else
            {
                MessageBox.Show("No Square was selected.\nPlease click a square you would like to move", "QGame");
            }

        }

        //controller that moves a square up if a square was selected
        private void btnUP_Click(object sender, EventArgs e)
        {
            Square square = VerifySquare(lastClickedSquare);
            if (square != null)
            {
                MoveSquare(Direction.Up, lastClickedSquare);
            }
            else
            {
                MessageBox.Show("No Square was selected.\nPlease click a square you would like to move", "QGame");
            }

        }

        //controller that moves a square left if a square was selected
        private void btnLeft_Click(object sender, EventArgs e)
        {
            Square square = VerifySquare(lastClickedSquare);
            if (square != null)
            {
                MoveSquare(Direction.Left, square);
            }
            else
            {
                MessageBox.Show("No Square was selected.\nPlease click a square you would like to move", "QGame");
            }

        }
        // Method closes the form, but asks if the user is sure if they already have a game open.
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string moves = txtNumberOfMoves.Text;
            if (moves != "") {
                DialogResult result = MessageBox.Show("Are you sure you would like to exit the game without completing?", "QGame", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                //only close if they want to
                if (result == DialogResult.Yes)
                {
                    
                    this.FindForm().Close();
                }
            }
            else
            {
                this.FindForm().Close();
            }
            
        }
    }
}
