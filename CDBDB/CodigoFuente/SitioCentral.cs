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
    public class SitioCentral
    {
        SqlConnection cnSitio1;
        SqlConnection cnSitioCentral;
        SqlCommand cmd;
        SqlDataAdapter da;
        DataTable dt;

        public SitioCentral()
        {
             
            
        }
      
        //Funcion que le paso por parametro el nombre de la tabla que solicita la informacion
        //regresa , sitios o sitio donde debe hacer la operacion , el tipo de fragmento que es y si condicion si es q existe
        public  void LeeEsquemaLocalizacion(string nombreTaablaSolicitada,ref List<string> idFragmentos,ref List<string> nombreTablaBDFragmento,ref string nombreTablaGeneral,ref List<string> sitios ,ref string tipoFragmento ,ref List<string> condicion)
        {
            ConexionSitioCentral();
            SqlDataAdapter datosSitioCentral = new SqlDataAdapter("Select * from Fragmentos", cnSitioCentral);
            DataTable tablaSitioCentral = new DataTable();
            datosSitioCentral.Fill(tablaSitioCentral);
            string info="";
            string nombreDeColumna = "";

            // For each row, print the values of each column.
            foreach (DataRow row in tablaSitioCentral.Rows)
            {
                //checar si el nombre de la tabla en el renglon que va es igual al nombre de la tabla solicitada
                string variable = row[tablaSitioCentral.Columns.IndexOf("tabla")].ToString();
                if (variable != nombreTaablaSolicitada)
                {
                    continue;
                }
                else
                {
                    foreach (DataColumn column in tablaSitioCentral.Columns)
                    {

                            nombreDeColumna += column.ColumnName;
                            info += row[column].ToString();
                            info += ",";

                       
                    }
                    string[] listTemp;
                    listTemp = info.Split(',');
                    idFragmentos.Add(listTemp.ElementAt(0));
                    nombreTablaBDFragmento.Add(listTemp.ElementAt(1));
                    nombreTablaGeneral = listTemp.ElementAt(2);
                    tipoFragmento = listTemp.ElementAt(3);
                    sitios.Add(listTemp.ElementAt(4));                    
                    condicion.Add(listTemp.ElementAt(5));
                }
                info = "";
                nombreDeColumna = "";
            }
            

        }

        public void LeeEsquemaAtributos(List<string> idFragmentos,ref List<string> atributosSitio1,ref List<string> atributosSitio2)
        {
            try
            {
                ConexionSitioCentral();
                SqlDataAdapter datosSitioCentral = new SqlDataAdapter("Select * from Atributos", cnSitioCentral);
                DataTable tablaSitioCentral = new DataTable();
                datosSitioCentral.Fill(tablaSitioCentral);

                string info = "";
                string nombreDeColumna = "";
                List<string> listTemp = new List<string>();
                // For each row, print the values of each column.
                foreach (DataRow row in tablaSitioCentral.Rows)
                {
                    //checar si el nombre de la tabla en el renglon que va es igual al nombre de la tabla solicitada
                    string variable = row[tablaSitioCentral.Columns.IndexOf("id_Atributo")].ToString();
                    if (idFragmentos.ElementAt(0).Contains(variable) || idFragmentos.ElementAt(1).Contains(variable))
                    {
                        foreach (DataColumn column in tablaSitioCentral.Columns)
                        {

                            nombreDeColumna += column.ColumnName;
                            info += row[column].ToString();
                            info += ",";


                        }
                        
                        listTemp.Add(info);
                       
                    }
                
                    info = "";
                    nombreDeColumna = "";
                }

                foreach (var row in listTemp)
                {
                    if (row.Contains(idFragmentos.ElementAt(0)))//sitio 1
                    {

                        atributosSitio1.Add(row);
                    }
                    else//sittio2
                    {
                        atributosSitio2.Add(row);
                    }

                }
            }
            catch (Exception)
            {
                
                throw;
            }
          
        
        
        }

        public void  ConexionSitio1()
        {


            try
            {
                cnSitio1 = new SqlConnection(@"Data Source=DESKTOP-D898I0K\SQLEXPRESS;initial catalog=Sitio1; integrated Security=true");
                cnSitio1.Open();
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
                cnSitioCentral = new SqlConnection(@"Data Source=DESKTOP-D898I0K\SQLEXPRESS;initial catalog=SitioCentral; integrated Security=true");
                cnSitioCentral.Open();
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
