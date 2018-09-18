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
    public partial class Producto : Form
    {
        //lista que contiene el id_proveedor y nombre
        List<string> listaidProvyNombre = new List<string>();
        List<int> listPrimaryKeysProducto;

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

        public Producto()
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
            //carga una lista con id y nombre de los productos
            SqlDataAdapter daProveedor = new SqlDataAdapter("Select * from " + nombreTablaBDFragmento.ElementAt(0) + "", cnSQL);
            DataTable dtProveedor = new DataTable();
            daProveedor.Fill(dtProveedor);
            // For each row, print the values of each column.
            foreach (DataRow row in dtProveedor.Rows)
            {
                listaidProvyNombre.Add(row[dtProveedor.Columns[0]].ToString() + "," + row[dtProveedor.Columns[1]].ToString());
            }
            

            cargaTabla();
        }

        private void Producto_Load(object sender, EventArgs e)
        {

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
        public void MezclaBDReplica(List<string> nombreTablaBDFragmento)
        {

            //codigo para hacer el select al sitio1
            da = new SqlDataAdapter("Select * from " + nombreTablaBDFragmento .ElementAt(0)+ "", cnSQL);
            dt = new DataTable();
            da.Fill(dt);
            //dt.Columns.RemoveAt(0);//Para quitar el campo RFC
            dataGridViewProducto.DataSource = dt;       
            //para esconder el campor RFC
            dataGridViewProducto.Columns[0].Visible = false;
            //para esconder el campo id venta
            dataGridViewProducto.Columns[6].Visible = false;
        }

        private void dataGridViewProducto_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                textBoxNombre.Text = dataGridViewProducto.CurrentRow.Cells[1].Value.ToString();
                numericPrecio.Value = Convert.ToInt32(dataGridViewProducto.CurrentRow.Cells[2].Value);
                dateTimePickerCaducidad.Text = dataGridViewProducto.CurrentRow.Cells[3].Value.ToString();
                numericCantidad.Value = Convert.ToInt32(dataGridViewProducto.CurrentRow.Cells[4].Value);
               // comboBoxidProveedor.Text = dataGridViewProducto.CurrentRow.Cells[5].Value.ToString();
                
                foreach (var item in listaidProvyNombre)
                {
                    if (item.Contains(dataGridViewProducto.CurrentRow.Cells[5].Value.ToString()))
                    {
                        string[] temp = item.Split(',');

                        comboBoxidProveedor.Text = temp.ElementAt(1);
                    }
                }
            }
            catch (System.Exception er)
            {

                MessageBox.Show("error en datagrid" + er);
            }

        }

        private bool validaDatos() 
        {
            if (textBoxNombre.Text == "" || comboBoxidProveedor.Text == "" || numericCantidad.Value <= 0 || numericCantidad.Value <= 0 )
            {
                MessageBox.Show("Error de datos");
                return false;
            }                     
            return true;        
        }

        private bool ChecaExistenciaProducto() {
            try
            {
                string producto = textBoxNombre.Text;
                string renglon = "";
                foreach (DataGridViewRow row in dataGridViewProducto.Rows)
                {
                    renglon = row.Cells["Nombre"].Value.ToString();
                    if (string.Equals(renglon, producto, StringComparison.CurrentCultureIgnoreCase))//Para comparar cadenas con caso insensitive
                    {
                        return true;
                    }

                }
            }
            catch (Exception)
            {
                
              
            }
            


            return false;
        }

        private void BtnInsertar_Click(object sender, System.EventArgs e)
        {
            if (!validaDatos())
                return;

            //CHECAR SI EL PRODUCTo YA EXISTE
            if (ChecaExistenciaProducto())//si existe manda mensaje de error y se sale de la funcion
            {
                MessageBox.Show("Error , Producto ya en existencía");
                return;
            }

            try
            {
                string fecha_caducidad = dateTimePickerCaducidad.Value.ToString("yyyy-MM-dd");
                string nombre = textBoxNombre.Text;
                var arreglo = new string[3];
                foreach (var renglon in listaidProvyNombre)
                {
                    if (renglon.Contains(comboBoxidProveedor.Text))
                    {
                        arreglo = renglon.Split(',');
                        break;
                    }
                }
                int idproveedor = Int32.Parse( (arreglo[0].ToString()));
                decimal precio = numericPrecio.Value;
                decimal cantidaad = numericCantidad.Value;               
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
                    case "Replica":

                        //si es insercion se hace en el sitio de la condicion
                        try
                        {
                            if(sitios.ElementAt(0).Contains("1"))
                            {  
                                //Insercion en sql server sitio1
                                string consulta = "Insert into " + nombreTablaBDFragmento[0].ToString() + " values('" + nombre + "'," + precio + ",'" + fecha_caducidad + "'," + cantidaad + ",'" + idproveedor + "','" + -1 + "')";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiarCampos();
                                //fin de insercion

                                //Insercion en NPG sitio2
                                NpgsqlCommand command = new NpgsqlCommand("Insert into " + nombreTablaBDFragmento[1].ToString() + "(nombre,precio,caducidad,cantidad,id_Proveedor,id_Venta) values('" + nombre + "'," + precio + ",'" + fecha_caducidad + "'," + cantidaad + ",'" + idproveedor + "','" + -1 + "')", conNPG);
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                //Insercion en sql server sitio1
                                string consulta = "Insert into " + nombreTablaBDFragmento[1].ToString() + " values('" + nombre + "'," + precio + ",'" + fecha_caducidad + "'," + cantidaad + ",'" + idproveedor + "','" + -1 + "')";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiarCampos();
                                //fin de insercion

                                //Insercion en NPG sitio2
                                NpgsqlCommand command = new NpgsqlCommand("Insert into " + nombreTablaBDFragmento[0].ToString() + "(nombre,precio,caducidad,cantidad,id_Proveedor,id_Venta) values('" + nombre + "'," + precio + ",'" + fecha_caducidad + "'," + cantidaad + ",'" + idproveedor + "','" + -1 + "')", conNPG);
                                command.ExecuteNonQuery();
                            }

                           

                        }
                        catch (Exception error)
                        {

                            MessageBox.Show("ERROR al insertar en NPG: " + error.Message);
                        }


                        //fin de insercion
                        break;
                    default:
                        switch (sitios.ElementAt(0))
	                    {
		                    case"1":
                                //Insercion en sql server sitio1
                                string consulta = "Insert into " + nombreTablaBDFragmento[0].ToString() + " values('" + nombre + "'," + precio + ",'" + fecha_caducidad + "'," + cantidaad + ",'" + idproveedor + "','" + -1 + "')";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiarCampos();
                                //fin de insercion

                                break;


                            case"2":
                                //Insercion en NPG sitio2
                                NpgsqlCommand command = new NpgsqlCommand("Insert into " + nombreTablaBDFragmento[0].ToString() + "(nombre,precio,caducidad,cantidad,id_Proveedor,id_Venta) values('" + nombre + "'," + precio + ",'" + fecha_caducidad + "'," + cantidaad + ",'" + idproveedor + "','" + -1 + "')", conNPG);
                                command.ExecuteNonQuery();
                                break;
	                    }
                        break;

                } 



            }
            catch (Exception ex)
            {
               MessageBox.Show("no se inserto" + ex.Message);
            }
            cargaTabla();
            limpiarCampos();
        }
        void limpiarCampos() {

            comboBoxidProveedor.Text = "";
            textBoxNombre.Text = "";
            numericPrecio.Value = 0;
            numericCantidad.Value = 0;
            dateTimePickerCaducidad.Value = DateTime.Now;
            
        
        }

        private void comboBoxidProveedor_Enter(object sender, System.EventArgs e)
        {
            comboBoxidProveedor.Items.Clear();
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

            switch (sitios.ElementAt(0))//por si esta en el sitio 1 o 2
            {
                case "1":

                    //agregar campos
                    SqlDataAdapter daProveedor = new SqlDataAdapter("Select nombre from " + nombreTablaBDFragmento.ElementAt(0) + "", cnSQL);
                    DataTable dtProveedor = new DataTable();
                    daProveedor.Fill(dtProveedor);
                    string dato = "";
                    // For each row, print the values of each column.
                    foreach (DataRow row in dtProveedor.Rows)
                    {
                        dato = row[dtProveedor.Columns[0]].ToString();//para todas las rows dame el dato de la columna posicion 1 osea el nombre de la sucursal
                        /* foreach (DataColumn column in dtSucursal.Columns)
                         {
                            ITERA TODOS LOS CAMPOS DE LAS COLUMNAS
                         }*/
                        comboBoxidProveedor.Items.Add(dato);//agrega como campo el nombre de la sucursal
                    }

                    break;

                case "2":
                    //agregar campos
                    NpgsqlDataAdapter daProveedorNP = new NpgsqlDataAdapter("Select nombre from " + nombreTablaBDFragmento.ElementAt(0) + "", conNPG);
                    DataTable dtProveedorNP = new DataTable();
                    daProveedorNP.Fill(dtProveedorNP);
                    string dat = "";
                    // For each row, print the values of each column.
                    foreach (DataRow row in dtProveedorNP.Rows)
                    {
                        dat = row[dtProveedorNP.Columns[0]].ToString();//para todas las rows dame el dato de la columna posicion 1 osea el nombre de la sucursal
                        /* foreach (DataColumn column in dtSucursal.Columns)
                         {
                            ITERA TODOS LOS CAMPOS DE LAS COLUMNAS
                         }*/
                        comboBoxidProveedor.Items.Add(dat);//agrega como campo el nombre de la sucursal
                    }

                    break;

                default:
                    break;

            }
        }

        private void BtnEliminar_Click(object sender, System.EventArgs e)
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
                    case "Horizontal":

                        break;
                    case "Vertical":
                       



                        break;
                    case "Replica":

                        //Eliminar en ambos sitios
                        try
                        {
                            //eliminar en sql server
                            if (sitios.ElementAt(0).Contains("1"))
                            {
                                string consulta = "DELETE FROM " + nombreTablaBDFragmento[0].ToString() + " WHERE id_Producto = " + dataGridViewProducto.CurrentRow.Cells[0].Value.ToString() + "";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                //eliminar en postgressql
                                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM " + nombreTablaBDFragmento[1].ToString() + " WHERE id_Producto = " + dataGridViewProducto.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                command.ExecuteNonQuery();
                                cargaTabla();
                                limpiarCampos();
                            }
                            else
                            {

                                string consulta = "DELETE FROM " + nombreTablaBDFragmento[1].ToString() + " WHERE id_Producto = " + dataGridViewProducto.CurrentRow.Cells[0].Value.ToString() + "";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                //eliminar en postgressql
                                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM " + nombreTablaBDFragmento[0].ToString() + " WHERE id_Producto = " + dataGridViewProducto.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                command.ExecuteNonQuery();
                                cargaTabla();
                                limpiarCampos();
                            
                            
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                        break;

                    default:

                        switch (sitios.ElementAt(0))
                        {
                            case "1":
                                //Insercion en sql server
                                string consulta = "DELETE FROM " + nombreTablaBDFragmento[0].ToString() + " WHERE id_Producto = " + dataGridViewProducto.CurrentRow.Cells[0].Value.ToString() + "";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiarCampos();

                                break;


                            case "2":

                                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM " + nombreTablaBDFragmento[1].ToString() + " WHERE id_Producto = " + dataGridViewProducto.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                command.ExecuteNonQuery();
                                cargaTabla();
                                limpiarCampos();

                                break;
                        }

                        break;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("no se elimino" + ex.Message);
            }
        }

        private void BtnModificar_Click(object sender, System.EventArgs e)
        {
            if (!validaDatos())
                return;
            try
            {


                string nombre = textBoxNombre.Text;
                string fecha_caducidad = dateTimePickerCaducidad.Value.ToString("yyyy-MM-dd");
                decimal precio = numericPrecio.Value;
                decimal cantidad = numericCantidad.Value;
                var idProveedor = dataGridViewProducto.CurrentRow.Cells[5].Value;

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
                    case "Horizontal":

                        break;
                    case "Vertical":
                 
                        break;
                    case "Replica":

                        if (sitios.ElementAt(0).Contains("1"))
                        {

                            //MODIFICACION en sql server
                            string consulta = "UPDATE " + nombreTablaBDFragmento[0].ToString() + " SET nombre = '" + nombre + "',precio=" + precio + ",caducidad='" + fecha_caducidad + "',cantidad=" + cantidad + ",id_Proveedor = " + idProveedor + "  WHERE id_Producto = " + dataGridViewProducto.CurrentRow.Cells[0].Value.ToString() + "";
                            cmd = new SqlCommand(consulta, cnSQL);
                            cmd.ExecuteNonQuery();
                            //modificar en postgresql
                            NpgsqlCommand command = new NpgsqlCommand("UPDATE " + nombreTablaBDFragmento[1].ToString() + " SET nombre = '" + nombre + "',precio=" + precio + ",caducidad='" + fecha_caducidad + "',cantidad=" + cantidad + ",id_Proveedor =" + idProveedor + "  WHERE id_Producto = " + dataGridViewProducto.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                            command.ExecuteNonQuery();
                            cargaTabla();
                            limpiarCampos();
                        }
                        else
                        {

                            //MODIFICACION en sql server
                            string consulta = "UPDATE " + nombreTablaBDFragmento[1].ToString() + " SET nombre = '" + nombre + "',precio=" + precio + ",caducidad='" + fecha_caducidad + "',cantidad=" + cantidad + ",id_Proveedor = " + idProveedor + "  WHERE id_Producto = " + dataGridViewProducto.CurrentRow.Cells[0].Value.ToString() + "";
                            cmd = new SqlCommand(consulta, cnSQL);
                            cmd.ExecuteNonQuery();
                            //modificar en postgresql
                            NpgsqlCommand command = new NpgsqlCommand("UPDATE " + nombreTablaBDFragmento[0].ToString() + " SET nombre = '" + nombre + "',precio=" + precio + ",caducidad='" + fecha_caducidad + "',cantidad=" + cantidad + ",id_Proveedor =" + idProveedor + "  WHERE id_Producto = " + dataGridViewProducto.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                            command.ExecuteNonQuery();
                            cargaTabla();
                            limpiarCampos();


                        }
                        break;

                    default:

                        switch (sitios.ElementAt(0))
                        {
                            case "1":
                                //Insercion en sql server
                                string consultaS = "UPDATE " + nombreTablaBDFragmento[0].ToString() + " SET nombre = '" + nombre + "',precio=" + precio + ",caducidad='" + fecha_caducidad + "',cantidad=" + cantidad + ",id_Proveedor = " + idProveedor + "  WHERE id_Producto = " + dataGridViewProducto.CurrentRow.Cells[0].Value.ToString() + "";
                                cmd = new SqlCommand(consultaS, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiarCampos();

                                break;


                            case "2":

                                NpgsqlCommand commandS = new NpgsqlCommand("UPDATE " + nombreTablaBDFragmento[1].ToString() + " SET nombre = '" + nombre + "',precio=" + precio + ",caducidad='" + fecha_caducidad + "',cantidad=" + cantidad + ",id_Proveedor =" + idProveedor + "  WHERE id_Producto = " + dataGridViewProducto.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                commandS.ExecuteNonQuery();
                                cargaTabla();
                                limpiarCampos();

                                break;
                        }

                        break;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("no se modifico" + ex.Message);
            }

        }
    }
}
