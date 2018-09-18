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


namespace ProyectoBasesDeDatosDistribuidas
{
    public partial class Venta : Form
    {//lista que contiene el id_proveedor y nombre
        List<string> listaidProductoNombrePrecioCantidad = new List<string>();
        List<string> listaOfertas = new List<string>();
        List<string> listaInventarioCarrito = new List<string>();
        bool banderaNoPermiteModificarListBox = false;
        decimal total;
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

        public struct VariablesNecesariasParaEsquemaLocalizacion//estructura para evitar estar repitiendo codigo
        {
            //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
            //variables necesarias para sacar datos del esquema de localizacion
            public List<string> idFragmentos ;
            public List<string> nombreTablaBDFragmento  ;
            public string nombreTablaGeneral ;
            public string tipoFragmento ;
            public List<string> sitios ;
            public List<string> condicion ;
            public SitioCentral st ;
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
        public Venta()
        {
            InitializeComponent();
            total = 0;
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

            CargalistaidProductoNombrePrecioCantidad();
            cargaCOMBOListBoxInventarioProductos();
            cargaTabla();

        }
        private int ChecaSiExisteOfertaYRegresaDescuento(string productoAChecar)
        {
            SqlDataReader dr;
            
            //PARA CARGAR LISTA DE PRODUCTOS
            //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
            //variables necesarias para sacar datos del esquema de localizacion
            VariablesNecesariasParaEsquemaLocalizacion v = new VariablesNecesariasParaEsquemaLocalizacion();
            v.DeclaraVariables();
            v.st.LeeEsquemaLocalizacion("Oferta", ref v.idFragmentos, ref v.nombreTablaBDFragmento, ref v.nombreTablaGeneral, ref v.sitios, ref v.tipoFragmento, ref v.condicion);
            //carga lista de ofertas
            SqlDataAdapter daOferta = new SqlDataAdapter("Select * from " + v.nombreTablaBDFragmento.ElementAt(0) + "", cnSQL);
            DataTable dtOferta = new DataTable();
            daOferta.Fill(dtOferta);
            listaOfertas = new List<string>();
            // For each row, print the values of each column.
            //idOferta,descripcon,fecha,descuento,idProducto
            foreach (DataRow row in dtOferta.Rows)
            {
                listaOfertas.Add(row[dtOferta.Columns[0]].ToString() + "," + row[dtOferta.Columns[1]].ToString() + "," + row[dtOferta.Columns[2]].ToString() + "," + row[dtOferta.Columns[3]].ToString()+ "," + row[dtOferta.Columns[4]].ToString());
            }
            string idProducto ="";
            string descuento = "";

            //checar si el producto tiene oferta
            foreach (string datoOferta in listaOfertas)
            {
                string[] substring = datoOferta.Split(',');
                descuento = substring.ElementAt(3).ToString();
                idProducto = substring.ElementAt(4).ToString();
                //ahora tengo el id del producto que tiene la oferta , ahora tengo q buscar que nombre de producto es ese id de producto
                //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
                //variables necesarias para sacar datos del esquema de localizacion
                v = new VariablesNecesariasParaEsquemaLocalizacion();
                v.DeclaraVariables();
                v.st.LeeEsquemaLocalizacion("Producto", ref v.idFragmentos, ref v.nombreTablaBDFragmento, ref v.nombreTablaGeneral, ref v.sitios, ref v.tipoFragmento, ref v.condicion);
                //codigo para convertir internamente el nombre de la sucursal al id de la sucursal               
                cmd = new SqlCommand("Select nombre from "+v.nombreTablaBDFragmento.ElementAt(0)+" where id_Producto = '" + idProducto + "'", cnSQL);
                dr = cmd.ExecuteReader();
                dr.Read();
                string nombreDelProductoConDescuento = (dr[0]).ToString();
                dr.Close();
            
                if (productoAChecar.Contains(nombreDelProductoConDescuento))
                {
                      
                    int descuentoTemp =Convert.ToInt32(datoOferta.Split(',').ElementAt(3));
                    return descuentoTemp;                 
                }
                
            }            
            return -1; 
        }
        private void CargalistaidProductoNombrePrecioCantidad()
        {

            //PARA CARGAR LISTA DE PRODUCTOS
            //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
            //variables necesarias para sacar datos del esquema de localizacion
            listaidProductoNombrePrecioCantidad = new List<string>();
            VariablesNecesariasParaEsquemaLocalizacion v = new VariablesNecesariasParaEsquemaLocalizacion();
            v.DeclaraVariables();
            v.st.LeeEsquemaLocalizacion("Producto", ref v.idFragmentos, ref v.nombreTablaBDFragmento, ref v.nombreTablaGeneral, ref v.sitios, ref v.tipoFragmento, ref v.condicion);
            //carga una lista con id y nombre de los productos
            SqlDataAdapter daProducto = new SqlDataAdapter("Select * from " + v.nombreTablaBDFragmento.ElementAt(0) + "", cnSQL);
            DataTable dtProducto = new DataTable();
            daProducto.Fill(dtProducto);
            // For each row, print the values of each column.
            foreach (DataRow row in dtProducto.Rows)
            {
                listaidProductoNombrePrecioCantidad.Add(row[dtProducto.Columns[0]].ToString() + "," + row[dtProducto.Columns[1]].ToString() + "," + row[dtProducto.Columns[2]].ToString() + "," + row[dtProducto.Columns[4]].ToString());
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
        public void MezclaBDReplica(List<string> nombreTablaBDFragmento)
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


        private void cargaTabla()
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
                        MezclaBDReplica(v.nombreTablaBDFragmento);
                        break;

                    default:
                        mezcaBDNormalDelSitio(v.sitios, v.nombreTablaBDFragmento);
                        break;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("No se pudo actualizar" + ex);
            }
        }

        private void Venta_Load(object sender, EventArgs e)
        {

        }

        private void cargaCOMBOListBoxInventarioProductos()
        {
            listBoxInventario.Items.Clear();
            foreach (var item in listaidProductoNombrePrecioCantidad)
            {
                //id producto , nombre , precio , cantidad en el stock
                string[] temp = item.Split(',');
                listBoxInventario.Items.Add(temp.ElementAt(1)+" x"+temp.ElementAt(3));
         
            }
          
        }

        private void listBoxInventario_MouseDown(object sender, MouseEventArgs e)
        {

            try
            {
                
                //codigo para restar un producto en la lista stock
                string dato = listBoxInventario.Items[listBoxInventario.SelectedIndex].ToString();
                string[] nombreycantidad = dato.Split('x');
                int cantidadmenos1 = Int32.Parse(nombreycantidad.ElementAt(1)) - 1;
                if (banderaNoPermiteModificarListBox)//si esta activada la bandera es por que se quiere pasar un producto del carrito al stock y no se puede
                {
                    MessageBox.Show("No se puede agregar mas Productos!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                  
                }
                else
                {
                    if (cantidadmenos1 == 0)// si el producto llega a la cantidad de 0 se elimina
                    {
                        listBoxInventario.Items.RemoveAt(listBoxInventario.SelectedIndex);
                    }
                    else//se agrega a la lista el producto con una unidad menos
                        listBoxInventario.Items[listBoxInventario.SelectedIndex] = nombreycantidad.ElementAt(0) + "x" + cantidadmenos1.ToString();

                    //codigo para mandar a la lista carrito el producto con el numero de veces que lo solicito (agregar al carrito)
                    //Buscar el producto tecleado y la cantidad para sacar la diferencia y mandar la cantidad de productos a vender
                    //Voy a buscar en el carrito si ya existe el producto del infentario
                    string productoEnCarritoTemp = "";
                    foreach (string productoCarrito in listBoxCarrito.Items)
                    {
                        if (productoCarrito.Contains(nombreycantidad.ElementAt(0)))
                            productoEnCarritoTemp = productoCarrito;
                    }
                    if (productoEnCarritoTemp != "")//si ya existe hay q sumarle una unidad a ese producto
                    {
                        string nombreProd = productoEnCarritoTemp.Split('x').ElementAt(0).ToString().Trim();
                        string infodelproducto = listaidProductoNombrePrecioCantidad.Find(x => x.Contains(nombreProd));
                        string[] arraytemp2 = infodelproducto.Split(',');//se obtiene toda la info del producto para obtener su precio mas adelante
                        string[] arraytemp = productoEnCarritoTemp.Split('x');
                        int cantidadTotal = Convert.ToInt32(arraytemp.ElementAt(1)) + 1;
                        string productoEnCarrito = nombreProd + " x" + cantidadTotal;//aqui se saca la diferencia osea resto el total menos los que llevaS
                        if (listBoxCarrito.FindString(nombreProd, 0) != -1)//si en el carrito ya exite el producto remplazamos la cadena
                        {
                            listBoxCarrito.Items[listBoxCarrito.FindString(nombreProd, 0)] = productoEnCarrito;
                        }
                        else //si no existe se agrega
                        {
                            listBoxCarrito.Items.Add(productoEnCarrito);
                           
                        }
                        //codigo para saber si el producto tiene DESCUENTO
                        int descuento = ChecaSiExisteOfertaYRegresaDescuento(productoEnCarrito);

                        if (descuento != -1)//si tiene descuento
                        {
                            //codigo para aumentar total con descuento
                            total += Convert.ToDecimal(arraytemp2[2]) - ((Convert.ToDecimal(arraytemp2[2]) * descuento) / 100);
                        }
                        else
                        {
                            //codigo para aumentar total
                            total += Convert.ToDecimal(arraytemp2[2]);
                        }

                        //richTextBoxTotal.SelectionColor = Color.Lime;

                        //richTextBoxTotal.SelectionFont = new Font("Verdana", 15, FontStyle.Regular);
                        //richTextBoxTotal.SelectionColor = Color.Lime;

                        richTextBoxTotal.Text = "$" + total.ToString();

                        //para quitar la selccion
                        // listBoxInventario.ClearSelected();
                    }
                    else//si no existe se agrega por primera vez
                    {
                        string nombreProd = nombreycantidad.ElementAt(0).Trim();
                        string infodelproducto = listaidProductoNombrePrecioCantidad.Find(x => x.Contains(nombreProd));
                        string[] arraytemp = infodelproducto.Split(',');
                        int cantidadTotal = 1;
                        string productoEnCarrito = nombreProd + " x" + cantidadTotal;//aqui se saca la diferencia osea resto el total menos los que llevaS
                        if (listBoxCarrito.FindString(nombreProd, 0) != -1)//si en el carrito ya exite el producto remplazamos la cadena
                        {
                            listBoxCarrito.Items[listBoxCarrito.FindString(nombreProd, 0)] = productoEnCarrito;
                        }
                        else //si no existe se agrega
                        {
                            listBoxCarrito.Items.Add(productoEnCarrito);
                        }
                        //codigo para saber si el producto tiene DESCUENTO
                        int descuento = ChecaSiExisteOfertaYRegresaDescuento(productoEnCarrito);
                
                        if (descuento != -1)//si tiene descuento
                        {
                            //codigo para aumentar total con descuento
                            total += Convert.ToDecimal(arraytemp[2])-((Convert.ToDecimal(arraytemp[2]) * descuento) / 100);
                        }
                        else
                        {
                            //codigo para aumentar total
                            total += Convert.ToDecimal(arraytemp[2]);
                        }

                        //richTextBoxTotal.SelectionColor = Color.Lime;

                        //richTextBoxTotal.SelectionFont = new Font("Verdana", 15, FontStyle.Regular);
                        //richTextBoxTotal.SelectionColor = Color.Lime;

                        richTextBoxTotal.Text = "$" + total.ToString();

                        //para quitar la selccion
                         listBoxInventario.ClearSelected();
                    }
                }
                listBoxInventario.ClearSelected();

                

            }
            catch (Exception ex)
            {
                if (dr!=null)
                {
                    dr.Close();
                }
              
            }
            
        }

        private void LimpiaListBoxCarrito()
        {
            int conttemp = 0;
            do
	        {
                string item = this.listBoxCarrito.Items[conttemp].ToString();
	            if (item == "")
                {
                    listBoxCarrito.Items.Remove(item);
                }
                conttemp++;

            } while (conttemp != this.listBoxCarrito.Items.Count && conttemp < this.listBoxCarrito.Items.Count);
                

	        
            
        }

        private void listBoxCarrito_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                //listBoxCarrito.Items.RemoveAt(listBoxCarrito.SelectedIndex);
                //listBoxCarrito.Items.Add(dato);
                CargalistaidProductoNombrePrecioCantidad();
                LimpiaListBoxCarrito();
                //codigo para restar un producto en la lista carrito
                string dato = listBoxCarrito.Items[listBoxCarrito.SelectedIndex].ToString();
                string[] nombreycantidad = dato.Split('x');
                int cantidadmenos1 = Int32.Parse(nombreycantidad.ElementAt(1)) - 1;

                if (banderaNoPermiteModificarListBox)//si esta activada la bandera es por que se quiere pasar un producto del carrito al stock y no se puede
                {
                    MessageBox.Show("No se aceptan devoluciones!!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                    
                }
                else
                {
                    if (cantidadmenos1 == 0)// si el producto llega a la cantidad de 0 se elimina
                    {
                        listBoxCarrito.Items.RemoveAt(listBoxCarrito.SelectedIndex);
                    }
                    else//se modifica la lista del producto con una unidad menos
                        listBoxCarrito.Items[listBoxCarrito.SelectedIndex] = nombreycantidad.ElementAt(0) + "x" + cantidadmenos1.ToString();

                    //codigo para mandar a la lista stock el producto con el numero de veces que lo solicito (agregar al carrito)
                    //Buscar el producto tecleado y la cantidad para sacar la diferencia y mandar la cantidad de productos a vender
                    string nombreProd = nombreycantidad.ElementAt(0).Trim();
                    string infodelproducto = listaidProductoNombrePrecioCantidad.Find(x => x.Contains(nombreProd));                                     
                        string[] arraytemp = infodelproducto.Split(',');
                        int cantidadTotal = 0;
                        if (cantidadmenos1 == 0)
                        {
                            cantidadTotal = Convert.ToInt32(arraytemp.ElementAt(3));
                        }
                        else
                            cantidadTotal = Convert.ToInt32(arraytemp.ElementAt(3)) - cantidadmenos1;
                        string productoEnCarrito = nombreProd + " x" + cantidadTotal;//aqui se saca la diferencia osea resto el total menos los que llevaS
                        if (listBoxInventario.FindString(nombreProd, 0) != -1)//si en el carrito ya exite el producto remplazamos la cadena
                        {
                            listBoxInventario.Items[listBoxInventario.FindString(nombreProd, 0)] = productoEnCarrito;
                        }
                        else //si no existe se agrega
                        {
                            listBoxInventario.Items.Add(productoEnCarrito);
                        }

                        //Codigo para disminuir total
                        int descuento = ChecaSiExisteOfertaYRegresaDescuento(productoEnCarrito);

                        if (descuento != -1)//si tiene descuento
                        {
                            //codigo para aumentar total con descuento
                            total -= Convert.ToDecimal(arraytemp[2])-((Convert.ToDecimal(arraytemp[2])*descuento) / 100);

                        }
                        else
                        {
                            //codigo para aumentar total
                            total -= Convert.ToDecimal(arraytemp[2]);
                        }
                        
                        richTextBoxTotal.Text = "$" + total.ToString();
                        listBoxCarrito.ClearSelected();
                    
                   
                }
                listBoxCarrito.ClearSelected();
            }
            catch (Exception ex)
            {
         
            }
            
        }

        private void richTextBoxTotal_TextChanged(object sender, EventArgs e)
        {
           
        }
        private bool validaDatosEntrada()
        {
            if (textBoxDescripcionVenta.Text == "" || dateTimePickerFecha.Text == "" || listBoxCarrito.Items.Count <= 0 )
            {
                MessageBox.Show("Error de datos");
                return false;
            }

            return true;
        }
        private void limpiaCampos()
        {

            textBoxDescripcionVenta.Clear();
            richTextBoxTotal.Clear();
            dateTimePickerFecha.Value = DateTime.Now;
            listBoxCarrito.Items.Clear();

        }
        private int calculaCantidadProductos()
        { 
        
            int cantidad =0;

            foreach (string  producto in listBoxCarrito.Items)
            {
                string[] nombreycant = producto.Split('x');
                cantidad += Int32.Parse(nombreycant.ElementAt(1).ToString());
            }

            return cantidad;
        
        }
        private void BtnInsertar_Click(object sender, EventArgs e)
        {
            if (!validaDatosEntrada())
                return;



            try
            {

                banderaNoPermiteModificarListBox = false;
                string fechaVenta = dateTimePickerFecha.Value.ToString("yyyy-MM-dd");
                string descripcion = textBoxDescripcionVenta.Text;
                string productos="";
                string quitarPrecio = "";
                string idproductonombreyNuevaCantidad = "";
                int cont = 1;
                List<string> listaDeProductoActualizado = new List<string>();
                foreach (string  producto in listBoxCarrito.Items)
                {
                    if (cont == listBoxCarrito.Items.Count)//para que no agrege una , al final

                    {
                        productos += producto ;
                        string[] nombreProdycantidadCarrito = producto.Split('x');//nombre y cantidad EN EL CARRITO
                        string nombreProducto = nombreProdycantidadCarrito.ElementAt(0).Trim();
                        string infodelproducto = listaidProductoNombrePrecioCantidad.Find(x => x.Contains(nombreProducto));
                        if (infodelproducto == null)
                            continue;
                        string[] idnombrecantidad = infodelproducto.Split(',');
                        List<string> listTemporal = new List<string>(idnombrecantidad);
                        listTemporal.RemoveAt(2);//para quitar el campo precio
                       // idnombrecantidad = idnombrecantidad.Where(w => w != idnombrecantidad[2]).ToArray(); 
                        int cantidadAModificarDelProducto = Int32.Parse(listTemporal.ElementAt(2)) - Int32.Parse(nombreProdycantidadCarrito.ElementAt(1));//el total de la cantidad del producto mas lo q se solicito en el carrito
                        idproductonombreyNuevaCantidad = listTemporal[0] + "," + listTemporal[1] + "," + cantidadAModificarDelProducto.ToString();
                        
                    }
                    else
                    {
                        productos += producto + ",";
                        string[] nombreProdycantidadCarrito = producto.Split('x');//nombre y cantidad
                        string nombreProducto = nombreProdycantidadCarrito.ElementAt(0).Trim();
                        string infodelproducto = listaidProductoNombrePrecioCantidad.Find(x => x.Contains(nombreProducto));
                        if (infodelproducto == null)
                            continue;
                        string[] idnombrecantidad = infodelproducto.Split(',');
                        List<string> listTemporal = new List<string>(idnombrecantidad);
                        listTemporal.RemoveAt(2);//para quitar el campo precio
                        // idnombrecantidad = idnombrecantidad.Where(w => w != idnombrecantidad[2]).ToArray(); 
                        int cantidadAModificarDelProducto = Int32.Parse(listTemporal.ElementAt(2)) - Int32.Parse(nombreProdycantidadCarrito.ElementAt(1));//el total de la cantidad del producto mas lo q se solicito en el carrito
                        idproductonombreyNuevaCantidad = listTemporal[0] + "," + listTemporal[1] + "," + cantidadAModificarDelProducto.ToString();
                    }
                    listaDeProductoActualizado.Add(idproductonombreyNuevaCantidad);
                    cont++;
                }//FIN DE FOREACH LISTBOXCARRITO PARA LLENAR LA LISTA ACTUALIZADA

                string totalS = richTextBoxTotal.Text.ToString();
                totalS = totalS.Remove(0,1);//hay q quitar el signo de pesos si no no lo converte a decimal
                decimal total = decimal.Parse(totalS);
                int cantidadProductos = calculaCantidadProductos();
                //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
                //variables necesarias para sacar datos del esquema de localizacion
                VariablesNecesariasParaEsquemaLocalizacion v = new VariablesNecesariasParaEsquemaLocalizacion();
                v.DeclaraVariables();
                v.st.LeeEsquemaLocalizacion("Venta", ref v.idFragmentos, ref v.nombreTablaBDFragmento, ref v.nombreTablaGeneral, ref v.sitios, ref v.tipoFragmento, ref v.condicion);

                switch (v.tipoFragmento)
                {
                    case "Horizontal":
                        //si es insercion se hace en el sitio de la condicion          

                        break;
                    case "Vertical":

                        break;
                    case "Replica":
                        //insertar en ambos sitios
                        //si es insercion se hace en el sitio de la condicion
                        try
                        {
                            if (v.sitios.ElementAt(0).Contains("1"))
                            {
                                //Insercion en sql server sitio1
                                string consulta = "Insert into " + v.nombreTablaBDFragmento.ElementAt(0).ToString() + " values('" + fechaVenta + "','" + descripcion + "','" + productos + "'," + cantidadProductos + "," + total + ")";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiaCampos();
                                //fin de insercion

                                //Insercion en NPG sitio2
                                NpgsqlCommand command = new NpgsqlCommand("Insert into " + v.nombreTablaBDFragmento.ElementAt(1).ToString() + " (fecha,descripción,productos,cantidad,total) values('" + fechaVenta +"','" + descripcion + "','" + productos + "'," + cantidadProductos + "," + total + ")", conNPG);
                                command.ExecuteNonQuery();

                                eliminaProductoOModifica(listaDeProductoActualizado);
                            }
                            else
                            {
                                //Insercion en sql server sitio1
                                string consulta = "Insert into " + v.nombreTablaBDFragmento.ElementAt(1).ToString() + " values('" + fechaVenta + "','" + descripcion + "','" + productos + "'," + cantidadProductos + "," + total + ")";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiaCampos();
                                //fin de insercion

                                //Insercion en NPG sitio2
                                NpgsqlCommand command = new NpgsqlCommand("Insert into " + v.nombreTablaBDFragmento.ElementAt(0).ToString() + " (fecha,descripción,productos,cantidad,total) values('" + fechaVenta + "','" + descripcion + "','" + productos + "'," + cantidadProductos + "," + total + ")", conNPG);
                                command.ExecuteNonQuery();

                                eliminaProductoOModifica(listaDeProductoActualizado);
                            }



                        }
                        catch (Exception error)
                        {

                            MessageBox.Show("ERROR al insertar : " + error.Message);
                        }

                        break;

                    default:
                        switch (v.sitios.ElementAt(0))
                        {
                            case "1":
                                //Insercion en sql server
                                string consulta = "Insert into " + v.nombreTablaBDFragmento.ElementAt(0).ToString() + " values('" + fechaVenta + "','" + descripcion + "','" + productos + "'," + cantidadProductos + "," + total + ")";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiaCampos();
                                eliminaProductoOModifica(listaDeProductoActualizado);

                                break;


                            case "2":

                                NpgsqlCommand command = new NpgsqlCommand("Insert into " + v.nombreTablaBDFragmento.ElementAt(0).ToString() + " (fecha,descripción,productos,cantidad,total) values('" + fechaVenta + "','" + descripcion + "','" + productos + "'," + cantidadProductos + "," + total + ")", conNPG);
                                command.ExecuteNonQuery();
                                cargaTabla();
                                limpiaCampos();
                                eliminaProductoOModifica(listaDeProductoActualizado);
                                ///fin de actualizacion
                                break;
                        }
                        break;
                }

                CargalistaidProductoNombrePrecioCantidad();
                cargaCOMBOListBoxInventarioProductos();
                total = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("no se inserto" + ex.Message);
            }
        }

        private void eliminaProductoOModifica(List<string> listaDeProductoActualizado)
        {

            //VALIDACION PARA EL PRODUCTO EN SITIOS
            //si se inserto hacer la actualizacion en la tabla productos--------------
            //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
            //variables necesarias para sacar datos del esquema de localizacion
            NpgsqlCommand command;
            VariablesNecesariasParaEsquemaLocalizacion v = new VariablesNecesariasParaEsquemaLocalizacion();
            v.DeclaraVariables();
            v.st.LeeEsquemaLocalizacion("Producto", ref v.idFragmentos, ref v.nombreTablaBDFragmento, ref v.nombreTablaGeneral, ref v.sitios, ref v.tipoFragmento, ref v.condicion);
            switch (v.tipoFragmento)
            {
                case "Horizontal":

                    break;
                case "Vertical":

                    break;
                case "Replica":

                    if (v.sitios.ElementAt(0).Contains("1"))
                    {
                        foreach (string item in listaDeProductoActualizado)
                        {
                            if (item.Split(',').ElementAt(2) != "0")//si aun existen productos
                            {
                                string consultaS = "UPDATE " + v.nombreTablaBDFragmento[0].ToString() + " SET cantidad=" + item.Split(',').ElementAt(2) + "  WHERE id_Producto = " + item.Split(',').ElementAt(0) + "";
                                cmd = new SqlCommand(consultaS, cnSQL);
                                cmd.ExecuteNonQuery();
                                //modificar en postgresql
                                command = new NpgsqlCommand("UPDATE " + v.nombreTablaBDFragmento[1].ToString() + " SET cantidad=" + item.Split(',').ElementAt(2) + "  WHERE id_Producto = " + item.Split(',').ElementAt(0) + "", conNPG);
                                command.ExecuteNonQuery();
                            }
                            else//so ya no existen productos se elimina
                            //se elimina el producto si se vendio toda la cantidad y SE ELIMINA LA OFERTA SI ES QUE EL PRODUCTO TIENE
                            {
                                string consultaS = "DELETE FROM " + v.nombreTablaBDFragmento[0].ToString() + " WHERE id_Producto = " + item.Split(',').ElementAt(0) + "";
                                cmd = new SqlCommand(consultaS, cnSQL);
                                cmd.ExecuteNonQuery();
                                //modificar en postgresql
                                command = new NpgsqlCommand("DELETE FROM " + v.nombreTablaBDFragmento[1].ToString() + " WHERE id_Producto = " + item.Split(',').ElementAt(0) + "", conNPG);
                                command.ExecuteNonQuery();

                                ChecaSiElProductoTieneOferta(item.Split(',').ElementAt(0));//se le manda el id del producto
                            }
                        }

                    }
                    else
                    {


                        foreach (string item in listaDeProductoActualizado)
                        {
                            if (item.Split(',').ElementAt(2) != "0")
                            {
                                string consultaS = "UPDATE " + v.nombreTablaBDFragmento[1].ToString() + " SET cantidad=" + item.Split(',').ElementAt(2) + "  WHERE id_Producto = " + item.Split(',').ElementAt(0) + "";
                                cmd = new SqlCommand(consultaS, cnSQL);
                                cmd.ExecuteNonQuery();
                                //modificar en postgresql
                                command = new NpgsqlCommand("UPDATE " + v.nombreTablaBDFragmento[0].ToString() + " SET cantidad=" + item.Split(',').ElementAt(2) + "  WHERE id_Producto = " + item.Split(',').ElementAt(0) + "", conNPG);
                                command.ExecuteNonQuery();
                            }
                            else
                            //se elimina el producto si se vendio toda la cantidad
                            {
                                string consultaS = "DELETE FROM " + v.nombreTablaBDFragmento[1].ToString() + " WHERE id_Producto = " + item.Split(',').ElementAt(0) + "";
                                cmd = new SqlCommand(consultaS, cnSQL);
                                cmd.ExecuteNonQuery();
                                //modificar en postgresql
                                command = new NpgsqlCommand("DELETE FROM " + v.nombreTablaBDFragmento[0].ToString() + " WHERE id_Producto = " + item.Split(',').ElementAt(0) + "", conNPG);
                                command.ExecuteNonQuery();
                                ChecaSiElProductoTieneOferta(item.Split(',').ElementAt(0));
                            }
                        }


                    }
                    break;

                default:

                    switch (v.sitios.ElementAt(0))
                    {
                        case "1":
                            foreach (string item in listaDeProductoActualizado)
                            {
                                if (item.Split(',').ElementAt(2) != "0")
                                {
                                    string consultaS = "UPDATE " + v.nombreTablaBDFragmento[0].ToString() + " SET cantidad=" + item.Split(',').ElementAt(2) + "  WHERE id_Producto = " + item.Split(',').ElementAt(0) + "";
                                    cmd = new SqlCommand(consultaS, cnSQL);
                                    cmd.ExecuteNonQuery();

                                }
                                else
                                //se elimina el producto si se vendio toda la cantidad
                                {
                                    string consultaS = "DELETE FROM " + v.nombreTablaBDFragmento[0].ToString() + " WHERE id_Producto = " + item.Split(',').ElementAt(0) + "";
                                    cmd = new SqlCommand(consultaS, cnSQL);
                                    cmd.ExecuteNonQuery();
                                    ChecaSiElProductoTieneOferta(item.Split(',').ElementAt(0));
                                }
                            }


                            break;


                        case "2":
                            foreach (string item in listaDeProductoActualizado)
                            {
                                if (item.Split(',').ElementAt(2) != "0")
                                {

                                    //modificar en postgresql
                                    command = new NpgsqlCommand("UPDATE " + v.nombreTablaBDFragmento[0].ToString() + " SET cantidad=" + item.Split(',').ElementAt(2) + "  WHERE id_Producto = " + item.Split(',').ElementAt(0) + "", conNPG);
                                    command.ExecuteNonQuery();
                                }
                                else
                                //se elimina el producto si se vendio toda la cantidad
                                {

                                    //modificar en postgresql
                                    command = new NpgsqlCommand("DELETE FROM " + v.nombreTablaBDFragmento[0].ToString() + " WHERE id_Producto = " + item.Split(',').ElementAt(0) + "", conNPG);
                                    command.ExecuteNonQuery();
                                    ChecaSiElProductoTieneOferta(item.Split(',').ElementAt(0));

                                }
                            }
                            break;
                    }

                    break;
            }
        
        
        }
        private void ChecaSiElProductoTieneOferta(string idProducto)
        {

            //eliminar la oferta si es q tenia producton
            NpgsqlCommand command;
            VariablesNecesariasParaEsquemaLocalizacion v = new VariablesNecesariasParaEsquemaLocalizacion();
            v.DeclaraVariables();
            v.st.LeeEsquemaLocalizacion("Oferta", ref v.idFragmentos, ref v.nombreTablaBDFragmento, ref v.nombreTablaGeneral, ref v.sitios, ref v.tipoFragmento, ref v.condicion);

            switch (v.tipoFragmento)
            {
                case "Horizontal":

                    break;
                case "Vertical":

                    break;
                case "Replica":
                    if (v.sitios.ElementAt(0).Contains("1"))//condicional para replica
                    {
                        //leemos si el id  del producto que se va a eliminar tiene oferta
                        try//normalmente entrara al catch si el producto no tiene oferta
                        {
                            //leemos si el id  del producto que se va a eliminar tiene oferta
                            cmd = new SqlCommand("Select id_Oferta from " + v.nombreTablaBDFragmento.ElementAt(0) + " where id_Producto = '" + idProducto + "'", cnSQL);
                            dr = cmd.ExecuteReader();
                            dr.Read();
                            string idOferta = (dr[0]).ToString();
                            dr.Close();
                            //se hace la eliminacion de la oferta cuyo porducto se vendio todo
                            string consultaS = "DELETE FROM " + v.nombreTablaBDFragmento[1].ToString() + " WHERE id_oferta = " + idOferta + "";
                            cmd = new SqlCommand(consultaS, cnSQL);
                            cmd.ExecuteNonQuery();
                            //modificar en postgresql
                            command = new NpgsqlCommand("DELETE FROM " + v.nombreTablaBDFragmento[0].ToString() + "  WHERE id_oferta = " + idOferta + "", conNPG);
                            command.ExecuteNonQuery();
                
                        }
                        catch (Exception)
                        {
                        }

                    }
                    else//si comienza por el sitio2
                    {
                        //leemos si el id  del producto que se va a eliminar tiene oferta
                        try//normalmente entrara al catch si el producto no tiene oferta
                        {
                            //leemos si el id  del producto que se va a eliminar tiene oferta
                            cmd = new SqlCommand("Select id_Oferta from " + v.nombreTablaBDFragmento.ElementAt(0) + " where id_Producto = '" + idProducto + "'", cnSQL);
                            dr = cmd.ExecuteReader();
                            dr.Read();
                            string idOferta = (dr[0]).ToString();
                            dr.Close();
                            //se hace la eliminacion de la oferta cuyo porducto se vendio todo
                            string consultaS = "DELETE FROM " + v.nombreTablaBDFragmento[0].ToString() + " WHERE id_oferta = " + idOferta + "";
                            cmd = new SqlCommand(consultaS, cnSQL);
                            cmd.ExecuteNonQuery();
                            //modificar en postgresql
                            command = new NpgsqlCommand("DELETE FROM " + v.nombreTablaBDFragmento[1].ToString() + " WHERE id_oferta = " + idOferta + "", conNPG);
                            command.ExecuteNonQuery();

                        }
                        catch (Exception)
                        {
                        }
                    }

                    break;
                default://si no es de ningun tipo de fragmento
                    switch (v.sitios.ElementAt(0))
                    {
                        case "1"://leemos si el id  del producto que se va a eliminar tiene oferta
                            //leemos si el id  del producto que se va a eliminar tiene oferta
                            try//normalmente entrara al catch si el producto no tiene oferta
                            {
                                //leemos si el id  del producto que se va a eliminar tiene oferta
                                cmd = new SqlCommand("Select id_Oferta from " + v.nombreTablaBDFragmento.ElementAt(0) + " where id_Producto = '" + idProducto + "'", cnSQL);
                                dr = cmd.ExecuteReader();
                                dr.Read();
                                string idOferta = (dr[0]).ToString();
                                dr.Close();
                                //se hace la eliminacion de la oferta cuyo porducto se vendio todo
                                string consultaS = "DELETE FROM " + v.nombreTablaBDFragmento[0].ToString() + " WHERE id_oferta = " + idOferta + "";
                                cmd = new SqlCommand(consultaS, cnSQL);
                                cmd.ExecuteNonQuery();
                             
                            }
                            catch (Exception)
                            {
                            }
                            break;

                        case "2":
                            //leemos si el id  del producto que se va a eliminar tiene oferta
                            try//normalmente entrara al catch si el producto no tiene oferta
                            {
                                //leemos si el id  del producto que se va a eliminar tiene oferta
                                cmd = new SqlCommand("Select id_Oferta from " + v.nombreTablaBDFragmento.ElementAt(0) + " where id_Producto = '" + idProducto + "'", cnSQL);
                                dr = cmd.ExecuteReader();
                                dr.Read();
                                string idOferta = (dr[0]).ToString();
                                dr.Close();
                                //se hace la eliminacion de la oferta cuyo porducto se vendio todo
                                command = new NpgsqlCommand("DELETE FROM " + v.nombreTablaBDFragmento[0].ToString() + " WHERE id_oferta = " + idOferta + "", conNPG);
                                command.ExecuteNonQuery();

                            }
                            catch (Exception)
                            {
                            }
                            break;
                    }
                    break;
            }                        
        }

        private void dataGridViewVenta_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                banderaNoPermiteModificarListBox = true;
                if (banderaNoPermiteModificarListBox)
                    BtnInsertar.Enabled = false;
                limpiaCampos();
                dateTimePickerFecha.Text = dataGridViewVenta.CurrentRow.Cells[1].Value.ToString();
                textBoxDescripcionVenta.Text = dataGridViewVenta.CurrentRow.Cells[2].Value.ToString();
                string[] productos = dataGridViewVenta.CurrentRow.Cells[3].Value.ToString().Split(',');
                foreach (string item in productos)
                {
                    listBoxCarrito.Items.Add(item);
                }
                string temporalTotal = richTextBoxTotal.Text = dataGridViewVenta.CurrentRow.Cells[5].Value.ToString();
                richTextBoxTotal.Text = richTextBoxTotal.Text.Insert(0, "$");
                total = decimal.Parse(temporalTotal);
                

            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error en el datagrid cellclick" + ex);
                BtnInsertar.Enabled = true ;
                banderaNoPermiteModificarListBox = false;
                CargalistaidProductoNombrePrecioCantidad();
                cargaCOMBOListBoxInventarioProductos();
                total = 0;
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (!validaDatosEntrada())
                return;

            try
            {
                banderaNoPermiteModificarListBox = false;
                //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
                //variables necesarias para sacar datos del esquema de localizacion
                VariablesNecesariasParaEsquemaLocalizacion v = new VariablesNecesariasParaEsquemaLocalizacion();
                v.DeclaraVariables();
                v.st.LeeEsquemaLocalizacion("Venta", ref v.idFragmentos, ref v.nombreTablaBDFragmento, ref v.nombreTablaGeneral, ref v.sitios, ref v.tipoFragmento, ref v.condicion);

                switch (v.tipoFragmento)
                {
                    case "Horizontal":

                        break;
                    case "Vertical":
                        break;
                    case "Replica":
                        //Eliminar en ambos sitios
                        try
                        {
                            if (v.sitios.ElementAt(0).Contains("1"))
                            {
                                //eliminar en sql server
                                string consulta = "DELETE FROM " + v.nombreTablaBDFragmento.ElementAt(0).ToString() + " WHERE id_Venta = " + dataGridViewVenta.CurrentRow.Cells[0].Value.ToString() + "";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                //eliminar en postgressql
                                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM " + v.nombreTablaBDFragmento.ElementAt(1).ToString() + " WHERE id_Venta = " + dataGridViewVenta.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                command.ExecuteNonQuery();
                                
                            }
                            else
                            {

                                //eliminar en sql server
                                string consulta = "DELETE FROM " + v.nombreTablaBDFragmento.ElementAt(1).ToString() + " WHERE id_Venta = " + dataGridViewVenta.CurrentRow.Cells[0].Value.ToString() + "";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                //eliminar en postgressql
                                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM " + v.nombreTablaBDFragmento.ElementAt(0).ToString() + " WHERE id_Venta = " + dataGridViewVenta.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                command.ExecuteNonQuery();
                         
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                        break;

                    default:
                        switch (v.sitios.ElementAt(0))
                        {
                            case "1":
                                //Insercion en sql server
                                string consulta = "DELETE FROM " + v.nombreTablaBDFragmento.ElementAt(0).ToString() + " WHERE id_Venta = " + dataGridViewVenta.CurrentRow.Cells[0].Value.ToString() + "";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                        

                                break;


                            case "2":

                                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM " + v.nombreTablaBDFragmento.ElementAt(0).ToString() + " WHERE id_Venta = " + dataGridViewVenta.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                command.ExecuteNonQuery();
                           

                                break;
                        }
                        break;
                }
         
                    cargaTabla();
                    limpiaCampos();
      
                BtnInsertar.Enabled = true;
                CargalistaidProductoNombrePrecioCantidad();
                cargaCOMBOListBoxInventarioProductos();
                total = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("no se inserto" + ex.Message);
            }
        }

        private void BtnModificar_Click(object sender, EventArgs e)
        {

            banderaNoPermiteModificarListBox = false;
            if (!validaDatosEntrada())
                return;

            try
            {

                string fecha = dateTimePickerFecha.Value.ToString("yyyy-MM-dd");
                string descripcion = textBoxDescripcionVenta.Text;

                //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
                //variables necesarias para sacar datos del esquema de localizacion
                VariablesNecesariasParaEsquemaLocalizacion v = new VariablesNecesariasParaEsquemaLocalizacion();
                v.DeclaraVariables();
                v.st.LeeEsquemaLocalizacion("Venta", ref v.idFragmentos, ref v.nombreTablaBDFragmento, ref v.nombreTablaGeneral, ref v.sitios, ref v.tipoFragmento, ref v.condicion);

                switch (v.tipoFragmento)
                {
                    case "Horizontal":
                        //si es insercion se hace en el sitio de la condicion          

                        break;
                    case "Vertical":

                        break;
                    case "Replica":
                        //modifcar en ambos sitios

                        if (v.sitios.ElementAt(0).Contains("1"))
                        {
                            //MODIFICACION en sql server
                            string consulta = "UPDATE " + v.nombreTablaBDFragmento.ElementAt(0).ToString() + " SET descripción = '" + descripcion + "',fecha ='" + fecha + "' WHERE id_Venta = " + dataGridViewVenta.CurrentRow.Cells[0].Value.ToString() + "";
                            cmd = new SqlCommand(consulta, cnSQL);
                            cmd.ExecuteNonQuery();
                            //modificar en postgresql
                            NpgsqlCommand command = new NpgsqlCommand("UPDATE " + v.nombreTablaBDFragmento.ElementAt(1).ToString() + " SET descripción = '" + descripcion + "',fecha ='" + fecha + "' WHERE id_Venta = " + dataGridViewVenta.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                            command.ExecuteNonQuery();
                            cargaTabla();
                            limpiaCampos();
                        }
                        else
                        {
                            //MODIFICACION en sql server
                            string consulta = "UPDATE " + v.nombreTablaBDFragmento.ElementAt(1).ToString() + " SET descripción = '" + descripcion + "',fecha ='" + fecha + "' WHERE id_Venta = " + dataGridViewVenta.CurrentRow.Cells[0].Value.ToString() + "";
                            cmd = new SqlCommand(consulta, cnSQL);
                            cmd.ExecuteNonQuery();
                            //modificar en postgresql
                            NpgsqlCommand command = new NpgsqlCommand("UPDATE " + v.nombreTablaBDFragmento.ElementAt(0).ToString() + " SET descripción = '" + descripcion + "',fecha ='" + fecha + "' WHERE id_Venta = " + dataGridViewVenta.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                            command.ExecuteNonQuery();
                            cargaTabla();
                            limpiaCampos();
                        }
                        break;


                    default:
                        switch (v.sitios.ElementAt(0))
                        {
                            case "1":
                                //Insercion en sql server
                                string consultaS = "UPDATE " + v.nombreTablaBDFragmento.ElementAt(0).ToString() + " SET descripción = '" + descripcion + "',fecha ='" + fecha + "' WHERE id_Venta = " + dataGridViewVenta.CurrentRow.Cells[0].Value.ToString() + "";
                                cmd = new SqlCommand(consultaS, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiaCampos();

                                break;


                            case "2":

                                NpgsqlCommand commandS = new NpgsqlCommand("UPDATE " + v.nombreTablaBDFragmento.ElementAt(0).ToString() + " SET descripción = '" + descripcion + "',fecha ='" + fecha + "' WHERE id_Venta = " + dataGridViewVenta.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                commandS.ExecuteNonQuery();
                                cargaTabla();
                                limpiaCampos();

                                break;
                        }
                        break;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("no se modifico" + ex.Message);
            }
            BtnInsertar.Enabled = true;
            CargalistaidProductoNombrePrecioCantidad();
            cargaCOMBOListBoxInventarioProductos();
            total = 0;
        }

        
    }
}
