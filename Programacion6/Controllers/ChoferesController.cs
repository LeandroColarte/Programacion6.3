using Microsoft.AspNetCore.Mvc;
using Programacion6.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Programacion6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChoferesController : ControllerBase
    {
        string connectionString = "Server=DESKTOP-DFFK8SQ;Database=ParcialProg6; Trusted_Connection=true;TrustServerCertificate=true;";

        [HttpPost]
        [Route("CrearOrden")]
        public IActionResult CrearOrden(Choferes C)
        {
            string query = "INSERT INTO Choferes (Nombre, Edad, Apellido, Unidad, DNI, Estado) VALUES (@Nombre, @Edad, @Apellido, @Unidad, @DNI, @Estado)";

            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                using (SqlCommand CM = new SqlCommand(query, sqlConn))
                {
                    CM.Parameters.AddWithValue("@Nombre", C.Nombre);
                    CM.Parameters.AddWithValue("@Edad", C.Edad);
                    CM.Parameters.AddWithValue("@Apellido", C.Apellido);
                    CM.Parameters.AddWithValue("@Unidad", C.Unidad);
                    CM.Parameters.AddWithValue("@DNI", C.DNI);
                    CM.Parameters.AddWithValue("@Estado", C.Estado); // Asegúrate de que el objeto Choferes tenga la propiedad Estado

                    try
                    {
                        CM.ExecuteNonQuery();
                        return Ok(new { resultado = "OK" });
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        return BadRequest(new { resultado = "NOK", mensaje = ex.Message });
                    }
                }
            }
        }

        [HttpGet]
        [Route("getChoferes")]
        public List<Choferes> getChoferes()
        {
            List<Choferes> LH = new List<Choferes>();

            string Query = "SELECT IdChofer, Nombre, EDAD, Apellido, Unidad, DNI, Estado FROM Choferes";

            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                using (SqlCommand sqlCM = new SqlCommand(Query, sqlConn))
                {
                    SqlDataReader dr = sqlCM.ExecuteReader();
                    while (dr.Read())
                    {
                        Choferes oh = new Choferes();
                        oh.IdChofer = int.Parse(dr[0].ToString());
                        oh.Nombre = dr[1].ToString();
                        oh.Edad = int.Parse(dr[2].ToString());
                        oh.Apellido = dr[3].ToString();
                        oh.Unidad = dr[4].ToString();
                        oh.DNI = int.Parse(dr[5].ToString());
                        oh.Estado = bool.Parse(dr[6].ToString()); // Asegúrate de que la propiedad Estado en la clase Choferes sea de tipo bool

                        LH.Add(oh);
                    }
                }
            }

            return LH;
        }

        [HttpGet]
        [Route("getChoferesUnidad")]
        public List<Choferes> getChoferesUnidad(string Unidad)
        {
            List<Choferes> lh = new List<Choferes>();

            // Utiliza parámetros en la consulta SQL para evitar la inyección de SQL
            string Query = "SELECT IdChofer, Nombre, EDAD, Apellido, Unidad, DNI, Estado FROM Choferes WHERE Unidad = @Unidad";

            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                using (SqlCommand sqlCM = new SqlCommand(Query, sqlConn))
                {
                    sqlCM.Parameters.AddWithValue("@Unidad", Unidad);
                    SqlDataReader dr = sqlCM.ExecuteReader();

                    while (dr.Read())
                    {
                        Choferes oh = new Choferes();
                        oh.IdChofer = int.Parse(dr[0].ToString());
                        oh.Nombre = dr[1].ToString();
                        oh.Edad = int.Parse(dr[2].ToString());
                        oh.Apellido = dr[3].ToString();
                        oh.Unidad = dr[4].ToString();
                        oh.DNI = int.Parse(dr[5].ToString());
                        oh.Estado = bool.Parse(dr[6].ToString()); // Asegúrate de que la propiedad Estado en la clase Choferes sea de tipo bool

                        lh.Add(oh);
                    }
                }
            }

            return lh;
        }

        [HttpPut]
        [Route("ModificarChofer")]
        public IActionResult ModificarChofer(Choferes C)
        {
            string query = "UPDATE Choferes SET Nombre = @Nombre, Edad = @Edad, Apellido = @Apellido, Unidad = @Unidad, DNI = @DNI, Estado = @Estado WHERE IdChofer = @IdChofer";

            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                using (SqlCommand CM = new SqlCommand(query, sqlConn))
                {
                    CM.Parameters.AddWithValue("@IdChofer", C.IdChofer);
                    CM.Parameters.AddWithValue("@Nombre", C.Nombre);
                    CM.Parameters.AddWithValue("@Edad", C.Edad);
                    CM.Parameters.AddWithValue("@Apellido", C.Apellido);
                    CM.Parameters.AddWithValue("@Unidad", C.Unidad);
                    CM.Parameters.AddWithValue("@DNI", C.DNI);
                    CM.Parameters.AddWithValue("@Estado", C.Estado);

                    try
                    {
                        int rowsAffected = CM.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Registrar cambios en la tabla de auditoría
                            RegistrarAuditoriaCambios(C);

                            return Ok(new { resultado = "OK", mensaje = "Chofer modificado exitosamente" });
                        }
                        else
                        {
                            return NotFound(new { resultado = "NOK", mensaje = "Chofer no encontrado" });
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception or handle it as needed
                        return BadRequest(new { resultado = "NOK", mensaje = ex.Message });
                    }
                }
            }
        }

        private void RegistrarAuditoriaCambios(Choferes choferModificado)
        {
            string auditoriaQuery = "INSERT INTO Choferes_Auditoria (IdChofer, Nombre, UnidadAnterior, EstadoAnterior) VALUES (@IdChofer, @Nombre, @UnidadAnterior, @EstadoAnterior)";

            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                using (SqlCommand auditoriaCommand = new SqlCommand(auditoriaQuery, sqlConn))
                {
                    // Obtener la información anterior del chofer desde la base de datos
                    Choferes choferAnterior = ObtenerDatosChoferAnterior(choferModificado.IdChofer);

                    // Registrar cambios en la tabla de auditoría
                    auditoriaCommand.Parameters.AddWithValue("@IdChofer", choferModificado.IdChofer);
                    auditoriaCommand.Parameters.AddWithValue("@Nombre", choferModificado.Nombre);
                    auditoriaCommand.Parameters.AddWithValue("@UnidadAnterior", choferAnterior.Unidad);
                    auditoriaCommand.Parameters.AddWithValue("@EstadoAnterior", choferAnterior.Estado);

                    auditoriaCommand.ExecuteNonQuery();
                }
            }
        }

        private Choferes ObtenerDatosChoferAnterior(int idChofer)
        {
            string selectAnteriorQuery = "SELECT Unidad, Estado FROM Choferes WHERE IdChofer = @IdChofer";

            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();
                using (SqlCommand selectAnteriorCommand = new SqlCommand(selectAnteriorQuery, sqlConn))
                {
                    selectAnteriorCommand.Parameters.AddWithValue("@IdChofer", idChofer);

                    SqlDataReader dr = selectAnteriorCommand.ExecuteReader();

                    if (dr.Read())
                    {
                        return new Choferes
                        {
                            IdChofer = idChofer,
                            Unidad = dr["Unidad"].ToString(),
                            Estado = bool.Parse(dr["Estado"].ToString())
                        };
                    }

                    return null;
                }
            }
        }
    }
}



// GET: api/<ChoferesController>

