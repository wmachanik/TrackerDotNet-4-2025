using System;
using System.Collections;  // for Array
using System.Collections.Generic;
using System.Linq;
using System.Web;
// using TrackerDotNet.DataSets.CustomersCls;
using TrackerDotNet.DataSets.CustomersDataSetTableAdapters;

namespace TrackerDotNet.DataSets
{

  public class CustomerDAL : BaseDAL
  {

    public CustomersCls insert(ArrayList ObjBaseModels)
    {
      bool _flag = true;
      CustomersCls ObjCustomers = (CustomersCls)ObjBaseModels[0];

      try
      {

        CustomersDataSetTableAdapters.CustomersTableAdapter taCustomers = new CustomersTableAdapter();

        // if not custoemr loaded 
        if (ObjCustomers.CustomerID == 0)
        {
        }
        else
        {
          taCustomers.UpdateCustomerByID(ObjCustomers.CustomerID);
        }

        return ObjCustomers;

      }
      catch (Exception exp)
      {
        string a = exp.Message;
        _flag = false;

      }
      if (_flag)
        ObjCustomers = null;    // perhaps not?

      return ObjCustomers;

    }

  }
}