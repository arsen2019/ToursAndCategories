using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using ToursAndCategories.Models;

namespace ToursAndCategories.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {

        public CategoryController(IConfiguration configuration)
        {
            Configuration = configuration;
            connectionString = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        private IConfiguration Configuration;
        SqlConnection connectionString;
        SqlCommand command;
        SqlDataAdapter dataAdapter;
        DataTable dataTable;
        SqlParameter parameter;

        [HttpGet]
        public JsonResult GetAll()
        {

            dataTable = new DataTable();
            command = new SqlCommand("select * from Category", connectionString);
            connectionString.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataTable);


            connectionString.Close();
            return Json(dataTable);
        }

        [Route("Create")]
        [HttpPost]
        public ActionResult Create(Category item)
        {

            connectionString.Open();

            using (SqlCommand command = new SqlCommand(@"insert into Category (Title)
	            values
                (@Title)
                ", connectionString))
            {
                parameter =  new SqlParameter() { ParameterName = "@Title", SqlDbType = SqlDbType.NVarChar, Value = item.Title };




                command.Parameters.Add(parameter);

                command.ExecuteNonQuery();


            }

            connectionString.Close();

            parameter = null;

            return Ok(new { Message = "Record Added" });
        }

        [Route("GetById/{id}")]
        [HttpGet]
        public JsonResult Read(int id)
        {



            parameter = new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = id };




            command = new SqlCommand("select * from Category where Id=@Id", connectionString);

            dataTable = new DataTable();

            try
            {
                connectionString.Open();

                command.Parameters.Add(parameter);

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
        public ActionResult Edit(int Id, string Title)
        {
            try
            {

                command = new SqlCommand(@"update Category set 
                                            [Title] = @Title       
                                        where Id=@Id", connectionString);

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = Id },
                    new SqlParameter() { ParameterName = "@Title", SqlDbType = SqlDbType.NVarChar, Value = Title },
                    
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

                parameter = new SqlParameter { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = id };



                command = new SqlCommand("delete from Category where Id=@Id", connectionString);
                connectionString.Open();
                command.Parameters.Add(parameter);
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
