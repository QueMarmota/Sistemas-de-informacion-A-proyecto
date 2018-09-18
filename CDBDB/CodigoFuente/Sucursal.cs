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
    public partial class Sucursal : Form
    {

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

        public Sucursal()
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

            cargaTabla();
        }

        private void MezclaBDReplica(List<string> nombreTablaBDFragmento)
        {

            //codigo para hacer el select al sitio1
            da = new SqlDataAdapter("Select * from " + nombreTablaBDFragmento.ElementAt(0) + "", cnSQL);
            dt = new DataTable();
            da.Fill(dt);
            //dt.Columns.RemoveAt(0);//Para quitar el campo RFC
            dataGridViewSucursal.DataSource = dt;
            //para esconder el campor RFC
            dataGridViewSucursal.Columns[0].Visible = false;
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
                st.LeeEsquemaLocalizacion("Sucursal", ref idFragmentos, ref nombreTablaBDFragmento, ref nombreTablaGeneral, ref sitios, ref tipoFragmento, ref condicion);
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
                        mezcaBDNormalDelSitio(sitios,nombreTablaBDFragmento);
                        break;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("No se pudo actualizar" + ex);
            }
        }

        private void mezcaBDNormalDelSitio(List<string> sitios ,List<string> nombreTablaBDFragmento)
        {
        
            switch (sitios.ElementAt(0))
	        {
		        case"1":
                     //codigo para hacer el select al sitio1
                    da = new SqlDataAdapter("Select * from " + nombreTablaBDFragmento .ElementAt(0)+ "", cnSQL);
                    dt = new DataTable();
                    da.Fill(dt);
                    //dt.Columns.RemoveAt(0);//Para quitar el campo RFC
                    dataGridViewSucursal.DataSource = dt;       
                    //para esconder el campor id
                    dataGridViewSucursal.Columns[0].Visible = false;

                  break;


                case"2":
                     //Codigo para hacer el select from a al sitio2             
                    NpgsqlDataAdapter add = new NpgsqlDataAdapter("select * from "+ nombreTablaBDFragmento .ElementAt(0)+"", conNPG);
                    dtNPG = new DataTable();
                    add.Fill(dtNPG);
                    dataGridViewSucursal.DataSource = dtNPG;       
                    //para esconder el campor id
                    dataGridViewSucursal.Columns[0].Visible = false;
                 break;
	        }
       
        }
        private void limpiaCampos() 
        {

            textBoxDireccion.Clear();
            textBoxSucursal.Clear();
            textBoxTelefono.Clear();
            numericCantDeEmpleados.Value = 0;

        }
        private bool validaDatosEntrada()
        {
            if (textBoxDireccion.Text == "" || textBoxTelefono.Text == "" || numericCantDeEmpleados.Value <= 0 || textBoxSucursal.Text == "" )
            {
                MessageBox.Show("Error de datos");
                return false;
            }
           
            return true;
        }

        private void dataGridViewSucursal_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                textBoxSucursal.Text = dataGridViewSucursal.CurrentRow.Cells[1].Value.ToString();
                textBoxDireccion.Text = dataGridViewSucursal.CurrentRow.Cells[2].Value.ToString();
                textBoxTelefono.Text = dataGridViewSucursal.CurrentRow.Cells[3].Value.ToString();
                numericCantDeEmpleados.Value = Int32.Parse(dataGridViewSucursal.CurrentRow.Cells[4].Value.ToString());
                dateTimePickerHoraApertura.Text = dataGridViewSucursal.CurrentRow.Cells[5].Value.ToString();
                dateTimePickerCierre.Text = dataGridViewSucursal.CurrentRow.Cells[6].Value.ToString();
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en el datagrid cellclick" + ex);
            }
        }

        private void BtnInsertar_Click(object sender, System.EventArgs e)
        {
            if (!validaDatosEntrada())
                return;

            try
            {

                string hora_Apertura = dateTimePickerHoraApertura.Value.ToString("hh:mm:ss");
                string hora_Cierre = dateTimePickerCierre.Value.ToString("hh:mm:ss");
                string nombre = textBoxSucursal.Text;
                string direccion = textBoxDireccion.Text;
                string telefono = textBoxTelefono.Text;
                decimal cantidad = numericCantDeEmpleados.Value;
                
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
                                string consulta = "Insert into " + nombreTablaBDFragmento.ElementAt(0).ToString() + " values('" + nombre + "','" + direccion + "','" + telefono + "'," + cantidad + ",'" + hora_Apertura + "','" + hora_Cierre + "')";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiaCampos();
                                //fin de insercion

                                //Insercion en NPG sitio2
                                NpgsqlCommand command = new NpgsqlCommand("Insert into " + nombreTablaBDFragmento.ElementAt(1).ToString() + "(nombre,dirección,teléfono,cantidad_Empleados,hora_Apertura,hora_Cierre)  values('" + nombre + "','" + direccion + "','" + telefono + "'," + cantidad + ",'" + hora_Apertura + "','" + hora_Cierre + "')", conNPG);
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                //Insercion en sql server sitio1
                                string consulta = "Insert into " + nombreTablaBDFragmento.ElementAt(1).ToString() + " values('" + nombre + "','" + direccion + "','" + telefono + "'," + cantidad + ",'" + hora_Apertura + "','" + hora_Cierre + "')";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiaCampos();
                                //fin de insercion

                                //Insercion en NPG sitio2
                                NpgsqlCommand command = new NpgsqlCommand("Insert into " + nombreTablaBDFragmento.ElementAt(0).ToString() + "(nombre,dirección,teléfono,cantidad_Empleados,hora_Apertura,hora_Cierre)  values('" + nombre + "','" + direccion + "','" + telefono + "'," + cantidad + ",'" + hora_Apertura + "','" + hora_Cierre + "')", conNPG);
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
                                string consulta = "Insert into " + nombreTablaBDFragmento.ElementAt(0).ToString() + " values('" + nombre + "','" + direccion + "','" + telefono + "'," + cantidad + ",'" + hora_Apertura + "','" + hora_Cierre + "')";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiaCampos();

                                break;


                            case "2":

                                NpgsqlCommand command = new NpgsqlCommand("Insert into " + nombreTablaBDFragmento.ElementAt(0).ToString() + "(nombre,dirección,teléfono,cantidad_Empleados,hora_Apertura,hora_Cierre)  values('" + nombre + "','" + direccion + "','" + telefono + "'," + cantidad + ",'" + hora_Apertura + "','" + hora_Cierre + "')", conNPG);
                                command.ExecuteNonQuery();
                                cargaTabla();
                                limpiaCampos();

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
            if (!validaDatosEntrada())
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
                st.LeeEsquemaLocalizacion("Sucursal", ref idFragmentos, ref nombreTablaBDFragmento, ref nombreTablaGeneral, ref sitios, ref tipoFragmento, ref condicion);
                
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
                            if (sitios.ElementAt(0).Contains("1"))
                            {
                                //eliminar en sql server
                                string consulta = "DELETE FROM " + nombreTablaBDFragmento.ElementAt(0).ToString() + " WHERE id_Sucursal = " + dataGridViewSucursal.CurrentRow.Cells[0].Value.ToString() + "";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                //eliminar en postgressql
                                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM " + nombreTablaBDFragmento.ElementAt(1).ToString() + " WHERE id_Sucursal = " + dataGridViewSucursal.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                command.ExecuteNonQuery();
                                cargaTabla();
                                limpiaCampos();
                            }
                            else
                            {

                                //eliminar en sql server
                                string consulta = "DELETE FROM " + nombreTablaBDFragmento.ElementAt(1).ToString() + " WHERE id_Sucursal = " + dataGridViewSucursal.CurrentRow.Cells[0].Value.ToString() + "";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                //eliminar en postgressql
                                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM " + nombreTablaBDFragmento.ElementAt(0).ToString() + " WHERE id_Sucursal = " + dataGridViewSucursal.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                command.ExecuteNonQuery();
                                cargaTabla();
                                limpiaCampos();
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
                                string consulta = "DELETE FROM " + nombreTablaBDFragmento.ElementAt(0).ToString() + " WHERE id_Sucursal = "+dataGridViewSucursal.CurrentRow.Cells[0].Value.ToString()+"";
                                cmd = new SqlCommand(consulta, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiaCampos();

                                break;


                            case "2":

                                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM " + nombreTablaBDFragmento.ElementAt(0).ToString() + " WHERE id_Sucursal = " + dataGridViewSucursal.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                command.ExecuteNonQuery();
                                cargaTabla();
                                limpiaCampos();

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
            if (!validaDatosEntrada())
                return;

            try
            {

                string hora_Apertura = dateTimePickerHoraApertura.Value.ToString("hh:mm:ss");
                string hora_Cierre = dateTimePickerCierre.Value.ToString("hh:mm:ss");
                string nombre = textBoxSucursal.Text;
                string direccion = textBoxDireccion.Text;
                string telefono = textBoxTelefono.Text;
                decimal cantidad = numericCantDeEmpleados.Value;

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

                switch (tipoFragmento)
                {
                    case "Horizontal":
                        //si es insercion se hace en el sitio de la condicion          

                        break;
                    case "Vertical":

                        break;
                    case "Replica":
                        //modifcar en ambos sitios

                        if (sitios.ElementAt(0).Contains("1"))
                        {
                            //MODIFICACION en sql server
                            string consulta = "UPDATE " + nombreTablaBDFragmento.ElementAt(0).ToString() + " SET nombre = '" + nombre + "',dirección ='" + direccion + "',teléfono = '" + telefono + "',cantidad_Empleados=" + cantidad + ",hora_Apertura ='" + hora_Apertura + "',hora_Cierre='" + hora_Cierre + "' WHERE id_Sucursal = " + dataGridViewSucursal.CurrentRow.Cells[0].Value.ToString() + "";
                            cmd = new SqlCommand(consulta, cnSQL);
                            cmd.ExecuteNonQuery();
                            //modificar en postgresql
                            NpgsqlCommand command = new NpgsqlCommand("UPDATE " + nombreTablaBDFragmento.ElementAt(1).ToString() + " SET nombre = '" + nombre + "',dirección ='" + direccion + "',teléfono = '" + telefono + "',cantidad_Empleados=" + cantidad + ",hora_Apertura ='" + hora_Apertura + "',hora_Cierre='" + hora_Cierre + "' WHERE id_Sucursal = " + dataGridViewSucursal.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                            command.ExecuteNonQuery();
                            cargaTabla();
                            limpiaCampos();
                        }
                        else
                        {
                            //MODIFICACION en sql server
                            string consulta = "UPDATE " + nombreTablaBDFragmento.ElementAt(1).ToString() + " SET nombre = '" + nombre + "',dirección ='" + direccion + "',teléfono = '" + telefono + "',cantidad_Empleados=" + cantidad + ",hora_Apertura ='" + hora_Apertura + "',hora_Cierre='" + hora_Cierre + "' WHERE id_Sucursal = " + dataGridViewSucursal.CurrentRow.Cells[0].Value.ToString() + "";
                            cmd = new SqlCommand(consulta, cnSQL);
                            cmd.ExecuteNonQuery();
                            //modificar en postgresql
                            NpgsqlCommand command = new NpgsqlCommand("UPDATE " + nombreTablaBDFragmento.ElementAt(0).ToString() + " SET nombre = '" + nombre + "',dirección ='" + direccion + "',teléfono = '" + telefono + "',cantidad_Empleados=" + cantidad + ",hora_Apertura ='" + hora_Apertura + "',hora_Cierre='" + hora_Cierre + "' WHERE id_Sucursal = " + dataGridViewSucursal.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                            command.ExecuteNonQuery();
                            cargaTabla();
                            limpiaCampos();
                        }
                        break;
     

                    default:
                        switch (sitios.ElementAt(0))
                        {
                            case "1":
                                //Insercion en sql server
                                string consultaS = "UPDATE " + nombreTablaBDFragmento.ElementAt(0).ToString() + " SET nombre = '" + nombre + "',dirección ='" + direccion + "',teléfono = '" + telefono + "',cantidad_Empleados=" + cantidad + ",hora_Apertura ='" + hora_Apertura + "',hora_Cierre='" + hora_Cierre + "' WHERE id_Sucursal = "+dataGridViewSucursal.CurrentRow.Cells[0].Value.ToString()+"";
                                cmd = new SqlCommand(consultaS, cnSQL);
                                cmd.ExecuteNonQuery();
                                cargaTabla();
                                limpiaCampos();

                                break;


                            case "2":

                                NpgsqlCommand commandS = new NpgsqlCommand("UPDATE " + nombreTablaBDFragmento.ElementAt(0).ToString() + " SET nombre = '" + nombre + "',dirección ='" + direccion + "',teléfono = '" + telefono + "',cantidad_Empleados=" + cantidad + ",hora_Apertura ='" + hora_Apertura + "',hora_Cierre='" + hora_Cierre + "' WHERE id_Sucursal = " + dataGridViewSucursal.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
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
        }
    
    
    
    }
}
