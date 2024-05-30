using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RolePermission
{
    class Program
    {
        public SqlConnection con = null;
        public void DbConnect()
        {
            try
            {
                // Creating Connection  
                con = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=user_registration2;Integrated Security=True");
            }
            catch (Exception e)
            {
                Console.WriteLine("OOPs, something went wrong.\n" + e.Message);
                Console.ReadKey();
            }
        }
        public void changePermission(int role)
        {
            int rolechoice = 0;
            int prid = 0;
            int changepermission = 0;
            int selectop = 0;
            Console.WriteLine("select one : 1.add permission to role wise \t 2.add special permission to user ");
            selectop = Convert.ToInt32(Console.ReadLine());
            if (selectop == 1)
            {
                Console.Write("1.admin \t 2.manager \t 3.user \t select one : ");
                rolechoice = Convert.ToInt32(Console.ReadLine());
                if (rolechoice >= 1 && rolechoice < 4)
                {
                    Console.WriteLine("role has following permissions");
                    try
                    {
                        DbConnect();
                        con.Open();
                        SqlCommand cmd = new SqlCommand("exec show_role_permission @role", con);
                        cmd.Parameters.AddWithValue("@role", rolechoice);
                        SqlDataReader data = cmd.ExecuteReader();
                        if (data.HasRows)
                        {
                            while (data.Read())
                            {
                                Console.WriteLine(data["permissionid"] + ". " + data["permsission"]);
                                Console.WriteLine("-----------------------------------------------");
                            }
                            Console.Write("select choice : 1.assign new permission \t 2.remove permission");
                            changepermission = Convert.ToInt32(Console.ReadLine());
                        }
                        else
                        {
                            Console.WriteLine("enter valid data");
                        }
                        data.Close();
                        if (changepermission == 1 || changepermission == 2)
                        {
                            SqlCommand cmd1 = new SqlCommand("exec show_available_permission", con);
                            SqlDataReader pdata = cmd1.ExecuteReader();
                            if (pdata.HasRows)
                            {
                                Console.WriteLine("you have following permission :");

                                while (pdata.Read())
                                {
                                    Console.WriteLine(pdata["permission_id"] + ". " + pdata["permsission"]);
                                }
                                Console.WriteLine("-----------------------------------------------");
                                Console.WriteLine("select permission : ");
                                prid = Convert.ToInt32(Console.ReadLine());
                            }
                            else
                            {
                                Console.WriteLine("select valid choice");
                            }
                            pdata.Close();
                            if (prid > 0 && prid < 9)
                            {
                                SqlCommand cmd2 = new SqlCommand("exec change_role_permission @role,@change,@prid", con);
                                cmd2.Parameters.AddWithValue("@role", rolechoice);
                                cmd2.Parameters.AddWithValue("@change", changepermission);
                                cmd2.Parameters.AddWithValue("@prid", prid);
                                int num = cmd2.ExecuteNonQuery();
                                if (num > 0)
                                {
                                    Console.WriteLine("changed successsfully");
                                }
                                else
                                {
                                    Console.WriteLine("something wrong");
                                }

                            }
                        }
                        con.Close();
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("enter valid choice");
                }
            }
            else if (selectop == 2)
            {
                try
                {
                    Console.WriteLine("there are following user");
                    showUsers(role);
                    DbConnect();
                    con.Open();
                    Console.WriteLine("enter user id of user");
                    int uid = Convert.ToInt32(Console.ReadLine());
                    SqlCommand cmd2 = new SqlCommand("exec show_user_permission @userid", con);
                    cmd2.Parameters.AddWithValue("@userid", uid);
                    SqlDataReader sppermissions = cmd2.ExecuteReader();
                    if (sppermissions.HasRows)
                    {
                        Console.WriteLine("users special permission :");
                        while (sppermissions.Read())
                        {
                            Console.WriteLine(sppermissions["permissionid"] + ". " + sppermissions["permsission"]);
                        }
                    }
                    else
                    {
                        Console.WriteLine("you  have no special permission");
                    }
                    Console.Write("select choice : 1.assign new permission \t 2.remove permission");
                    changepermission = Convert.ToInt32(Console.ReadLine());
                    sppermissions.Close();
                    if (changepermission == 1 || changepermission == 2)
                        {
                            SqlCommand cmd1 = new SqlCommand("exec show_available_permission", con);
                            SqlDataReader pdata = cmd1.ExecuteReader();
                            if (pdata.HasRows)
                            {
                                Console.WriteLine("you have following permission :");

                                while (pdata.Read())
                                {
                                    Console.WriteLine(pdata["permission_id"] + ". " + pdata["permsission"]);
                                }
                                Console.WriteLine("-----------------------------------------------");
                                Console.WriteLine("select permission : ");
                                prid = Convert.ToInt32(Console.ReadLine());
                            }
                            else
                            {
                                Console.WriteLine("select valid choice");
                            }
                            pdata.Close();
                            if (prid > 0 && prid < 9)
                            {
                                SqlCommand cmd3 = new SqlCommand("exec give_user_permission @userid,@change,@prid", con);
                                cmd3.Parameters.AddWithValue("@userid", uid);
                                cmd3.Parameters.AddWithValue("@change", changepermission);
                                cmd3.Parameters.AddWithValue("@prid", prid);
                                int num = cmd3.ExecuteNonQuery();
                                if (num > 0)
                                {
                                    Console.WriteLine("changed successsfully");
                                }
                                else
                                {
                                    Console.WriteLine("something wrong");
                                }

                            }
                        }
                        con.Close();
                    
                    
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    
                }

            }
        }
        public void DeleteUser()
        {
            Console.Write("enter user id for delete user : ");
            int userid = Convert.ToInt32(Console.ReadLine());
            try
            {
                DbConnect();
                con.Open();
                SqlCommand cmd = new SqlCommand("exec delete_user @userid", con);
                cmd.Parameters.AddWithValue("@userid", userid);
                int num = cmd.ExecuteNonQuery();
                if (num > 0)
                {
                    Console.Write("user deleted successfully");
                }
                else
                {
                    Console.WriteLine("user not deleted");
                }
                con.Close();
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void updatedetail(int userid)
        {
            ShowOwnDetail(userid);
            Console.WriteLine("enter details for update");
            Console.WriteLine("enter firstname");
            String fname = Console.ReadLine();
            Console.WriteLine("enter lastname");
            String lname = Console.ReadLine();
            Console.WriteLine("enter email");
            String email = Console.ReadLine();
            Console.WriteLine("enter mobileno");
            String mobileno = Console.ReadLine();
            Console.WriteLine("enter city");
            String city = Console.ReadLine();
            Console.WriteLine("enter state");
            String state = Console.ReadLine();

            try
            {
                DbConnect();
                con.Open();
                SqlCommand cm = new SqlCommand("exec update_user @fname,@lname,@email,@mobileno,@city,@state,@userid", con);
                cm.Parameters.AddWithValue("@fname", fname);
                cm.Parameters.AddWithValue("@lname", lname);
                cm.Parameters.AddWithValue("@email", email);
                cm.Parameters.AddWithValue("@mobileno", mobileno);
                cm.Parameters.AddWithValue("@city", city);
                cm.Parameters.AddWithValue("@state", state);
                cm.Parameters.AddWithValue("@userid", userid);
                // Executing the SQL query  
                int num = cm.ExecuteNonQuery();
                if (num > 0)
                {
                    Console.WriteLine("details updated successfully");
                }
                else
                {
                    Console.WriteLine("not added");
                }
                con.Close();
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("OOPs, something went wrong.\n" + e.Message);
                Console.ReadKey();
            }

        }
        public void changePassword(int userid)
        {
            Console.WriteLine("enter neew Password");
            String password = Console.ReadLine();
            try
            {
                DbConnect();
                con.Open();
                SqlCommand cmd = new SqlCommand("exec change_password @userid,@password", con);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@password", password);
                int num = cmd.ExecuteNonQuery();
                if (num > 0)
                {
                    Console.WriteLine("Password changed");
                }
                else
                {
                    Console.WriteLine("password not changed");
                }
                Console.ReadKey();
                con.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void AddUser()
        {
            Console.Write("enter username : ");
            String username = Console.ReadLine();
            Console.Write("enter Password : ");
            String password = Console.ReadLine();
            String salt = "abcd";
            Console.Write("select role : 1.admin \t 2.manager \t 3.user : ");
            int role = Convert.ToInt32(Console.ReadLine());
            try
            {
                // Creating Connection  
                DbConnect();
                con.Open();
                SqlCommand cmd = new SqlCommand("exec create_user @username,@password,@salt,@role", con);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@salt", salt);
                cmd.Parameters.AddWithValue("@role", role);
                int num = cmd.ExecuteNonQuery();
                if (num > 0)
                {
                    Console.WriteLine("user added successfully");
                }
                else
                {
                    Console.WriteLine("user not added");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("OOPs, something went wrong.\n" + e.Message);
                Console.ReadKey();
            }
            con.Close();
        }
        public void showUsers(int role)
        {
            try
            {
                DbConnect();
                con.Open();
                SqlCommand cmd = new SqlCommand("exec show_users @role", con);
                cmd.Parameters.AddWithValue("@role", role);
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        for (int i = 0; i < data.FieldCount; i++)
                        {
                            Console.WriteLine(data.GetName(i) + " : " + data[data.GetName(i)]);
                        }
                        Console.WriteLine("-----------------------------------------------");
                    }
                }
                else
                {
                    Console.WriteLine("no users");
                }
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
            con.Close();
        }
        public void ShowOwnDetail(int userid)
        {
            try
            {
                DbConnect();
                con.Open();
                SqlCommand cmd = new SqlCommand("exec show_Own_detail @userid", con);
                cmd.Parameters.AddWithValue("@userid", userid);
                SqlDataReader data = cmd.ExecuteReader();
                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        Console.WriteLine("-----------------------------------------------");
                        for (int i = 0; i < data.FieldCount; i++)
                        {
                            Console.WriteLine(data.GetName(i) + " : " + data[data.GetName(i)]);
                        }
                        Console.WriteLine("-----------------------------------------------");
                    }
                }
                else
                {
                    Console.WriteLine("no users");
                }
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }
            con.Close();
        }
        public void AddOwnDetail(int userid)
        {
            Console.WriteLine("enter firstname");
            String fname = Console.ReadLine();
            Console.WriteLine("enter lastname");
            String lname = Console.ReadLine();
            Console.WriteLine("enter email");
            String email = Console.ReadLine();
            Console.WriteLine("enter mobileno");
            String mobileno = Console.ReadLine();
            Console.WriteLine("enter city");
            String city = Console.ReadLine();
            Console.WriteLine("enter state");
            String state = Console.ReadLine();

            try
            {

                DbConnect();
                con.Open();

                SqlCommand cm = new SqlCommand("exec add_own_details @fname,@lname,@email,@mobileno,@city,@state,@userid", con);
                cm.Parameters.AddWithValue("@fname", fname);
                cm.Parameters.AddWithValue("@lname", lname);
                cm.Parameters.AddWithValue("@email", email);
                cm.Parameters.AddWithValue("@mobileno", mobileno);
                cm.Parameters.AddWithValue("@city", city);
                cm.Parameters.AddWithValue("@state", state);
                cm.Parameters.AddWithValue("@userid", userid);
                // Executing the SQL query  
                int num = cm.ExecuteNonQuery();
                if (num > 0)
                {
                    Console.WriteLine("details added successfully");
                }
                else
                {
                    Console.WriteLine("not added");
                }
                con.Close();
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("OOPs, something went wrong.\n" + e.Message);
                Console.ReadKey();
            }
        }
        static void Main(string[] args)
        {
            Program p1 = new Program();
            int role = 0;
            int userid = 0;
            List<int> permissionid = new List<int>();
            Console.WriteLine("------------welcome to user registration-----------------");
            Console.Write("enter username : ");
            String username = Console.ReadLine();
            Console.Write("enter password : ");
            String password = Console.ReadLine();
            try
            {
                int i = 1;
                int uch = 1;
                while (uch == 1)
                {
                    p1.DbConnect();
                    p1.con.Open();
                    SqlCommand cmd = new SqlCommand("exec check_user @username,@password", p1.con);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    SqlDataReader userpermissions = cmd.ExecuteReader();

                    if (userpermissions.HasRows)
                    {
                        while (userpermissions.Read())
                        {
                            if (i == 1)
                            {
                                role = Convert.ToInt32(userpermissions["role"].ToString());
                                userid = Convert.ToInt32(userpermissions["userid"].ToString());
                            }
                            Console.WriteLine(userpermissions["permissionid"] + ". " + userpermissions["permsission"]);
                            permissionid.Add(Convert.ToInt32(userpermissions["permissionid"].ToString()));
                            i++;
                        }
                    }

                    else
                    {
                        Console.WriteLine("enter valid data");
                        Console.ReadKey();
                        Environment.Exit(1);
                    }
                    userpermissions.Close();
                    if (userid > 0)
                    {
                        SqlCommand cmd2 = new SqlCommand("exec show_user_permission @userid", p1.con);
                        cmd2.Parameters.AddWithValue("@userid", userid);
                        SqlDataReader sppermissions = cmd2.ExecuteReader();
                        if (sppermissions.HasRows)
                        {
                            Console.WriteLine("users special permission :");
                            while (sppermissions.Read())
                            {
                                Console.WriteLine(sppermissions["permissionid"] + ". " + sppermissions["permsission"]);
                                permissionid.Add(Convert.ToInt32(sppermissions["permissionid"].ToString()));
                            }
                        }

                        else
                        {
                            Console.WriteLine("you  have no special permission");
                        }

                    }
                    Console.WriteLine("select choice");
                        int ch = Convert.ToInt32(Console.ReadLine());
                        switch (ch)
                        {
                            case 1:
                                if (permissionid.Contains(ch))
                                {
                                    p1.AddUser();
                                }
                                else
                                {
                                    Console.WriteLine("enter valid choice");
                                }
                                break;
                            case 2:
                                if (permissionid.Contains(ch))
                                {
                                    p1.updatedetail(userid);
                                }
                                else
                                {
                                    Console.WriteLine("enter valid choice");
                                }
                                break;
                            case 3:
                                if (permissionid.Contains(ch))
                                {
                                    p1.DeleteUser();
                                }
                                else
                                {
                                    Console.WriteLine("enter valid choice");
                                }
                                break;
                            case 4:
                                if (permissionid.Contains(ch))
                                {
                                    p1.showUsers(role);
                                }
                                else
                                {
                                    Console.WriteLine("enter valid choice");
                                }
                                break;
                            case 5:
                                if (permissionid.Contains(ch))
                                {
                                    p1.ShowOwnDetail(userid);
                                }
                                else
                                {
                                    Console.WriteLine("enter valid choice");
                                }
                                break;
                            case 6:
                                if (permissionid.Contains(ch))
                                {
                                    p1.changePassword(userid);
                                }
                                else
                                {
                                    Console.WriteLine("enter valid choice");
                                }
                                break;
                            case 7:
                                if (permissionid.Contains(ch))
                                {
                                    Console.WriteLine("add role");
                                }
                                else
                                {
                                    Console.WriteLine("enter valid choice");
                                }
                                break;
                            case 8:
                                if (permissionid.Contains(ch))
                                {
                                    Console.WriteLine("add permission");
                                    p1.changePermission(role);
                                }
                                else
                                {
                                    Console.WriteLine("enter valid choice");
                                }
                                break;
                            case 9:
                                if (permissionid.Contains(ch))
                                {
                                    p1.AddOwnDetail(userid);
                                }
                                else
                                {
                                    Console.WriteLine("enter valid choice");
                                }
                                break;
                            default:
                                Environment.Exit(1);
                                break;
                        }
                        Console.WriteLine("do you wish to continue 1/0");
                        uch = Convert.ToInt32(Console.ReadLine());
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
        }
        }
    }
