using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PathFinding
{
    public partial class Help : Form
    {
        string text;
        public Help()
        {
            InitializeComponent();
            label1.Text =                       "Всем привет!";
            text = "             Программа реализует три популярных алгоритма поиска пути. \n" + 
                   "Впрочем, следует отметить, что А-звёздочка является объединением алгоритмов \n" +
                   "Лучший-Первый и Дийкстры. Реализация приведена для сеточной 2D карты, но, \n " +
                   "конечно, алгоритмы могут быть реализованы в пространстве любой размерности. \n " +
                   "Для детального изучения работы алгоритмов предусмотрена задержка при расчётах \n" +
                   "(TimeOut). Графический редактор позволяет рисовать препятствия с заданной \n" +
                   "стоимостью прохождения. Зелёной рамкой помечается клетка, занесённая в список \n" +
                   "Open. Чёрной рамкой - клетка с наилучшим показателей накопленной стоимости пути,\n" +
                   "извлечённая из списка Open.\n" +
                   "             Созданную карту Вы можете сохранять в файле и загружать из файла. Для\n" +
                   "навигации по карте можно использовать правую кнопку мыши.\n";
            label2.Text = text;

        }

        private void Help_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void Close_btn_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        
    }
}
