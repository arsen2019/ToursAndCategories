using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using ToursAndCategories.Models;
using ToursAndCategories.BLL;


namespace ToursAndCategories.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {

        List<SqlParameter> parameter;

        [HttpGet]
        public JsonResult GetAll()
        {
            string sql = "select * from Category";
            return new ToursAndCategories_BLL().GetAllBLL(sql);
        }

        [Route("Create")]
        [HttpPost]
        public ActionResult Create(Category item)
        {
            string sql = @"insert into Category (Title)
        	            values
                        (@Title)";

            parameter = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "@Title", SqlDbType = SqlDbType.NVarChar, Value = item.Title }
            };

            return new ToursAndCategories_BLL().CreateCategoryBLL(sql, parameter);

        }       

        [Route("GetById/{id}")]
        [HttpGet]
        public JsonResult Read(int id)
        {

            parameter = new List<SqlParameter>
            {
                new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = id }
            };

            string sql = "select * from Category where Id=@Id";
           
            return new ToursAndCategories_BLL().ReadBLL(sql, parameter);
        }

        [Route("Edit")]
        [HttpPut]
        public ActionResult Edit(int Id, string Title)
        {

            string sql = @"update Category set 
                           [Title] = @Title       
                           where Id=@Id";

             parameter = new List<SqlParameter>
                        {
                            new SqlParameter() { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = Id },
                            new SqlParameter() { ParameterName = "@Title", SqlDbType = SqlDbType.NVarChar, Value = Title },

                        };

            return new ToursAndCategories_BLL().EditBLL(sql, parameter);
        }

        [Route("Delete/{id}")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            string sql = "delete from Category where Id=@Id";

            parameter = new List<SqlParameter>
            {
                new SqlParameter { ParameterName = "@Id", SqlDbType = SqlDbType.Int, Value = id }
            };

          
            return new ToursAndCategories_BLL().DeleteBLL(sql, parameter);
        }

        [Route("Delete/MultipleCategories")]
        [HttpDelete]
        public ActionResult DeleteMultiple(List<int> IDs)
        {
            string sql = "DeleteCategory";

            return new ToursAndCategories_BLL().DeleteMultipleBLL(sql, IDs);
        }
    }
}
