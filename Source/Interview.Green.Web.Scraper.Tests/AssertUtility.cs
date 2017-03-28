using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Interview.Green.Web.Scrapper.Tests
{
    /// <summary>
    /// A collection of test helper methods.
    /// </summary>
    public static class AssertUtility
    {
        /// <summary>
        /// Mimics assert throw functionality found in other test frameworks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        public static void ThrowsException<T>(Action action)
            where T : Exception
        {
            try
            {
                action.Invoke();
            }
            catch(T)
            {
                return;
            }
            catch(Exception ex)
            {
                Assert.Fail("Expected exception '{0}', received exception '{1}'.", typeof(T), ex.GetType());
            }

            Assert.Fail(string.Format("Expected exception '{0}', no exception encountered.", typeof(T)));
        }

        /// <summary>
        /// Mimics assert throw functionality found in other test frameworks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="expectedMessage"></param>
        public static void ThrowsException<T>(Action action, string expectedMessage)
            where T : Exception
        {
            try
            {
                action.Invoke();
            }
            catch (T tx)
            {
                if (string.Equals(expectedMessage, tx.Message))
                    return;

                Assert.Fail("Expected exception message '{0}', received exception message '{1}' for '{2}'.", expectedMessage, tx.Message, typeof(T));
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected exception '{0}', received exception '{1}'.", typeof(T), ex.GetType());
            }

            Assert.Fail(string.Format("Expected exception '{0}', no exception encountered.", typeof(T)));
        }
    }
}
