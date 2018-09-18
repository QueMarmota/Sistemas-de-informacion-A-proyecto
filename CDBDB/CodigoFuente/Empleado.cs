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
    public partial class Empleado : Form
    {
        string GlobalPuesto = "";
        bool bandModifica = false;
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
                    case"Horizontal":
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

        public void MezclaBDHorizontal() {


            //codigo para hacer el select al sitio1
            da = new SqlDataAdapter("Select * from Empleado1", cnSQL);
            dt = new DataTable();
            da.Fill(dt);
            //dt.Columns.RemoveAt(0);//Para quitar el campo RFC
            dataGridViewEmpleado.DataSource = dt;


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
            dataGridViewEmpleado.DataSource = dt;

            //para esconder el campor RFC
            dataGridViewEmpleado.Columns[0].Visible = false;                                      
        }

        public Empleado()
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

        private void Empleado_Load(object sender, EventArgs e)
        {

        }
        private void comboBoxidSucursal_TextChanged(object sender, EventArgs e)
        {
            comboBoxidSucursal.Items.Clear();
            //agregar campos
            SqlDataAdapter daSucursal = new SqlDataAdapter("Select * from Sucursal1", cnSQL);
            DataTable dtSucursal = new DataTable();
            daSucursal.Fill(dtSucursal);
            string dato = "";
            // For each row, print the values of each column.
            foreach (DataRow row in dtSucursal.Rows)
            {
                dato = row[dtSucursal.Columns[1]].ToString();//para todas las rows dame el dato de la columna posicion 1 osea el nombre de la sucursal
                /* foreach (DataColumn column in dtSucursal.Columns)
                 {
                    ITERA TODOS LOS CAMPOS DE LAS COLUMNAS
                 }*/
                comboBoxidSucursal.Items.Add(dato);//agrega como campo el nombre de la sucursal
            }
        }
        

        private void comboBoxidSucursal_Leave(object sender, EventArgs e)
        {
            //borrar campos
           
        }

        private void BtnInsertar_Click(object sender, EventArgs e)
        {
            if (!validaDatosEntrada())
                return;

            try
            {
                string fecha_nacimiento = dateTimePickerFechaNacimiento.Value.ToString("yyyy-MM-dd");
                string hora_Entrada = dateTimePickerHoraEntrada.Value.ToString("hh:mm:ss");
                string hora_Salida = dateTimePickerHoraSalida.Value.ToString("hh:mm:ss");
                string genero = (radioButtonFemenino.Checked == true) ? "F" : "M";
                //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
                //variables necesarias para sacar datos del esquema de localizacion
                List<string> idFragmentos = new List<string>();
                List<string> nombreTablaBDFragmento = new List<string>();
                string nombreTablaGeneral="" ;
                string tipoFragmento="";
                List<string> sitios = new List<string>();
                List<string> condicion = new List<string>();               
                SitioCentral st = new SitioCentral();
                st.LeeEsquemaLocalizacion("Sucursal", ref idFragmentos, ref nombreTablaBDFragmento, ref nombreTablaGeneral, ref sitios, ref tipoFragmento, ref condicion);
               
                //codigo para convertir internamente el nombre de la sucursal al id de la sucursal
                SqlDataAdapter idSucursal = new SqlDataAdapter("Select id_Sucursal from " + nombreTablaBDFragmento[0] + " where nombre = '" + comboBoxidSucursal.Text + "'", cnSQL);
                cmd = new SqlCommand("Select id_Sucursal from " + nombreTablaBDFragmento[0] + " where nombre = '" + comboBoxidSucursal.Text + "'", cnSQL);
                dr = cmd.ExecuteReader();
                dr.Read();
                int idSuc= (Convert.ToInt32(dr[0]));
                //Validacion en esquema de localizacion---------------------------------------------------------------------------------------------------------------
                //variables necesarias para sacar datos del esquema de localizacion
                idFragmentos = new List<string>();
                nombreTablaBDFragmento = new List<string>();
                nombreTablaGeneral="" ;
                tipoFragmento="";
                sitios = new List<string>();
                condicion = new List<string>();               
                st = new SitioCentral();
                st.LeeEsquemaLocalizacion("Empleado", ref idFragmentos, ref nombreTablaBDFragmento, ref nombreTablaGeneral, ref sitios, ref tipoFragmento, ref condicion);
               //validacion de datos obtenidos
                //Juntamos los datos en renglones
                string renglon = "";
                String[] atributos=new String[20] ;            
                for (int i = 0; i < idFragmentos.Count; i++)
                {
                    renglon += idFragmentos.ElementAt(i).ToString() + "," + nombreTablaBDFragmento.ElementAt(i).ToString() + "," + nombreTablaGeneral + "," + tipoFragmento +","+ sitios.ElementAt(i).ToString() + "," + condicion.ElementAt(i).ToString();
                    //Orden : idfragmento , nombrefragmento , tabla , tipo , sitio , condicion
                    atributos = renglon.Split(',');
                    if(atributos.ElementAt(5).Contains(ComboBoxPuesto.Text))
                    { break; }
                    renglon = "";
                }
                switch (tipoFragmento)
                {
                    case "Horizontal":
                   
                            //si es insercion se hace en el sitio de la condicion
                            switch (ComboBoxPuesto.Text)
                            {
                                //Sitio1
                                case "Empleado":
                                    switch (atributos[4].ToString())
	                                {
                                        case"1":
                                            //Insercion en sql server
                                            string consulta = "Insert into " + atributos[1].ToString() + " values('" + TextBoxNombre.Text + "'," + NumericSueldo.Value + ",'" + ComboBoxPuesto.Text + "','" + TextBoxTelefono.Text + "','" + TextBoxDireccion.Text + "','" + fecha_nacimiento + "','" + genero + "','" + hora_Entrada + "','" + hora_Salida + "'," + idSuc + ")";
                                            cmd = new SqlCommand(consulta, cnSQL);
                                            dr.Close();
                                            cmd.ExecuteNonQuery();
                                            cargaTabla();
                                            limpiarCampos();
                                            //fin de insercion
                                            break;
                                        case "2":
                                            try
                                    {
                                        NpgsqlCommand command = new NpgsqlCommand("Insert into " + atributos[1].ToString() + "(nombre,sueldo,puesto,teléfono,dirección,fecha_nacimiento,genero,hora_entrada,hora_salida,id_Sucursal) values('" + TextBoxNombre.Text + "'," + NumericSueldo.Value + ",'" + ComboBoxPuesto.Text + "','" + TextBoxTelefono.Text + "','" + TextBoxDireccion.Text + "','" + fecha_nacimiento + "','" + genero + "','" + hora_Entrada + "','" + hora_Salida + "'," + idSuc + ")", conNPG);
                                        dr.Close();
                                        command.ExecuteNonQuery();
                                        
                                    }
                                    catch (Exception error)
                                    {

                                        MessageBox.Show("ERROR al insertar en NPG: " + error.Message);
                                    }
             
                                    cargaTabla();
                                    limpiarCampos();
                                    //fin de insercion
                                            break;
		                                default:
                                            break;
	                                }
                                    
                                    break;
                                //Sitio2
                                case "Administrador":
                                    //Insercion en NPG
                                    switch (atributos[4].ToString())
                                    {
                                        case "1":
                                            //Insercion en sql server
                                            string consulta = "Insert into " + atributos[1].ToString() + " values('" + TextBoxNombre.Text + "'," + NumericSueldo.Value + ",'" + ComboBoxPuesto.Text + "','" + TextBoxTelefono.Text + "','" + TextBoxDireccion.Text + "','" + fecha_nacimiento + "','" + genero + "','" + hora_Entrada + "','" + hora_Salida + "'," + idSuc + ")";
                                            cmd = new SqlCommand(consulta, cnSQL);
                                            dr.Close();
                                            cmd.ExecuteNonQuery();
                                            cargaTabla();
                                            limpiarCampos();
                                            //fin de insercion
                                            break;
                                        case "2":
                                            try
                                            {
                                                NpgsqlCommand command = new NpgsqlCommand("Insert into " + atributos[1].ToString() + "(nombre,sueldo,puesto,teléfono,dirección,fecha_nacimiento,genero,hora_entrada,hora_salida,id_Sucursal) values('" + TextBoxNombre.Text + "'," + NumericSueldo.Value + ",'" + ComboBoxPuesto.Text + "','" + TextBoxTelefono.Text + "','" + TextBoxDireccion.Text + "','" + fecha_nacimiento + "','" + genero + "','" + hora_Entrada + "','" + hora_Salida + "'," + idSuc + ")", conNPG);
                                                dr.Close();
                                                command.ExecuteNonQuery();

                                            }
                                            catch (Exception error)
                                            {

                                                MessageBox.Show("ERROR al insertar en NPG: " + error.Message);
                                            }

                                            cargaTabla();
                                            limpiarCampos();
                                            //fin de insercion
                                            break;
                                        default:
                                            break;
                                    }
                                    break;

                                default:
                                    break;
                            }


                        
                        break;
                    case "Vertical":
                        break;
                    case "Replica":

                        //insertar en ambos sitios


                        break;

                    default:
                        break;
                }             
           
               
            }
            catch (Exception ex)
            {
                Console.WriteLine("no se inserto"+ex.Message);
            }

        }

        private bool validaDatosEntrada()
        {
            if (TextBoxNombre.Text == "" || TextBoxTelefono.Text == "" || NumericSueldo.Value <= 0 || TextBoxDireccion.Text == "" || ComboBoxPuesto.SelectedItem.ToString() == "" || comboBoxidSucursal.Text.ToString() == "")
            {
                MessageBox.Show("Error de datos");
                return false;
            }

            if (!radioButtonFemenino.Checked && !radioButtonMasculino.Checked)
            {
                MessageBox.Show("Error de datos");
                return false;
            }
            return true;
        }

        private void limpiarCampos()
        {

            TextBoxDireccion.Clear();
            TextBoxNombre.Clear();
            TextBoxTelefono.Clear();
            ComboBoxPuesto.Text = "";
            comboBoxidSucursal.Text = "";
            NumericSueldo.Value = 0;
            radioButtonFemenino.Checked = false;
            radioButtonMasculino.Checked = false;
            dateTimePickerFechaNacimiento.Value = DateTime.Now;
            dateTimePickerHoraEntrada.Value = DateTime.Now;
            dateTimePickerHoraEntrada.Format = DateTimePickerFormat.Time;
            dateTimePickerHoraSalida.Value = DateTime.Now;
            dateTimePickerHoraSalida.Format = DateTimePickerFormat.Time;
            cargaTabla();

        
        
        }

        private void dataGridViewEmpleado_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                TextBoxNombre.Text = dataGridViewEmpleado.CurrentRow.Cells[1].Value.ToString();
                NumericSueldo.Value = Int32.Parse(dataGridViewEmpleado.CurrentRow.Cells[2].Value.ToString());
                ComboBoxPuesto.Text = GlobalPuesto = dataGridViewEmpleado.CurrentRow.Cells[3].Value.ToString();
                TextBoxTelefono.Text = dataGridViewEmpleado.CurrentRow.Cells[4].Value.ToString();
                TextBoxDireccion.Text = dataGridViewEmpleado.CurrentRow.Cells[5].Value.ToString();
                dateTimePickerFechaNacimiento.Text = dataGridViewEmpleado.CurrentRow.Cells[6].Value.ToString();
                radioButtonFemenino.Checked = dataGridViewEmpleado.CurrentRow.Cells[7].Value.ToString() == "F" ? true : false;
                radioButtonMasculino.Checked = dataGridViewEmpleado.CurrentRow.Cells[7].Value.ToString() == "M" ? true : false;
                cmd = new SqlCommand("Select nombre from Sucursal1 where id_Sucursal = " + dataGridViewEmpleado.CurrentRow.Cells[10].Value + "", cnSQL);
                dr = cmd.ExecuteReader();
                dr.Read();
                string nombreSuc = dr[0].ToString();
                dr.Close();
                comboBoxidSucursal.Text = nombreSuc;
                dateTimePickerHoraEntrada.Text = dataGridViewEmpleado.CurrentRow.Cells[8].Value.ToString();
                dateTimePickerHoraSalida.Text = dataGridViewEmpleado.CurrentRow.Cells[9].Value.ToString();
        
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en el datagrid cellclick" + ex);
            }
        }

        private void BtnElimina_Click(object sender, EventArgs e)
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
                st.LeeEsquemaLocalizacion("Empleado", ref idFragmentos, ref nombreTablaBDFragmento, ref nombreTablaGeneral, ref sitios, ref tipoFragmento, ref condicion);
                //validacion de datos obtenidos
                //Juntamos los datos en renglones
                string renglon = "";
                String[] atributos = new String[20];
                for (int i = 0; i < idFragmentos.Count; i++)
                {
                    renglon += idFragmentos.ElementAt(i).ToString() + "," + nombreTablaBDFragmento.ElementAt(i).ToString() + "," + nombreTablaGeneral + "," + tipoFragmento + "," + sitios.ElementAt(i).ToString() + "," + condicion.ElementAt(i).ToString();
                    //Orden : idfragmento , nombrefragmento , tabla , tipo , sitio , condicion
                    atributos = renglon.Split(',');
                    if (atributos.ElementAt(5).Contains(GlobalPuesto))
                    { break; }
                    renglon = "";
                }
                switch (tipoFragmento)
                {
                    case "Horizontal":
                        if (tipoFragmento == "Horizontal")
                        {
                            //si es insercion se hace en el sitio de la condicion
                            switch (GlobalPuesto)
                            {
                                //Sitio1
                                case "Empleado":

                                    switch (atributos[4].ToString())
                                    {
                                        case "1":
                                            //eliminar en sql server
                                            string consulta = "DELETE FROM " + atributos[1].ToString() + " WHERE RFC = " + dataGridViewEmpleado.CurrentRow.Cells[0].Value.ToString() + "";
                                            cmd = new SqlCommand(consulta, cnSQL);
                                            dr.Close();
                                            cmd.ExecuteNonQuery();
                                            //fin de eliminacion
                                            break;
                                        case "2":
                                            //eliminar en NPG
                                            try
                                            {
                                                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM " + atributos[1].ToString() + " WHERE RFC = " + dataGridViewEmpleado.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                                dr.Close();
                                                command.ExecuteNonQuery();

                                            }
                                            catch (Exception error)
                                            {

                                                MessageBox.Show("ERROR al eliminar en NPG: " + error.Message);
                                            }
                                            break;
                                        default:
                                            break;
                                    }                                   
                                    break;
                                //Sitio2
                                case "Administrador":
                                   switch (atributos[4].ToString())
                                    {
                                        case "1":
                                            //eliminar en sql server
                                            string consulta = "DELETE FROM " + atributos[1].ToString() + " WHERE RFC = " + dataGridViewEmpleado.CurrentRow.Cells[0].Value.ToString() + "";
                                            cmd = new SqlCommand(consulta, cnSQL);
                                            dr.Close();
                                            cmd.ExecuteNonQuery();
                                            //fin de eliminacion
                                            break;
                                        case "2":
                                            //eliminar en NPG
                                            try
                                            {
                                                NpgsqlCommand command = new NpgsqlCommand("DELETE FROM " + atributos[1].ToString() + " WHERE RFC = " + dataGridViewEmpleado.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                                dr.Close();
                                                command.ExecuteNonQuery();

                                            }
                                            catch (Exception error)
                                            {

                                                MessageBox.Show("ERROR al eliminar en NPG: " + error.Message);
                                            }
                                            break;                                            
                                        default:
                                            break;
                                    }                                   
                                    break;

                                default:
                                    break;
                            }
                        }
                        break;
                    case "Vertical":
                        break;
                    case "Replica":
                        //insertar en ambos sitios
                        break;

                    default:
                        break;
                }

                if (!bandModifica)
                {
                    cargaTabla();
                    limpiarCampos();
                }//fin de eliminacion
            }
            catch (Exception ex)
            {
                Console.WriteLine("no se inserto" + ex.Message);
            }

        }

        private void BtnModifica_Click(object sender, EventArgs e)
        {
            if (!validaDatosEntrada())
                return;

            try
            {
                bandModifica = true;
                BtnElimina_Click(this, null);
                BtnInsertar_Click(this, null);
                bandModifica = false;
                cargaTabla();
                limpiarCampos();
            }
            catch (Exception ex)
            {
                
                throw;
            }
            
            /*
            try
            {
                string fecha_nacimiento = dateTimePickerFechaNacimiento.Value.ToString("yyyy-MM-dd");
                string hora_Entrada = dateTimePickerHoraEntrada.Value.ToString("hh:mm:ss");
                string hora_Salida = dateTimePickerHoraSalida.Value.ToString("hh:mm:ss");
                string genero = (radioButtonFemenino.Checked == true) ? "F" : "M";
                //codigo para convertir internamente el nombre de la sucursal al id de la sucursal
                SqlDataAdapter idSucursal = new SqlDataAdapter("Select id_Sucursal from Sucursal1 where nombre = '" + comboBoxidSucursal.Text + "'", cnSQL);
                cmd = new SqlCommand("Select id_Sucursal from Sucursal1 where nombre = '" + comboBoxidSucursal.Text + "'", cnSQL);
                dr = cmd.ExecuteReader();
                dr.Read();
                int idSuc = (Convert.ToInt32(dr[0]));
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
                //validacion de datos obtenidos
                //Juntamos los datos en renglones
                string renglon = "";
                String[] atributos = new String[20];
                for (int i = 0; i < idFragmentos.Count; i++)
                {
                    renglon += idFragmentos.ElementAt(i).ToString() + "," + nombreTablaBDFragmento.ElementAt(i).ToString() + "," + nombreTablaGeneral + "," + tipoFragmento + "," + sitios.ElementAt(i).ToString() + "," + condicion.ElementAt(i).ToString();
                    //Orden : idfragmento , nombrefragmento , tabla , tipo , sitio , condicion
                    atributos = renglon.Split(',');
                    if (atributos.ElementAt(5).Contains(ComboBoxPuesto.Text))
                    { break; }
                    renglon = "";
                }
                switch (tipoFragmento)
                {
                    case "Horizontal":
                        if (tipoFragmento == "Horizontal")
                        {
                            //si es insercion se hace en el sitio de la condicion
                            switch (ComboBoxPuesto.Text)
                            {
                                //Sitio1
                                case "Empleado":
                                    //MODIFICACION en sql server
                                    string consulta = "UPDATE " + atributos[1].ToString() + " SET nombre = '" + TextBoxNombre.Text + "', sueldo = " + NumericSueldo.Value + ", puesto = '" + ComboBoxPuesto.Text + "', teléfono = '" + TextBoxTelefono.Text + "',dirección ='" + TextBoxDireccion.Text + "',fecha_nacimiento ='" + fecha_nacimiento + "',genero='" + genero + "',hora_entrada='" + hora_Entrada + "',hora_salida ='" + hora_Salida + "',id_Sucursal=" + idSuc + " WHERE RFC = " + dataGridViewEmpleado.CurrentRow.Cells[0].Value.ToString() + "";
                                    cmd = new SqlCommand(consulta, cnSQL);
                                    dr.Close();
                                    cmd.ExecuteNonQuery();
                                    cargaTabla();
                                    limpiarCampos();
                                    //fin de insercion
                                    break;
                                //Sitio2
                                case "Administrador":
                                    //MODIFICACION en NPG
                                    try
                                    {
                                        NpgsqlCommand command = new NpgsqlCommand("UPDATE " + atributos[1].ToString() + " SET nombre = '" + TextBoxNombre.Text + "', sueldo = " + NumericSueldo.Value + ", puesto = '" + ComboBoxPuesto.Text + "', teléfono = '" + TextBoxTelefono.Text + "',dirección ='" + TextBoxDireccion.Text + "',fecha_nacimiento ='" + fecha_nacimiento + "',genero='" + genero + "',hora_entrada='" + hora_Entrada + "',hora_salida ='" + hora_Salida + "',id_Sucursal=" + idSuc + " WHERE RFC = " + dataGridViewEmpleado.CurrentRow.Cells[0].Value.ToString() + "", conNPG);
                                        dr.Close();
                                        command.ExecuteNonQuery();

                                    }
                                    catch (Exception error)
                                    {

                                        MessageBox.Show("ERROR al insertar en NPG: " + error.Message);
                                    }

                                    cargaTabla();
                                    limpiarCampos();
                                    //fin de insercion
                                    break;

                                default:
                                    break;
                            }


                        }
                        break;
                    case "Vertical":
                        break;
                    case "Replica":

                        //insertar en ambos sitios


                        break;

                    default:
                        break;
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("no se inserto" + ex.Message);
            }
            */
        }

        private void comboBoxidSucursal_Enter_1(object sender, EventArgs e)
        {
            comboBoxidSucursal.Items.Clear();
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

            switch (sitios.ElementAt(0))//por si esta en el sitio 1 o 2
            {
                case "1":

                    //agregar campos
                    SqlDataAdapter daSucursal = new SqlDataAdapter("Select nombre from " + nombreTablaBDFragmento .ElementAt(0)+ "", cnSQL);
                    DataTable dtSucursal = new DataTable();
                    daSucursal.Fill(dtSucursal);
                    string dato = "";
                    // For each row, print the values of each column.
                    foreach (DataRow row in dtSucursal.Rows)
                    {
                        dato = row[dtSucursal.Columns[0]].ToString();//para todas las rows dame el dato de la columna posicion 1 osea el nombre de la sucursal
                        /* foreach (DataColumn column in dtSucursal.Columns)
                         {
                            ITERA TODOS LOS CAMPOS DE LAS COLUMNAS
                         }*/
                        comboBoxidSucursal.Items.Add(dato);//agrega como campo el nombre de la sucursal
                    }

                    break;

                case"2":
                    //agregar campos
                    NpgsqlDataAdapter daSucursalNP = new NpgsqlDataAdapter("Select nombre from " + nombreTablaBDFragmento.ElementAt(0) + "", conNPG);
                    DataTable dtSucursalNP = new DataTable();
                    daSucursalNP.Fill(dtSucursalNP);
                    string dat = "";
                    // For each row, print the values of each column.
                    foreach (DataRow row in dtSucursalNP.Rows)
                    {
                        dat = row[dtSucursalNP.Columns[0]].ToString();//para todas las rows dame el dato de la columna posicion 1 osea el nombre de la sucursal
                        /* foreach (DataColumn column in dtSucursal.Columns)
                         {
                            ITERA TODOS LOS CAMPOS DE LAS COLUMNAS
                         }*/
                        comboBoxidSucursal.Items.Add(dat);//agrega como campo el nombre de la sucursal
                    }

                    break;

                default:
                    break;

            }
           
        }

    


        
    }
}
