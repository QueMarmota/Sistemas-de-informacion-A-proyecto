using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProyectoBasesDeDatosDistribuidas
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void BtnEmpleado_Click(object sender, EventArgs e)
        {
            Empleado emp = new Empleado();
            emp.ShowDialog();
            emp.Dispose();
        }

        private void Index_Load(object sender, EventArgs e)
        {   
            /*Image imagen = new Bitmap(@"..\..\..\logo.jpg");
            this.BackgroundImage = imagen;*/
        
            

        }

        private void BtnSucursal_Click(object sender, EventArgs e)
        {
            Sucursal suc = new Sucursal();
            suc.ShowDialog();
        }

        private void BtnProveedor_Click(object sender, EventArgs e)
        {
            Proveedor prov = new Proveedor();
            prov.ShowDialog();
        }

        private void BtnVenta_Click(object sender, EventArgs e)
        {
            Venta venta = new Venta();
            venta.ShowDialog();
        }

        private void BtnOferta_Click(object sender, EventArgs e)
        {
            Oferta of = new Oferta();
            of.ShowDialog();
        }

        private void BtnProducto_Click(object sender, EventArgs e)
        {
            Producto prod = new Producto();
            prod.ShowDialog();
        }

        private void btn_reporte_Click(object sender, EventArgs e)
        {
            FormReporteEmpleadoFiltro reporte = new FormReporteEmpleadoFiltro();
            reporte.ShowDialog();

        }

        private void btn_ReporteProveedor_Click(object sender, EventArgs e)
        {
            FormReporteProveedor reporte = new FormReporteProveedor();
            reporte.ShowDialog();
        }

        private void btn_Factura_Click(object sender, EventArgs e)
        {
            formReporteFactura factura = new formReporteFactura();
            factura.ShowDialog();
           // factura.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MyInstaller installer = new MyInstaller();
            installer.InstallApplication(@"C:\Users\rafael.DESKTOP-D898I0K\Documents\GitHub\Bases-de-datos-B\ProyectoBasesDeDatosDistribuidas\bin\Release\ProyectoBasesDeDatosDistribuidas.application");
            MessageBox.Show("Installer object created.");  

        }

   

    }
}
