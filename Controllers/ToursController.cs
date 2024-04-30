using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using ToursAndCategories.Models;


namespace ToursAndCategories.Controllers
{
    [Route("api/[controller]")]
   // [ApiController]
    public class ToursController : Controller
    {

        public ToursController(IConfiguration configuration)
        {
            Configuration = configuration;
            connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        private IConfiguration Configuration;
        SqlConnection connection;
        SqlCommand command;
        SqlDataAdapter dataAdapter;
        DataTable dataTable;
        List<SqlParameter> parameters = null;

        [HttpGet]
        public JsonResult GetAll()
        {

            dataTable = new DataTable();
            command = new SqlCommand("select * from Tour", connection);
            connection.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataTable);


            connection.Close();
            return Json(dataTable);
        }

        [Route("Create")]
        [HttpPost]
        public ActionResult Create(Tour item, List<string> category)
        {

            connection.Open();

            using (SqlCommand command = new SqlCommand(@"insert into Tour (Title, StartDate, EndDate)
	            values
                (@Title, @StartDate, @EndDate)
                ", connection))
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

            try
            {
                SqlCommand commandGetLastRecord = new SqlCommand("SELECT TOP 1 * FROM Tour ORDER BY Id DESC", connection);
                dataTable = new DataTable();

                dataAdapter = new SqlDataAdapter(commandGetLastRecord);
                dataAdapter.Fill(dataTable);

                int TourId = (int)dataTable.Rows[0]["Id"];

                SqlCommand addNewCategory = new SqlCommand("AddTourWithCategory", connection);

                addNewCategory.Parameters.Add(new SqlParameter() { ParameterName = "@TourId", SqlDbType = SqlDbType.Int, Value = TourId });
                addNewCategory.Parameters.Add(new SqlParameter() { ParameterName = "@CategoryName", SqlDbType = SqlDbType.NVarChar, Value = category });

                addNewCategory.CommandType = CommandType.StoredProcedure;

                int effectedRows = addNewCategory.ExecuteNonQuery();

                if (effectedRows > 0)
                {
                    return Ok(new { Message = $"Record Added with Category: {category}" });
                }
                else
                {
                    return Problem("Something went wrong!");
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
            finally
            {
                connection.Close();
            }

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
            command = new SqlCommand("select * from Tour where Id=@Id", connection);

            dataTable = new DataTable();

            try
            {
                connection.Open();

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
                connection.Close();
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
                                        where Id=@Id", connection);
                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = Id },
                    new SqlParameter() { ParameterName = "@Title", SqlDbType = SqlDbType.NVarChar, Value = Title },
                    new SqlParameter() { ParameterName = "@StartDate", SqlDbType = SqlDbType.DateTime, Value = StartDate },
                    new SqlParameter() { ParameterName = "@EndDate", SqlDbType = SqlDbType.DateTime, Value = EndDate }
                };
                connection.Open();

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
            finally { connection.Close(); }
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


                command = new SqlCommand("delete from Tour where Id=@Id", connection);
                connection.Open();
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
            finally { connection.Close(); }
        }

        [Route("FilterByCategory/{id}")]
        [HttpGet]

        public JsonResult FilterByCategory(int id)
        {
            dataTable = new DataTable();
            parameters = new List<SqlParameter>
            {
                new SqlParameter{ParameterName = "@CategoryId", SqlDbType = SqlDbType.Int, Value = id  }
            };
            try
            {
                command = new SqlCommand("FilterTourByCategory", connection);

                connection.Open();

                command.Parameters.AddRange(parameters.ToArray());

                command.CommandType = CommandType.StoredProcedure;

                dataAdapter = new SqlDataAdapter(command);
                dataAdapter.Fill(dataTable);

                if(dataTable.Rows.Count > 0)
                {
                    return Json(dataTable);
                }
                else
                {
                    return Json("No Records Found");
                }
                

            }
            catch(Exception ex) 
            { 
                return Json(ex.Message);
            }
            finally { connection.Close(); }

        }
    }
}
