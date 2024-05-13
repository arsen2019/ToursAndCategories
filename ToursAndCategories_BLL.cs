using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;


namespace ToursAndCategories.BLL
{
    public class ToursAndCategories_BLL : Controller
    {

        private static readonly SqlConnection _connection = new SqlConnection("Server=(localdb)\\Tours; Database=ToursAndCategories; Trusted_connection=True;");

        public JsonResult GetAllBLL(string sql)
        {
            DataTable DataTable = new DataTable();
            SqlCommand Command = new SqlCommand(sql, _connection);
            _connection.Open();

            SqlDataAdapter DataAdapter = new SqlDataAdapter(Command);
            DataAdapter.Fill(DataTable);

            _connection.Close();

            return Json(DataTable);

        }

        public ActionResult CreateBLL(string sql, List<SqlParameter> Parameters, string category, string sqlCategory = "", string procedure = "")
        {

            try
            {
                _connection.Open();
                using (SqlCommand Command = new SqlCommand(sql, _connection))
                {
                    Command.Parameters.AddRange(Parameters.ToArray());

                    Command.ExecuteNonQuery();
                }
                SqlCommand commandGetLastRecord = new SqlCommand(sqlCategory, _connection);
                DataTable DataTable = new DataTable();

                SqlDataAdapter DataAdapter = new SqlDataAdapter(commandGetLastRecord);
                DataAdapter.Fill(DataTable);

                int TourId = (int)DataTable.Rows[0]["Id"];

                SqlCommand addNewCategory = new SqlCommand(procedure, _connection);

                addNewCategory.Parameters.Add(new SqlParameter() { ParameterName = "@TourId", SqlDbType = SqlDbType.Int, Value = TourId });
                addNewCategory.Parameters.Add(new SqlParameter() { ParameterName = "@CategoryName", SqlDbType = SqlDbType.NVarChar, Value = category });


                addNewCategory.CommandType = CommandType.StoredProcedure;

                int effectedRows = addNewCategory.ExecuteNonQuery();

                if (effectedRows > 0)
                {
                    return Ok(new { Message = $"Record Added with Category" });
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
                _connection.Close();
            }

        }

        public ActionResult CreateCategoryBLL(string sql, List<SqlParameter> Parameters)
        {
            _connection.Open();

            using (SqlCommand Command = new SqlCommand(sql, _connection))
            {
                Command.Parameters.AddRange(Parameters.ToArray());

                Command.ExecuteNonQuery();
            }

            _connection.Close();

            return Ok(new { Message = "Record Added" });
        }

        public JsonResult ReadBLL(string sql, List<SqlParameter> Parameters)
        {
            SqlCommand Command = new SqlCommand(sql, _connection);
            DataTable DataTable = new DataTable();

            try
            {
                _connection.Open();

                Command.Parameters.AddRange(Parameters.ToArray());

                SqlDataAdapter DataAdapter = new SqlDataAdapter(Command);
                DataAdapter.Fill(DataTable);

                if (DataTable.Rows.Count > 0)
                {
                    return Json(DataTable);
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
                _connection.Close();
            }
        }

        public ActionResult EditBLL(string sql, List<SqlParameter> Parameters)
        {
            try
            {

                SqlCommand Command = new SqlCommand(sql, _connection);

                _connection.Open();

                Command.Parameters.AddRange(Parameters.ToArray());

                int effectedRows = Command.ExecuteNonQuery();
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
            finally { _connection.Close(); }
        }


        public ActionResult DeleteBLL(string sql, List<SqlParameter> Parameters)
        {
            try
            {
                SqlCommand Command = new SqlCommand(sql, _connection);
                _connection.Open();
                Command.Parameters.AddRange(Parameters.ToArray());
                int rowsEffected = Command.ExecuteNonQuery();
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
            finally { _connection.Close(); }
        }

        public JsonResult FilterByCategoryBLL(string sql, List<SqlParameter> Parameters)
        {
            DataTable DataTable = new DataTable();


            try
            {
                SqlCommand Command = new SqlCommand(sql, _connection);

                _connection.Open();

                Command.Parameters.AddRange(Parameters.ToArray());

                Command.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter DataAdapter = new SqlDataAdapter(Command);
                DataAdapter.Fill(DataTable);

                if (DataTable.Rows.Count > 0)
                {
                    return Json(DataTable);
                }
                else
                {
                    return Json("No Records Found");
                }


            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            finally { _connection.Close(); }

        }

        public ActionResult DeleteMultipleBLL(string sql, List<int> IDs)
        {
            try
            {


                _connection.Open();

                foreach (int ID in IDs)
                {
                    SqlCommand Command = new SqlCommand(sql, _connection); ;
                    Command.CommandType = CommandType.StoredProcedure;
                    Command.Parameters.AddWithValue("@CategoryId", ID);
                    Command.ExecuteNonQuery();

                }


                return Ok(new { Message = $"Deleted {IDs.Count} Records!" });


            }
            catch (Exception ef)
            {
                return BadRequest(ef);
            }
            finally { _connection.Close(); }
        }
    }
}
