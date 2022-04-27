﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOLGame
{
    public partial class Form1 : Form
    {
        // gridpattern true = tordoial
        // gridpatt false = finite
        bool gridPatt = Properties.Settings.Default.gridPatt;      
        bool gridDraw = Properties.Settings.Default.gridDraw; // Draw the grid Yes/No
        bool hudDraw = Properties.Settings.Default.hudDraw;
        bool neighborTog = Properties.Settings.Default.neighborTog; // Neighbor toggle

        // The universe array
        int uniWidth = Properties.Settings.Default.uniWidth;
        int uniHeight = Properties.Settings.Default.uniHeight;
        bool[,] universe = new bool[Properties.Settings.Default.uniWidth, Properties.Settings.Default.uniHeight];
        bool[,] altverse = new bool[Properties.Settings.Default.uniWidth, Properties.Settings.Default.uniHeight];

        // Drawing colors
        Color gridColor = Properties.Settings.Default.gridColor;
        Color cellColor = Properties.Settings.Default.cellColor;
        Color backColor = Properties.Settings.Default.backColor;

        // The Timer class
        Timer timer = new Timer();
        int timeInterval = Properties.Settings.Default.TimeInterval;

        // global ints
        int numalive = 0;
        int generations = 0; // Generation count
        int seed = Properties.Settings.Default.Seed; // The current seed

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = timeInterval; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running

            StatusStripUpdate(); // Update the status strip to display all info
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            CopyUniverse(); // Takes the current universe and copies it to the altverse for editing
          
            int numneighbors; // var for the number of alive neighbrs a cell has

            // cycle through the universe
            for (int x = 0; x < universe.GetLength(0); x++)
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    // if the universe is toroidal, then count neighbors like so; else, cont neighbors as finite.
                    if (gridPatt == true)                    
                        numneighbors = CountNeighborsToroidal(x, y);                   
                    else                    
                        numneighbors = CountNeighborsFinite(x, y);                   

                    // My condenced version of the 4 rules...
                    // if the cell is alive with 2 or 3 living neibors, then stay alive; else, die
                    if (universe[x, y] == true && (numneighbors == 2 || numneighbors == 3))                    
                        continue;                   
                    else                  
                        altverse[x, y] = false;                  
                    // if the cell is dead and has 3 living neighbors, then become alive.
                    if (universe[x, y] == false && numneighbors == 3)                   
                        altverse[x, y] = true;                  
                }
            }
           
            SetUniverse(); // Sets the universe to the altered altverse for diaplaying.
            generations++; // Increment generation count            
            StatusStripUpdate(); // Update status strip generations            
            graphicsPanel1.Invalidate(); // Refresh the rendered screen
        }

        // Update status strip generations
        private void StatusStripUpdate()
        {
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString() + "  Interval = " + timeInterval.ToString() + "  Alive = " + numalive.ToString() + "  Seed = " + seed.ToString();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void ClearUniverse() // Empty the universe and the altverse
        {
            for (int x = 0; x < universe.GetLength(0); x++)
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    universe[x, y] = false;
                    altverse[x, y] = false;
                    generations = 0;
                }
            }
            graphicsPanel1.Invalidate();
        }
        private void CopyUniverse() // Take what's in the universe and put it in the altverse
        {
            for (int x = 0; x < universe.GetLength(0); x++)
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {                   
                    altverse[x, y] = universe[x,y];
                }
            }
        }
        private void SetUniverse() // Take what's in the altverse and put it in the universe
        {
            for (int x = 0; x < altverse.GetLength(0); x++)
            {
                for (int y = 0; y < altverse.GetLength(1); y++)
                {                    
                    universe[x, y] = altverse[x, y];                   
                }
            }
        }

        private int CountNeighborsFinite(int x, int y) // counts the neighbors as if the universe was finite
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then continue
                    if(xCheck < 0)
                    {
                        continue;
                    }
                    // if yCheck is less than 0 then continue
                    if(yCheck < 0)
                    {
                        continue;
                    }
                    // if xCheck is greater than or equal too xLen then continue
                    if(xCheck >= xLen)
                    {
                        continue;
                    }
                    // if yCheck is greater than or equal too yLen then continue
                    if(yCheck >= yLen)
                    {
                        continue;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }
        private int CountNeighborsToroidal(int x, int y) // counts the neighbors as if the universe was infinite
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (yOffset == 0 && xOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then set to xLen - 1
                    if(xCheck < 0)
                    {
                        xCheck = xLen - 1;
                    }
                    // if yCheck is less than 0 then set to yLen - 1
                    if (yCheck < 0)
                    {
                        yCheck = yLen - 1;
                    }
                    // if xCheck is greater than or equal too xLen then set to 0
                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }
                    // if yCheck is greater than or equal too yLen then set to 0
                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }
        
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e) // Paints the screen
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = (float)graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = (float)graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);
            Pen BigPen = new Pen(gridColor, 3);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);
            Brush backBrush = new SolidBrush(backColor);
            Color hudColor = Color.FromArgb(100, 0, 0, 255);
            Brush hudbrush = new SolidBrush(hudColor);

            // String format for neighbor count
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            // String format for HUD
            StringFormat HudFormat = new StringFormat();
            HudFormat.Alignment = StringAlignment.Near;
            HudFormat.LineAlignment = StringAlignment.Far;           

            // Draw a number in the center of the neighbors cells
            Font font = new Font("Arial", 20f);

            numalive = 0;

            // Iterate through the universe in the y, top to bottom
            for (float y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (float x = 0; x < universe.GetLength(0); x++)
                {
                    if (universe[(int)x, (int)y] == true)
                    {
                        numalive++;
                    }

                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = RectangleF.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;
                    RectangleF rect = new RectangleF(cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);

                    // Fill the cell with a brush if alive
                    if (universe[(int)x, (int)y] == true)                    
                        e.Graphics.FillRectangle(cellBrush, cellRect);                   
                    else                    
                        e.Graphics.FillRectangle(backBrush, cellRect);

                    // change the grid to infinite or finite
                    int neighbors = 0;
                    if (gridPatt == true) // infinite                   
                        neighbors = CountNeighborsToroidal((int)x, (int)y);
                    else                  // finite                   
                        neighbors = CountNeighborsFinite((int)x, (int)y);                   

                    // Draw the number of neighbors in the cell
                    if (neighbors != 0 && neighborTog == true)
                    {
                        if (neighbors == 3)                        
                            e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, rect, stringFormat);  
                        else if (neighbors == 2 && universe[(int)x, (int)y] == true)
                            e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Green, rect, stringFormat);
                        else
                            e.Graphics.DrawString(neighbors.ToString(), font, Brushes.Red, rect, stringFormat);
                    }

                    // Outline the cell with a pen
                    if (gridDraw == true)   
                    {
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);                    
                    }
                }
            }
            Rectangle unirect = new Rectangle(universe.GetLength(0), universe.GetLength(1), graphicsPanel1.ClientSize.Width, graphicsPanel1.ClientSize.Height-25);
            // HUD
            if (hudDraw == true)
            {
                if (gridPatt == true)
                {
                    e.Graphics.DrawString(
                        "Generations: " + generations.ToString() + '\n' +
                        "Cell Count: " + numalive.ToString() + '\n' +
                        "BoundaryType: " + "Tordial" + '\n' +
                        "Universe Size: Width=" + uniWidth + " Height=" + uniHeight, font, hudbrush, unirect, HudFormat);
                }
                else
                {
                    e.Graphics.DrawString(
                        "Generations: " + generations.ToString() + '\n' +
                        "Cell Count: " + numalive.ToString() + '\n' +
                        "BoundaryType: " + "Finite" + '\n' +
                        "Universe Size: Width=" + uniWidth + " Height=" + uniHeight, font, hudbrush, unirect, HudFormat);
                }
            }
             
            StatusStripUpdate();

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }
        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e) // Update the graphics pannel every mouse click
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                float cellWidth = (float)graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                float cellHeight = (float)graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                float x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                float y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[(int)x, (int)y] = !universe[(int)x, (int)y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) // called when you click the exit tool strip menu item
        {
            this.Close();
        }
        private void startToolStripMenuItem_Click(object sender, EventArgs e) // called when you click the start tool strip menu item
        {
            timer.Enabled = true;
            playtoolStripButton.Enabled = false;
            startToolStripMenuItem.Enabled = false;
            pausetoolStripButton.Enabled = true;
            pauseToolStripMenuItem.Enabled = true;
            graphicsPanel1.Invalidate();
        }
        private void pauseToolStripMenuItem_Click(object sender, EventArgs e) // called when you click the pause tool strip menu item
        {
            timer.Enabled = false;
            playtoolStripButton.Enabled = true;
            startToolStripMenuItem.Enabled = true;
            pausetoolStripButton.Enabled = false;
            pauseToolStripMenuItem.Enabled = false;
            graphicsPanel1.Invalidate();
        }
        private void nextToolStripMenuItem_Click(object sender, EventArgs e) // called when you click the next tool strip menu item
        {
            NextGeneration();
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e) // called when you click the new tool strip menu item
        {
            ClearUniverse();
            generations = 0;
            StatusStripUpdate();
            graphicsPanel1.Invalidate();
        }
        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)// called when you click the HUD tool strip menu item
        {
            if (hudDraw == true)
            {
                hUDToolStripMenuItem.Checked = false;
                hudDraw = false;
            }
            else
            {
                hUDToolStripMenuItem.Checked = true;
                hudDraw = true;
            }
            graphicsPanel1.Invalidate();
        }
        private void gridToolStripMenuItem_Click(object sender, EventArgs e) // called when you click the grid tool strip menu item
        {
            if (gridDraw == true)
            {
                gridToolStripMenuItem.Checked = false;
                gridDraw = false;
            }
            else
            {
                gridToolStripMenuItem.Checked = true;
                gridDraw = true;
            }
            graphicsPanel1.Invalidate();
        }
        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e) // called when you click the neighborcount tool strip menu item
        {
            if (neighborTog == true)
            {
                neighborCountToolStripMenuItem.Checked = false;
                neighborTog = false;
            }
            else
            {
                neighborCountToolStripMenuItem.Checked = true;
                neighborTog = true;
            }
            graphicsPanel1.Invalidate();
        }
        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e) // called when you click the torodial tool strip menu item
        {
            gridPatt = true;
            finiteToolStripMenuItem.Checked = false;
            toroidalToolStripMenuItem.Checked = true;
            graphicsPanel1.Invalidate();
        }
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e) // called when you click the finite tool strip menu item
        {
            gridPatt = false;
            toroidalToolStripMenuItem.Checked = false;
            finiteToolStripMenuItem.Checked = true;
            graphicsPanel1.Invalidate();
        }

        private void playtoolStripButton_Click(object sender, EventArgs e) // called when you click the play button
        {
            timer.Enabled = true;
            playtoolStripButton.Enabled = false;
            startToolStripMenuItem.Enabled = false;
            pausetoolStripButton.Enabled = true;
            pauseToolStripMenuItem.Enabled = true;
            graphicsPanel1.Invalidate();
        }
        private void pausetoolStripButton_Click(object sender, EventArgs e) // called when you click the pause button
        {
            timer.Enabled = false;
            playtoolStripButton.Enabled = true;
            startToolStripMenuItem.Enabled = true;
            pausetoolStripButton.Enabled = false;
            pauseToolStripMenuItem.Enabled = false;
            graphicsPanel1.Invalidate();
        }
        private void nexttoolStripButton_Click(object sender, EventArgs e) // called when you click the next button
        {
            NextGeneration();
        }
        private void newToolStripButton_Click(object sender, EventArgs e) // called when you click the new button
        {
            ClearUniverse();
            generations = 0;
            StatusStripUpdate();
            graphicsPanel1.Invalidate();
        }

        public void RandomizeTime() // logic for randomizing with time
        {
            int newseed = (int)DateTime.Now.Ticks;
            seed = newseed;
            Random generator = new Random(seed);
            for (int x = 0; x < universe.GetLength(0); x++)
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    int temp = generator.Next(0, 2);
                    if (temp == 0)
                    {
                        altverse[x, y] = true;
                    }
                    else
                        altverse[x, y] = false;                 
                }
            }
        }
        public void Randomize() // logic for randomizing without changing the seed
        {
            Random generator = new Random(seed);
            for (int x = 0; x < universe.GetLength(0); x++)
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    int temp = generator.Next(0, 2);
                    if (temp == 0)
                    {
                        altverse[x, y] = true;
                    }
                    else
                        altverse[x, y] = false;
                }
            }
        }
        public void Randomize(int newseed) // logic for randomizing with a new seed
        {
            seed = newseed;
            Random generator = new Random(seed);
            for (int x = 0; x < universe.GetLength(0); x++)
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    int temp = generator.Next(0, 2);
                    if (temp == 0)
                    {
                        altverse[x, y] = true;
                    }
                    else
                        altverse[x, y] = false;
                }
            }
        }

        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e) // called when you click the fromtime tool strip menu item
        {
            RandomizeTime();
            SetUniverse();
            generations = 0;
            StatusStripUpdate();
            graphicsPanel1.Invalidate();
        }
        private void fromCurrentSeedToolStripMenuItem_Click(object sender, EventArgs e) // called when you click the fromcurrentseed tool strip menu item
        {
            Randomize();
            SetUniverse();
            generations = 0;
            StatusStripUpdate();
            graphicsPanel1.Invalidate();
        }
        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e) // called when you click the fromseed tool strip menu item
        {
            RandDialog dlg = new RandDialog(); // open the dialog

            dlg.myInteger = seed; // set the properties

            if (DialogResult.OK == dlg.ShowDialog()) // if it exisits, randomize the univese.
            {
                seed = dlg.myInteger;               
                Randomize(seed);
                SetUniverse();
                generations = 0;
            }
            StatusStripUpdate();
            graphicsPanel1.Invalidate();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingDialog setdlg = new SettingDialog();

            setdlg.Interval = timeInterval;
            setdlg.settWidth = uniWidth;
            setdlg.settHeight = uniHeight;

            if (DialogResult.OK == setdlg.ShowDialog())
            {
                timeInterval = setdlg.Interval;
                uniWidth = setdlg.settWidth;
                uniHeight = setdlg.settHeight;

                universe = new bool[uniWidth, uniHeight];
                //SetUniverse();
                altverse = new bool[uniWidth, uniHeight];
                //CopyUniverse();
            }
            StatusStripUpdate();
            graphicsPanel1.Invalidate();
        }

        private void backColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog backcolordlg = new ColorDialog();
            backcolordlg.Color = backColor;
            if (DialogResult.OK == backcolordlg.ShowDialog())
            {
                backColor = backcolordlg.Color;
            }
            graphicsPanel1.Invalidate();
        }
        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog cellcolordlg = new ColorDialog();
            cellcolordlg.Color = cellColor;
            if (DialogResult.OK == cellcolordlg.ShowDialog())
            {
                cellColor = cellcolordlg.Color;
            }
            graphicsPanel1.Invalidate();
        }
        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog gridcolordlg = new ColorDialog();
            gridcolordlg.Color = gridColor;
            if (DialogResult.OK == gridcolordlg.ShowDialog())
            {
                gridColor = gridcolordlg.Color;
            }
            graphicsPanel1.Invalidate();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) // called when the form closes
        {
            Properties.Settings.Default.Save();
        }
    }
}