using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;


namespace PathFinding
{
    public partial class Form1 : Form
    {
        #region Callback's
        delegate void SetCallback(Cell cl1, Cell cl2, float penWidth);
        delegate void SetCallbackList(Cell cl, Color c);
        delegate void SetCallbackButtonCondition(Button btn, bool condition);
        delegate void SetCallbackText(Status status);
        delegate void SetCallbackData(List<Cell> path);
        #endregion
        //Форма с установками параметров новой карты :
        private NewMapSettings newMapSettingsForm = new NewMapSettings();

        private Help helpForm = new Help();
        // Тип используемого алгоритма:
        AlgType algorithm = 0;
        // Переменная используется для стирания резульtатов поиска 
        // если снова нажимается Run без перерисовки карты(создания новой или стирания карты) :
        private bool isOnceRunned;
        
        // Массив (6 элементов для каждого типа территории) структуры, содержащей две булевские переменные.
        // Используется вслучае изменения стоимости прохода по территории, если
        // территория была нарисована до этого изменения. 
        private GrInfo[] grInfo;
        
        private byte labelColorIndex;
        // Для start и finish используются координаты в клетках, а не в пикселях!
        // Т.е. фактически это индексы элемента массива.
        private Cell start, finish;
        private  Map[,] map;
        private Cell nullCell;
        private Status pathFindingStatus;
        
        // Координаты старта и финиша в пикселях
        private Point startCoord, finishCoord;
        // Размеры карты в клетках!
        private  int MapWidth { get; set; }
        private  int MapHeight{ get; set; }
        private  int CellSize { get; set; }
        private PathFinder pathFinder;
         
        public Form1()
        {
            MapWidth = 50;
            MapHeight = 43;
            CellSize = 13;
            grInfo = new GrInfo[6];
            
            nullCell = new Cell(0, 0); 
            InitializeComponent();
            openFileDialog1.InitialDirectory = saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            
            label16.BackColor = label1.BackColor;
            AddOwnedForm(newMapSettingsForm);
            AddOwnedForm(helpForm);
            pictureBox1.Width = MapWidth * (CellSize + 1) + 1;
            pictureBox1.Height = MapHeight * (CellSize + 1) + 1;
            newMapSettingsForm.numericUpDown1.Value = MapWidth;
            newMapSettingsForm.numericUpDown2.Value = MapHeight;
            newMapSettingsForm.numericUpDown3.Value = CellSize; 
            drawClearMap();
            MapInit();
            setStatus(Status.Stopped);
            //Status_lbl.DataBindings.Add("Text", this, "(int)pathFindingStatus");
            
            SettingsDGR();
         }

        #region Functions
        private void SettingsDGR()
        {
            Results_dgv.TopLeftHeaderCell.Value = "№";
            Results_dgv.AutoSize = true;
            Results_dgv.ColumnCount = 2;
            
        }
        private void setStatus(Status status)
        {
            pathFindingStatus = status;
            if (Status_lbl.InvokeRequired)
            {
                SetCallbackText d = new SetCallbackText(setStatus);
                this.Invoke(d, new object[] { status });
            }
            else
                Status_lbl.Text = status.ToString();
        }
        private void setData(List<Cell> data)
        {
            if (Results_dgv.InvokeRequired)
            {
                SetCallbackData d = new SetCallbackData(setData);
                this.Invoke(d, new object[] { data });
            }
            else
            {
                //Results_dgv.DataSource = data;
                Results_dgv.RowCount = data.Count;

                int i = 0;
                foreach (Cell c in data)
                {
                    Results_dgv[0, i].Value = c.xIndex;
                    Results_dgv[1, i].Value = c.yIndex;
                    i++;

                }
            }
        }
        
        /// <summary>
        /// Рисует пустую сеточную карту
        /// </summary>
        private void drawClearMap()
        {
            // Размер карты в пискелях.
            int mapWidthPxls = MapWidth * (CellSize + 1) + 1,
                mapHeightPxls = MapHeight * (CellSize + 1) + 1;
            Bitmap mapImg = new Bitmap(mapWidthPxls, mapHeightPxls);
            Graphics g = Graphics.FromImage(mapImg);
            
            // Заливаем весь битмап:
            g.Clear(Color.White);
            
            // Рисуем сетку:
            for (int x = 0; x <= MapWidth; x++)
                 g.DrawLine(Pens.LightGray, x * (CellSize + 1), 0, x * (CellSize + 1), mapHeightPxls);
            for (int y = 0; y <= MapHeight; y++)
                 g.DrawLine(Pens.LightGray, 0, y * (CellSize + 1), mapWidthPxls, y * (CellSize + 1));
            PictureBox p = pictureBox1;
            if (p.Image != null)
                p.Image.Dispose();

            pictureBox1.Image = mapImg;
            g.Dispose();
        }

        private void MapInit()
        {
            // Иниацилизируем массив  пустой карты:
            try
            {
                map = new Map[MapWidth + 2, MapHeight + 2];
            }
            catch (OutOfMemoryException exc)
            {
                MessageBox.Show("Not enough memmory to create such huge map! Try enter smaller map size values.", exc.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                MapWidth = 50;
                MapHeight = 43;
                CellSize = 13;
                map = new Map[MapWidth + 2, MapHeight + 2];
            }

            for (int i = 1; i <= MapWidth; i++)
                for (int j = 1; j <= MapHeight; j++)
                {
                    map[i, j].StepCoast = 1;
                    map[i, j].type = tType.Road;
                    map[i, j].cell = new Cell(i, j);
                }
            // Края карты обрамляем стенами:
            for (int i = 0; i <= MapWidth + 1; i++)
            {
                map[i, 0].StepCoast = -1;
                map[i, 0].type = tType.Wall;
                map[i, 0].cell = new Cell(i, 0);
                map[i, MapHeight + 1].StepCoast = -1;
                map[i, MapHeight + 1].type = tType.Wall;
                map[i, MapHeight + 1].cell = new Cell(i, MapHeight + 1);
            }
            for (int i = 0; i <= MapHeight + 1; i++)
            {
                map[0, i].StepCoast = -1;
                map[0, i].type = tType.Wall;
                map[0, i].cell = new Cell(0, i);
                map[MapWidth + 1, i].StepCoast = -1;
                map[MapWidth + 1, i].type = tType.Wall;
                map[MapWidth + 1, i].cell = new Cell(MapWidth + 1, i);
            }
            start = nullCell;
            finish = nullCell;
            grInfo = new GrInfo[6];
        }
        // Рисует созданную в массиве карту (используется при загрузке карты из файла,
        // а также при перерисовке карты в случае повторного запуска при выборе другого алгоритма
        // или после очистки результатов расчётов)
        private void reDrawMap()
        {
            drawClearMap();
            Point p = new Point();
            Color c = new Color();
            for (int i = 1; i <= MapWidth; i++)
                for (int j = 1; j <= MapHeight; j++)
                {
                    p.X = (i - 1) * (CellSize + 1) + 2;
                    p.Y = (j - 1) * (CellSize + 1) + 2;
                    int index = (int)map[i, j].type + 1;
                    c = groupBox3.Controls["label"+ index].BackColor;
                    FillRegion(c, p);
                }
            drawStart(startCoord);
            drawFinish(finishCoord);
        }
        // Рисуем стартовую и финишную точки:
        // Параметр передается в пикселях!
        private void drawStart(Point point)
        {
            if (start == nullCell)
                return;
            FillRegion(Color.White,  point);
            point.X = point.X - point.X % (CellSize + 1);
            point.Y = point.Y - point.Y % (CellSize + 1)-1;
            Rectangle rect = new Rectangle(point.X + 1, point.Y + 2, CellSize -1, CellSize - 1);
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.FillEllipse(Brushes.LimeGreen, rect);
            g.Dispose();

        }

        // Параметр передается в пикселях!
        private void drawFinish(Point point)
        {
            if (finish == nullCell)
                return;
            FillRegion(Color.White,  point);
            point.X = point.X - point.X % (CellSize + 1);
            point.Y = point.Y - point.Y % (CellSize + 1) - 1;
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            Rectangle rect = new Rectangle(point.X + 1, point.Y + 2, CellSize - 1, CellSize - 1);
            g.FillEllipse(Brushes.Red, rect);
            g.Dispose();
        }
        // Заливает клетку, на которую кликнули.
        private void FillRegion(Color color, Point point)
        {
            SolidBrush brush = new SolidBrush(color);
            //Проверяем цвет кликнутого пикселя:
            Color c = (pictureBox1.Image as Bitmap).GetPixel(point.X, point.Y);
            Graphics g = Graphics.FromImage(pictureBox1.Image);
            
            //Если цвет области совпадает с заливочным, выходим:
            //if (c.ToArgb() == label16.BackColor.ToArgb())
            //{
            //    Console.WriteLine(" sovpadenie cvetov!!! ");
            //    return;
            //}
            
            point.X = point.X - point.X % (CellSize + 1) + 1;
            point.Y = point.Y - point.Y % (CellSize + 1) + 1;
            g.FillRectangle(brush, point.X, point.Y, CellSize, CellSize);
            brush.Dispose();
            g.Dispose();
        }
        // Возвращает индекс элемента массива, который определяет точка на карте:
        private Cell GetIndexByPozition(Point point)
        {
            Cell index = new Cell(0, 0);
            index.xIndex = (point.X / (CellSize + 1)) + 1;
            index.yIndex = (point.Y / (CellSize + 1)) + 1; 
            return index;
        }
        // Возвращает стоимость прохода по клетке:
        private int getCurrentCoast()
        {
            if (label16.BackColor == label1.BackColor) return -1;
            if (label16.BackColor == label2.BackColor) return (int)numericUpDown2.Value;
            if (label16.BackColor == label3.BackColor) return (int)numericUpDown3.Value;
            if (label16.BackColor == label4.BackColor) return (int)numericUpDown4.Value;
            if (label16.BackColor == label5.BackColor) return (int)numericUpDown5.Value;
            if (label16.BackColor == label6.BackColor) return (int)numericUpDown6.Value;
            return 1;
        }
        // Используется вспомогательным потоком вычислений для отображения результатов:
        private void drawDir(Cell cl1, Cell cl2, float penWidth)
        {
           if ( (Math.Abs(cl1.xIndex - cl2.xIndex ) > 1) || (Math.Abs(cl1.yIndex - cl2.yIndex) > 1) )
             return;
            if (pictureBox1.InvokeRequired)
            {
                SetCallback d = new SetCallback(drawDir);
                this.Invoke(d, new object[] { cl1, cl2, penWidth });
            }
            else
            {
                Graphics g = Graphics.FromImage(pictureBox1.Image);
                Pen pen = new Pen(Color.Red,penWidth);
                Point p1 = new Point();
                Point p2 = new Point();
                //pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                p1.X = (CellSize + 1) * (cl1.xIndex - 1) + (int)((CellSize + 1) / 2);
                p1.Y = (CellSize + 1) * (cl1.yIndex - 1) + (int)((CellSize + 1) / 2);
                
                p2.X = (CellSize + 1) * (cl2.xIndex - 1) + (int)((CellSize + 1) / 2);
                p2.Y = (CellSize + 1) * (cl2.yIndex - 1) + (int)((CellSize + 1) / 2);
                g.DrawLine(pen, p1, p2);
                pen.Dispose();
                g.Dispose();
                pictureBox1.Invalidate();
            }
         }
        // Обозначает рамками на карте точки в open и close списках :
        private void drawRect(Cell cl ,Color c)
        {
            if (this.pictureBox1.InvokeRequired)
            {
                SetCallbackList d = new SetCallbackList(drawRect);
                this.Invoke(d, new object[] {cl,c}); 
            }
            else
            {
                Graphics g = Graphics.FromImage(pictureBox1.Image);
                Pen pen = new Pen(c, 1);
                Point p = new Point();
                p.X = (CellSize + 1) * (cl.xIndex - 1) + 1;
                p.Y = (CellSize + 1) * (cl.yIndex - 1) + 1;
                Rectangle rect = new Rectangle(p.X,p.Y,CellSize -1, CellSize -1);
                g.DrawRectangle(pen, rect);
                pen.Dispose();
                g.Dispose();
            }

        }
        // Итак, если изменения стоимости прохода были произведены после отрисовки соответствующих
        // типов территории, перед запуском поиска и перед сохранением карты необходимо внести 
        // соответствующие изменения в массив карты.(прописать новую стоимость)
        private void SetNewCoasts()
        {
            // Если после рисования препятствий стоимость не менялась, выходим:
           if ( (!grInfo[1].terrCoastWasChanged ) && (!grInfo[2].terrCoastWasChanged ) 
               && (!grInfo[3].terrCoastWasChanged ) && (!grInfo[4].terrCoastWasChanged )
               && (!grInfo[5].terrCoastWasChanged ) )
               return;
           NumericUpDown temp;  
           for (int i = 1; i <= MapWidth; i++)
                for (int j = 1; j <= MapHeight; j++)
                {
                    byte typeIndex = (byte)map[i,j].type;
                    if (grInfo[ typeIndex ].terrCoastWasChanged)
                    {
                        temp = groupBox3.Controls["numericUpDown" + (typeIndex + 1)] as NumericUpDown;
                        map[i, j].StepCoast = (int)temp.Value;

                        //switch (typeIndex)
                        //{
                        //    case 1:
                        //        map[i, j].StepCoast = (int)numericUpDown2.Value;
                        //        break;
                        //    case 2:
                        //        map[i, j].StepCoast = (int)numericUpDown3.Value;
                        //        break;
                        //    case 3:
                        //        map[i, j].StepCoast = (int)numericUpDown4.Value;
                        //        break;
                        //    case 4:
                        //        map[i, j].StepCoast = (int)numericUpDown5.Value;
                        //        break;
                        //    case 5:
                        //        map[i, j].StepCoast = (int)numericUpDown6.Value;
                        //        break;
                        //}
                    }
                        
                }
        }

        #endregion

        #region Handlers

        #region NotInteresting
       
        //Clear Map button :
        private void button5_Click(object sender, EventArgs e)  
        {
            drawClearMap();
            MapInit();
        }

        private void label1_Click(object sender, EventArgs e) // Wall type of tile, Color Navy.
        {
            int i = (sender as Label).TabIndex + 1;
            label16.BackColor = groupBox3.Controls["label" + i].BackColor;
            label16.Text = "";
            labelColorIndex = (byte)(i - 1);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            label16.BackColor = Color.White;
            label16.ForeColor = Color.Black;
            label16.Font = new Font("Verdana", 8, FontStyle.Bold);
            label16.Text = "S";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            label16.BackColor = Color.Black;
            label16.ForeColor = Color.White;
            label16.Font = new Font("Verdana", 8, FontStyle.Bold);
            label16.Text = "F";
        }
        // Eraze results button:
        private void button6_Click(object sender, EventArgs e)
        {
            reDrawMap();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (!rb.Checked)
                return;
            algorithm = (AlgType)rb.TabIndex;
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown nud = sender as NumericUpDown;
            int index = nud.TabIndex - 15;
            if (grInfo[index].terrTypeWasUsed)
                grInfo[index].terrCoastWasChanged = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(" Sure ?", " Exit programm ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
               // Передаём пасфайндеру команду на выход из метода:
                if (pathFinder != null)
                {
                    pathFinder.StopTheSearch = true;
                    pathFinder_SearchFinished(pathFinder, new SearchHandlerArgs());
                    // В ответ ожидаем завершения рабочего потока:
                    Application.DoEvents();
                    int n = 0;
                    while ((pathFinder.PFThreadIsAlive == true) && (n < 10))
                    {
                        n++;
                        Thread.Sleep(300);
                    }
                }
            }
            else
                e.Cancel = true;
        }

        private void ViewResults_btn_Click(object sender, EventArgs e)
        {
            Results_pnl.Visible = !Results_pnl.Visible;
            if (Results_pnl.Visible)
                (sender as Button).Text = "Close results table";
            else
                (sender as Button).Text = "View results ";
        }

        #endregion

        #region MouseHandles
        Point startScrPoint;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Cursor = Cursors.Hand;
                startScrPoint = e.Location;
            }

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Cell p = GetIndexByPozition(e.Location);
                switch (label16.Text) 
                {
                    case "S" :
                        if (start != nullCell)
                            FillRegion(label4.BackColor, startCoord);
                        
                        startCoord = e.Location;
                        start = p;
                        drawStart(startCoord);
                        if (start == finish)
                        {
                            finish = nullCell;
                        }
                        break;
                    case "F" :
                        if (finish != nullCell)
                           FillRegion(label4.BackColor, finishCoord);
                        finishCoord = e.Location;
                        finish = p;
                        drawFinish(finishCoord);
                        if (start == finish)
                        {
                            start = nullCell;
                        }
                        break;
                    case "" :
                        FillRegion(label16.BackColor,e.Location);
                        map[p.xIndex, p.yIndex].type = (tType)labelColorIndex;
                        grInfo[labelColorIndex].terrTypeWasUsed = true;
                        break;
                }
                map[p.xIndex , p.yIndex].StepCoast = getCurrentCoast();
                
                pictureBox1.Invalidate();
            }
            
        }

        bool stopper = true;
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if ((label16.Text == "S") || (label16.Text == "F")) return;
                FillRegion(label16.BackColor, e.Location);
                pictureBox1.Invalidate();

                Cell p = GetIndexByPozition(e.Location);
                map[p.xIndex, p.yIndex].StepCoast = getCurrentCoast();
                map[p.xIndex, p.yIndex].type = (tType)labelColorIndex;
                grInfo[labelColorIndex].terrTypeWasUsed = true;
            }           
            // Навигация по карте с помощью мыши:
            if (e.Button == MouseButtons.Right) 
            {
                if (stopper)
                {
                    Point p = panel1.AutoScrollPosition;
                    
                    p.X = -p.X;
                    p.Y = -p.Y;
                    //p.X += e.Location.X - startScrPoint.X;
                    //p.Y += e.Location.Y - startScrPoint.Y;

                    p.X += -e.Location.X + startScrPoint.X;
                    p.Y += -e.Location.Y + startScrPoint.Y;
                              
                    panel1.AutoScrollPosition = p;
                    startScrPoint = e.Location;
                    stopper = false;
                 }
                 else stopper=true;
                startScrPoint = e.Location;
             }
            
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
        }
#endregion

        #region RunStopPause buttons Handlers
        private void Stop_btn_Click(object sender, EventArgs e)
        {
            
            Run_btn.Enabled = true;
            Pause_btn.Enabled = false;
            Stop_btn.Enabled = false;
            setStatus(Status.Stopped);

            if (pathFinder != null)
                pathFinder.StopTheSearch = true;
        }

        private void Pause_btn_Click(object sender, EventArgs e)
        {
            Pause_btn.Enabled = !Pause_btn.Enabled;
            Run_btn.Enabled = true;
            setStatus(Status.Paused);
            if (pathFinder != null)
               pathFinder.PauseTheSearch = !pathFinder.PauseTheSearch;
        }
        
        // Кнопка RUN
        private void button9_Click(object sender, EventArgs e)
        {
            if (start == nullCell || finish == nullCell )
            {
                MessageBox.Show("You must define START and FINISH points both!","Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Run_btn.Enabled = false;
            Stop_btn.Enabled = true;
            Pause_btn.Enabled = true;
            setData(new List<Cell> { nullCell });
            if (pathFindingStatus == Status.Paused)
            {
                pathFinder.PauseTheSearch = false;
                setStatus(Status.Running);
                return;
            }
            setStatus(Status.Running);
            // Проверяем, не обновлялись ли стоимости прохода после прорисовки карты :
            SetNewCoasts();
            // оОбнуляем массив изменений стоимости прохода :
            for (int i = 0; i < grInfo.Length; i++)
                grInfo[i].terrCoastWasChanged = false;
            // Если поиск не был ниразу запущен, карту НЕ перерисовываем:
            if (!isOnceRunned)
                isOnceRunned = true;
            else
                reDrawMap();
            
            int waitTimeOut = 0;
            if (checkBox1.Checked)  waitTimeOut = (int)numericUpDown1.Value ;

            pathFinder = new PathFinder(start, finish, map, algorithm, waitTimeOut);

            pathFinder.PointCheked += new PointHandler(pathFinder_PointCheked);
            pathFinder.PopBestPointFromOpenList += new ListHandler(pathFinder_PopBestPointFromOpenList);
            pathFinder.PointAddedInOpenList += new ListHandler(pathFinder_PointAddedInOpenList);
            pathFinder.PathPoint += new PointHandler(pathFinder_PathPoint);
            // Обработчик окончания поиска пути:
            pathFinder.SearchFinished += new SearchResultHandler(pathFinder_SearchFinished);
      
        }
/// <summary>
/// Из дочернего потока устанавливает свойство enabled кнопок .
/// </summary>
/// <param name="btn"></param>
/// <param name="condition"></param>
        void SetButtonCondition(Button btn, bool condition)
        {
            if (btn.InvokeRequired)
            {
                SetCallbackButtonCondition d = new SetCallbackButtonCondition(SetButtonCondition);
                this.Invoke(d, new object[] { btn, condition });
            }
            else
                btn.Enabled = condition;
        }
        #endregion

        #region PathFinding Handlers
        void pathFinder_SearchFinished(object sender, SearchHandlerArgs args)
        {
            if (!Run_btn.Enabled) SetButtonCondition(Run_btn, true);

            if (args.IsFinded)
            {
                SetButtonCondition(Pause_btn, false);
                SetButtonCondition(Stop_btn, false);
                setData(args.Path);
            }
            else
                setData(new List<Cell> { nullCell });
            
            Console.WriteLine(args.message);
            setStatus(Status.Stopped);
        }

        void pathFinder_PathPoint(object sender, PointEventArgs args)
        {
            drawDir(args.parent, args.succesor, 2);
        }

        void pathFinder_PointAddedInCloseList(object sender, ListEventArgs args)
        {
            drawRect(args.parent, Color.Black);
        }

        void pathFinder_PointAddedInOpenList(object sender, ListEventArgs args)
        {
            drawRect(args.parent, Color.LightGreen);
        }
           
        void pathFinder_PopBestPointFromOpenList(object sender, ListEventArgs args)
        {
            drawRect(args.parent, Color.Black);
        }
        // При обработке точки отрисовывает стреклу, указывающкю направление от род. точки к обрабатываемой:
        void pathFinder_PointCheked(object sender, PointEventArgs args)
        {
            drawDir(args.parent, args.succesor, 1);
        }
        #endregion

        #region MenuStrip

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newMapSettingsForm.ActiveControl = newMapSettingsForm.numericUpDown1;
            if (newMapSettingsForm.ShowDialog() == DialogResult.OK)
            {
                MapWidth = (int)newMapSettingsForm.numericUpDown1.Value;
                MapHeight = (int)newMapSettingsForm.numericUpDown2.Value;
                CellSize = (int)newMapSettingsForm.numericUpDown3.Value;
                drawClearMap();
                MapInit();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string s = saveFileDialog1.FileName;
                // В случае изменения стоимости прохода, обновляем поле StepCoast массива карты:
                SetNewCoasts();
                BinaryWriter mapOut;
                try
                {
                    mapOut = new  BinaryWriter(new FileStream(s, FileMode.Create));
                }
                catch (IOException exc)
                {
                    MessageBox.Show(exc.Message, "Cannot Open File For Output.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    mapOut.Write(MapWidth );
                    mapOut.Write(MapHeight );
                    // Далее записываем текущие значения стоимостей прохода по клеткам :
                    for (int i = 2; i < 7; i++)
                    {
                        //int index = 2 + i;
                        NumericUpDown temp = groupBox3.Controls["numericUpDown" + i] as NumericUpDown;
                        mapOut.Write((int)temp.Value );
                    }

                    mapOut.Write((string)start);
                    mapOut.Write((string)finish);
                    
                    for (int i = 0; i <= MapWidth + 1; i++)
                        for (int j = 0; j <= MapHeight + 1; j++)
                            mapOut.Write((string)map[i, j]);
                       
                }
                catch (IOException exc)
                {
                    MessageBox.Show(exc.Message, "Error writing file.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                mapOut.Close();

            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string s = openFileDialog1.FileName;
                BinaryReader mapIn;

                try
                {
                    mapIn = new BinaryReader(new FileStream(s, FileMode.Open));
                }
                catch (IOException exc)
                {
                    MessageBox.Show(exc.Message, "Cannot Open File For Input.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                   MapWidth = mapIn.ReadInt32();
                   MapHeight = mapIn.ReadInt32();
                   drawClearMap();
                   // Извлекаем стоимости прохода :
                   for (int i = 2; i < 7; i++)
                   {
                       NumericUpDown temp = groupBox3.Controls["numericUpDown" + i] as NumericUpDown;
                       temp.Value = mapIn.ReadInt32();
                   }
                    
                   // Извлекаем идексы точек старта и финиша: 
                   start = (Cell) mapIn.ReadString();
                   finish =(Cell) mapIn.ReadString();
                  
                   for (int i = 0; i <= MapWidth + 1; i++)
                       for (int j = 0; j <= MapHeight + 1; j++)
                            map[i,j] = (Map)mapIn.ReadString();
                      
                   // переводим индексы массива карты в пикссели :
                   if (start != nullCell)
                   {
                       startCoord.X = (start.xIndex - 1) * (CellSize + 1) + (int)(CellSize / 2);
                       startCoord.Y = (start.yIndex - 1) * (CellSize + 1) + (int)(CellSize / 2);
                       drawStart(startCoord);
                   }
                   if (finish != nullCell)
                   {
                       finishCoord.X = (finish.xIndex - 1) * (CellSize + 1) + (int)(CellSize / 2);
                       finishCoord.Y = (finish.yIndex - 1) * (CellSize + 1) + (int)(CellSize / 2);
                       drawFinish(finishCoord);
                   }
                   reDrawMap();
                }
                catch (IOException exc)
                {
                    MessageBox.Show(exc.Message, "Error Reading File.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                mapIn.Close();
                openFileDialog1.FileName = "";

            }
        }
        
        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            helpForm.Visible = true;
        }
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            drawClearMap();
            MapInit();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #endregion
   }
}
