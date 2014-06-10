﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KarateManagement.Properties;
using MySql.Data.MySqlClient;


namespace KarateManagement
{
    /// <summary>
    /// This class is used to Create and initialize the database, as well as any other maitanance functions
    /// </summary>
    public static class SqlHelper
    {
        private static MySqlConnection m_connection;
        
        async public static Task Connect(string connectionString)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);

            Task initializeDb = null;

            bool connected = false;
            try
            {
                await connection.OpenAsync();

                m_connection = connection;
                connected = true;

                Console.WriteLine("State: {0}", connection.State);
                Console.WriteLine("ConnectionString: {0}",
                    connection.ConnectionString);

                
                
                string useDB = String.Format("Use karatemanagement;");
                MySqlCommand cmd = new MySqlCommand(useDB, m_connection);
                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine("Using KarateManagement");
            }
            catch (MySqlException e)
            {
                if(connected)
                    initializeDb = InitializeDB();
                else
                    ErrorLogger.Logger.Write(e.ToString());
            }

            if (initializeDb != null)
            {
                await initializeDb;
            }
            
        }

        /// <summary>
        /// This method should be called to create all the tables and use the table
        /// </summary>
        async private static Task InitializeDB()
        {
            //Database doesnt exist. Must create database. Disconnect and try again
            try
            {
                await CreateDB();

                string useDB = String.Format("Use karatemanagement;");
                MySqlCommand cmd = new MySqlCommand(useDB, m_connection);
                await cmd.ExecuteNonQueryAsync();

                CreateTable();
            }
            catch (Exception e)
            {
                ErrorLogger.Logger.Write(e.ToString());
            }
            
        }

        /// <summary>
        /// Creates a Database called "KarateManagement"
        /// </summary>
        /// <returns></returns>
        async private static Task CreateDB()
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(Resources.CreateDB, m_connection);
                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine("Createding DB");

            }
            catch (Exception e)
            {
                ErrorLogger.Logger.Write(e.ToString());
            }
        }


        private static void CreateTable()
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(Resources.CreateTable, m_connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                ErrorLogger.Logger.Write(e.ToString());
            }
        }

        /// <summary>
        /// Creates a student in the database
        /// </summary>
        /// <param name="student">A student object to insert</param>
        async public static Task CreateStudent(Student student)
        {
            try
            {
                string createStudent = String.Format(Resources.CreateStudent, student.ID, student.FirstName, student.LastName, student.DateOfBirth,
                student.Address, student.PostalCode, student.PhoneNumber, student.Email, student.Hours, student.Belt, student.Balance, student.MembershipEndDate);
                MySqlCommand cmd = new MySqlCommand(createStudent, m_connection);
                Task<int> t = cmd.ExecuteNonQueryAsync();

                await t;
            }
            catch (Exception e)
            {
                ErrorLogger.Logger.Write(e.ToString());
            }
        }

        /// <summary>
        /// Gets the Highest Student ID in the database
        /// </summary>
        /// <returns></returns>
        async public static Task<int> GetHighestID()
        {
            string query = "Select MAX(ID) as ID from Student";
            MySqlCommand cmd = new MySqlCommand(query, m_connection);
            int id = 0;
            try
            {
                Task<object> t = cmd.ExecuteScalarAsync();
                var obj = await t;
                
                if (DBNull.Value.Equals(obj))
                {
                    id = 0;
                }
                else
                {
                    id = Convert.ToInt32(obj);
                }
            }
            catch (InvalidOperationException e)
            {
                ErrorLogger.Logger.Write(e.ToString());
            }
           
            return id;
        }

        /// <summary>
        /// Deletes a student from the database
        /// </summary>
        /// <param name="id">ID of the student to delete</param>
        /// <returns>A task that can be awaited</returns>
        async public static Task DeleteStudent(int id)
        {
            String script = "Delete From student where ID = {0}";
            String deleteStudent = String.Format(script, id);
            MySqlCommand cmd = new MySqlCommand(deleteStudent, m_connection);
            try
            {
                Task<int> t = cmd.ExecuteNonQueryAsync();

                await t;
            }
            catch (Exception e)
            {
                ErrorLogger.Logger.Write(e.ToString());
            }
        }


        /// <summary>
        /// Gets a Student object from the database
        /// </summary>
        /// <param name="id">The ID of the student</param>
        /// <returns>A student object with all its fields populated</returns>
        async public static Task<Student> GetStudent(int id)
        {
            string script = String.Format("select * from student where id = {0}", id);
            MySqlCommand cmd = new MySqlCommand(script, m_connection);
            DbDataReader reader;
            DataTable dt = new DataTable();
            Student s = new Student();

            try
            {
                reader = await cmd.ExecuteReaderAsync();

                dt.Load(reader);

                foreach (DataRow row in dt.Rows)
                {
                    s.ID = Convert.ToInt32(row["ID"].ToString());
                    s.FirstName = row["FirstName"].ToString();
                    s.LastName = row["LastName"].ToString();
                    s.DateOfBirth = Convert.ToDateTime(row["DateOfBirth"]);
                    s.Address = row["Address"].ToString();
                    s.PhoneNumber = s.Address = row["PhoneNumber"].ToString();
                    s.PostalCode = row["PostalCode"].ToString();
                    s.Email = row["Email"].ToString();
                    s.Hours = Convert.ToInt32(row["Hours"].ToString());
                    s.Belt = (Belt)Convert.ToInt32(row["Belt"].ToString());
                    s.Balance = Convert.ToDecimal(row["Balance"].ToString());
                    s.MembershipEndDate = Convert.ToDateTime(row["MembershipEndDate"]);
                }
            }
            catch (Exception e)
            {
                ErrorLogger.Logger.Write(e.ToString());
            }

            return s;
        }


        /*
         * async public static Task Update()
         * INSERT into mytable (logins) 
         * SELECT max(logins) + 1 
         * FROM mytable
         */

        
    }
}
