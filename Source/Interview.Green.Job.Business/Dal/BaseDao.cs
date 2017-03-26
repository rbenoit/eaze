using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Green.Job.Business.Dal
{
    /// <summary>
    /// Base class for sql dao type implementation.
    /// </summary>
    public abstract class BaseDao
    {
        /// <summary>
        /// Gets the connection string used for this object.
        /// </summary>
        protected string ConnectionString { get; private set; }

        /// <summary>
        /// Called from a derived class during instantiation, uses 'default' connection string.
        /// </summary>
        protected BaseDao()
            : this("default")
        {
        }

        /// <summary>
        /// Called from a derived class during instantiation, uses the given connection string key to pull a specific connection string from config.
        /// </summary>
        /// <param name="connectionKey"></param>
        protected BaseDao(string connectionKey)
        {
            ConnectionString = ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString;
        }

        /// <summary>
        /// Provides a new sql connection based on the current objects connection string.
        /// </summary>
        /// <returns></returns>
        protected SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Populates a type based on the given data and hydration method.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="reader">The data reader containing the data to hydrate with.</param>
        /// <param name="hydrateMethod">A method used to populate a new instance of the given type with the given data.</param>
        /// <returns></returns>
        protected T HydrateItem<T>(IDataReader reader, Func<IDataReader, T> hydrateMethod)
        {
            return hydrateMethod(reader);
        }



    }
}
