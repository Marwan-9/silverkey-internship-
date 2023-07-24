module default {
    type Contact {
        required first_name: str; 
        required last_name: str; 
        required email: str; 
        required title: str; 
        required birth_date: str; 
        required marital_status: bool; 
        required user_name: str; 
        required password: str; 
        required user_role: str; 
        required salt: bytes; 
        description: str;
    }
}