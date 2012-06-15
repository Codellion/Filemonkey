using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FileMonkey
{
    public partial class FrmPrincipal : Form
    {
        Boolean sonarActivo = true;


        public FrmPrincipal()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnEstadoSonar_Click(object sender, EventArgs e)
        {
            sonarActivo = !sonarActivo;

            if (sonarActivo)
            {
                lblEstadoSonar.ForeColor = Color.Red;
                lblEstadoSonar.Text = language.Default.lblEstadoInactivoES;
            }
            else
            {
                lblEstadoSonar.ForeColor = Color.Green;
                lblEstadoSonar.Text = language.Default.lblEstadoActivoES;
            }            
        }

    }
}
