using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Web;

// base database class

namespace TrackerDotNet.DataSets
{
  public class BaseDAL
  {
    // constants
    public const string TRACKER_CONNSTR = "Tracker08ConnectionString";

    // variables
    public OleDbCommand ObjCmd;
    public OleDbConnection ObjConn;
    public OleDbDataReader ObjDR;
    public OleDbTransaction ObjTr;
    public DataSet ObjDs;
    public OleDbDataAdapter ObjDA;
    public DataTable ObjDt;


      //  protected properties exposing stuff like connection and command stuff to inherited classes
//    public abstract void Delete(int id);

//    public abstract DataTable ReadAll();

//    public abstract DataTable Read(int id);

//    public abstract bool Save(DataTable dt);


    public virtual bool OpenBaseConnection(string pSpNameStr)
    {
      try
      {
        string _ConString = System.Configuration.ConfigurationManager.ConnectionStrings[TRACKER_CONNSTR].ToString();

        ObjConn = new OleDbConnection(_ConString);
        ObjConn.Open();

        ObjCmd = new OleDbCommand();
        ObjCmd.Connection = ObjConn;
        ObjCmd.CommandType = System.Data.CommandType.TableDirect;  // at the moment this is the only option
        ObjCmd.CommandText = pSpNameStr;

        return (true);
      }
      catch (Exception exp)
      {
        string a = exp.Message;

        return (false);
      }
    }

    public virtual void BaseGarbageCollector()
    {
      if ((ObjConn != null) && (ObjConn.State == ConnectionState.Open))
      {
        ObjConn.Close();
      }
      if (ObjCmd != null)
      {
        ObjCmd = null;
      }
      if (ObjDR != null)
      {
        ObjDR.Close();
        ObjDR = null;
      }
      if (ObjDs != null)
      {
        ObjDs = null;
      }
      if (ObjDR != null)
      {
        ObjDR = null;
      }
    }

    public void KillBase()
    {
      BaseGarbageCollector();
    }
  }


}