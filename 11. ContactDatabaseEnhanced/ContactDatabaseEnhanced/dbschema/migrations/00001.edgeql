CREATE MIGRATION m1qiniw2anbmd5n5ojq5zu64p3b2qxihwtdp7zgtgknjgj2fjxsvma
    ONTO initial
{
  CREATE TYPE default::Contact {
      CREATE REQUIRED PROPERTY birth_date: std::str;
      CREATE PROPERTY description: std::str;
      CREATE REQUIRED PROPERTY email: std::str;
      CREATE REQUIRED PROPERTY first_name: std::str;
      CREATE REQUIRED PROPERTY last_name: std::str;
      CREATE REQUIRED PROPERTY marital_status: std::bool;
      CREATE REQUIRED PROPERTY title: std::str;
  };
};
