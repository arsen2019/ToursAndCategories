using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using ToursAndCategories.Models;


namespace ToursAndCategories.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToursController : Controller
    {

        public ToursController(IConfiguration configuration)
        {
            Configuration = configuration;
            connectionString = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        private IConfiguration Configuration;
        SqlConnection connectionString;
        SqlCommand command;
        SqlDataAdapter dataAdapter;
        DataTable dataTable;
        List<SqlParameter> parameters = null;

        [HttpGet]
        public JsonResult GetAll()
        {

            dataTable = new DataTable();
            command = new SqlCommand("select * from Tour", connectionString);
            connectionString.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataTable);


            connectionString.Close();
            return Json(dataTable);
        }

        [Route("Create")]
        [HttpPost]
        public ActionResult Create(Tour item)
        {

            connectionString.Open();

            using (SqlCommand command = new SqlCommand(@"insert into Tour (Title, StartDate, EndDate)
	            values
                (@Title, @StartDate, @EndDate)
                ", connectionString))
            {
                parameters = new List<SqlParameter>
                {

                    new SqlParameter() { ParameterName = "@Title", SqlDbType = SqlDbType.NVarChar, Value = item.Title },
                    new SqlParameter() { ParameterName = "@StartDate", SqlDbType = SqlDbType.DateTime, Value = item.StartDate },
                    new SqlParameter() { ParameterName = "@EndDate", SqlDbType = SqlDbType.DateTime, Value = item.EndDate },

                };




                command.Parameters.AddRange(parameters.ToArray());

                command.ExecuteNonQuery();


            }

            connectionString.Close();

            parameters = null;

            return Ok(new { Message = "Record Added" });
        }
        
        [Route("GetById/{id}")]
        [HttpGet]
        public JsonResult Read(int id)
        {



            parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = id }
             };
            command = new SqlCommand("select * from Tour where Id=@Id", connectionString);

            dataTable = new DataTable();

            try
            {
                connectionString.Open();

                command.Parameters.AddRange(parameters.ToArray());

                dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    return Json(dataTable);
                }
                else
                {
                    return Json("Not Found");
                }

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            finally
            {
                connectionString.Close();
            }
        }
        

        [Route("Edit")]
        [HttpPut]
        public ActionResult Edit(int Id, string Title, DateTime StartDate, string EndDate)
        {
            try
            {

                command = new SqlCommand(@"update Tour set 
                                            [Title] = @Title,
                                            [StartDate] = @StartDate,
                                            [EndDate] = @EndDate           
                                        where Id=@Id", connectionString);
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = Id },
                    new SqlParameter() { ParameterName = "@Title", SqlDbType = SqlDbType.NVarChar, Value = Title },
                    new SqlParameter() { ParameterName = "@StartDate", SqlDbType = SqlDbType.DateTime, Value = StartDate },
                    new SqlParameter() { ParameterName = "@EndDate", SqlDbType = SqlDbType.DateTime, Value = EndDate }
                };
                connectionString.Open();

                command.Parameters.AddRange(parameters.ToArray());

                int effectedRows = command.ExecuteNonQuery();
                if (effectedRows > 0)
                {
                    return Ok(new { Message = "Record Updated" });
                }
                return BadRequest(new { Message = "Record Not found!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            finally { connectionString.Close(); }
        }
        

        [Route("Delete/{id}")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {

                 parameters = new List<SqlParameter>
                 {
                    new SqlParameter { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = id }
                 };


                command = new SqlCommand("delete from Tour where Id=@Id", connectionString);
                connectionString.Open();
                command.Parameters.AddRange(parameters.ToArray());
                int rowsEffected = command.ExecuteNonQuery();
                if (rowsEffected > 0)
                {
                    return Ok(new { Message = "Record Deleted!" });
                }
                return BadRequest(new { Message = "Record Not found!" });
            }
            catch (Exception ef)
            {
                return BadRequest(ef.Message);
            }
            finally { connectionString.Close(); }
        }
    }
}
