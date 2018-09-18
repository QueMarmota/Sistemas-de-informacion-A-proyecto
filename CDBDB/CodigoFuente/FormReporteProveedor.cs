﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using NpgsqlTypes;

using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ProyectoBasesDeDatosDistribuidas
{
    public partial class FormReporteProveedor : Form
    {
        List<int> listPrimaryKeysProveedor;
        string[] TuplaPDFProveedor = new string[1];
        List<string[]> listaProductosDelProveedor = new List<string[]>();
        List<string> ProveedorEnLista = new List<string>();
        List<string> ProductoEnLista = new List<string>();
        //varaibles para sql
        SqlConnection cnSQL; //conexion
        SqlCommand cmd; //comando
        SqlDataAdapter da; //select
        DataTable dt; //datos a tabla
        SqlDataReader dr;///le lo que devuelve la consulta

        //Variables para postgres
        string parametros = "Server=localhost;Port=5432;User id=postgres;Password=root;Database=Sitio2";
        DataSet datos = new DataSet();
        DataTable dtNPG = new DataTable();
        NpgsqlConnection conNPG = new NpgsqlConnection();
        public FormReporteProveedor()
        {
            InitializeComponent();
         //Conexion a SQL SERVER SITIO1
            try
            {
                cnSQL = new SqlConnection(@"Data Source=DESKTOP-D898I0K\SQLEXPRESS;initial catalog=Sitio1; integrated Security=true");
                cnSQL.Open();
                //MessageBox.Show("abierto");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error con la conexion a la base de datos" + ex.ToString());
            }
            //COONEXION A POSTGERSQL SITIO2

            conNPG.ConnectionString = parametros;
            try
            {
                conNPG.Open();
            }
            catch (Exception error)
            {

                MessageBox.Show("Error en la conexion con NpgSQL" + error.Message);
            }
            cargaTablaProveedor();//carga datagridproveedor
            cargaTablaProductos();//carga datagrdproductos
            //cargar el listview
            cargaListView();
        }
        private void cargaTablaProductos()
        {
            try
            {
                //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
                //variables necesarias para sacar datos del esquema de localizacion
                List<string> idFragmentos = new List<string>();
                List<string> nombreTablaBDFragmento = new List<string>();
                string nombreTablaGeneral = "";
                string tipoFragmento = "";
                List<string> sitios = new List<string>();
                List<string> condicion = new List<string>();
                SitioCentral st = new SitioCentral();
                st.LeeEsquemaLocalizacion("Producto", ref idFragmentos, ref nombreTablaBDFragmento, ref nombreTablaGeneral, ref sitios, ref tipoFragmento, ref condicion);
                switch (tipoFragmento)
                {
                    //si es horizontal tiene las mismas columnas y solo hacemos merge en las rows
                    case "Horizontal":


                        break;
                    case "Vertical":

                        break;
                    case "Replica":
                        MezclaBDReplica(nombreTablaBDFragmento);
                        break;

                    default:
                        mezcaBDNormalDelSitio(sitios, nombreTablaBDFragmento);
                        break;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("No se pudo actualizar" + ex);
            }
        }
        private void mezcaBDNormalDelSitio(List<string> sitios, List<string> nombreTablaBDFragmento)
        {

            switch (sitios.ElementAt(0))
            {
                case "1":
                    //codigo para hacer el select al sitio1
                    da = new SqlDataAdapter("Select * from " + nombreTablaBDFragmento.ElementAt(0) + "", cnSQL);
                    dt = new DataTable();
                    da.Fill(dt);
                    //dt.Columns.RemoveAt(0);//Para quitar el campo RFC
                    dataGridViewProducto.DataSource = dt;
                    //para esconder el campor id
                    dataGridViewProducto.Columns[0].Visible = false;
                    //para esconder el campo id venta
                    dataGridViewProducto.Columns[6].Visible = false;

                    break;


                case "2":
                    //Codigo para hacer el select from a al sitio2             
                    NpgsqlDataAdapter add = new NpgsqlDataAdapter("select * from " + nombreTablaBDFragmento.ElementAt(0) + "", conNPG);
                    dtNPG = new DataTable();
                    add.Fill(dtNPG);
                    dataGridViewProducto.DataSource = dtNPG;
                    //para esconder el campor id
                    dataGridViewProducto.Columns[0].Visible = false;
                    //para esconder el campo id venta
                    dataGridViewProducto.Columns[6].Visible = false;
                    break;
            }

        }       
        public void MezclaBDReplica(List<string> nombreTablaBDFragmento)
        {

            //codigo para hacer el select al sitio1
            da = new SqlDataAdapter("Select * from " + nombreTablaBDFragmento.ElementAt(0) + "", cnSQL);
            dt = new DataTable();
            da.Fill(dt);
            //dt.Columns.RemoveAt(0);//Para quitar el campo RFC
            dataGridViewProducto.DataSource = dt;
            //para esconder el campor RFC
            dataGridViewProducto.Columns[0].Visible = false;
            //para esconder el campo id venta
            dataGridViewProducto.Columns[6].Visible = false;
        }
        private void cargaListView()
        {
            listBoxProveedor.Items.Clear();
            ProveedorEnLista.Clear();
            foreach (DataGridViewRow row in dataGridViewProveedor.Rows)
            {
                if (row.Cells[1].Value != null)
                {
                    listBoxProveedor.Items.Add(row.Cells[1].Value.ToString());
                    string dato = row.Cells[0].Value.ToString() + "," + row.Cells[1].Value.ToString() + "," + row.Cells[2].Value.ToString() + "," + row.Cells[3].Value.ToString() + "," + row.Cells[4].Value.ToString() + "," + row.Cells[5].Value.ToString() + "," + row.Cells[6].Value.ToString() + "";
                    ProveedorEnLista.Add(dato);
                }
                //More code here
            }
        }

        private void cargaTablaProveedor()
        {
            try
            {
                //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
                //variables necesarias para sacar datos del esquema de localizacion
                List<string> idFragmentos = new List<string>();
                List<string> nombreTablaBDFragmento = new List<string>();
                string nombreTablaGeneral = "";
                string tipoFragmento = "";
                List<string> sitios = new List<string>();
                List<string> condicion = new List<string>();
                SitioCentral st = new SitioCentral();
                st.LeeEsquemaLocalizacion("Proveedor", ref idFragmentos, ref nombreTablaBDFragmento, ref nombreTablaGeneral, ref sitios, ref tipoFragmento, ref condicion);

                switch (tipoFragmento)
                {
                    case "Horizontal":
                        MezclaBDHorizontal();
                        break;
                    case "Vertical":
                        MezclaBDVertical();

                        break;
                    case "Replica":

                        break;

                    default:
                        break;

                }


            }
            catch (Exception ex)
            {

                MessageBox.Show("No se pudo actualizar" + ex);
            }
        }
        private void MezclaBDVertical()
        {
            /*CODIGO PARA ITERAR LAS CELDAS DE UN DATATABLE POR RENGLONES
          int numberOfColumns = dt.Columns.Count;

           // go through each row
           foreach (DataRow dr in dt.Rows)
           {
               // go through each column in the row
               for (int i = 0; i < numberOfColumns; i++)
               {
                   // access cell as set or get
                   // dr[i] = "something";
                   // string something = Convert.ToString(dr[i]);
               }
           }
           */
            NpgsqlDataAdapter add;
            //codigo para hacer el select al sitio1
            da = new SqlDataAdapter("Select * from Proveedor1 ORDER BY id_Proveedor DESC", cnSQL);
            dt = new DataTable();
            da.Fill(dt);
            //dt.Columns.RemoveAt(0);//Para quitar el campo RFC
            dataGridViewProveedor.DataSource = dt;


            //Codigo para hacer el select from a al sitio2             
            add = new NpgsqlDataAdapter("select * from Proveedor2 ORDER BY id_Proveedor DESC", conNPG);
            //add.Fill(datos);
            dtNPG = new DataTable();
            add.Fill(dtNPG);
            // dtNPG.Columns.RemoveAt(0);//para quitar el campo RFC

            // dt.Merge(dtNPG);

            //Hacer merge entre los dos select del sito1 y el sitio2
            foreach (var column in dtNPG.Columns)
            {
                if (column.ToString().Contains("id"))//esto con la finalidad de corregir por q el ide copia el daato en las celdas que se llaman igual
                    dt.Columns.Add(column.ToString() + "2");
                else
                    dt.Columns.Add(column.ToString());
            }

            int numberOfColumns = dt.Columns.Count;
            int contadorRows = 0;
            //para esconder el campor id
            // dataGridViewProveedor.Columns[0].Visible = false;
            // dataGridViewProveedor.Columns[4].Visible = false;

            // go through each row
            int contadorPK = 0;
            listPrimaryKeysProveedor = new List<int>();
            foreach (DataRow dr in dt.Rows)
            {
                // go through each column in the row
                for (int i = dtNPG.Columns.Count; i < numberOfColumns - 1; i++)//-1 por q adelante le sumo mas 1
                {
                    // access cell as set or get
                    var dato = dtNPG.Rows[contadorRows][(i - dtNPG.Columns.Count)];
                    if (contadorPK == 0)//se agrega la llave primaria a la lista global para evaluar una autonumerica programada
                    {
                        listPrimaryKeysProveedor.Add(Convert.ToInt32(dato));
                    }
                    contadorPK++;
                    dr[i + 1] = dato;//+| para evitar q copie el idproveedor de la base de datos de npg
                    //string something = Convert.ToString(dr[i+2]);
                }
                contadorRows++;
            }

            //dt tiene los campos del sitio1 y del sitio2           
            dataGridViewProveedor.DataSource = dt;


            //para esconder el campor id
            dataGridViewProveedor.Columns[0].Visible = false;
            dataGridViewProveedor.Columns[4].Visible = false;



        }
        private void MezclaBDHorizontal()
        {
            NpgsqlDataAdapter add;
            //codigo para hacer el select al sitio1
            da = new SqlDataAdapter("Select * from Proveedor1", cnSQL);
            dt = new DataTable();
            da.Fill(dt);
            //dt.Columns.RemoveAt(0);//Para quitar el campo RFC
            dataGridViewProveedor.DataSource = dt;


            //Codigo para hacer el select from a al sitio2             
            add = new NpgsqlDataAdapter("select * from Proveedor2", conNPG);
            //add.Fill(datos);
            dtNPG = new DataTable();
            add.Fill(dtNPG);
            // dtNPG.Columns.RemoveAt(0);//para quitar el campo RFC

            //Hacer merge entre los dos select del sito1 y el sitio2
            for (int i = 0; i < dtNPG.Rows.Count; i++)
            {
                dt.ImportRow(dtNPG.Rows[i]);
            }

            //dt tiene los campos del sitio1 y del sitio2
            dataGridViewProveedor.DataSource = dt;

            //para esconder el campor id
            dataGridViewProveedor.Columns[0].Visible = false;

        }
        private void btn_Genera_Click(object sender, EventArgs e)
        {
            try
            {
                if (TuplaPDFProveedor[0] == null)
                {
                    MessageBox.Show("Proveedor aun no seleccionado"); return;
                }
                int totalProductos = 0;
                Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
                PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream("../../../Reporte-Proveedor.pdf", FileMode.Create));
                doc.Open();
                //codigo para agregar bordes de pagina al pdf
                var content = wri.DirectContent;
                var pageBorderRect = new iTextSharp.text.Rectangle(doc.PageSize);

                pageBorderRect.Left += doc.LeftMargin;
                pageBorderRect.Right -= doc.RightMargin;
                pageBorderRect.Top -= doc.TopMargin;
                pageBorderRect.Bottom += doc.BottomMargin;

                content.SetColorStroke(BaseColor.BLACK);
                content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height + 30);
                content.Stroke();

                //Para agregar imagen
                iTextSharp.text.Image PNG = iTextSharp.text.Image.GetInstance("../../../logo.jpg");
                PNG.ScalePercent(25f);
                //para posicion
                //PNG.SetAbsolutePosition(doc.PageSize.Width - 36f - 40f, doc.PageSize.Height - 36f - 50f);
                PNG.SetAbsolutePosition(171, 700);
                //para agregar un borde a la imagen
                PNG.Border = iTextSharp.text.Rectangle.BOX;
                PNG.BorderColor = iTextSharp.text.BaseColor.BLACK;
                PNG.BorderWidth = 5f;
                doc.Add(PNG);


                //write some content   

                Chunk chunk = new Chunk("" + TuplaPDFProveedor[1] + "", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));
                Phrase ph = new Phrase("\n\r \n\r \n\r Nombre : ");
                Paragraph p = new Paragraph(ph);
                p.Alignment = Element.ALIGN_LEFT;
                // p.SpacingBefore = spaceBefore;
                p.IndentationLeft = 120;
                p.Add((new Chunk(chunk)));//agregamos el nombre con linea
                //se agrega direccion
                ph = new Phrase("\t      Dirección : ");
                p.Add(ph);
                chunk = new Chunk("" + TuplaPDFProveedor[5] + "", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));
                p.Add((new Chunk(chunk)));
                //se agrega fecha nacimiento
                ph = new Phrase("\n\r Teléfono : ");
                p.Add(ph);
                chunk = new Chunk("" + TuplaPDFProveedor[3] + "", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));
                p.Add((new Chunk(chunk)));
                //se agrega direccion
                ph = new Phrase("\t      Tipo : ");
                p.Add(ph);
                chunk = new Chunk("" + TuplaPDFProveedor[6] + "", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));
                p.Add((new Chunk(chunk)));        
                //se agrega espacio
                ph = new Phrase("\n\r \n\r");
                p.Add(ph);

                doc.Add(p);

                PdfPTable table = new PdfPTable(2);//el 3 es el numero de columnas
                //para header
                PdfPCell cell = new PdfPCell(new Phrase("Nombre Producto", new iTextSharp.text.Font(iTextSharp.text.Font.NORMAL, 14F, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)));
                cell.Colspan = 1;
                cell.HorizontalAlignment = 1;//0 left , 1 center , 2right
                table.AddCell(cell);
                //para header
                cell = new PdfPCell(new Phrase("Cantidad", new iTextSharp.text.Font(iTextSharp.text.Font.NORMAL, 14F, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)));
                cell.Colspan = 1;
                cell.HorizontalAlignment = 1;//0 left , 1 center , 2right
                table.AddCell(cell);
                //se rellana los campos de la tabla
                foreach (string[] tupla in listaProductosDelProveedor)
                {
                    table.AddCell(tupla[1]);
                    table.AddCell(tupla[4]);
                    totalProductos += Convert.ToInt32(tupla[4]);
                }
               
                doc.Add(table);
                
                //agregar total productos
                p.Clear();
                ph = new Phrase("\n\r Total de productos : ");
                p.Add(ph);
                chunk = new Chunk("" + totalProductos + "", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));
                p.Add((new Chunk(chunk)));
                doc.Add(p);

                doc.Close();
                System.Diagnostics.Process.Start("..\\..\\..\\Reporte-Proveedor.pdf");
      
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
    
            }
        }
        //en esta funcion se carga la tupla del proveedor y la lista de los productos del proveedor
        private void listBoxProveedor_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                //obtener la tupla para el proveedor
                TuplaPDFProveedor = new string[1];
                listaProductosDelProveedor.Clear();
                string dato = listBoxProveedor.Items[listBoxProveedor.SelectedIndex].ToString();
                string[] tupla = ProveedorEnLista.Find(x => x.Contains(dato)).Split(',');//regresa el empleado con todos sus datos necesarios
                TuplaPDFProveedor = tupla;//tenemos la tupla del proveedor que se le hara el reporte
                //buscar los productos de ese proveedor
                foreach (DataGridViewRow row in dataGridViewProducto.Rows)
                {
                    //if para evitar que se meta cuando es null
                    if (row.Cells[1].Value != null)
                    {
                        //si el producto es del proveedor que se le hara el reporte
                        if (row.Cells[5].Value.ToString().Contains(TuplaPDFProveedor[0]))
                        {
                            dato = row.Cells[0].Value.ToString() + "," + row.Cells[1].Value.ToString() + "," + row.Cells[2].Value.ToString() + "," + row.Cells[3].Value.ToString() + "," + row.Cells[4].Value.ToString() + "," + row.Cells[5].Value.ToString() + "," + row.Cells[6].Value.ToString() + "";
                            listaProductosDelProveedor.Add(dato.Split(','));//se agrega el producto a la lista
                        }            
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error en listbox");

            }
        }

        private void FormReporteProveedor_Load(object sender, EventArgs e)
        {

        }
    }
}
