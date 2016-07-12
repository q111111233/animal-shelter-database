using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace AnimalShelter
{
  public class Animal
  {
    private int _id;
    private string _name;
    private string _gender;
    private string _breed;
    private int _TypeId;
    private DateTime _dateOfAdmittance;

    public Animal(string Name, string gender, string breed, int TypeId, DateTime dateOfAdmittance, int Id = 0)
    {
      _id = Id;
      _name = Name;
      _gender = gender;
      _breed = breed;
      _TypeId = TypeId;
      _dateOfAdmittance = dateOfAdmittance;
    }

    public override bool Equals(System.Object otherAnimal)
    {
      if (!(otherAnimal is Animal))
      {
        return false;
      }
      else
      {
        Animal newAnimal = (Animal) otherAnimal;
        bool idEquality = this.GetId() == newAnimal.GetId();
        bool nameEquality = this.GetName() == newAnimal.GetName();
        bool typeEquality = this.GetTypeId() == newAnimal.GetTypeId();
        bool dateOfAdmittanceEquality = this.GetDateOfAdmittance() == newAnimal.GetDateOfAdmittance();
        return (idEquality && nameEquality && typeEquality && dateOfAdmittanceEquality);
      }
    }

    public static Animal Find(int id)
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM animal WHERE id = @AnimalId;", conn);
      SqlParameter animalIdParameter = new SqlParameter();
      animalIdParameter.ParameterName = "@AnimalId";
      animalIdParameter.Value = id.ToString();
      cmd.Parameters.Add(animalIdParameter);
      rdr = cmd.ExecuteReader();

      int foundAnimalId = 0;
      string foundAnimalName = null;
      string foundAnimalGender = null;
      string foundAnimalBreed = null;
      int foundAnimalTypeId = 0;
      DateTime foundAnimalDateOfAdmittance = new DateTime(1900,1,1);

      while(rdr.Read())
      {
        foundAnimalId = rdr.GetInt32(0);
        foundAnimalName = rdr.GetString(1);
        foundAnimalTypeId = rdr.GetInt32(2);
        foundAnimalDateOfAdmittance = rdr.GetDateTime(3);
        foundAnimalGender = rdr.GetString(4);
        foundAnimalBreed = rdr.GetString(5);
      }
      Animal foundAnimal = new Animal(foundAnimalName,foundAnimalGender, foundAnimalBreed, foundAnimalTypeId, foundAnimalDateOfAdmittance, foundAnimalId);

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return foundAnimal;
    }

    public void Save()
    {
      SqlConnection conn = DB.Connection();
      SqlDataReader rdr;
      conn.Open();

      SqlCommand cmd = new SqlCommand("INSERT INTO animal (name, type_id, date_of_admittance, gender, breed) OUTPUT INSERTED.id VALUES (@AnimalName, @AnimalTypeId, @AnimalDateOfAdmittance, @AnimalGender, @AnimalBreed);", conn);

      SqlParameter nameParameter = new SqlParameter();
      nameParameter.ParameterName = "@AnimalName";
      nameParameter.Value = this.GetName();

      SqlParameter genderParameter = new SqlParameter();
      genderParameter.ParameterName = "@AnimalGender";
      genderParameter.Value = this.GetGender();

      SqlParameter breedParameter = new SqlParameter();
      breedParameter.ParameterName = "@AnimalBreed";
      breedParameter.Value = this.GetBreed();

      SqlParameter TypeIdParameter = new SqlParameter();
      TypeIdParameter.ParameterName = "@AnimalTypeId";
      TypeIdParameter.Value = this.GetTypeId();

      SqlParameter dateOfAdmittanceParameter = new SqlParameter();
      dateOfAdmittanceParameter.ParameterName = "@AnimalDateOfAdmittance";
      dateOfAdmittanceParameter.Value = this.GetDateOfAdmittance();

      cmd.Parameters.Add(nameParameter);
      cmd.Parameters.Add(genderParameter);
      cmd.Parameters.Add(breedParameter);
      cmd.Parameters.Add(TypeIdParameter);
      cmd.Parameters.Add(dateOfAdmittanceParameter);

      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        this._id = rdr.GetInt32(0);
      }

      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
    }

    public int GetId()
    {
      return _id;
    }
    public string GetName()
    {
      return _name;
    }
    public void SetName(string newName)
    {
      _name = newName;
    }
    public string GetGender()
    {
      return _gender;
    }
    public void SetGender(string newGender)
    {
      _gender = newGender;
    }
    public string GetBreed()
    {
      return _breed;
    }
    public void SetBreed(string newBreed)
    {
      _breed = newBreed;
    }
    public int GetTypeId()
    {
      return _TypeId;
    }
    public void SetTypeId(int newTypeId)
    {
      _TypeId = newTypeId;
    }
    public DateTime GetDateOfAdmittance()
    {
      return _dateOfAdmittance;
    }
    public static List<Animal> GetAll(string order)
    {
      List<Animal> AllAnimals = new List<Animal>{};

      SqlConnection conn = DB.Connection();
      SqlDataReader rdr = null;
      conn.Open();

      SqlCommand cmd = new SqlCommand("SELECT * FROM animal ORDER BY "+order+";", conn);
      rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        int animalId = rdr.GetInt32(0);
        string animalName = rdr.GetString(1);
        int animalTypeId = rdr.GetInt32(2);
        DateTime animalDateOfAdmittance = rdr.GetDateTime(3);
        string animalGender = rdr.GetString(4);
        string animalBreed = rdr.GetString(5);
        Animal newAnimal = new Animal(animalName, animalGender, animalBreed, animalTypeId, animalDateOfAdmittance, animalId);
        AllAnimals.Add(newAnimal);
      }
      if (rdr != null)
      {
        rdr.Close();
      }
      if (conn != null)
      {
        conn.Close();
      }
      return AllAnimals;
    }

    public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM animal;", conn);
      cmd.ExecuteNonQuery();
    }
  }
}
