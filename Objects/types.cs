using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace AnimalShelter
{
  public class Type
  {
    private int _id;
    private string _nameOfType;

    public Type(string Name, int Id = 0)
    {
      _id = Id;
      _nameOfType = Name;
    }

    public override bool Equals(System.Object otherType)
    {
        if (!(otherType is Type))
        {
          return false;
        }
        else
        {
          Type newType = (Type) otherType;
          bool idEquality = this.GetId() == newType.GetId();
          bool nameOfTypeEquality = this.GetName() == newType.GetName();
          return (idEquality && nameOfTypeEquality);
        }
    }

    public int GetId()
    {
      return _id;
    }
    public string GetName()
    {
      return _nameOfType;
    }
    public void SetName(string newName)
    {
      _nameOfType = newName;
    }
    public static List<Type> GetAll()
    {
      List<Type> allTypes = new List<Type>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM types;", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int typeId = rdr.GetInt32(0);
        string typeName = rdr.GetString(1);
        Type newType = new Type(typeName, typeId);
        allTypes.Add(newType);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }

      return allTypes;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO types (name_of_type) OUTPUT INSERTED.id VALUES (@TypeName);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@TypeName";
      nameParameter.Value = this.GetName();
      cmd.Parameters.Add(nameParameter);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if(conn != null)
      {
        conn.Close();
      }
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM types;", conn);
      cmd.ExecuteNonQuery();
    }

    public static Type Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM types WHERE id = @TypeId;", conn);
      SqlParameter typeIdParameter = new SqlParameter();
      typeIdParameter.ParameterName = "@TypeId";
      typeIdParameter.Value = id.ToString();
      cmd.Parameters.Add(typeIdParameter);
      rdr = cmd.ExecuteReader();

      int foundTypeId = 0;
      string foundTypeDescription = null;

      while(rdr.Read())
      {
        foundTypeId = rdr.GetInt32(0);
        foundTypeDescription = rdr.GetString(1);
      }
      Type foundType = new Type(foundTypeDescription, foundTypeId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundType;
    }

    public List<Animal> GetAnimals()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM animal WHERE type_id = @TypeId ORDER BY date_of_admittance;", conn);
      SqlParameter typeIdParameter = new SqlParameter();
      typeIdParameter.ParameterName = "@TypeId";
      typeIdParameter.Value = this.GetId();
      cmd.Parameters.Add(typeIdParameter);
      rdr = cmd.ExecuteReader();

      List<Animal> Animals = new List<Animal> {};
      while(rdr.Read())
      {
        int animalId = rdr.GetInt32(0);
        string animalDescription = rdr.GetString(1);
        int animalTypeId = rdr.GetInt32(2);
        DateTime animalDueDate = rdr.GetDateTime(3);
        string animalGender = rdr.GetString(4);
        string animalBreed = rdr.GetString(5);
        Animal newAnimal = new Animal(animalDescription, animalGender, animalBreed, animalTypeId, animalDueDate, animalId);
        Animals.Add(newAnimal);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return Animals;
    }
  }
}
