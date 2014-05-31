﻿using System;
using System.Collections.Generic;
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
            
            await connection.OpenAsync();

            m_connection = connection;

            Console.WriteLine("State: {0}", connection.State);
            Console.WriteLine("ConnectionString: {0}",
                connection.ConnectionString);

            Task initializeDb = null;
            try
            {
                string useDB = String.Format("Use karatemanagement;");
                MySqlCommand cmd = new MySqlCommand(useDB, m_connection);
                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine("Using KarateManagement");
            }
            catch (MySqlException e)
            {
                initializeDb = InitializeDB();
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
                CreateDB();

                string useDB = String.Format("Use karatemanagement;");
                MySqlCommand cmd = new MySqlCommand(useDB, m_connection);
                await cmd.ExecuteNonQueryAsync();

                CreateTable();
            }
            catch (Exception e)
            {
                //TODO Log, System.Exit? Cant connect to DB
                throw;
            }
            
        }

        private static void CreateDB()
        {
            try
            {
                string createDB = String.Format(Resources.CreateDB, "KarateManagement"); 
                MySqlCommand cmd = new MySqlCommand(createDB, m_connection);
                cmd.ExecuteNonQuery();

                Console.WriteLine("Created DB");

            }
            catch (Exception e)
            {
                //TODO; Maybe log the exception
                throw e;
            }
        }

        private static void CreateTable()
        {
            try
            {
                string createTable = String.Format(Resources.CreateTable, "KarateManagement");
                MySqlCommand cmd = new MySqlCommand(createTable, m_connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //TODO; Maybe log the exception
                throw e;
            }
        }

        /// <summary>
        /// Creates a student in the database
        /// </summary>
        /// <param name="student">A student object to insert</param>
        async public static Task CreateStudent(Student student)
        {
            string createStudent = String.Format(Resources.CreateStudent, student.ID, student.FirstName, student.LastName, student.DateOfBirth,
                student.Address, student.PostalCode, student.PhoneNumber, student.Email, student.Hours, student.Belt, student.Balance, student.MembershipEndDate);
            MySqlCommand cmd = new MySqlCommand(createStudent, m_connection);
            Task<int> t = cmd.ExecuteNonQueryAsync();

            await t;

        }

        /// <summary>
        /// Gets the Highest Student ID in the database
        /// </summary>
        /// <returns></returns>
        async public static Task<int> GetHighestID()
        {
            string query = "Select MAX(ID) as ID from Student";
            MySqlCommand cmd = new MySqlCommand(query, m_connection);
            Task<object> t = cmd.ExecuteScalarAsync();
            int id = Convert.ToInt32(await t);
            return id;

        }

        public static void Read(int[] id)
        {
            
        }

        public static void Update(int id)
        {
            
        }

        public static void Delete(int id)
        {
            
        }
    }
}
