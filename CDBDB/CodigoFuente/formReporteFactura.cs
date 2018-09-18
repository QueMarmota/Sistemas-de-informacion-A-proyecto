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
using System.Security.Cryptography;

namespace ProyectoBasesDeDatosDistribuidas
{
    public partial class formReporteFactura : Form
    {
        List<object> datosParaPdf = new List<object>();
        List<int> listPrimaryKeysProveedor;
        string[] TuplaPDFProveedor = new string[1];
        List<string[]> listaProductosDelProveedor = new List<string[]>();
        List<string> ProveedorEnLista = new List<string>();
        List<string> ProductoEnLista = new List<string>();
        List<string> VentaEnLista = new List<string>();
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

        public formReporteFactura()
        {
            InitializeComponent();
           // this.StartPosition = FormStartPosition.Manual;
           // this.Location = new Point(-5000, -5000);//para que no aparezca en la pantalla
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
            cargaTablaVentas();//carga el datagridventas
            //cargar el listview
            cargaListView();
        }
        public struct VariablesNecesariasParaEsquemaLocalizacion//estructura para evitar estar repitiendo codigo
        {
            //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
            //variables necesarias para sacar datos del esquema de localizacion
            public List<string> idFragmentos;
            public List<string> nombreTablaBDFragmento;
            public string nombreTablaGeneral;
            public string tipoFragmento;
            public List<string> sitios;
            public List<string> condicion;
            public SitioCentral st;
            public void DeclaraVariables()
            {

                this.idFragmentos = new List<string>();
                this.nombreTablaBDFragmento = new List<string>();
                this.nombreTablaGeneral = "";
                this.tipoFragmento = "";
                this.sitios = new List<string>();
                this.condicion = new List<string>();
                this.st = new SitioCentral();
            }

        }       
        private void cargaTablaVentas()
        {
            try
            {
                //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
                //variables necesarias para sacar datos del esquema de localizacion
                VariablesNecesariasParaEsquemaLocalizacion v = new VariablesNecesariasParaEsquemaLocalizacion();
                v.DeclaraVariables();
                v.st.LeeEsquemaLocalizacion("Venta", ref v.idFragmentos, ref v.nombreTablaBDFragmento, ref v.nombreTablaGeneral, ref v.sitios, ref v.tipoFragmento, ref v.condicion);
                switch (v.tipoFragmento)
                {
                    //si es horizontal tiene las mismas columnas y solo hacemos merge en las rows
                    case "Horizontal":


                        break;
                    case "Vertical":

                        break;
                    case "Replica":
                        MezclaBDReplicaVenta(v.nombreTablaBDFragmento);
                        break;

                    default:
                        mezcaBDNormalDelSitioVenta(v.sitios, v.nombreTablaBDFragmento);
                        break;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("No se pudo actualizar" + ex);
            }
        }
        private void mezcaBDNormalDelSitioVenta(List<string> sitios, List<string> nombreTablaBDFragmento)
        {

            switch (sitios.ElementAt(0))
            {
                case "1":
                    //codigo para hacer el select al sitio1
                    da = new SqlDataAdapter("Select * from " + nombreTablaBDFragmento.ElementAt(0) + "", cnSQL);
                    dt = new DataTable();
                    da.Fill(dt);
                    //dt.Columns.RemoveAt(0);//Para quitar el campo RFC
                    dataGridViewVenta.DataSource = dt;
                    //para esconder el campor id
                    dataGridViewVenta.Columns[0].Visible = false;

                    break;


                case "2":
                    //Codigo para hacer el select from a al sitio2             
                    NpgsqlDataAdapter add = new NpgsqlDataAdapter("select * from " + nombreTablaBDFragmento.ElementAt(0) + "", conNPG);
                    dtNPG = new DataTable();
                    add.Fill(dtNPG);
                    dataGridViewVenta.DataSource = dtNPG;
                    //para esconder el campor id
                    dataGridViewVenta.Columns[0].Visible = false;
                    break;
            }

        }
        public void MezclaBDReplicaVenta(List<string> nombreTablaBDFragmento)
        {

            //codigo para hacer el select al sitio1
            da = new SqlDataAdapter("Select * from " + nombreTablaBDFragmento.ElementAt(0) + "", cnSQL);
            dt = new DataTable();
            da.Fill(dt);
            //dt.Columns.RemoveAt(0);//Para quitar el campo RFC
            dataGridViewVenta.DataSource = dt;
            //para esconder el campor RFC
            dataGridViewVenta.Columns[0].Visible = false;
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
                        MezclaBDReplicaProducto(nombreTablaBDFragmento);
                        break;

                    default:
                        mezcaBDNormalDelSitioProducto(sitios, nombreTablaBDFragmento);
                        break;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("No se pudo actualizar" + ex);
            }
        }
        private void mezcaBDNormalDelSitioProducto(List<string> sitios, List<string> nombreTablaBDFragmento)
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
        public void MezclaBDReplicaProducto(List<string> nombreTablaBDFragmento)
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
        private void cargaListView()//aqui se cargan todas las listas
        {
            listBoxVenta.Items.Clear();
            VentaEnLista.Clear();
            ProductoEnLista.Clear();
            ProveedorEnLista.Clear();
            //cargamos la lista de ventas
            foreach (DataGridViewRow row in dataGridViewVenta.Rows)
            {
                if (row.Cells[1].Value != null)
                {
                    listBoxVenta.Items.Add("id Venta :  " + row.Cells[0].Value.ToString() + " , Fecha :  " + row.Cells[1].Value.ToString().Substring(0,10));
                    string dato = row.Cells[0].Value.ToString() + "," + row.Cells[1].Value.ToString() + "," + row.Cells[2].Value.ToString() + "," + row.Cells[3].Value.ToString() + "," + row.Cells[4].Value.ToString() + "," + row.Cells[5].Value.ToString() + "";
                    VentaEnLista.Add(dato);
                }
                //More code here
            }

            //cargamos lista producto
            foreach (DataGridViewRow row in dataGridViewProducto.Rows)
            {
                if (row.Cells[1].Value != null)
                {
                    string dato = row.Cells[0].Value.ToString() + "," + row.Cells[1].Value.ToString() + "," + row.Cells[2].Value.ToString() + "," + row.Cells[3].Value.ToString() + "," + row.Cells[4].Value.ToString() + "," + row.Cells[5].Value.ToString() + "," + row.Cells[6].Value.ToString() + "";
                    ProductoEnLista.Add(dato);
                }
                //More code here
            }
            //cargamos lista proveedor
            foreach (DataGridViewRow row in dataGridViewProveedor.Rows)
            {
                if (row.Cells[1].Value != null)
                {
                    string dato = row.Cells[0].Value.ToString() + "," + row.Cells[1].Value.ToString() + "," + row.Cells[2].Value.ToString() + "," + row.Cells[3].Value.ToString() + "," + row.Cells[4].Value.ToString() + "," + row.Cells[5].Value.ToString() + "";
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
        private void formReporteFactura_Load(object sender, EventArgs e)
        {

        }

        private void listBoxVenta_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                //obtener la tupla para el proveedor
                datosParaPdf = new List<object>();
                TuplaPDFProveedor = new string[1];
                listaProductosDelProveedor.Clear();
                string nombreDeLosProveedores = "";//<---------------------------------------------------------Datos para pdf
                string fechaVenta = "";//<---------------------------------------------------------------------DATO PARA PDF
                string DescripcionVenta = "";//<---------------------------------------------------------------DATO PARA PDF
                List<string> listapreciosProductos = new List<string>();
                List<string> listaidProveedoresDelProductoEnVenta = new List<string>();
                string totalDeVenta = "";//<--------------------------------------------------------------------DATO PARA PDF
                string datoVenta = listBoxVenta.Items[listBoxVenta.SelectedIndex].ToString();
                string[] cadtemp = datoVenta.Split(' ');//cadtem[4] tieme el id de la venta el cul se buscara en id producto
                string[] tuplaVenta = VentaEnLista.Find(x => x.Contains(cadtemp[4])).Split(',');//regresa el proveedor con todos sus datos necesarios
                totalDeVenta = tuplaVenta[tuplaVenta.Length-1].ToString() + " pesos";
                DescripcionVenta = tuplaVenta[2].ToString();
                fechaVenta = tuplaVenta[1].ToString().Substring(0,11);
                string productoEnVenta="";
                //siclo para obtener producto y cant
                for (int i = 3; i < tuplaVenta.Length-2; i++)
                {
                    productoEnVenta += tuplaVenta[i].ToString()+",";
                }
                productoEnVenta= productoEnVenta.Remove(productoEnVenta.LastIndexOf(','));//quitamos la ultima ,
                string[] productosEnVentaArray = productoEnVenta.Split(',');//variable con productos y cantidad pero en arreglo
                string[] arregloparaobtenerlacantidaddeproductos = new string[productosEnVentaArray.Length];//variable unicamente con la cantidad del producto<------------------------------para pdf
                string[] arregloProductos = new string[productosEnVentaArray.Length];//variable unicamente con el nombre del producto------------------------------------------para pdf
                int cont = 0;
                int canditadTotalDeProductos = 0;
                foreach (var item in productosEnVentaArray)
                {
                    arregloparaobtenerlacantidaddeproductos[cont] = item.Split('x').ElementAt(1);
                    arregloProductos[cont] = item.Split('x').ElementAt(0);
                    canditadTotalDeProductos += Convert.ToInt32(arregloparaobtenerlacantidaddeproductos[cont].ToString());//<---------------------------------------DATO PARA EL PDF
                    cont++;
                }
                
                
                TuplaPDFProveedor = tuplaVenta;//tenemos la tupla del proveedor del cual se vendieron sus productos
                //buscar los productos de esa venta y una vez obtenidos los productos tener el id del proveedor y su nombre
                foreach (DataGridViewRow row in dataGridViewProducto.Rows)
                {
                    //if para evitar que se meta cuando es null
                    if (row.Cells[1].Value != null)
                    {
                        //for que itera el arreglo productos para encontrar el producto igual en el data grid que en el arreglo
                        for (int i = 0; i < arregloProductos.Length; i++)
                        {
                            String cadDatagrid = row.Cells[1].Value.ToString();                            
                            String cadArray = arregloProductos[i].ToString();
                           // bool equal = String.Equals(cadDatagrid, cadArray, StringComparison.CurrentCultureIgnoreCase);
                            if (cadDatagrid.Contains(cadArray.Substring(0,2)))
                            {
                                
                                string tuplaProducto = row.Cells[0].Value.ToString() + "," + row.Cells[1].Value.ToString() + "," + row.Cells[2].Value.ToString() + "," + row.Cells[3].Value.ToString() + "," + row.Cells[4].Value.ToString() + "," + row.Cells[5].Value.ToString() + "," + row.Cells[6].Value.ToString() + "";                                
                                //con la tupla del producto buscar el proveedor de ese producto y agregarlo a la lista
                                string cadTemporal =tuplaProducto.Split(',').ElementAt(5);
                                string tuplaProveedor=ProveedorEnLista.Find(x => x.Split(',').ElementAt(0).Contains(cadTemporal));//obtenemos la tupla del proveedor con ese producto
                                nombreDeLosProveedores += tuplaProveedor.Split(',').ElementAt(1).ToString()+",";
                            }
                        }
                       
                    }
                }
                string[] arreglopreciosProducto = new string[productosEnVentaArray.Length];//<------------------------DATO PARA PDF----------------------------------------------
                //Buscar los precios de los productos
                int contadorPrecios = 0;
                for (int i = 0; i < arregloProductos.Length; i++)
                {
                    //if para evitar que se meta cuando es null
                    foreach (DataGridViewRow row in dataGridViewProducto.Rows)
                    {
                        if (row.Cells[1].Value != null)
                        {
                            String cadDatagrid = row.Cells[1].Value.ToString();
                            String cadArray = arregloProductos[i].ToString();
                            // bool equal = String.Equals(cadDatagrid, cadArray, StringComparison.CurrentCultureIgnoreCase);
                            if (cadDatagrid.Contains(cadArray.Substring(0, cadArray.Length-2)))
                            {

                                string tuplaProducto = row.Cells[0].Value.ToString() + "," + row.Cells[1].Value.ToString() + "," + row.Cells[2].Value.ToString() + "," + row.Cells[3].Value.ToString() + "," + row.Cells[4].Value.ToString() + "," + row.Cells[5].Value.ToString() + "," + row.Cells[6].Value.ToString() + "";
                                //con la tupla del producto buscar el proveedor de ese producto y agregarlo a la lista
                                string cadTemporal = tuplaProducto.Split(',').ElementAt(2);
                                arreglopreciosProducto[contadorPrecios] = cadTemporal;
                                contadorPrecios++;
                            }
                        }

                    }
                }//fin for
                //quitar los repetidos en nombre de los proveedores
                string[] arraytemp = nombreDeLosProveedores.Split(',');
                arraytemp = arraytemp.Distinct().ToArray();
                nombreDeLosProveedores = string.Join(",", arraytemp);
                //agregar los datos necesarios para generar el pdf a una lista generica global
                datosParaPdf.Add(arreglopreciosProducto);
                datosParaPdf.Add(canditadTotalDeProductos);
                datosParaPdf.Add(arregloparaobtenerlacantidaddeproductos);
                datosParaPdf.Add(arregloProductos);
                datosParaPdf.Add(totalDeVenta);
                datosParaPdf.Add(DescripcionVenta);
                datosParaPdf.Add(fechaVenta);
                datosParaPdf.Add(nombreDeLosProveedores);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error en listbox");

            }
        }

        private void btn_Genera_Click(object sender, EventArgs e)
        {
            try
            {
                if (datosParaPdf.Count == 0)
                {
                    MessageBox.Show("venta aun no seleccionada"); return;
                }
                int totalProductos = 0;
                Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
                PdfWriter wri = PdfWriter.GetInstance(doc, new FileStream("../../../Reporte-Factura.pdf", FileMode.Create));
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
                PNG.SetAbsolutePosition(50, 700);
                //para agregar un borde a la imagen
                PNG.Border = iTextSharp.text.Rectangle.BOX;
                PNG.BorderColor = iTextSharp.text.BaseColor.BLACK;
                PNG.BorderWidth = 5f;
                doc.Add(PNG);


                //write some content   
                Random rnd = new Random();
                int numfact = rnd.Next(100, 1001); 
                Random random = new Random();                                
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                string cadena = new string(Enumerable.Repeat(chars, 5)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                string idfactura = cadena + "-" + numfact;
                Phrase ph = new Phrase("                                                                                      Numero de factura : " + idfactura);
                Paragraph p = new Paragraph(ph);
                p.Alignment = Element.ALIGN_LEFT;                
                p.IndentationLeft = 120;
                //se agrega nombre
                p.IndentationLeft = 50;
                ph = new Phrase("\n\r \n\r      Nombre de proveedores : ");
                Chunk chunk = new Chunk(datosParaPdf.ElementAt(7).ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));                
                p.Add(ph);
                p.Add((new Chunk(chunk)));
                //se agrega fecha venta
                p.IndentationLeft = 50;
                ph = new Phrase("    Fecha de venta : ");
                chunk = new Chunk(datosParaPdf.ElementAt(6).ToString(), FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));
                p.Add(ph);
                p.Add((new Chunk(chunk)));
                ph = new Phrase(" \n\r \n\r");              
                p.Add(ph);
                doc.Add(p);

                PdfPTable table = new PdfPTable(3);//el 3 es el numero de columnas
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
                //para header
                cell = new PdfPCell(new Phrase("Precio", new iTextSharp.text.Font(iTextSharp.text.Font.NORMAL, 14F, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK)));
                cell.Colspan = 1;
                cell.HorizontalAlignment = 1;//0 left , 1 center , 2right
                table.AddCell(cell);
                //estructura de datosparapdf
                //arrayprecio , cantidadtotalproudctos,cantidadde1producto,productos,totaldevental,descripcion,fecha,nombreproveedores
               
                //codigo para convertir de object a string[]
                object[] res = datosParaPdf.ElementAt(3) as object[];      
                string[] nombreProductos = res.OfType<string>().ToArray();
                res = datosParaPdf.ElementAt(0) as object[];
                string[] precioproductos = res.OfType<string>().ToArray();
                res = datosParaPdf.ElementAt(2) as object[];
                string[] cantidadde1producto = res.OfType<string>().ToArray();

                //se rellana los campos de la tabla
                for (int i = 0; i < nombreProductos.Length; i++)
                {
                    table.AddCell(nombreProductos[i]);
                    table.AddCell(cantidadde1producto[i]);
                    table.AddCell("$"+precioproductos[i]);
                }
 

                doc.Add(table);

                //agregar Descripcion de venta
                p.Clear();
                ph = new Phrase("\n\r Descripcion de venta : ");
                p.Add(ph);
                chunk = new Chunk("" + datosParaPdf.ElementAt(5).ToString() + "", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));
                p.Add((new Chunk(chunk)));
                //agregar total de productos
                ph = new Phrase("\n\r \n\r   Total de productos : ");
                p.Add(ph);
                chunk = new Chunk("" + datosParaPdf.ElementAt(1).ToString() + "", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));
                p.Add((new Chunk(chunk)));
                //agregar total de venta
                ph = new Phrase("                                                Total de venta : ");
                p.Add(ph);
                chunk = new Chunk("" + datosParaPdf.ElementAt(4).ToString() + "", FontFactory.GetFont(FontFactory.TIMES_ROMAN, 12.0f, iTextSharp.text.Font.BOLD | iTextSharp.text.Font.UNDERLINE));
                p.Add((new Chunk(chunk)));



                doc.Add(p);

                doc.Close();
                System.Diagnostics.Process.Start("..\\..\\..\\Reporte-Factura.pdf");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
    
    
    
    }
}
