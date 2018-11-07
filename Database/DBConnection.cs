using System;
using System.Data;
using System.Data.SqlClient;
using TimeManager.Models;

namespace TimeManager.Database
{
  public class DBConnection
  {

    //Created with x.udl
    private static String ConnString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TimeManager;Data Source=localhost\\Home";
    public static UserModel CurrentUser;

    public static Boolean Login(String username, String password)
    {
      using (SqlConnection conn = new SqlConnection(DBConnection.ConnString))
      {
        SqlCommand cmd = new SqlCommand("IF( (CONVERT(varchar(100), (DECRYPTBYPASSPHRASE('SECURE1', (SELECT [Password] FROM [dbo].[User] WHERE Username = @username))))) = @password) SELECT t0.[ID], t0.[IdPermission], t0.[Firstname], t0.[Lastname], t0.[Username], CONVERT(varchar(100), (DECRYPTBYPASSPHRASE('SECURE1', [Password]))) AS Password, t0.[Department], t1.[Level] FROM [User] AS t0 INNER JOIN [Permission] AS t1 ON t0.[IdPermission] = t1.[ID] WHERE Username = @username", conn);
        cmd.Parameters.Add(new SqlParameter("@username", SqlDbType.NVarChar) { Value = username });
        cmd.Parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar) { Value = password });

        conn.Open();
        SqlDataReader reader = cmd.ExecuteReader();

        try
        {
          while (reader.Read())
          {
            CurrentUser = new UserModel()
            {
              ID = new Guid(reader["ID"].ToString()),
              IdPermission = new Guid(reader["IdPermission"].ToString()),
              Firstname = reader["Firstname"] != DBNull.Value ? (String)reader["Firstname"] : "",
              Lastname = reader["Lastname"] != DBNull.Value ? (String)reader["Lastname"] : "",
              Username = reader["Username"] != DBNull.Value ? (String)reader["Username"] : "",
              Password = reader["Password"] != DBNull.Value ? (String)reader["Password"] : "",
              Department = reader["Department"] != DBNull.Value ? (String)reader["Department"] : "",
              Level = (Byte)reader["Level"]
            };
          }
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
        }
        finally
        {
          reader.Close();
        }
      }
      if (DBConnection.CurrentUser != null) 
        return true;
      
      return false;
    }

  }
}