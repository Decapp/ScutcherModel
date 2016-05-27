using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace App
{
    public partial class TestForm : Form
    {
        int N;
        Random rnd;

        public TestForm()
        {
            InitializeComponent();

            N = 1000;
            rnd = new Random();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < N; i++)
            {
                chart1.Series[0].Points.Add(genNormNumber(55,15));
            }

            for (int i = 0; i < N; i++)
            {
                chart2.Series[0].Points.Add(genNormNumber(6, 6));
            }

            for (int i = 0; i < N; i++)
            {
                chart3.Series[0].Points.Add(genNormNumber(0, Math.PI/4));
            }
        }

        private double genNormNumber(double expectedValue, double variance)
        {
            double number = 0;
                      
            for (int i = 0; i < 12; i++)
            {
                number += rnd.NextDouble();
            }

            number -= 6;
            number = expectedValue + Math.Pow(variance, 0.5) * number;

            return number;
        }
    }
}
