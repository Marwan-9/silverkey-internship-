CREATE MIGRATION m1towrctqp2t6dkwt7kcshfy3fdnf27l4flh4435kzl5ory4sew22a
    ONTO initial
{
  CREATE TYPE default::Contact {
      CREATE REQUIRED PROPERTY birth_date: std::str;
      CREATE PROPERTY description: std::str;
      CREATE REQUIRED PROPERTY email: std::str;
      CREATE REQUIRED PROPERTY first_name: std::str;
      CREATE REQUIRED PROPERTY last_name: std::str;
      CREATE REQUIRED PROPERTY marital_status: std::bool;
      CREATE REQUIRED PROPERTY password: std::str;
      CREATE REQUIRED PROPERTY salt: std::bytes;
      CREATE REQUIRED PROPERTY title: std::str;
      CREATE REQUIRED PROPERTY user_name: std::str;
      CREATE REQUIRED PROPERTY user_role: std::str;
  };
};
