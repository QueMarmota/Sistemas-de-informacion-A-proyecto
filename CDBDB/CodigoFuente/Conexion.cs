using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Npgsql;
using NpgsqlTypes;

namespace ProyectoBasesDeDatosDistribuidas
{
    class Conexion
    {
        SqlConnection cn;
        SqlCommand cmd;
        SqlDataAdapter da;
        DataTable dt;
        public Conexion(ref SqlConnection cn)
        {
           
            
            try
            {
                cn = new SqlConnection(@"Data Source=DESKTOP-D898I0K\SQLEXPRESS;initial catalog=Sitio1; integrated Security=true");
                cn.Open();
                //MessageBox.Show("abierto");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error con la conexion a la base de datos" + ex.ToString());
            }

        }
       

        public void ConexionSitio1()
        {


            try
            {
                cn = new SqlConnection(@"Data Source=DESKTOP-D898I0K\SQLEXPRESS;initial catalog=Sitio1; integrated Security=true");
                cn.Open();
                //MessageBox.Show("abierto");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error con la conexion a la base de datos" + ex.ToString());
            }

        }


        public void ConexionSitioCentral()
        {


            try
            {
                cn = new SqlConnection(@"Data Source=DESKTOP-D898I0K\SQLEXPRESS;initial catalog=SitioCentral; integrated Security=true");
                cn.Open();
                //MessageBox.Show("abierto");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error con la conexion a la base de datos" + ex.ToString());
            }

        }

        public void ConexionSitio2()
        {
            string parametros = "Server=localhost;Port=5432;User id=postgres;Password=root;Database=Sitio2";
            DataSet datos = new DataSet();
            NpgsqlConnection con = new NpgsqlConnection();
            con.ConnectionString = parametros;
            try
            {
                con.Open();
            }
            catch (Exception error)
            {

                MessageBox.Show("Error en la conexion con NpgSQL" + error.Message);
            }
            /*Cargar datos en datagrid
             query= select * from tabla;
             * NpgsqlDataAdapter add = new NpgsqlDataAdapter(query,con);
             * add.fill(datos);
             * dataGridView1.DataSource = data.Tables[0];
             * con.close();
             */

            /*Insertar registro
             
             
              try
            {
                NpgsqlCommand command = new NpgsqlCommand("INSERT INTO tabla(atributos) VALUES('" + "" + "');",con);
                command.ExecuteNonQuery();
            }
            catch (Exception error)
            {

                MessageBox.Show("ERROR al insertar en NPG: " + error.Message);
            }
             
             
             */


        }
    }
}
