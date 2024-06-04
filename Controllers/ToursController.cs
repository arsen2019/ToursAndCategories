using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using ToursAndCategories.BLL;
using ToursAndCategories.Models;



namespace ToursAndCategories.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToursController : Controller
    {
        List<SqlParameter> parameters = null;

        [HttpGet]
        public JsonResult GetAll()
        {
            string sql = "select * from Tour";
            return new ToursAndCategories_BLL().GetAllBLL(sql);
        }

        [Route("Create")]
        [HttpPost]
        public ActionResult Create(Tour item, string category)
        {

            string sql = @"insert into Tour (Title, StartDate, EndDate)
	            values
                (@Title, @StartDate, @EndDate)
                ";

            string sqlCategory = "SELECT TOP 1 * FROM Tour ORDER BY Id DESC";

            string procedure = "AddTourWithCategory";

            List<SqlParameter> parameters = new List<SqlParameter>
                {

                    new SqlParameter() { ParameterName = "@Title", SqlDbType = SqlDbType.NVarChar, Value = item.Title },
                    new SqlParameter() { ParameterName = "@StartDate", SqlDbType = SqlDbType.DateTime, Value = item.StartDate },
                    new SqlParameter() { ParameterName = "@EndDate", SqlDbType = SqlDbType.DateTime, Value = item.EndDate },
                    
                };

            return new ToursAndCategories_BLL().CreateBLL(sql, parameters, category, sqlCategory, procedure);
        
    }
        
        [Route("GetById/{id}")]
        [HttpGet]
        public JsonResult Read(int id)
        {
            parameters = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = id }
             };

            string sql = "select * from Tour where Id=@Id";

            return new ToursAndCategories_BLL().ReadBLL(sql, parameters);
         

        }
        

        [Route("Edit")]
        [HttpPut]
        public ActionResult Edit(int Id, string Title, DateTime StartDate, DateTime EndDate)
        {
            string sql = @"update Tour set 
                                            [Title] = @Title,
                                            [StartDate] = @StartDate,
                                            [EndDate] = @EndDate           
                                        where Id=@Id";

            List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = Id },
                    new SqlParameter() { ParameterName = "@Title", SqlDbType = SqlDbType.NVarChar, Value = Title },
                    new SqlParameter() { ParameterName = "@StartDate", SqlDbType = SqlDbType.DateTime, Value = StartDate },
                    new SqlParameter() { ParameterName = "@EndDate", SqlDbType = SqlDbType.DateTime, Value = EndDate }
                };
            return new ToursAndCategories_BLL().EditBLL(sql, parameters);
        }
        

        [Route("Delete/{id}")]
        [HttpDelete, Authorize]
        public ActionResult Delete(int id)
        {

            string sql = "delete from Tour where Id=@Id";

            parameters = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = id }
            };

            return new ToursAndCategories_BLL().DeleteBLL(sql, parameters);    
        }

        [Route("FilterByCategory/{id}")]
        [HttpGet]

        public JsonResult FilterByCategory(int id)
        {
            string sql = "FilterTourByCategory";
            parameters = new List<SqlParameter>
            {
                new SqlParameter{ParameterName = "@CategoryId", SqlDbType = SqlDbType.Int, Value = id  }
            };

            return new ToursAndCategories_BLL().FilterByCategoryBLL(sql, parameters);
   

        }
    }
}
