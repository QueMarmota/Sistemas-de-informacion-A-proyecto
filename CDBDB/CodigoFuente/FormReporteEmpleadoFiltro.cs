using System;
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
    public partial class FormReporteEmpleadoFiltro : Form
    {

        string[] TuplaPDF = new string[1];
        List<string> EmpleadosEnLista = new List<string>();
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
        public FormReporteEmpleadoFiltro()
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
            cargaTabla();//carga datagrid

            //cargar el listview
            cargaListView();

        }
        private void cargaListView()
        {
            listBoxEmpleado.Items.Clear();
            EmpleadosEnLista.Clear();
            foreach (DataGridViewRow row in dataGridViewEmpleados.Rows)
            {
                //if para evitar que se meta cuando es null
                if (row.Cells[1].Value != null)
                {
                    listBoxEmpleado.Items.Add(row.Cells[1].Value.ToString());
                    string dato = row.Cells[0].Value.ToString() + "," + row.Cells[1].Value.ToString() + "," + row.Cells[2].Value.ToString() + "," + row.Cells[3].Value.ToString() + "," + row.Cells[4].Value.ToString() + "," + row.Cells[5].Value.ToString() + "," + row.Cells[6].Value.ToString() + "," + row.Cells[7].Value.ToString() + "," + row.Cells[8].Value.ToString() + "," + row.Cells[9].Value.ToString() + "," + row.Cells[10].Value.ToString();
                    EmpleadosEnLista.Add(dato);
                }
              
            }                        
        }
        private void cargaTabla()
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
                st.LeeEsquemaLocalizacion("Empleado", ref idFragmentos, ref nombreTablaBDFragmento, ref nombreTablaGeneral, ref sitios, ref tipoFragmento, ref condicion);
                switch (tipoFragmento)
                {
                    //si es horizontal tiene las mismas columnas y solo hacemos merge en las rows
                    case "Horizontal":
                        MezclaBDHorizontal();

                        break;
                    case "Vertical":

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

        public void MezclaBDHorizontal()
        {


            //codigo para hacer el select al sitio1
            da = new SqlDataAdapter("Select * from Empleado1", cnSQL);
            dt = new DataTable();
            da.Fill(dt);
            //dt.Columns.RemoveAt(0);//Para quitar el campo RFC
            dataGridViewEmpleados.DataSource = dt;


            //Codigo para hacer el select from a al sitio2             
            NpgsqlDataAdapter add = new NpgsqlDataAdapter("select * from Empleado2", conNPG);
            //add.Fill(datos);
            dtNPG = new DataTable();
            add.Fill(dtNPG);
            // dtNPG.Columns.RemoveAt(0);//para quitar el campo RFC

            //Hacer merge entre los dos select del sito1 y el sitio2
            for (int i = 0; i < dtNPG.Rows.Count; i++)//itera renglones
            {
                dt.ImportRow(dtNPG.Rows[i]);
            }

            //dt tiene los campos del sitio1 y del sitio2
            dataGridViewEmpleados.DataSource = dt;

            //para esconder el campor RFC
            dataGridViewEmpleados.Columns[0].Visible = false;
        }


        private void FormReporteFiltro_Load(object sender, EventArgs e)
        {
            //cargar todos los empleados el datagrid oculto 


        }

        private void btn_Genera_Click(object sender, EventArgs e)
        {
            try
            {
                if (TuplaPDF[0] == null)
                {
                    MessageBox.Show("Empleado aun no seleccionado"); return;

                }
                Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
                PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream("../../../Reporte-Empleado.pdf", FileMode.Create));
                doc.Open();
                //codigo para agregar bordes de pagina al pdf
                var content = wri.DirectContent;
                var pageBorderRect = new iTextSharp.text.Rectangle(doc.PageSize);

                pageBorderRect.Left += doc.LeftMargin;
                pageBorderRect.Right -= doc.RightMargin;
                pageBorderRect.Top -= doc.TopMargin;
                pageBorderRect.Bottom += doc.BottomMargin;

                content.SetColorStroke(BaseColor.BLACK);
                content.Rectangle(pageBorderRect.Left, pageBorderRect.Bottom, pageBorderRect.Width, pageBorderRect.Height+30);
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

                Chunk chunk = new Chunk("" + TuplaPDF[1] + "", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));               
                Phrase ph = new Phrase("\n\r \n\r \n\r Nombre : ");
                Paragraph p = new Paragraph(ph);
                p.Alignment = Element.ALIGN_LEFT;
                // p.SpacingBefore = spaceBefore;
                p.IndentationLeft = 120;
                p.Add((new Chunk(chunk)));//agregamos el nombre con linea
                //se agrega direccion
                ph = new Phrase("\t      Dirección : ");
                p.Add(ph);
                chunk = new Chunk("" + TuplaPDF[5] + "", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));
                p.Add((new Chunk(chunk)));
                //se agrega fecha nacimiento
                ph = new Phrase("\n\r Fecha de nacimiento : ");
                p.Add(ph);
                chunk = new Chunk("" + TuplaPDF[6].Substring(0,10) + "", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));
                p.Add((new Chunk(chunk)));
                //se agrega telefono
                ph = new Phrase("\t      Telefono : ");
                p.Add(ph);
                chunk = new Chunk("" + TuplaPDF[4] + "", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));
                p.Add((new Chunk(chunk)));
                //se agrega Genero
                ph = new Phrase("\n\r Género : ");
                p.Add(ph);
                string genero = TuplaPDF[7] == "F" ? "Femenino" : "Masculino";
                chunk = new Chunk("" + genero + "", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));
                p.Add((new Chunk(chunk)));
                //se agrega Hora entrada
                ph = new Phrase("\n\r Hora entrada : ");
                p.Add(ph);
                chunk = new Chunk("" + TuplaPDF[8]+"am"+"", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));
                p.Add((new Chunk(chunk)));
                //se agrega hora salida
                ph = new Phrase("\t      Hora salida : ");
                p.Add(ph);
                chunk = new Chunk("" + TuplaPDF[9]+"pm" + "", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));
                p.Add((new Chunk(chunk)));
                //se agrega Sucursal donde trabaja
                ph = new Phrase("\n\r Sucursal : ");
                p.Add(ph);
                chunk = new Chunk("" + TuplaPDF[10]+"", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));
                p.Add((new Chunk(chunk)));
                //se agrega Sueldo
                ph = new Phrase("\n\r Sueldo : ");
                p.Add(ph);
                chunk = new Chunk("" + TuplaPDF[2]+" pesos" + "", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));
                p.Add((new Chunk(chunk)));


                doc.Add(p);

                
                /*Phrase ph = new Phrase("\n\r \n\r \n\r Nombre : " + TuplaPDF[1] + " Dirección : " + TuplaPDF[5] + "");
                Paragraph p = new Paragraph(ph);
                p.Alignment = Element.ALIGN_LEFT;
               // p.SpacingBefore = spaceBefore;
                p.IndentationLeft = 100;   
                doc.Add(p);
                ph = new Phrase("\n\r \n\r \n\r Nombre : " + TuplaPDF[1] + " Dirección : " + TuplaPDF[5] + "");
                p = new Paragraph(ph);
                p.Alignment = Element.ALIGN_LEFT;
                // p.SpacingBefore = spaceBefore;
                p.IndentationLeft = 100;
                doc.Add(p);

                */

                /*
                //Para crear una lista
                List list = new List(List.UNORDERED);
                list.Add(new ListItem("One"));
                list.Add("two");
                list.Add("two");
                list.Add("two");
                list.Add("two");
                list.Add(new ListItem("One"));
                doc.Add(list);
                 * */
                /*
                //para crear tabla en el pdf

                PdfPTable table = new PdfPTable(3);//el 3 es el numero de columnas
                //para header
                PdfPCell cell = new PdfPCell(new Phrase("Header spanning 3 columns", new iTextSharp.text.Font(iTextSharp.text.Font.NORMAL, 8F, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.YELLOW)));
                cell.Colspan = 3;
                cell.HorizontalAlignment = 1;//0 left , 1 center , 2right
                table.AddCell(cell);

                table.AddCell("Col1 row1");
                table.AddCell("Col2 row1");
                table.AddCell("Col3 row1");
            
                table.AddCell("Col1 row2");
                table.AddCell("Col2 row2");
                table.AddCell("Col3 row2");

                doc.Add(table);
                */
                /*
                #region codigo para pasar datos del datagrid a pdf
                //codigo para pasar a una tabla la informacion del datagridview
                PdfPTable table = new PdfPTable(dataGridViewEmpleados.Columns.Count);

                //add the headers from the DGV to the table
                for (int j = 0; j < dataGridViewEmpleados.Columns.Count; j++)
                {
                    table.AddCell(new Phrase(dataGridViewEmpleados.Columns[j].HeaderText));   
                }

                //flag the first row as a header
                table.HeaderRows = 1;

                //Add the actual rows from the DGV to the table
                for (int i = 0; i < dataGridViewEmpleados.Rows.Count; i++)
                {
                    for (int k = 0; k < dataGridViewEmpleados.Columns.Count; k++)
                    {
                        if (dataGridViewEmpleados[k, i].Value != null)
                        {
                            table.AddCell(new Phrase(dataGridViewEmpleados[k, i].Value.ToString()));
                        }
                    }
                }

                doc.Add(table);
                #endregion*/
                doc.Close();
                System.Diagnostics.Process.Start("..\\..\\..\\Reporte-Empleado.pdf");
              //  MessageBox.Show("Reporte Creado!!!");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                
            }
           
        }
        private void CreateBorder(PdfContentByte cb, PdfWriter writer, Document doc)
        {
            iTextSharp.text.Rectangle r = doc.PageSize;
            float left = r.Left + 30;
            float right = r.Right - 30;
            float top = r.Top - 30;
            float bottom = r.Bottom + 30;
            float width = right - left;
            float height = top - bottom;

            PdfPTable tab = new PdfPTable(1);
            tab.TotalWidth = width;
            tab.LockedWidth = true;

            PdfPCell t = new PdfPCell(new Phrase(String.Empty));
            t.BackgroundColor = new BaseColor(250, 235, 215);
            t.FixedHeight = height;
            t.BorderWidth = 3;
            tab.AddCell(t);
            Paragraph pa = new Paragraph();
            pa.Add(tab);

            float h = tab.TotalHeight;
            PdfTemplate temp = cb.CreateTemplate(tab.TotalWidth, h);
            tab.WriteSelectedRows(0, -1, 0.0F, h, temp);
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(temp);
            img.SetAbsolutePosition(30, 30);
            cb.AddImage(img);
        }

        private void listBoxEmpleado_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                TuplaPDF = new string[1];
                string dato = listBoxEmpleado.Items[listBoxEmpleado.SelectedIndex].ToString();
                string[] tupla = EmpleadosEnLista.Find(x => x.Contains(dato)).Split(',');//regresa el empleado con todos sus datos necesarios
                //codigo para convertir internamente el nombre de la sucursal al id de la sucursal
                //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
                //variables necesarias para sacar datos del esquema de localizacion
                List<string> idFragmentos = new List<string>();
                List<string> nombreTablaBDFragmento = new List<string>();
                string nombreTablaGeneral = "";
                string tipoFragmento = "";
                List<string> sitios = new List<string>();
                List<string> condicion = new List<string>();
                SitioCentral st = new SitioCentral();
                st.LeeEsquemaLocalizacion("Sucursal", ref idFragmentos, ref nombreTablaBDFragmento, ref nombreTablaGeneral, ref sitios, ref tipoFragmento, ref condicion);
           
                SqlDataAdapter idSucursal = new SqlDataAdapter("Select nombre from " + nombreTablaBDFragmento[0] + " where id_Sucursal = '" + tupla[10] + "'", cnSQL);
                cmd = new SqlCommand("Select nombre from " + nombreTablaBDFragmento[0] + "  where id_Sucursal = '" + tupla[10] + "'", cnSQL);
                dr = cmd.ExecuteReader();
                dr.Read();
                string nombreSucural = (dr[0].ToString());
                dr.Close();
                //sustituimos el idsucursal por el nombre obtenido para mandarlo al pdf
                
                 tupla[10] = tupla.ElementAt(10).Replace(tupla.ElementAt(10), nombreSucural);
                 TuplaPDF = tupla;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error en listbox");
              
            }
            
        }
    }
}
