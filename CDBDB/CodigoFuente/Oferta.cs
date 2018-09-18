using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class Oferta : Form
    {
        List<string> ListaIdNombre;
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
        public Oferta()
        {
            InitializeComponent();
            ListaIdNombre = new List<string>(); 
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
            st.LeeEsquemaLocalizacion("Producto", ref idFragmentos, ref nombreTablaBDFragmento, ref nombreTablaGeneral, ref sitios, ref tipoFragmento, ref condicion);
            //carga una lista con id y nombre de los productos
            SqlDataAdapter daProducto = new SqlDataAdapter("Select * from " + nombreTablaBDFragmento.ElementAt(0) + "", cnSQL);
            DataTable dtProducto = new DataTable();
            daProducto.Fill(dtProducto);
            // For each row, print the values of each column.
            foreach (DataRow row in dtProducto.Rows)
            {               
                ListaIdNombre.Add(row[dtProducto.Columns[0]].ToString() + "," + row[dtProducto.Columns[1]].ToString());                
            }

            cargaTabla();

        }

        private void Oferta_Load(object sender, EventArgs e)
        {

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
                st.LeeEsquemaLocalizacion("Oferta", ref idFragmentos, ref nombreTablaBDFragmento, ref nombreTablaGeneral, ref sitios, ref tipoFragmento, ref condicion);
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
                    //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
                    //variables necesarias para sacar datos del esquema de localizacion
                    List<string> idFragmentos = new List<string>();
                    List<string> nombreTablaBDFragmentoProd = new List<string>();
                    string nombreTablaGeneral = "";
                    string tipoFragmento = "";
                    List<string> sitiosprod = new List<string>();
                    List<string> condicion = new List<string>();
                    SitioCentral st = new SitioCentral();
                    st.LeeEsquemaLocalizacion("Producto", ref idFragmentos, ref nombreTablaBDFragmentoProd, ref nombreTablaGeneral, ref sitiosprod, ref tipoFragmento, ref condicion);
                    //codigo para hacer el select al sitio1
                    string query ="select id_Oferta , descripcion , vigencia , descuento, Prod.nombre, cantidad from " + nombreTablaBDFragmento.ElementAt(0) + " as OFE," + nombreTablaBDFragmentoProd[0] + " as Prod where OFE.id_Producto= Prod.id_Producto ";
                    da = new SqlDataAdapter(query, cnSQL);          
                    dt = new DataTable();
                    da.Fill(dt);
                    //dt.Columns.RemoveAt(0);//Para quitar el campo RFC
                    dataGridViewOferta.DataSource = dt;
                    //para esconder el campor id
                    dataGridViewOferta.Columns[0].Visible = false;

                    break;


                case "2":
                    //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
                    //variables necesarias para sacar datos del esquema de localizacion
                    idFragmentos = new List<string>();
                     nombreTablaBDFragmentoProd = new List<string>();
                    nombreTablaGeneral = "";
                     tipoFragmento = "";
                    sitiosprod = new List<string>();
                     condicion = new List<string>();
                     st = new SitioCentral();
                    st.LeeEsquemaLocalizacion("Producto", ref idFragmentos, ref nombreTablaBDFragmentoProd, ref nombreTablaGeneral, ref sitiosprod, ref tipoFragmento, ref condicion);
                   
                    //Codigo para hacer el select from a al sitio2             
                    NpgsqlDataAdapter add = new NpgsqlDataAdapter("select id_Oferta , descripcion , vigencia , descuento, Prod.nombre, cantidad from " + nombreTablaBDFragmento.ElementAt(0) + " as OFE," + nombreTablaBDFragmentoProd[0] + " as Prod where OFE.id_Producto= Prod.id_Producto ", conNPG);
                    dtNPG = new DataTable();
                    add.Fill(dtNPG);
                    dataGridViewOferta.DataSource = dtNPG;
                    //para esconder el campor id
                    dataGridViewOferta.Columns[0].Visible = false;
                    break;
            }

        }

        private void MezclaBDReplica(List<string> nombreTablaBDFragmento)
        {
            //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
                //variables necesarias para sacar datos del esquema de localizacion
                List<string> idFragmentos = new List<string>();
                List<string> nombreTablaBDFragmentoProd = new List<string>();
                string nombreTablaGeneral = "";
                string tipoFragmento = "";
                List<string> sitios = new List<string>();
                List<string> condicion = new List<string>();
                SitioCentral st = new SitioCentral();
                st.LeeEsquemaLocalizacion("Producto", ref idFragmentos, ref nombreTablaBDFragmentoProd, ref nombreTablaGeneral, ref sitios, ref tipoFragmento, ref condicion);
                //codigo para hacer el select al sitio1
                string query = "select id_Oferta , descripcion , vigencia , descuento, Prod.nombre, cantidad from " + nombreTablaBDFragmento.ElementAt(0) + " as OFE," + nombreTablaBDFragmentoProd[0] + " as Prod where OFE.id_Producto= Prod.id_Producto ";
                da = new SqlDataAdapter(query, cnSQL);
                dt = new DataTable();
                da.Fill(dt);
                //dt.Columns.RemoveAt(0);//Para quitar el campo RFC
                dataGridViewOferta.DataSource = dt;
                //para esconder el campor RFC
                dataGridViewOferta.Columns[0].Visible = false;
        }

        private void comboBoxidProducto_Enter(object sender, System.EventArgs e)
        {
            comboBoxidProducto.Items.Clear();
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

            switch (sitios.ElementAt(0))//por si esta en el sitio 1 o 2
            {
                case "1":

                    //agregar campos
                    SqlDataAdapter daProducto = new SqlDataAdapter("Select Nombre from " + nombreTablaBDFragmento.ElementAt(0) + "", cnSQL);
                    DataTable dtProducto = new DataTable();
                    daProducto.Fill(dtProducto);
                    string dato = "";
                    // For each row, print the values of each column.
                    foreach (DataRow row in dtProducto.Rows)
                    {
                        dato = row[dtProducto.Columns[0]].ToString();//para todas las rows dame el dato de la columna posicion 1 osea el nombre de la sucursal                       
                        /* foreach (DataColumn column in dtSucursal.Columns)
                         {
                            ITERA TODOS LOS CAMPOS DE LAS COLUMNAS
                         }*/
                        comboBoxidProducto.Items.Add(dato);//agrega como campo el nombre de la sucursal
                    }

                    break;

                case "2":
                    //agregar campos
                    NpgsqlDataAdapter daProductoNP = new NpgsqlDataAdapter("Select nombre from " + nombreTablaBDFragmento.ElementAt(0) + "", conNPG);
                    DataTable dtProductoNP = new DataTable();
                    daProductoNP.Fill(dtProductoNP);
                    string dat = "";
                    // For each row, print the values of each column.
                    foreach (DataRow row in dtProductoNP.Rows)
                    {
                        dat = row[dtProductoNP.Columns[0]].ToString();//para todas las rows dame el dato de la columna posicion 1 osea el nombre de la sucursal
                        /* foreach (DataColumn column in dtSucursal.Columns)
                         {
                            ITERA TODOS LOS CAMPOS DE LAS COLUMNAS
                         }*/
                        comboBoxidProducto.Items.Add(dat);//agrega como campo el nombre de la sucursal
                    }

                    break;

                default:
                    break;

            }
        }
        void limpiarCampos()
        {
            textBoxDescripcion.Text = "";
            numericDescuento.Value = 0;
            comboBoxidProducto.Text = "";
            dateTimePickerVigencia.Value = DateTime.Now;
        }

        private bool validaDatos()
        {
            if (textBoxDescripcion.Text == ""  || numericDescuento.Value <= 0)
            {
                MessageBox.Show("Error de datos");
                return false;
            }
            return true;
        }

        private void dataGridViewOferta_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                textBoxDescripcion.Text = dataGridViewOferta.CurrentRow.Cells[1].Value.ToString();
                dateTimePickerVigencia.Text = dataGridViewOferta.CurrentRow.Cells[2].Value.ToString();
                numericDescuento.Value = Convert.ToInt32(dataGridViewOferta.CurrentRow.Cells[3].Value);
                foreach (var item in ListaIdNombre)
                {
                    if(item.Contains(dataGridViewOferta.CurrentRow.Cells[4].Value.ToString()))
                    {
                        string[] temp = item.Split(',');

                        comboBoxidProducto.Text = temp.ElementAt(1);
                    }
                }
               
            }
            catch (System.Exception er)
            {

                MessageBox.Show("error en datagrid" + er);
            }
        }

        private void BtnInsertar_Click(object sender, System.EventArgs e)
        {
            if (!validaDatos())
                return;

            try
            {

                string Vigencia = dateTimePickerVigencia.Value.ToString("yyyy-MM-dd");                
                string descripcion = textBoxDescripcion.Text;
                string idProducto = "";
                foreach (var item in ListaIdNombre)
	            {
		                if(item.Contains(comboBoxidProducto.Text))
                        {
                            string[] cadenasplit = item.Split(',');
                            idProducto = cadenasplit.ElementAt(0).ToString();
                        }
	            } 
                
                decimal Descuento = numericDescuento.Value;

                //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
                //variables necesarias para sacar datos del esquema de localizacion
                List<string> idFragmentos = new List<string>();
                List<string> nombreTablaBDFragmento = new List<string>();
                string nombreTablaGeneral = "";
                string tipoFragmento = "";
                List<string> sitios = new List<string>();
                List<string> condicion = new List<string>();
                SitioCentral st = new SitioCentral();
                st.LeeEsquemaLocalizacion("Oferta", ref idFragmentos, ref nombreTablaBDFragmento, ref nombreTablaGeneral, ref sitios, ref tipoFragmento, ref condicion);

                switch (tipoFragmento)
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
                            if (sitios.ElementAt(0).Contains("1"))
                            {
                                //Insercion en sql server sitio1
                                string consulta = "Insert into " + nombreTablaBDFragmento.ElementAt(0).ToString() + " values('" + descripcion + "','" + Vigencia + "'," + Descuento+ ",'" + Convert.ToInt32(idProducto) + "')";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();

                                //fin de insercion

                                //Insercion en NPG sitio2
                                NpgsqlCommand command = new NpgsqlCommand("Insert into " + nombreTablaBDFragmento.ElementAt(1).ToString() + "(descripcion,vigencia,descuneto,id_Producto)  values('" + descripcion + "','" + Vigencia + "'," + Descuento + ",'" + Convert.ToInt32(idProducto) + "')", conNPG);
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                //Insercion en sql server sitio1
                                string consulta = "Insert into " + nombreTablaBDFragmento.ElementAt(1).ToString() + " values('" + descripcion + "','" + Vigencia + "'," + Descuento + ",'" + Convert.ToInt32(idProducto) + "')";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiarCampos();
                                //fin de insercion

                                //Insercion en NPG sitio2
                                NpgsqlCommand command = new NpgsqlCommand("Insert into " + nombreTablaBDFragmento.ElementAt(0).ToString() + "(descripcion,vigencia,descuneto,id_Producto)  values('" + descripcion + "','" + Vigencia + "'," + Descuento + ",'" + Convert.ToInt32(idProducto) + "')", conNPG);
                                command.ExecuteNonQuery();
                            }



                        }
                        catch (Exception error)
                        {

                            MessageBox.Show("ERROR al insertar : " + error.Message);
                        }

                        break;

                    default:
                        switch (sitios.ElementAt(0))
                        {
                            case "1":
                                //Insercion en sql server
                                string consulta = "Insert into " + nombreTablaBDFragmento.ElementAt(0).ToString() + " values('" + descripcion + "','" + Vigencia + "'," + Descuento + "," + idProducto + ")";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiarCampos();

                                break;


                            case "2":

                                NpgsqlCommand command = new NpgsqlCommand("Insert into " + nombreTablaBDFragmento.ElementAt(0).ToString() + "(descripcion,vigencia,descuneto,id_Producto)  values('" + descripcion + "','" + Vigencia + "'," + Descuento + "," + idProducto + ")", conNPG);
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
                Console.WriteLine("no se inserto" + ex.Message);
            }
        }

        private void BtnModificar_Click(object sender, System.EventArgs e)
        {
            if (!validaDatos())
                return;

            try
            {

                string Vigencia = dateTimePickerVigencia.Value.ToString("yyyy-MM-dd");
                string descripcion = textBoxDescripcion.Text;
                string idProducto = "";
                foreach (var item in ListaIdNombre)
                {
                    if (item.Contains(comboBoxidProducto.Text))
                    {
                        idProducto = item.ElementAt(0).ToString();
                    }
                }

                decimal Descuento = numericDescuento.Value;

                //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
                //variables necesarias para sacar datos del esquema de localizacion
                List<string> idFragmentos = new List<string>();
                List<string> nombreTablaBDFragmento = new List<string>();
                string nombreTablaGeneral = "";
                string tipoFragmento = "";
                List<string> sitios = new List<string>();
                List<string> condicion = new List<string>();
                SitioCentral st = new SitioCentral();
                st.LeeEsquemaLocalizacion("Oferta", ref idFragmentos, ref nombreTablaBDFragmento, ref nombreTablaGeneral, ref sitios, ref tipoFragmento, ref condicion);

                switch (tipoFragmento)
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
                            if (sitios.ElementAt(0).Contains("1"))
                            {
                                //Insercion en sql server sitio1
                                string consulta = "UPDATE " + nombreTablaBDFragmento.ElementAt(0).ToString() + " SET descripcion = '" + descripcion + "',vigencia = '" + Vigencia + "',descuento = " + Descuento + " WHERE id_Oferta = " + dataGridViewOferta.CurrentRow.Cells[0].Value.ToString() + "";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiarCampos();
                                //fin de insercion

                                //Insercion en NPG sitio2
                                NpgsqlCommand command = new NpgsqlCommand("UPDATE " + nombreTablaBDFragmento.ElementAt(1).ToString() + " SET descripcion = '" + descripcion + "',vigencia = '" + Vigencia + "',descuento = " + Descuento + " WHERE id_Oferta = " + dataGridViewOferta.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                //Insercion en sql server sitio1
                                string consulta = "UPDATE " + nombreTablaBDFragmento.ElementAt(1).ToString() + " SET descripcion = '" + descripcion + "',vigencia = '" + Vigencia + "',descuento = " + Descuento + " WHERE id_Oferta = " + dataGridViewOferta.CurrentRow.Cells[0].Value.ToString() + "";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiarCampos();
                                //fin de insercion

                                //Insercion en NPG sitio2
                                NpgsqlCommand command = new NpgsqlCommand("UPDATE " + nombreTablaBDFragmento.ElementAt(0).ToString() + " SET descripcion = '" + descripcion + "',vigencia = '" + Vigencia + "',descuento = " + Descuento +" WHERE id_Oferta = " + dataGridViewOferta.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                command.ExecuteNonQuery();
                            }



                        }
                        catch (Exception error)
                        {

                            MessageBox.Show("ERROR al insertar : " + error.Message);
                        }

                        break;

                    default:
                        switch (sitios.ElementAt(0))
                        {
                            case "1":
                                //Insercion en sql server
                                string consulta = "UPDATE  " + nombreTablaBDFragmento.ElementAt(0).ToString() + " SET descripcion = '" + descripcion + "',vigencia = '" + Vigencia + "',descuento = " + Descuento + " WHERE id_Oferta = " + dataGridViewOferta.CurrentRow.Cells[0].Value.ToString() + "";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiarCampos();

                                break;


                            case "2":

                                NpgsqlCommand command = new NpgsqlCommand("UPDATE " + nombreTablaBDFragmento.ElementAt(0).ToString() + "  SET descripcion = '" + descripcion + "',vigencia = '" + Vigencia + "',descuento = " + Descuento + "WHERE id_Oferta = " + dataGridViewOferta.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
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
                Console.WriteLine("no se inserto" + ex.Message);
            }
        }

        private void BtnEliminar_Click(object sender, System.EventArgs e)
        {
            if (!validaDatos())
                return;

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
                st.LeeEsquemaLocalizacion("Oferta", ref idFragmentos, ref nombreTablaBDFragmento, ref nombreTablaGeneral, ref sitios, ref tipoFragmento, ref condicion);

                switch (tipoFragmento)
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
                            if (sitios.ElementAt(0).Contains("1"))
                            {
                                //Insercion en sql server sitio1
                                string consulta = "DELETE FROM " + nombreTablaBDFragmento.ElementAt(0).ToString() + " WHERE id_Oferta = " + dataGridViewOferta.CurrentRow.Cells[0].Value.ToString() + "";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiarCampos();
                                //fin de insercion

                                //Insercion en NPG sitio2
                                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM " + nombreTablaBDFragmento.ElementAt(1).ToString() + " WHERE id_Oferta = " + dataGridViewOferta.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                //Insercion en sql server sitio1
                                string consulta = "DELETE FROM " + nombreTablaBDFragmento.ElementAt(1).ToString() + " WHERE id_Oferta = " + dataGridViewOferta.CurrentRow.Cells[0].Value.ToString() + "";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiarCampos();
                                //fin de insercion

                                //Insercion en NPG sitio2
                                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM " + nombreTablaBDFragmento.ElementAt(0).ToString() + " WHERE id_Oferta = " + dataGridViewOferta.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                command.ExecuteNonQuery();
                            }



                        }
                        catch (Exception error)
                        {

                            MessageBox.Show("ERROR al insertar : " + error.Message);
                        }

                        break;

                    default:
                        switch (sitios.ElementAt(0))
                        {
                            case "1":
                                //Insercion en sql server
                                string consulta = "DELETE FROM " + nombreTablaBDFragmento.ElementAt(0).ToString() + " WHERE id_Oferta = " + dataGridViewOferta.CurrentRow.Cells[0].Value.ToString() + "";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiarCampos();

                                break;


                            case "2":

                                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM " + nombreTablaBDFragmento.ElementAt(0).ToString() + " WHERE id_Oferta = " + dataGridViewOferta.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
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
                Console.WriteLine("no se inserto" + ex.Message);
            }
        }

       

    }
}
