using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Interview.Green.Web.Scrapper.Tests.Unit.Test
{
    /// <summary>
    /// Basic tests for the utility test methods, to ensure we don't get false negatives by broken testing code. AKA Who watches the watchers?
    /// </summary>
    [TestClass]
    public class AssertUtilityTests
    {
        [TestMethod, TestCategory("Unit")]
        public void ThrowsExceptionTest()
        {
            // Should not fail or throw exception
            AssertUtility.ThrowsException<ArgumentException>(() => throw new ArgumentException());

            // Should not fail or throw exception
            AssertUtility.ThrowsException<ApplicationException>(() => throw new ApplicationException("Some test message"), "Some test message");

            // Should fail for not throwing any exception
            bool success = false;
            try
            {
                AssertUtility.ThrowsException<ArgumentException>(() => this.ToString());
            }
            catch(Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException tex)
            {
                if (string.Equals("Assert.Fail failed. Expected exception 'System.ArgumentException', no exception encountered.",
                        tex.Message))
                    success = true;            
            }
            if (!success)
                Assert.Fail("No exception test failed.");

            // Should fail for throwing wrong exception type
            success = false;
            try
            {
                AssertUtility.ThrowsException<ArgumentException>(() => throw new ApplicationException());
            }
            catch (Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException tex)
            {
                if (string.Equals("Assert.Fail failed. Expected exception 'System.ArgumentException', received exception 'System.ApplicationException'.",
                        tex.Message))
                    success = true;
            }
            if (!success)
                Assert.Fail("Wrong exception type test failed.");

            // Should fail for throwing wrong exception message
            success = false;
            try
            {
                AssertUtility.ThrowsException<ApplicationException>(() => throw new ApplicationException("A different test message"), "Some test message");
            }
            catch (Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException tex)
            {
                if (string.Equals("Assert.Fail failed. Expected exception message 'Some test message', received exception message 'A different test message' for 'System.ApplicationException'.",
                        tex.Message))
                    success = true;
            }
            if (!success)
                Assert.Fail("Wrong exception message test failed.");
        }
    }
}
