using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.model;
using MySql.Data.MySqlClient;

namespace GameServer.dao
{
    class ResultDao
    {
        public Result GetResultByUserId(MySqlConnection conn, int userId)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from result where userid = @userid", conn);
                cmd.Parameters.AddWithValue("userid", userId);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    int totalCount = reader.GetInt32("totalcount");
                    int winCount = reader.GetInt32("winCount");
                    Result res = new Result(id, userId, totalCount, winCount);
                    return res;
                }
                else
                {
                    return new Result(-1, userId, 0, 0);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("在GetResultByUserId时出现异常");
                return null;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
